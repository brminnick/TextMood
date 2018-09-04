using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using TextMood.Backend.Common;
using TextMood.Shared;

namespace TextMood.Functions.Functions
{
	[StorageAccount(QueueNameConstants.AzureWebJobsStorage)]
	public static class SendUpdate
	{
		#region Constant Fields
		static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnectionBuilder().WithUrl(SignalRConstants.SignalRHubUrl).Build());
        #endregion

        #region Properties
        static HubConnection Hub => _hubHolder.Value;
		#endregion

		#region Methods
		[FunctionName(nameof(SendUpdate))]
		public static async Task Run([QueueTrigger(QueueNameConstants.SendUpdate)]TextMoodModel textModel, TraceWriter log) =>
			await Hub.InvokeAsync(SignalRConstants.SendNewTextMoodModelName, textModel).ConfigureAwait(false);
		#endregion
	}
}
