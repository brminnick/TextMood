using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using TextMood.Backend.Common;

namespace TextMood.Functions
{
    public static class AnalyzeTextSentiment
    {
        readonly static Lazy<JsonSerializer> _serializerHolder = new Lazy<JsonSerializer>();

        static JsonSerializer Serializer => _serializerHolder.Value;

        [FunctionName(nameof(AnalyzeTextSentiment))]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage httpRequest, TraceWriter log)
        {
            log.Info("Text Message Received");

            log.Info("Parsing Request Message");
            var httpRequestBody = await httpRequest.Content.ReadAsStringAsync().ConfigureAwait(false);

            log.Info("Creating New Text Model");
            var textMessage = new TextModel(GetTextMessage(httpRequestBody, log));

            log.Info("Retrieving Sentiment Score");
            var sentimentScore = await TextAnalysisServices.GetSentiment(textMessage.Text).ConfigureAwait(false);

            log.Info("Saving TextModel to Database");
            await TextMoodDatabase.InsertTextModel(textMessage).ConfigureAwait(false);

            return httpRequest.CreateResponse($"Text Sentiment: {EmojiServices.GetEmoji(sentimentScore)}");
        }

        static string GetTextMessage(string httpRequestBody, TraceWriter log)
        {
            var formValues = httpRequestBody?.Split('&')
                                ?.Select(value => value.Split('='))
                                ?.ToDictionary(pair => Uri.UnescapeDataString(pair[0]).Replace("+", " "),
                                              pair => Uri.UnescapeDataString(pair[1]).Replace("+", " "));

            foreach (var value in formValues)
                log.Info($"Key: {value.Key}, Value: {value.Value}");

            var textMessageKeyValuePair = formValues.Where(x => x.Key?.ToUpper()?.Equals("BODY") ?? false)?.FirstOrDefault();
            return textMessageKeyValuePair?.Value;
        }
    }
}
