using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using TextMood.Shared;

namespace TextMood.SignalR
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddSignalR();

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSignalR(route => route.MapHub<TextMoodModelHub>($"/{SignalRConstants.TextMoodModelHubName}"));
        }
    }
}
