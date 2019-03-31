using System;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;

using Microsoft.AspNetCore.SignalR.Client;

namespace TextMood.Shared
{
    public abstract class BaseSignalRService
    {
        #region Constant Fields
        readonly static WeakEventManager<string> _initializationFailedEventManager = new WeakEventManager<string>();
        readonly static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnectionBuilder().WithUrl(SignalRConstants.SignalRHubUrl).Build());
        #endregion

        #region Events
        public static event EventHandler<string> InitializationFailed
        {
            add => _initializationFailedEventManager.AddEventHandler(value);
            remove => _initializationFailedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Properties
        public static HubConnectionState HubConnectionState => Hub.State;

        static HubConnection Hub => _hubHolder.Value;
        #endregion

        #region Methods
        protected static async ValueTask<HubConnection> GetConnection()
        {
            if (HubConnectionState.Equals(HubConnectionState.Disconnected))
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

        static void OnInitializationFailed(string message) => _initializationFailedEventManager?.HandleEvent(null, message, nameof(InitializationFailed));
        #endregion
    }
}
