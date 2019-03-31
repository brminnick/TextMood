using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using TextMood.Backend.Common;
using TextMood.Shared;

namespace TextMood.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    public class SendUpdate : BaseSignalRService
    {
        [FunctionName(nameof(SendUpdate))]
        public static async Task Run([QueueTrigger(QueueNameConstants.SendUpdate)]TextMoodModel textModel, ILogger log)
        {
            InitializationFailed += HandleInitializationFailed;

            try
            {
                var hub = await GetConnection().ConfigureAwait(false);
                await hub.InvokeAsync(SignalRConstants.SendNewTextMoodModelName, textModel).ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
            finally
            {
                InitializationFailed -= HandleInitializationFailed;
            }

            void HandleInitializationFailed(object sender, string e) => log.LogInformation($"Initialization Failed: {e}");
        }
    }
}
