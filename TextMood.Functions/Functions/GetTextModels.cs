using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using TextMood.Backend.Common;

namespace TextMood.Functions
{
    public static class GetTextModels
    {
        [FunctionName(nameof(GetTextModels))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequest httpRequest, ILogger log)
        {
            log.LogInformation("Retrieving Text Models from Database");

            try
            {
                var textModelList = await TextMoodDatabase.GetAllTextModels().ConfigureAwait(false);

                log.LogInformation($"Success");
                return new OkObjectResult(textModelList);
            }
            catch (System.Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
        }
    }
}
