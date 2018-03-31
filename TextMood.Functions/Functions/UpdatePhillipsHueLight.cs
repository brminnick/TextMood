using System;
using System.Collections.Generic;
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
        static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnection(SignalRConstants.SignalRHubUrl));
        static Lazy<IHubProxy> _proxyHolder = new Lazy<IHubProxy>(() =>
        {
            var proxy = Hub.CreateHubProxy(SignalRConstants.TextMoodModelHub);
            Hub.Start().GetAwaiter().GetResult();
            return proxy;
        });

        static HubConnection Hub => _hubHolder.Value;
        static IHubProxy Proxy => _proxyHolder.Value;

		[FunctionName(nameof(SendUpdate))]
		public static async Task Run([QueueTrigger(QueueNameConstants.SendUpdate)]TextMoodModel textModel, TraceWriter log)
		{
           await Proxy.Invoke(SignalRConstants.TextMoodModelHub, textModel).ConfigureAwait(false);
		}
	}
}
