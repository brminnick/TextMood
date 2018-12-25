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
        #region Constant Fields
        readonly WeakEventManager<string> _errorTriggeredEventManager = new WeakEventManager<string>();
        readonly WeakEventManager _philipsHueBridgeConnectionFailedEventManager = new WeakEventManager();
        #endregion

        #region Fields
        bool _isRefreshing;
        Color _backgroundColor;
        ObservableCollection<ITextMoodModel> _textList;
        ICommand _pullToRefreshCommand, _addTextMoodModelCommand;
        #endregion

        #region Events
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
        #endregion

        #region Properties
        public ICommand AddTextMoodModelCommand => _addTextMoodModelCommand ??
            (_addTextMoodModelCommand = new AsyncCommand<ITextMoodModel>(ExecuteAddTextMoodModelCommand, continueOnCapturedContext: false));


        public ICommand PullToRefreshCommand => _pullToRefreshCommand ??
            (_pullToRefreshCommand = new AsyncCommand(ExecutePullToRefreshCommand, continueOnCapturedContext: false));

        public ObservableCollection<ITextMoodModel> TextList
        {
            get => _textList;
            set => SetProperty(ref _textList, value);
        }

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
        #endregion

        #region Methods
        async Task ExecutePullToRefreshCommand()
        {
            IsRefreshing = true;

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

        Task ExecuteAddTextMoodModelCommand(ITextMoodModel textMoodModel)
        {
            if (TextList.Any(x => x.Id.Equals(textMoodModel.Id)))
                return Task.CompletedTask;

            TextList.Insert(0, textMoodModel);

            var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

            SetTextResultsListBackgroundColor(averageSentiment);

            return UpdatePhilipsHueLight(averageSentiment);
        }

        async Task UpdateTextResultsListFromRemoteDatabase()
        {
            try
            {
                var textMoodList = await TextResultsService.GetTextModels().ConfigureAwait(false);
                var recentTextMoodList = TextMoodModelServices.GetRecentTextModels(new List<ITextMoodModel>(textMoodList), TimeSpan.FromHours(1));

                TextList = new ObservableCollection<ITextMoodModel>(recentTextMoodList.OrderByDescending(x => x.CreatedAt));
            }
            catch (Exception e) when (e?.InnerException?.Message != null)
            {
                DebugServices.Report(e);
                OnErrorTriggered(e.InnerException.Message);
            }
            catch (Exception e)
            {
                DebugServices.Report(e);
                OnErrorTriggered(e.Message);
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
        #endregion
    }
}
