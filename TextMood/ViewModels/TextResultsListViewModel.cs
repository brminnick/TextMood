using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Linq;

namespace TextMood
{
    public class TextResultsListViewModel : BaseViewModel
    {
        #region Fields
        bool _isRefreshing;
		IList<TextModel> _textList;
        ICommand _pullToRefreshCommand;
        #endregion

        #region Events
        public event EventHandler<string> ErrorTriggered;
        #endregion

        #region Properties
        public ICommand PullToRefreshCommand => _pullToRefreshCommand ??
            (_pullToRefreshCommand = new Command(async () => await ExecutePullToRefreshCommand()));

		public IList<TextModel> TextList
        {
            get => _textList;
            set => SetProperty(ref _textList, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        #endregion

        #region Methods
        Task ExecutePullToRefreshCommand() => UpdateTextResultsList();

		async Task UpdateTextResultsList()
        {
            try
            {
                IsRefreshing = true;

				var textList = await TextResultsService.GetTextModels().ConfigureAwait(false);
				TextList = textList.OrderByDescending(x => x.CreatedAt).ToList();
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

        void OnErrorTriggered(string message) => ErrorTriggered?.Invoke(this, message);
        #endregion
    }
}
