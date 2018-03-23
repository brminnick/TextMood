
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TextMood.Functions
{
    public static class AnalyzeTextSentiment
    {
        [FunctionName(nameof(AnalyzeTextSentiment))]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            return new OkObjectResult("Text Message Received");
        }
    }
}
