namespace TextMood
{
    public class App : Xamarin.Forms.Application
    {
        readonly SignalRService _signalRService;

        public App(SignalRService signalRService, TextResultsListPage textResultsListPage)
        {
            _signalRService = signalRService;
            MainPage = new BaseNavigationPage(textResultsListPage);
        }

        protected override async void OnStart()
        {
            base.OnStart();

            await _signalRService.Subscribe().ConfigureAwait(false);
        }
    }
}
