using System;
using System.Threading.Tasks;

using Microsoft.AspNet.SignalR.Client;

using TextMood.Shared;

namespace TextMood
{
	public abstract class BaseSignalRService
	{
		#region Constant Fields
		static Lazy<IHubProxy> _proxyHolder = new Lazy<IHubProxy>(() => Hub.CreateHubProxy(SignalRConstants.TextMoodModelHubName));
		static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnection(SignalRConstants.SignalRHubUrl));
		#endregion

		#region Fields
		static bool _isInitialized;
		#endregion

		#region Properties
		static HubConnection Hub => _hubHolder.Value;
		static IHubProxy Proxy => _proxyHolder.Value;
		#endregion

		#region Methods
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
		#endregion
	}
}
