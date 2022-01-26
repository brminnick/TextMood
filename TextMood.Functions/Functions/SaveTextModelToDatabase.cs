using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
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

        [Function(nameof(SaveTextModelToDatabase))]
        public Task Run([Microsoft.Azure.Functions.Worker.QueueTrigger(QueueNameConstants.TextModelForDatabase)] TextMoodModel textModel, FunctionContext context)
        {
            var log = context.GetLogger<SaveTextModelToDatabase>();
            log.LogInformation("Saving TextModel to Database");

            if (textModel.Text.Length > 128)
                textModel.Text = textModel.Text[..128];

            return _textMoodDatabase.InsertTextModel(textModel);
        }
    }
}
