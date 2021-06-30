#nullable enable

namespace Microsoft.Maui.Hosting
{
	public static class AppHost
	{
		public static IAppHostBuilder CreateDefaultBuilder()
		{
			var builder = new AppHostBuilder();

			builder.UseMicrosoftExtensionsServiceProviderFactory();
			builder.ConfigureFonts();
			builder.ConfigureImageSources();
			builder.ConfigureAnimations();

			return builder;
		}
	}
}