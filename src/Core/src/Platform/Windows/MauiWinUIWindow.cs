﻿using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui
{
	public class MauiWinUIWindow : UI.Xaml.Window
	{
		public MauiWinUIWindow()
		{
			Activated += OnActivated;
			Closed += OnClosed;
			VisibilityChanged += OnVisibilityChanged;
		}

		protected virtual void OnActivated(object sender, UI.Xaml.WindowActivatedEventArgs args)
		{
			MauiWinUIApplication.Current.Services?.InvokeLifecycleEvents<WindowsLifecycle.OnActivated>(del => del(this, args));
		}

		protected virtual void OnClosed(object sender, UI.Xaml.WindowEventArgs args)
		{
			MauiWinUIApplication.Current.Services?.InvokeLifecycleEvents<WindowsLifecycle.OnClosed>(del => del(this, args));
		}

		protected virtual void OnVisibilityChanged(object sender, UI.Xaml.WindowVisibilityChangedEventArgs args)
		{
			MauiWinUIApplication.Current.Services?.InvokeLifecycleEvents<WindowsLifecycle.OnVisibilityChanged>(del => del(this, args));
		}
	}
}