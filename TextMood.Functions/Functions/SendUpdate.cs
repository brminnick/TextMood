using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using TextMood.Backend.Common;
using TextMood.Shared;

namespace TextMood.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    public abstract class SendUpdate : BaseSignalRService
    {
        #region Methods
        [FunctionName(nameof(SendUpdate))]
        public static async Task Run([QueueTrigger(QueueNameConstants.SendUpdate)]TextMoodModel textModel, ILogger log)
        {
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
        }
        #endregion
    }
}
