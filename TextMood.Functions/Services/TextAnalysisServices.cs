using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;

namespace TextMood.Functions
{
    static class TextAnalysisServices
    {
        readonly static Lazy<TextAnalyticsClient> _textAnalyticsApiClientHolder = new Lazy<TextAnalyticsClient>(() =>
            new TextAnalyticsClient(new ApiKeyServiceClientCredentials(CognitiveServicesConstants.TextSentimentAPIKey)) { Endpoint = CognitiveServicesConstants.BaseUrl });

        static TextAnalyticsClient TextAnalyticsApiClient => _textAnalyticsApiClientHolder.Value;

        public static async Task<double?> GetSentiment(string text)
        {
            var sentimentDocument = new MultiLanguageBatchInput(new List<MultiLanguageInput> { { new MultiLanguageInput(id: "1", text: text) } });

            var sentimentResults = await TextAnalyticsApiClient.SentimentBatchAsync(sentimentDocument).ConfigureAwait(false);

            if (sentimentResults?.Errors?.Any() is true)
            {
                var exceptionList = sentimentResults.Errors.Select(x => new Exception($"Id: {x.Id}, Message: {x.Message}"));
                throw new AggregateException(exceptionList);
            }

            var documentResult = sentimentResults?.Documents?.FirstOrDefault();

            return documentResult?.Score;
        }

        class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            readonly string _subscriptionKey;

            public ApiKeyServiceClientCredentials(string subscriptionKey) => _subscriptionKey = subscriptionKey;

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request is null)
                    throw new ArgumentNullException(nameof(request));

                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                return Task.CompletedTask;
            }
        }
    }
}
