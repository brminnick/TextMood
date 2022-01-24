using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using TextMood.Backend.Common;

namespace TextMood.Functions
{
    class GetTextModels
    {
        readonly TextMoodDatabase _textMoodDatabase;

        public GetTextModels(TextMoodDatabase textMoodDatabase) => _textMoodDatabase = textMoodDatabase;

        [FunctionName(nameof(GetTextModels))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest httpRequest, ILogger log)
        {
            log.LogInformation("Retrieving Text Models from Database");

            try
            {
                var textModelList = await _textMoodDatabase.GetAllTextModels().ConfigureAwait(false);

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
