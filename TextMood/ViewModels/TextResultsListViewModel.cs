using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

using TextMood.Shared;

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
			SetTextResultsListBackgroundColor(TextList);
		}

		async Task UpdateTextResultsList()
		{
			try
			{
				IsRefreshing = true;

				var textMoodList = await TextResultsService.GetTextModels().ConfigureAwait(false);
				var recentTextMoodList = TextModelServices.GetRecentTextModels(new List<ITextMoodModel>(textMoodList), TimeSpan.FromHours(1));

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

		void SetTextResultsListBackgroundColor(IList<ITextMoodModel> textMoodModelList)
		{
			var averageSentiment = (double)TextModelServices.GetAverageSentimentScore(textMoodModelList);
			if (averageSentiment < 0 || averageSentiment > 1)
				BackgroundColor = default;
			else
				BackgroundColor = Color.FromRgba(1 - averageSentiment, averageSentiment, 0, 0.5);
		}

		void OnErrorTriggered(string message) => ErrorTriggered?.Invoke(this, message);
		#endregion
	}
}
