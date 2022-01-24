using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using TextMood.Shared;

namespace TextMood
{
    public class TextResultsListViewModel : BaseViewModel
    {
        readonly WeakEventManager<string> _errorTriggeredEventManager = new WeakEventManager<string>();
        readonly WeakEventManager _philipsHueBridgeConnectionFailedEventManager = new WeakEventManager();

        readonly TextResultsService _textResultsService;
        readonly PhilipsHueServices _philipsHueServices;
        readonly PhilipsHueBridgeSettingsService _philipsHueBridgeSettingsService;

        bool _isRefreshing;
        Xamarin.Forms.Color _backgroundColor;

        public TextResultsListViewModel(TextResultsService textResultsService,
                                            PhilipsHueServices philipsHueServices,
                                            PhilipsHueBridgeSettingsService philipsHueBridgeSettingsService)
        {
            _textResultsService = textResultsService;
            _philipsHueServices = philipsHueServices;
            _philipsHueBridgeSettingsService = philipsHueBridgeSettingsService;

            PullToRefreshCommand = new AsyncCommand(ExecutePullToRefreshCommand);
        }

        public event EventHandler<string> ErrorTriggered
        {
            add => _errorTriggeredEventManager.AddEventHandler(value);
            remove => _errorTriggeredEventManager.RemoveEventHandler(value);
        }

        public event EventHandler PhilipsHueBridgeConnectionFailed
        {
            add => _philipsHueBridgeConnectionFailedEventManager.AddEventHandler(value);
            remove => _philipsHueBridgeConnectionFailedEventManager.RemoveEventHandler(value);
        }

        public IAsyncCommand PullToRefreshCommand { get; }

        public ObservableCollection<ITextMoodModel> TextList { get; } = new ObservableCollection<ITextMoodModel>();

        public Xamarin.Forms.Color BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public async Task AddTextMoodModel(TextMoodModel textMoodModel)
        {
            if (TextList.Any(x => x.Id.Equals(textMoodModel.Id)))
                return;

            TextList.Insert(0, textMoodModel);

            var averageSentiment = TextList.GetAverageSentimentScore();

            SetTextResultsListBackgroundColor(averageSentiment);

            await UpdatePhilipsHueLight(averageSentiment).ConfigureAwait(false);
        }

        async Task ExecutePullToRefreshCommand()
        {
            try
            {
                await UpdateTextResultsListFromRemoteDatabase().ConfigureAwait(false);

                var averageSentiment = TextList.GetAverageSentimentScore();

                SetTextResultsListBackgroundColor(averageSentiment);

                await UpdatePhilipsHueLight(averageSentiment).ConfigureAwait(false);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        async Task UpdateTextResultsListFromRemoteDatabase()
        {
            try
            {
                var textMoodList = await _textResultsService.GetTextModels();
                var recentTextMoodList = textMoodList.FilterRecentTextModels(TimeSpan.FromHours(1));

                TextList.Clear();

                foreach (var textMoodModel in recentTextMoodList.OrderByDescending(x => x.CreatedAt))
                    TextList.Add(textMoodModel);
            }
            catch (Exception e)
            {
                DebugServices.Report(e);
                OnErrorTriggered(e.InnerException?.Message ?? e.Message);
            }
        }

        void SetTextResultsListBackgroundColor(double averageSentiment)
        {
            var (red, green, blue) = averageSentiment.GetRGBValues();
            BackgroundColor = Xamarin.Forms.Color.FromRgba(red, green, blue, 0.5);
        }

        async Task UpdatePhilipsHueLight(double averageSentiment)
        {
            if (!_philipsHueBridgeSettingsService.IsEnabled)
                return;

            try
            {
                var (red, green, blue) = averageSentiment.GetRGBValues();
                var hue = Shared.PhilipsHueServices.ConvertToHue(red, green, blue);

                await _philipsHueServices.UpdateLightBulbColor(_philipsHueBridgeSettingsService.Username, hue).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                DebugServices.Report(e);
                OnPhilipsHueBridgeConnectionFailed();
            }
        }

        void OnErrorTriggered(string message) => _errorTriggeredEventManager.RaiseEvent(this, message, nameof(ErrorTriggered));
        void OnPhilipsHueBridgeConnectionFailed() => _philipsHueBridgeConnectionFailedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(PhilipsHueBridgeConnectionFailed));
    }
}
