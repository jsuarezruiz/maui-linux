#nullable enable

using System;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui.Controls.Compatibility.Hosting
{

	public static partial class AppHostBuilderExtensions
	{

		internal static MauiAppBuilder ConfigureCompatibilityLifecycleEvents(this MauiAppBuilder builder) =>
			builder.ConfigureLifecycleEvents(events => events.AddGtk(OnConfigureLifeCycle));

		static void OnConfigureLifeCycle(IGtkLifecycleBuilder gtk)
		{
			gtk
			   .OnMauiContextCreated((mauiContext) =>
				{
					// This is the final Init that sets up the real context from the application.

					var state = new ActivationState(mauiContext);
					Forms.Init(state);
				});
		}

	}

}