using System;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;

namespace TextMood
{
    public static class ServiceCollection
	{
		static Lazy<IServiceProvider> _serviceProviderHolder = new(CreateContainer);

		public static IServiceProvider ServiceProvider => _serviceProviderHolder.Value;

		static IServiceProvider CreateContainer()
		{
			var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

			// Refit Services
			services.AddSingleton(RefitExtensions.For<IPhilipsHueApi>("https://www.meethue.com/api"));
			services.AddSingleton(RefitExtensions.For<ITextModelApi>(BackendConstants.GetEmotionResultsAPIUrl));

			// TextMood Services
			services.AddSingleton<DebugServices>();
			services.AddSingleton<PhilipsHueServices>();
			services.AddSingleton<PhilipsHueBridgeSettingsService>();
			services.AddSingleton<SignalRService>();
			services.AddSingleton<SignalRService>();
			services.AddSingleton<TextResultsService>();

			// ViewModels
			services.AddTransient<HueBridgeSetupViewModel>();
			services.AddTransient<TextResultsListViewModel>();

			// Pages
			services.AddTransient<HueBridgeSetupPage>();
			services.AddTransient<TextResultsListPage>();

			// Xamarin.Essentials
			services.AddSingleton<IMainThread, MainThreadImplementation>();
			services.AddSingleton<IPreferences, PreferencesImplementation>();

			// App
			services.AddSingleton<App>();

			return services.BuildServiceProvider();
		}
	}
}