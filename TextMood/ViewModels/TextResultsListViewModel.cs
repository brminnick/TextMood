using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;

using TextMood.Shared;

using Xamarin.Forms;

namespace TextMood
{
    public class TextResultsListViewModel : BaseViewModel
    {
        #region Fields
        bool _isRefreshing;
        Color _backgroundColor;
        ObservableCollection<ITextMoodModel> _textList;
        ICommand _pullToRefreshCommand;
        Command<ITextMoodModel> _addTextMoodModelCommand;
        #endregion

        #region Events
        public event EventHandler<string> ErrorTriggered;
        public event EventHandler PhilipsHueBridgeConnectionFailed;
        #endregion

        #region Properties
        public Command<ITextMoodModel> AddTextMoodModelCommand => _addTextMoodModelCommand ??
            (_addTextMoodModelCommand = new Command<ITextMoodModel>(async textMoodModel => await ExecuteAddTextMoodModelCommand(textMoodModel).ConfigureAwait(false)));


        public ICommand PullToRefreshCommand => _pullToRefreshCommand ??
            (_pullToRefreshCommand = new Command(async () => await ExecutePullToRefreshCommand().ConfigureAwait(false)));

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
            await UpdateTextResultsListFromRemoteDatabase().ConfigureAwait(false);

            var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

            SetTextResultsListBackgroundColor(averageSentiment);

            await UpdatePhilipsHueLight(averageSentiment).ConfigureAwait(false);
        }

        Task ExecuteAddTextMoodModelCommand(ITextMoodModel textMoodModel)
        {
            TextList.Insert(0, textMoodModel);

            var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

            SetTextResultsListBackgroundColor(averageSentiment);

            return UpdatePhilipsHueLight(averageSentiment);
        }

        async Task UpdateTextResultsListFromRemoteDatabase()
        {
            try
            {
                IsRefreshing = true;

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
            finally
            {
                IsRefreshing = false;
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
                var hue = PhilipsHueServices.ConvertToHue(red, green, blue);

                await PhilipsHueBridgeAPIServices.UpdateLightBulbColor(PhilipsHueBridgeSettings.IPAddress.ToString(),
                                                                        PhilipsHueBridgeSettings.Username,
                                                                        hue).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                DebugServices.Report(e);
                OnPhilipsHueBridgeConnectionFailed();
            }
        }

        void OnErrorTriggered(string message) => ErrorTriggered?.Invoke(this, message);
        void OnPhilipsHueBridgeConnectionFailed() => PhilipsHueBridgeConnectionFailed?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
