using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Rest;
using TextMood.Backend.Common;

namespace TextMood.Functions
{
    public class Program
    {
        readonly static string _connectionString = Environment.GetEnvironmentVariable("TextMoodDatabaseConnectionString") ?? string.Empty;

        static Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(builder => builder.AddCommandLine(args))
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    services.AddLogging();

                    services.AddHttpClient();

                    services.AddTransient(serviceProvider => new TextAnalyticsClient(new ApiKeyServiceClientCredentials(CognitiveServicesConstants.TextSentimentAPIKey)) { Endpoint = CognitiveServicesConstants.BaseUrl });
                    services.AddTransient<HttpRequestServices>();
                    services.AddTransient<TwilioServices>();
                    services.AddTransient<TextMoodDatabase>();

                    services.AddDbContext<TextMoodDatabaseContext>(options => options.UseSqlServer(_connectionString));
                }).Build();

            return host.RunAsync();
        }

        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            readonly string _subscriptionKey;

            public ApiKeyServiceClientCredentials(string subscriptionKey) => _subscriptionKey = subscriptionKey;

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                ArgumentNullException.ThrowIfNull(request);

                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                return Task.CompletedTask;
            }
        }
    }
}
