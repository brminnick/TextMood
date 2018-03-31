using System;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Client;
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
		static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnection(SignalRConstants.SignalRHubUrl));
		static Lazy<IHubProxy> _proxyHolder = new Lazy<IHubProxy>(() =>
		{
			var proxy = Hub.CreateHubProxy(SignalRConstants.TextMoodModelHubName);
			Hub.Start().GetAwaiter().GetResult();
			return proxy;
		});
		#endregion

		#region Properties
		static HubConnection Hub => _hubHolder.Value;
		static IHubProxy Proxy => _proxyHolder.Value;
		#endregion

		#region Methods
		[FunctionName(nameof(SendUpdate))]
		public static async Task Run([QueueTrigger(QueueNameConstants.SendUpdate)]TextMoodModel textModel, TraceWriter log) =>
			await Proxy.Invoke(SignalRConstants.SendNewTextMoodModelName, textModel).ConfigureAwait(false);
		#endregion
	}
}
