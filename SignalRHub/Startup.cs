using Microsoft.Owin;

using Owin;

[assembly: OwinStartup(typeof(SignalRHub.Startup))]
namespace SignalRHub
{
    public class Startup
    {
        public void Configuration(IAppBuilder app) => app.MapSignalR();
    }
}
