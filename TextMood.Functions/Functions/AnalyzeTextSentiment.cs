using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TextMood.Functions
{
	public static class AnalyzeTextSentiment
	{
		[FunctionName(nameof(AnalyzeTextSentiment))]
		public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest httpRequest, TraceWriter log)
		{
			log.Info("Text Message Received");

			var httpRequestBody = GetHttpRequestBody(httpRequest, log);
			var textMessage = GetTextMessage(httpRequestBody, log);

			var sentimentScore = await TextAnalysisServices.GetSentiment(textMessage).ConfigureAwait(false);

			return new OkObjectResult($"Text Sentiment: {EmojiServices.GetEmoji(sentimentScore)}");
		}

		static string GetHttpRequestBody(HttpRequest req, TraceWriter log)
		{
			var data = string.Empty;

			req.EnableRewind();

			using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
				data = reader.ReadToEnd();

			req.Body.Position = 0;

			return data;
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
