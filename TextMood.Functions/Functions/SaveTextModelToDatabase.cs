using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TextMood.Backend.Common;

namespace TextMood.Functions
{
    [StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
    class SaveTextModelToDatabase
    {
        readonly TextMoodDatabase _textMoodDatabase;

        public SaveTextModelToDatabase(TextMoodDatabase textMoodDatabase) => _textMoodDatabase = textMoodDatabase;

        [FunctionName(nameof(SaveTextModelToDatabase))]
        public Task Run([QueueTrigger(QueueNameConstants.TextModelForDatabase)]TextMoodModel textModel, ILogger log)
        {
            log.LogInformation("Saving TextModel to Database");

            if (textModel.Text.Length > 128)
                textModel.Text = textModel.Text.Substring(0, 128);

            return _textMoodDatabase.InsertTextModel(textModel);
        }
    }
}
