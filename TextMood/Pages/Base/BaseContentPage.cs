using Xamarin.Forms;

namespace TextMood
{
	public abstract class BaseContentPage<TViewModel> : ContentPage where TViewModel : BaseViewModel
	{
		protected BaseContentPage(TViewModel viewModel) => base.BindingContext = viewModel;

		public new TViewModel BindingContext => (TViewModel)base.BindingContext;
	}
}
