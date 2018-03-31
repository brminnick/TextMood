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

			await UpdatePhillipsHueLight(averageSentiment).ConfigureAwait(false);
		}

		async Task ExecuteAddTextMoodModelCommand(ITextMoodModel textMoodModel)
		{
			var textList = TextList;
			textList.Add(textMoodModel);

			TextList = new List<ITextMoodModel>(textList.OrderByDescending(x => x.CreatedAt).ToList());

			var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

            SetTextResultsListBackgroundColor(averageSentiment);

            await UpdatePhillipsHueLight(averageSentiment).ConfigureAwait(false);
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
			catch (Exception e)
			{
				OnErrorTriggered(e.InnerException.Message);
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

		async Task UpdatePhillipsHueLight(float averageSentiment)
		{
			try
			{
				var (red, green, blue) = TextMoodModelServices.GetRGBFromSentimentScore(averageSentiment);
				var hue = PhillipsHueServices.ConvertToHue(red, green, blue);

				await PhillipsHueBridgeServices.UpdateLightBulbColor(hue).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				OnErrorTriggered(e.Message);
			}
		}

		void OnErrorTriggered(string message) => ErrorTriggered?.Invoke(this, message);
		#endregion
	}
}
