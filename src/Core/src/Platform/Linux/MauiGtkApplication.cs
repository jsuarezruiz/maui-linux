﻿using System;
using System.Diagnostics;
using Gdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Gtk;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{

	public class MauiGtkApplication<TStartup> : MauiGtkApplication
		where TStartup : IStartup, new()
	{

		public void Run()
		{
			Launch(new EventArgs());
		}

		protected void RegisterLifecycleEvents(Gtk.Application app)
		{
			isfired = false;
#pragma warning disable 162
			if (true)
			{
				// if called after event-register, the events are not called
				// if called before event-register, the events are immidiatly called (to early)
				var c = new GLib.Cancellable();

				app.Register(c);
				Debug.WriteLine($"{nameof(GLib.Cancellable)}");
				isfired = true;

			}

#pragma warning restore 162

			app.Startup += OnStartup!;
			app.Shutdown += OnShutdown!;
			app.Opened += OnOpened;
			app.WindowAdded += OnWindowAdded;
			app.Activated += OnActivated!;
			app.WindowRemoved += OnWindowRemoved!;
			app.CommandLine += OnCommandLine!;

		}

#if DEBUG
		static bool isfired = false;
#endif
		protected void OnStartup(object sender, EventArgs args)
		{

#if DEBUG
			Debug.WriteLine($"{nameof(OnStartup)}");
			isfired = true;
#endif
			Services.InvokeLifecycleEvents<LinuxLifecycle.OnStartup>(del => del(CurrentGtkApplication, args));
		}

		protected void OnOpened(object o, GLib.OpenedArgs args)
		{
			Services?.InvokeLifecycleEvents<LinuxLifecycle.OnOpened>(del => del(CurrentGtkApplication, args));
		}

		protected void OnActivated(object sender, EventArgs args)
		{
			StartupMainWindow();
			StartupLauch(sender, args);

			Services?.InvokeLifecycleEvents<LinuxLifecycle.OnApplicationActivated>(del => del(CurrentGtkApplication, args));
		}

		protected void OnShutdown(object sender, EventArgs args)
		{
			Services?.InvokeLifecycleEvents<LinuxLifecycle.OnShutdown>(del => del(CurrentGtkApplication, args));
			// current is null, so this has no effect
			GLib.Cancellable.Current?.Cancel();

			MauiGtkApplication.DispatchPendingEvents();
			MauiGtkApplication.Invoke(() => Gtk.Application.Quit());
			;
		}

		protected void OnCommandLine(object o, GLib.CommandLineArgs args)
		{
			;
		}

		protected void OnWindowRemoved(object o, WindowRemovedArgs args)
		{
			;
		}

		protected void OnWindowAdded(object o, WindowAddedArgs args)
		{
			;
		}

		// https://developer.gnome.org/gio/stable/GApplication.html#g-application-id-is-valid
		// TODO: find a better algo for id
		public string ApplicationId => $"{typeof(TStartup).Namespace}.{typeof(TStartup).Name}.{base.Name}".PadRight(255, ' ').Substring(0, 255).Trim();

		Widget CreateRootContainer(Widget nativePage)
		{
			var b = new Box(Orientation.Vertical, 0)
			{
				Fill = true,
				Expand = true,
			};

			b.PackStart(nativePage, true, true, 0);

			return b;
		}

		protected void StartupLauch(object sender, EventArgs args)
		{
			var startup = new TStartup();

			var host = startup
			   .CreateAppHostBuilder()
			   .ConfigureServices(ConfigureNativeServices)
			   .ConfigureUsing(startup)
			   .Build();

			Services = host.Services;
			Application = Services.GetRequiredService<IApplication>();

			var mauiContext = new MauiContext(Services);

			var activationState = new ActivationState(mauiContext);
			var window = Application.CreateWindow(activationState);
			window.MauiContext = mauiContext;

			var content = (window.Page as IView) ?? window.Page.View;
			var nativeContent = content.ToNative(window.MauiContext);

			var canvas = TopContainerOverride?.Invoke(nativeContent) ?? CreateRootContainer(nativeContent);
			nativeContent.SetBackgroundColor(Colors.White);

			MainWindow.Child = canvas;
			MainWindow.QueueDraw();
			MainWindow.ShowAll();

			Services?.InvokeLifecycleEvents<LinuxLifecycle.OnLaunched>(del => del(CurrentGtkApplication, args));
		}

		void StartupMainWindow()
		{
			MainWindow = new MauiGtkMainWindow();
			CurrentGtkApplication.AddWindow(MainWindow);

		}

		protected void Launch(EventArgs args)
		{

			Gtk.Application.Init();
			var app = new Gtk.Application(ApplicationId, GLib.ApplicationFlags.None);

			RegisterLifecycleEvents(app);

			CurrentGtkApplication = app;

			Current = this;

			((GLib.Application)app).Run();

#pragma warning disable 162

			if (false) // if called before Gtk.Application.Run(), gtk_main_quit: assertion 'main_loops != NULL' failed
			{
				Services?.InvokeLifecycleEvents<LinuxLifecycle.OnLaunched>(del => del(CurrentGtkApplication, args));
			}
#pragma warning restore 162

			Gtk.Application.Run();

#if DEBUG
			if (!isfired)
			{
				Debug.WriteLine("lifecycle broken");
			}
#endif
		}

		protected void ConfigureNativeServices(HostBuilderContext ctx, IServiceCollection services)
		{ }

	}

	public abstract class MauiGtkApplication
	{

		/// <summary>
		/// overrides creation of rootcontainer
		/// rootcontainer is MainWindow 's <see cref="Gtk.Window.Child"/>
		/// paramter is Maui's Mainwindows <see cref="IWindow.Page"/> as Gtk.Widget
		/// </summary>
		public Func<Widget, Widget> TopContainerOverride { get; set; } = null!;

		protected MauiGtkApplication()
		{ }

		string? _name;

		// https://developer.gnome.org/gio/stable/GApplication.html#g-application-id-is-valid
		public string? Name
		{
			get => _name ??= $"A{Guid.NewGuid()}";
			set { _name = value; }
		}

		// https://developer.gnome.org/gtk3/stable/GtkApplication.html
		public static Gtk.Application CurrentGtkApplication { get; internal set; } = null!;

		public static MauiGtkApplication Current { get; internal set; } = null!;

		public MauiGtkMainWindow MainWindow { get; protected set; } = null!;

		public IServiceProvider Services { get; protected set; } = null!;

		public IApplication Application { get; protected set; } = null!;

		public static void DispatchPendingEvents()
		{
			// The loop is limited to 1000 iterations as a workaround for an issue that some users
			// have experienced. Sometimes EventsPending starts return 'true' for all iterations,
			// causing the loop to never end.

			int n = 1000;
#pragma warning disable 612
			Gdk.Threads.Enter();
#pragma warning restore 612

			while (Gtk.Application.EventsPending() && --n > 0)
			{
				Gtk.Application.RunIteration(false);
			}

#pragma warning disable 612
			Gdk.Threads.Leave();
#pragma warning restore 612
		}

		public static void Invoke(System.Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");

			// Switch to no Invoke(Action) once a gtk# release is done.
			Gtk.Application.Invoke((o, args) =>
			{
				action();
			});
		}

	}

}