using System;
using GtkSharp.BlazorWebKit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Handlers;
using WebViewWidget = WebKit.WebView;

#pragma warning disable RS0016

namespace Microsoft.AspNetCore.Components.WebView.Maui
{

	/// <summary>
	/// A <see cref="ViewHandler"/> for <see cref="BlazorWebView"/>.
	/// </summary>
	public partial class BlazorWebViewHandler : ViewHandler<IBlazorWebView, WebViewWidget>
	{

		private WebViewManager? _webviewManager;

		/// <inheritdoc />
		protected override WebViewWidget CreatePlatformView()
		{
			var native = new WebViewWidget();

			return native;
		}

		/// <inheritdoc />
		public virtual IFileProvider CreateFileProvider(string contentRootDir) => throw new NotSupportedException();

		/// <inheritdoc />
		protected override void DisconnectHandler(WebViewWidget platformView)
		{
			if (_webviewManager != null)
			{
				// Dispose this component's contents and block on completion so that user-written disposal logic and
				// Blazor disposal logic will complete.
				_webviewManager?
				   .DisposeAsync()
				   .AsTask()
				   .GetAwaiter()
				   .GetResult();

				_webviewManager = null;
			}
		}

		private void StartWebViewCoreIfPossible()
		{
			if (!RequiredStartupPropertiesSet || _webviewManager != null)
			{
				return;
			}

			if (PlatformView == null)
			{
				throw new InvalidOperationException($"Can't start {nameof(BlazorWebView)} without platform web view instance.");
			}
			
			// We assume the host page is always in the root of the content directory, because it's
			// unclear there's any other use case. We can add more options later if so.
			var contentRootDir = System.IO.Path.GetDirectoryName(HostPage!) ?? string.Empty;
			var hostPageRelativePath = System.IO.Path.GetRelativePath(contentRootDir, HostPage!);

			var fileProvider = VirtualView.CreateFileProvider(contentRootDir);

			Uri missingAppBaseUri = new Uri(contentRootDir);
			_webviewManager = new GtkWebViewManager(
				this.PlatformView,
				new BlazorWebViewOptions(),
				Services!,
				new MauiDispatcher(Services!.GetRequiredService<IDispatcher>()),
				missingAppBaseUri,
				fileProvider,
				VirtualView.JSComponents,
				hostPageRelativePath);

			StaticContentHotReloadManager.AttachToWebViewManagerIfEnabled(_webviewManager);

			VirtualView.BlazorWebViewInitializing(new BlazorWebViewInitializingEventArgs());
			VirtualView.BlazorWebViewInitialized(new BlazorWebViewInitializedEventArgs
			{
				WebView = PlatformView,
			});

			if (RootComponents != null)
			{
				foreach (var rootComponent in RootComponents)
				{
					// Since the page isn't loaded yet, this will always complete synchronously
					_ = rootComponent.AddToWebViewManagerAsync(_webviewManager);
				}
			}
			_webviewManager.Navigate("/");

		}

		bool RequiredStartupPropertiesSet => false;

	}

}