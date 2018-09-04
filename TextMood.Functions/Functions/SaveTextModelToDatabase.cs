using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using TextMood.Backend.Common;

namespace TextMood.Functions
{
	[StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
	public static class SaveTextModelToDatabase
	{
		[FunctionName(nameof(SaveTextModelToDatabase))]
        public static async Task Run(
			[QueueTrigger(QueueNameConstants.TextModelForDatabase)]TextMoodModel textModel,
			TraceWriter log)
		{
			log.Info("Saving TextModel to Database");

			if (textModel.Text.Length > 128)
				textModel.Text = textModel.Text.Substring(0, 128);

            await TextMoodDatabase.InsertTextModel(textModel).ConfigureAwait(false);
		}
	}
}
