using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using GtkSharpUpstream;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using WebKit;

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8604 // Possible null reference argument.

namespace GtkSharp.BlazorWebKit;

public partial class GtkWebViewManager : Microsoft.AspNetCore.Components.WebView.WebViewManager
{
	protected string _scheme;
	string _hostPageRelativePath;
	Uri _appBaseUri;

	public delegate void WebMessageHandler(IntPtr contentManager, IntPtr jsResult, IntPtr arg);

	public WebView WebView { get; protected set; }

	protected ILogger<GtkWebViewManager>? _logger;

	protected GtkWebViewManager(IServiceProvider provider, Dispatcher dispatcher, Uri appBaseUri, IFileProvider fileProvider, JSComponentConfigurationStore jsComponents, string hostPageRelativePath) :
		base(provider, dispatcher, appBaseUri, fileProvider, jsComponents, hostPageRelativePath)
	{
		_appBaseUri = appBaseUri;
		_hostPageRelativePath = hostPageRelativePath;
	}


	public GtkWebViewManager(
		WebView webView, string scheme,
		IServiceProvider provider, Dispatcher dispatcher, Uri appBaseUri,
		IFileProvider fileProvider,
		JSComponentConfigurationStore jsComponents, string hostPageRelativePath) : base(provider, dispatcher, appBaseUri, fileProvider, jsComponents, hostPageRelativePath)

	{
		_scheme = scheme;
		_hostPageRelativePath = hostPageRelativePath;
		_appBaseUri = appBaseUri;
		_logger = provider.GetService<ILogger<GtkWebViewManager>>();

		WebView = webView;
	}

	delegate bool TryGetResponseContentHandler(string uri, bool allowFallbackOnHostPage, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers);

	static readonly Dictionary<IntPtr, (string _hostPageRelativePath, TryGetResponseContentHandler tryGetResponseContent)> UriSchemeRequestHandlers = new();

	static bool HandleUriSchemeRequestIsRegistered = false;

	/// <summary>
	/// RegisterUriScheme can only called once per scheme
	/// so it's needed to have a list of all WebViews registered
	/// </summary>
	/// <param name="request"></param>
	/// <exception cref="Exception"></exception>
	static void HandleUriSchemeRequest(URISchemeRequest request)
	{
		if (!UriSchemeRequestHandlers.TryGetValue(request.WebView.Handle, out var uriSchemeHandler))
		{
			throw new Exception($"Invalid scheme \"{request.Scheme}\"");
		}


		var uri = request.Uri;

		if (request.Path == "/")
		{
			uri += uriSchemeHandler._hostPageRelativePath;
		}


		if (uriSchemeHandler.tryGetResponseContent(uri, false, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers))
		{
			using var inputStream = content.AsInputStream();
			request.Finish(inputStream, content.Length, headers["Content-Type"]);
		}
		else
		{
			throw new Exception($"Failed to serve \"{uri}\". {statusCode} - {statusMessage}");
		}
	}

	void RegisterUriSchemeRequestHandler()
	{
		if (!UriSchemeRequestHandlers.TryGetValue(WebView.Handle, out var uriSchemeHandler))
		{
			UriSchemeRequestHandlers.Add(WebView.Handle, (_hostPageRelativePath, TryGetResponseContent));
		}
	}

	protected override void NavigateCore(Uri absoluteUri)
	{
		_logger?.LogInformation($"Navigating to \"{absoluteUri}\"");
		var loadUri = absoluteUri.ToString();

		WebView.LoadUri(loadUri);
	}

	void SignalHandler(IntPtr contentManager, IntPtr jsResultHandle, IntPtr arg)
	{
		var jsResult = new global::WebKit.Upstream.JavascriptResult(jsResultHandle);

		var jsValue = jsResult.JsValue;

		if (!jsValue.IsString) return;

		var s = jsValue.ToString();

		if (s is not null)
		{
			_logger?.LogDebug($"Received message `{s}`");

			try
			{
				MessageReceived(_appBaseUri, s);
			}
			finally { }
		}
	}

	public string MessageQueueId = "webview";

	public string JsScript(string messageQueueId) =>
		"""
		window.__receiveMessageCallbacks = [];

		window.__dispatchMessageCallback = function(message) {
		   window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });
		};

		window.external = {
		   sendMessage: function(message) {
		"""
		+
		$"""
		        window.webkit.messageHandlers.{MessageQueueId}.postMessage(message);
		 """
		+
		"""
		   },
		   receiveMessage: function(callback) {
		       window.__receiveMessageCallbacks.push(callback);
		   }
		};
		""";

	global::WebKit.Upstream.UserScript _script;

	protected virtual void Attach()
	{
		if (!HandleUriSchemeRequestIsRegistered)
		{
			WebView.Context.RegisterUriScheme(AppHostScheme, HandleUriSchemeRequest);
			HandleUriSchemeRequestIsRegistered = true;
		}

		RegisterUriSchemeRequestHandler();

		var jsScript = JsScript(MessageQueueId);

		_script = new global::WebKit.Upstream.UserScript(
			jsScript,
			UserContentInjectedFrames.AllFrames,
			UserScriptInjectionTime.Start,
			null, null);

		WebView.UserContentManager.AddScript(_script);

		WebView.UserContentManager.SignalConnectData<WebMessageHandler>($"script-message-received::{MessageQueueId}",
			SignalHandler,
			IntPtr.Zero, IntPtr.Zero, (global::GLib.ConnectFlags)0);

		WebView.UserContentManager.RegisterScriptMessageHandler(MessageQueueId);
	}


	protected virtual void Detach()
	{
		WebView.Context.RemoveSignalHandler($"script-message-received::{MessageQueueId}", SignalHandler);
		WebView.UserContentManager.UnregisterScriptMessageHandler(MessageQueueId);
		WebView.UserContentManager.RemoveScript(_script);
	}

	protected override void SendMessage(string message)
	{
		_logger?.LogDebug($"Dispatching `{message}`");

		var script = $"__dispatchMessageCallback(\"{HttpUtility.JavaScriptStringEncode(message)}\")";

		WebView.RunJavascript(script);
	}

	protected override async ValueTask DisposeAsyncCore()
	{
		Detach();
		await base.DisposeAsyncCore();
	}
}