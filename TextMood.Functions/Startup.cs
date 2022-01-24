using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using TextMood.Backend.Common;
using TextMood.Functions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TextMood.Functions
{
    public class Startup : FunctionsStartup
    {
        readonly static string _connectionString = Environment.GetEnvironmentVariable("TextMoodDatabaseConnectionString") ?? string.Empty;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(new TextAnalyticsClient(new ApiKeyServiceClientCredentials(CognitiveServicesConstants.TextSentimentAPIKey)) { Endpoint = CognitiveServicesConstants.BaseUrl });
            builder.Services.AddSingleton<HttpRequestServices>();
            builder.Services.AddSingleton<TwilioServices>();
            builder.Services.AddSingleton<TextMoodDatabase>();
        }

        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            readonly string _subscriptionKey;

            public ApiKeyServiceClientCredentials(string subscriptionKey) => _subscriptionKey = subscriptionKey;

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request is null)
                    throw new ArgumentNullException(nameof(request));

                request?.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                return Task.CompletedTask;
            }
        }
    }
}
