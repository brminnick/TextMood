using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using TextMood.Backend.Common;

namespace TextMood.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    public static class SaveTextModelToDatabase
    {
        [FunctionName(nameof(SaveTextModelToDatabase))]
        public static void Run(
            [QueueTrigger(QueueNameConstants.TextModelForDatabase)]TextModel textModel, 
            [Queue(QueueNameConstants.UpdatePhillipsHueLight)] out string queueMessage,
            TraceWriter log)
        {
            log.Info("Saving TextModel to Database");
            TextMoodDatabase.InsertTextModel(textModel).GetAwaiter().GetResult();

            queueMessage = QueueNameConstants.UpdatePhillipsHueLight;
        }
    }
}
