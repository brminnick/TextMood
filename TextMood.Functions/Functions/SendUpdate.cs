using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TextMood.Backend.Common;
using TextMood.Shared;

namespace TextMood.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    public class SendUpdate : BaseSignalRService
    {
        [Function(nameof(SendUpdate))]
        public async Task Run([Microsoft.Azure.Functions.Worker.QueueTrigger(QueueNameConstants.SendUpdate)]TextMoodModel textModel, FunctionContext context)
        {
            var log = context.GetLogger<SendUpdate>();

            InitializationFailed += HandleInitializationFailed;

            try
            {
                var hub = await GetConnection().ConfigureAwait(false);
                await hub.InvokeAsync(SignalRConstants.SendNewTextMoodModelMethod, textModel).ConfigureAwait(false);
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

            void HandleInitializationFailed(object? sender, string message) => log.LogInformation($"Initialization Failed: {message}");
        }
    }
}
