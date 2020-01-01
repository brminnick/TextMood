namespace TextMood
{
    public class App : Xamarin.Forms.Application
    {
        public App() => MainPage = new BaseNavigationPage(new TextResultsListPage());

        protected override async void OnStart()
        {
            base.OnStart();

            await SignalRService.Subscribe().ConfigureAwait(false);
        }
    }
}
