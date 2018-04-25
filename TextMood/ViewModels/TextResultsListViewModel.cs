using System;
using System.Collections.Generic;
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
		IList<ITextMoodModel> _textList;
		ICommand _pullToRefreshCommand;
		Command<ITextMoodModel> _addTextMoodModelCommand;
		#endregion

		#region Events
		public event EventHandler<string> ErrorTriggered;
		public event EventHandler PhilipsHueBridgeConnectionFailed;
		#endregion

		#region Properties
		public Command<ITextMoodModel> AddTextMoodModelCommand => _addTextMoodModelCommand ??
			(_addTextMoodModelCommand = new Command<ITextMoodModel>(async textMoodModel => await ExecuteAddTextMoodModelCommand(textMoodModel)));


		public ICommand PullToRefreshCommand => _pullToRefreshCommand ??
			(_pullToRefreshCommand = new Command(async () => await ExecutePullToRefreshCommand().ConfigureAwait(false)));

		public IList<ITextMoodModel> TextList
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

		async Task ExecuteAddTextMoodModelCommand(ITextMoodModel textMoodModel)
		{
			var textList = TextList;
			textList.Add(textMoodModel);

			TextList = new List<ITextMoodModel>(textList.OrderByDescending(x => x.CreatedAt).ToList());

			var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

			SetTextResultsListBackgroundColor(averageSentiment);

			await UpdatePhilipsHueLight(averageSentiment).ConfigureAwait(false);
		}

		async Task UpdateTextResultsListFromRemoteDatabase()
		{
			try
			{
				IsRefreshing = true;

				var textMoodList = await TextResultsService.GetTextModels().ConfigureAwait(false);
				var recentTextMoodList = TextMoodModelServices.GetRecentTextModels(new List<ITextMoodModel>(textMoodList), TimeSpan.FromHours(1));

				TextList = recentTextMoodList.OrderByDescending(x => x.CreatedAt).ToList();
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

		void SetTextResultsListBackgroundColor(float averageSentiment)
		{
			var (red, green, blue) = TextMoodModelServices.GetRGBFromSentimentScore(averageSentiment);
			BackgroundColor = Color.FromRgba(red, green, blue, 0.5);
		}

		async Task UpdatePhilipsHueLight(float averageSentiment)
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
