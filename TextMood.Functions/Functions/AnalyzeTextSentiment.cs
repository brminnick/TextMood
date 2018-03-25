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
            var textMessage = new TextModel(TwilioServices.GetTextMessageBody(httpRequestBody, log));

            log.Info("Retrieving Sentiment Score");
            textMessage.SentimentScore = await TextAnalysisServices.GetSentiment(textMessage.Text).ConfigureAwait(false);

            log.Info("Saving TextModel to Database");
            await TextMoodDatabase.InsertTextModel(textMessage).ConfigureAwait(false);

            var response = $"Text Sentiment: {EmojiServices.GetEmoji(textMessage.SentimentScore)}";

            log.Info($"Sending OK Response: {response}");
            return httpRequest.CreateResponse(System.Net.HttpStatusCode.OK, TwilioServices.CreateTwilioResponse(response));
        }
    }
}
