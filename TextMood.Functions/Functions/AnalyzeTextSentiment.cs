using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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

        [Function(nameof(AnalyzeTextSentiment))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
        {
            var log = context.GetLogger<AnalyzeTextSentiment>();

            try
            {
                log.LogInformation("Text Message Received");

                log.LogInformation("Parsing Request Body");
                var httpRequestBody = await HttpRequestServices.GetContentAsString(req.Body).ConfigureAwait(false);

                log.LogInformation("Creating New Text Model");
                var textMessageBody = _twilioServices.GetTextMessageBody(httpRequestBody, log) ?? throw new NullReferenceException("Text Message Body Null");
                var textMoodModel = new TextMoodModel(textMessageBody);

                log.LogInformation("Retrieving Sentiment Score");
                textMoodModel.SentimentScore = await _textAnalysisServices.GetSentiment(textMoodModel.Text).ConfigureAwait(false) ?? -1;

                log.LogInformation("Adding TextMoodModel to Storage Queue");
                var output = new AnalyzeTextSentimentOutput(textMoodModel);

                var sentimentResponseText = $"Text Sentiment: {EmojiServices.GetEmoji(textMoodModel.SentimentScore)}";
                log.LogInformation($"Sending OK Response: {sentimentResponseText}");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync(_twilioServices.CreateTwilioResponse(sentimentResponseText)).ConfigureAwait(false);
                response.Headers.Add("Content-Type", "application/xml");

                return response;
            }
            catch (Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
        }
    }

    class AnalyzeTextSentimentOutput
    {
        public AnalyzeTextSentimentOutput(TextMoodModel textMoodModel)
        {
            TextModelForDatabase = TextModelForSendUpdate = textMoodModel;
        }

        [QueueOutput(QueueNameConstants.TextModelForDatabase)]
        public TextMoodModel TextModelForDatabase { get; }

        [QueueOutput(QueueNameConstants.SendUpdate)]
        public TextMoodModel TextModelForSendUpdate { get; }
    }
}
