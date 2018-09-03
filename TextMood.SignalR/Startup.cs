[assembly: OwinStartup(typeof(TextMood.SignalR.Startup))]
namespace TextMood.SignalR
{
    public class Startup
    {
        public void Configuration(Microsoft.AspNetCore.SpaServices.ISpaBuilder app) => app.MapSignalR();
    }
}
