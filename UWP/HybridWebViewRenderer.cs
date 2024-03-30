using System;
using Xamarin.Forms.Platform.UWP;
using CustomRenderer;
using CustomRenderer.UWP;
using Xamarin.Forms;
using Windows.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
// Ensure you've added the Microsoft.Web.WebView2 NuGet package to the UWP project.
[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]

namespace CustomRenderer.UWP
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Microsoft.UI.Xaml.Controls.WebView2>
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.chrome.webview.postMessage(data);}";

        protected override async void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new Microsoft.UI.Xaml.Controls.WebView2();
                await webView.EnsureCoreWebView2Async();
                SetNativeControl(webView);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe from old element events
                Control.CoreWebView2.WebMessageReceived -= OnWebMessageReceived;
            }

            if (e.NewElement != null)
            {
                // Initialize or re-initialize
                Control.Source = new Uri("https://fixity.io/?hybrid=1");
                Control.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(JavaScriptFunction);
                Control.CoreWebView2.WebMessageReceived += OnWebMessageReceived;

                // Implement additional setup or event subscription as needed
            }
        }

        private void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            // Handle messages received from web content
            var message = args.TryGetWebMessageAsString();
            Element?.InvokeAction(message);
        }

        // Additional methods and event handlers as necessary
    }
}
