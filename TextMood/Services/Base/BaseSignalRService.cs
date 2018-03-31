using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Client;

using TextMood.Shared;

namespace TextMood
{
	public abstract class BaseSignalRService
	{
		static bool _isInitialized;

		static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnection(SignalRConstants.SignalRHubUrl));
		static Lazy<IHubProxy> _proxyHolder = new Lazy<IHubProxy>(() => Hub.CreateHubProxy(SignalRConstants.TextMoodModelHubName));

		static HubConnection Hub => _hubHolder.Value;
		static IHubProxy Proxy => _proxyHolder.Value;

		protected static async ValueTask<IHubProxy> GetProxy()
		{
			Proxy.ToString();

			if (!_isInitialized)
			{
				await Hub.Start().ConfigureAwait(false);
				_isInitialized = true;
			}

			return Proxy;
		}
	}
}
