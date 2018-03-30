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
		#endregion

		#region Events
		public event EventHandler<string> ErrorTriggered;
		#endregion

		#region Properties
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
			await UpdateTextResultsList().ConfigureAwait(false);

			var averageSentiment = TextMoodModelServices.GetAverageSentimentScore(TextList);

			SetTextResultsListBackgroundColor(averageSentiment);

			await UpdatePhillipsHueLight(averageSentiment).ConfigureAwait(false);
		}

		async Task UpdateTextResultsList()
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

		ValueTask<HttpResponseMessage> UpdatePhillipsHueLight(float averageSentiment)
		{
			var (red, green, blue) = TextMoodModelServices.GetRGBFromSentimentScore(averageSentiment);
			var hue = PhillipsHueServices.ConvertToHue(red, green, blue);

			return PhillipsHueBridgeServices.UpdateLightBulbColor(hue);
		}
        
		void OnErrorTriggered(string message) => ErrorTriggered?.Invoke(this, message);
		#endregion
	}
}
