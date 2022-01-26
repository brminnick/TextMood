using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace TextMood.Functions
{
    class TextAnalysisServices
    {
        readonly TextAnalyticsClient _textAnalyticsApiClient;

        public TextAnalysisServices(TextAnalyticsClient textAnalyticsClient) => _textAnalyticsApiClient = textAnalyticsClient;

        public async Task<double?> GetSentiment(string text)
        {
            var sentimentDocument = new MultiLanguageBatchInput(new List<MultiLanguageInput> { { new MultiLanguageInput(id: "1", text: text) } });

            var sentimentResults = await _textAnalyticsApiClient.SentimentBatchAsync(sentimentDocument).ConfigureAwait(false);

            if (sentimentResults?.Errors?.Any() is true)
            {
                var exceptionList = sentimentResults.Errors.Select(x => new Exception($"Id: {x.Id}, Message: {x.Message}"));
                throw new AggregateException(exceptionList);
            }

            var documentResult = sentimentResults?.Documents?.FirstOrDefault();

            return documentResult?.Score;
        }
    }
}
