using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using TextMood.Backend.Common;

namespace TextMood.Functions
{
    class GetTextModels
    {
        readonly TextMoodDatabase _textMoodDatabase;

        public GetTextModels(TextMoodDatabase textMoodDatabase) => _textMoodDatabase = textMoodDatabase;

        [Function(nameof(GetTextModels))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, FunctionContext context)
        {
            var log = context.GetLogger<GetTextModels>();
            log.LogInformation("Retrieving Text Models from Database");

            try
            {
                var textModelList = await _textMoodDatabase.GetAllTextModels().ConfigureAwait(false);

                log.LogInformation($"Success");

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteAsJsonAsync(textModelList).ConfigureAwait(false);

                return response;
            }
            catch (System.Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
        }
    }
}
