using System;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;

using Microsoft.AspNetCore.SignalR.Client;

namespace TextMood.Shared
{
    public abstract class BaseSignalRService
    {
        readonly static WeakEventManager<string> _initializationFailedEventManager = new WeakEventManager<string>();
        readonly static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnectionBuilder().WithUrl(SignalRConstants.SignalRHubUrl).Build());

        public static event EventHandler<string> InitializationFailed
        {
            add => _initializationFailedEventManager.AddEventHandler(value);
            remove => _initializationFailedEventManager.RemoveEventHandler(value);
        }

        public static HubConnectionState HubConnectionState => Hub.State;

        static HubConnection Hub => _hubHolder.Value;

        protected static async ValueTask<HubConnection> GetConnection()
        {
            if (HubConnectionState is HubConnectionState.Disconnected)
            {
                try
                {
                    await Hub.StartAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    OnInitializationFailed(e.Message);
                }
            }

            return Hub;
        }

        static void OnInitializationFailed(string message) => _initializationFailedEventManager.HandleEvent(null, message, nameof(InitializationFailed));
    }
}
