using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

using AsyncAwaitBestPractices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace TextMood.Shared
{
    public abstract class BaseSignalRService
    {
        readonly static WeakEventManager<string> _initializationFailedEventManager = new WeakEventManager<string>();
        readonly static Lazy<HubConnection> _hubHolder = new Lazy<HubConnection>(() =>
        {
            return new HubConnectionBuilder().WithUrl(SignalRConstants.SignalRHubUrl).ConfigureLogging(logging =>
            {
                logging.AddProvider(new DebugLoggerProvider());
                logging.SetMinimumLevel(LogLevel.Debug);
            }).Build();
        });

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

                    while (HubConnectionState is HubConnectionState.Connecting)
                        await Task.Delay(100).ConfigureAwait(false);

                    if (HubConnectionState != HubConnectionState.Connected)
                        throw new HubException($"{nameof(HubConnectionState)} not connected\n{nameof(HubConnectionState)}:{HubConnectionState}");
                }
                catch (Exception e)
                {
                    OnInitializationFailed(e.Message);
                }
            }

            return Hub;
        }

        static void OnInitializationFailed(string message) => _initializationFailedEventManager.HandleEvent(null, message, nameof(InitializationFailed));

        class DebugLoggerProvider : ILoggerProvider
        {
            private readonly ConcurrentDictionary<string, ILogger> _loggers;

            public DebugLoggerProvider() => _loggers = new ConcurrentDictionary<string, ILogger>();

            public void Dispose()
            {
            }

            public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, new DebugLogger());

            class DebugLogger : ILogger
            {
                public void Log<TState>(
                   LogLevel logLevel, EventId eventId,
                   TState state, Exception exception,
                   Func<TState, Exception, string> formatter)
                {
                    if (formatter != null)
                    {
                        Debug.WriteLine(formatter(state, exception));
                    }
                }

                public bool IsEnabled(LogLevel logLevel) => true;

                public IDisposable? BeginScope<TState>(TState state) => null;
            }
        }
    }
}
