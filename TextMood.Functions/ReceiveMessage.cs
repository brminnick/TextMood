using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace TextMood.Functions
{
    public static class ReceiveMessage
    {
        [FunctionName(nameof(ReceiveMessage))]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var data = await req.Content.ReadAsStringAsync().ConfigureAwait(false);

            var formValues = data.Split('&').Select(x => x.Split('='))
                .ToDictionary(x => Uri.EscapeDataString(x[0]).Replace("+", " "), x => Uri.EscapeDataString(x[1]).Replace("+", " "));

            foreach(var value in formValues)
                log.Info($"Key: {value.Key}, Value: {value.Value}");

            return req.CreateResponse(System.Net.HttpStatusCode.OK, "Text Message Received");
        }
    }
}
