using System;
using System.Net.Http;
using System.Text;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using Newtonsoft.Json;

using TextMood.Backend.Common;
using TextMood.Shared;

namespace TextMood.Functions
{
	[StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
	public static class AnalyzeTextSentiment
	{
		readonly static Lazy<JsonSerializer> _serializerHolder = new Lazy<JsonSerializer>();

		static JsonSerializer Serializer => _serializerHolder.Value;

		[FunctionName(nameof(AnalyzeTextSentiment))]
		public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage httpRequest,
            [Queue(QueueNameConstants.TextModelForDatabase)] out TextMoodModel textModelForDatabase, TraceWriter log)
		{
			log.Info("Text Message Received");

			log.Info("Parsing Request Message");
            var httpRequestBody = httpRequest.Content.ReadAsStringAsync().GetAwaiter().GetResult();

			log.Info("Creating New Text Model");
            textModelForDatabase = new TextMoodModel(TwilioServices.GetTextMessageBody(httpRequestBody, log));

			log.Info("Retrieving Sentiment Score");
            textModelForDatabase.SentimentScore = TextAnalysisServices.GetSentiment(textModelForDatabase.Text).GetAwaiter().GetResult() ?? -1;

            var response = $"Text Sentiment: {EmojiServices.GetEmoji(textModelForDatabase.SentimentScore)}";

			log.Info($"Sending OK Response: {response}");
			return new HttpResponseMessage { Content = new StringContent(TwilioServices.CreateTwilioResponse(response), Encoding.UTF8, "application/xml") };
		}
	}
}
