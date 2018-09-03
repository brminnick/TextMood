using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using TextMood.Backend.Common;

namespace TextMood.Functions
{
    public static class GetTextModels
    {
        [FunctionName(nameof(GetTextModels))]
        public static async Task<ActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest httpRequest, TraceWriter log)
        {
            log.Info("Retrieving Text Models from Database");
            try
            {
                var textModelList = await TextMoodDatabase.GetAllTextModels().ConfigureAwait(false);

                log.Info($"Success");
                return new OkObjectResult(textModelList);
            }
            catch (System.Exception e)
            {
                log.Info($"Failed: {e.Message}");
                throw;
            }
        }
    }
}
