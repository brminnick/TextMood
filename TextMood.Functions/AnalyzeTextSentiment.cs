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
		public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
		{
			log.Info("Text Message Received");

			req.EnableRewind();

			string data = string.Empty;

			using (var reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
				data = reader.ReadToEnd();

			req.Body.Position = 0;

			var formValues = data.Split('&')
				.Select(value => value.Split('='))
				.ToDictionary(pair => Uri.UnescapeDataString(pair[0]).Replace("+", " "),
							  pair => Uri.UnescapeDataString(pair[1]).Replace("+", " "));


			foreach (var value in formValues)
				log.Info($"Key: {value.Key}, Value: {value.Value}");

			return new OkObjectResult(null);
		}
	}
}
