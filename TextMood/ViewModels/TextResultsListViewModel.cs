using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using TextMood.Shared;
using Xamarin.Forms;

namespace TextMood
{
    public class TextResultsListViewModel : BaseViewModel
    {
        readonly WeakEventManager<string> _errorTriggeredEventManager = new WeakEventManager<string>();
        readonly WeakEventManager _philipsHueBridgeConnectionFailedEventManager = new WeakEventManager();

        bool _isRefreshing;
        Color _backgroundColor;
        ICommand? _pullToRefreshCommand;

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

        public ICommand PullToRefreshCommand =>
            _pullToRefreshCommand ??= new AsyncCommand(ExecutePullToRefreshCommand);

        public ObservableCollection<ITextMoodModel> TextList { get; } = new ObservableCollection<ITextMoodModel>();

        public Color BackgroundColor
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

            await Device.InvokeOnMainThreadAsync(() => TextList.Insert(0, textMoodModel)).ConfigureAwait(false);

            var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

            SetTextResultsListBackgroundColor(averageSentiment);

            await UpdatePhilipsHueLight(averageSentiment).ConfigureAwait(false);
        }

        async Task ExecutePullToRefreshCommand()
        {
            try
            {
                await UpdateTextResultsListFromRemoteDatabase().ConfigureAwait(false);

                var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

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
                var textMoodList = await TextResultsService.GetTextModels();
                var recentTextMoodList = TextMoodModelServices.GetRecentTextModels(new List<ITextMoodModel>(textMoodList), TimeSpan.FromHours(1));

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
            var (red, green, blue) = TextMoodModelServices.GetRGBFromSentimentScore(averageSentiment);
            BackgroundColor = Color.FromRgba(red, green, blue, 0.5);
        }

        async Task UpdatePhilipsHueLight(double averageSentiment)
        {
            if (!PhilipsHueBridgeSettings.IsEnabled)
                return;

            try
            {
                var (red, green, blue) = TextMoodModelServices.GetRGBFromSentimentScore(averageSentiment);
                var hue = Shared.PhilipsHueServices.ConvertToHue(red, green, blue);

                await PhilipsHueServices.UpdateLightBulbColor(PhilipsHueBridgeSettings.Username, hue).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                DebugServices.Report(e);
                OnPhilipsHueBridgeConnectionFailed();
            }
        }

        void OnErrorTriggered(string message) => _errorTriggeredEventManager?.HandleEvent(this, message, nameof(ErrorTriggered));
        void OnPhilipsHueBridgeConnectionFailed() => _philipsHueBridgeConnectionFailedEventManager?.HandleEvent(this, EventArgs.Empty, nameof(PhilipsHueBridgeConnectionFailed));
    }
}
