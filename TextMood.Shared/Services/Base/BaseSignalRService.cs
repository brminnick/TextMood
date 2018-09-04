using System;
using System.Data;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;

namespace TextMood.Shared
{
    public abstract class BaseSignalRService
    {
        #region Constant Fields
        static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() => new HubConnectionBuilder().WithUrl(SignalRConstants.SignalRHubUrl).Build());
        #endregion

        #region Events
        public static event EventHandler<string> InitializationFailed;
        #endregion

        #region Properties
        public static ConnectionState HubConnectionState { get; private set; } = ConnectionState.Closed;

        static HubConnection Hub => _hubHolder.Value;
        #endregion

        #region Methods
        protected static async ValueTask<HubConnection> GetConnection()
        {
            if (HubConnectionState.Equals(ConnectionState.Closed))
            {
                try
                {
                    await Hub.StartAsync().ConfigureAwait(false);
                    HubConnectionState = ConnectionState.Open;
                }
                catch (Exception e)
                {
                    OnInitializationFailed(e.Message);
                }
            }

            return Hub;
        }

        static void OnInitializationFailed(string message) => InitializationFailed?.Invoke(null, message);
        #endregion
    }
}
