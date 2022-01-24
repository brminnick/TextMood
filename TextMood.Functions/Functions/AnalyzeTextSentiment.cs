using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using TextMood.Backend.Common;
using TextMood.Shared;

namespace TextMood.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    class AnalyzeTextSentiment
    {
        readonly TwilioServices _twilioServices;
        readonly TextAnalysisServices _textAnalysisServices;

        public AnalyzeTextSentiment(TwilioServices twilioServices, TextAnalysisServices textAnalysisServices) =>
            (_twilioServices, _textAnalysisServices) = (twilioServices, textAnalysisServices);

        [FunctionName(nameof(AnalyzeTextSentiment))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")]HttpRequest req,
            [Queue(QueueNameConstants.TextModelForDatabase)]ICollector<TextMoodModel> textModelForDatabaseCollection,
            [Queue(QueueNameConstants.SendUpdate)]ICollector<TextMoodModel> sendUpdateCollection, ILogger log)
        {
            try
            {
                log.LogInformation("Text Message Received");

                log.LogInformation("Parsing Request Body");
                var httpRequestBody = await HttpRequestServices.GetContentAsString(req).ConfigureAwait(false);

                log.LogInformation("Creating New Text Model");
                var textMessageBody = _twilioServices.GetTextMessageBody(httpRequestBody, log) ?? throw new NullReferenceException("Text Message Body Null");
                var textMoodModel = new TextMoodModel(textMessageBody);

                log.LogInformation("Retrieving Sentiment Score");
                textMoodModel.SentimentScore = await _textAnalysisServices.GetSentiment(textMoodModel.Text).ConfigureAwait(false) ?? -1;

                log.LogInformation("Adding TextMoodModel to Storage Queue");
                textModelForDatabaseCollection.Add(textMoodModel);
                sendUpdateCollection.Add(textMoodModel);

                var response = $"Text Sentiment: {EmojiServices.GetEmoji(textMoodModel.SentimentScore)}";

                log.LogInformation($"Sending OK Response: {response}");

                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = _twilioServices.CreateTwilioResponse(response),
                    ContentType = "application/xml"
                };
            }
            catch (Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
        }
    }
}
