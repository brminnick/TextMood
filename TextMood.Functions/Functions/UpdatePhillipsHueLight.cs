using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using TextMood.Backend.Common;

namespace TextMood.Functions.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    public static class UpdatePhillipsHueLight
    {
        [FunctionName(nameof(UpdatePhillipsHueLight))]
        public static async Task Run([QueueTrigger(QueueNameConstants.UpdatePhillipsHueLight)]string message, TraceWriter log)
        {
            log.Info("Retrieving All Text Models From Database");
            System.Collections.Generic.IList<TextModel> allTextModelsFromDatabase = new System.Collections.Generic.List<TextModel>();
            try
            {
                allTextModelsFromDatabase = await TextMoodDatabase.GetAllTextModels().ConfigureAwait(false);
            }
            catch(Exception e)
            {
                log.Info(e.Message);
            }
            log.Info($"Retrived {allTextModelsFromDatabase?.Count ?? -1} Text Models");
            log.Info($"Current Utc Time: {DateTimeOffset.UtcNow}");

            foreach (var textModel in allTextModelsFromDatabase)
            {
                log.Info("");
                log.Info($"{textModel.UpdatedAt.ToUniversalTime()}");
                var isWithinPastHour = DateTimeOffset.Compare(textModel?.UpdatedAt.ToUniversalTime() ?? default(DateTimeOffset), DateTimeOffset.UtcNow.AddHours(-1)) > -1;
                log.Info($"Is Within Past Hour: {isWithinPastHour}");
            }

            log.Info("Retrieving Text Models From Past Hour");
            var textModelsFromPastHour = allTextModelsFromDatabase?.Where(
                                            x => x?.SentimentScore >= 0 &&
                                            DateTimeOffset.Compare(x?.UpdatedAt.ToUniversalTime() ?? default(DateTimeOffset), DateTimeOffset.UtcNow.AddHours(-1)) > -1)?.ToList() ?? new System.Collections.Generic.List<TextModel>();
            log.Info($"Retrived {textModelsFromPastHour?.Count ?? -1} Text Models");

            log.Info("Retrieving Sentiment Score List");
            var sentimentListOverPastHour = textModelsFromPastHour?.Select(x => x.SentimentScore);

            log.Info("Get Average Sentiment Score");
            var averageSentiment = sentimentListOverPastHour?.Average();

            log.Info($"One Hour Running Sentiment Average: {averageSentiment}");
        }
    }
}
