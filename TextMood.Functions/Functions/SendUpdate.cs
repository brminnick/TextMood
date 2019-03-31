using System;
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
    public class SendUpdate : BaseSignalRService
    {
        static ILogger _log;

        public SendUpdate() => InitializationFailed += HandleInitializationFailed;

        #region Methods
        [FunctionName(nameof(SendUpdate))]
        public static async Task Run([QueueTrigger(QueueNameConstants.SendUpdate)]TextMoodModel textModel, ILogger log)
        {
            if (_log is null)
                _log = log;

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

        void HandleInitializationFailed(object sender, string e) => _log?.LogInformation($"Initialization Failed: {e}");
        #endregion
    }
}
