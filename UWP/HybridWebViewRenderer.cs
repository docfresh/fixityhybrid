using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomRenderer;
using CustomRenderer.UWP;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;

[assembly:ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace CustomRenderer.UWP
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Windows.UI.Xaml.Controls.WebView>
    {
        const string JavaScriptFunction = "function invokeCSharpAction(data){window.external.notify(data);}";

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new Windows.UI.Xaml.Controls.WebView());
            }
            if (e.OldElement != null)
            {
                Control.NavigationCompleted -= OnWebViewNavigationCompleted;
                Control.PermissionRequested += OnPermissionRequested;
                Control.NewWindowRequested += OnNewWindowRequested;
                Control.NavigationStarting += OnNavigationStarting;
                Control.ScriptNotify -= OnWebViewScriptNotify;
            }
            if (e.NewElement != null)
            {
                Control.NavigationCompleted += OnWebViewNavigationCompleted;
                Control.PermissionRequested += OnPermissionRequested;
                Control.NewWindowRequested  += OnNewWindowRequested;
                Control.NavigationStarting += OnNavigationStarting;
                Control.ScriptNotify += OnWebViewScriptNotify;
                //Control.Source = new Uri(string.Format("ms-appx-web:///Content//{0}", Element.Uri));
                Control.Source = new Uri("https://fixity.io/?hybrid=1");
            }
        }

        async void OnWebViewNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                // Inject JS script
                await Control.InvokeScriptAsync("eval", new[] { JavaScriptFunction });
            }
        }

        void OnWebViewScriptNotify(object sender, NotifyEventArgs e)
        {
            Element.InvokeAction(e.Value);
        }

        void OnPermissionRequested(WebView sender, WebViewPermissionRequestedEventArgs args)
        {
            if (args.PermissionRequest.PermissionType == WebViewPermissionType.Geolocation) 
            {
                args.PermissionRequest.Allow();
            }
            if (args.PermissionRequest.PermissionType == WebViewPermissionType.Media)
            {
                args.PermissionRequest.Allow();
            }

        }

        //https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.webview.newwindowrequested
        void OnNewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {

        }

        //https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.webview.navigationstarting
        void OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            //https://stackoverflow.com/questions/49431603/download-pdf-to-localfolder-in-uwp

            string url = args.Uri.ToString();
            if (url.ToLower().Contains("download.wx"))
            {
                //wisej download: cancel navigation and open in new Safari window.
                //decisionHandler(WKNavigationActionPolicy.Cancel);
                //Device.OpenUri(new Uri(url.AbsoluteString));

                //Ideally, this would download the file to the users's home folder and then open it locally.
                args.Cancel = true; //cancel navigation , we are handling it.
                //Xamarin.Forms.DependencyService.Get<IMessage>().ShortAlert("Downloading...");
                var fileName = MyDownloader.DownloadAndWriteFile(url);
                //Xamarin.Forms.DependencyService.Get<IMessage>().CancelAlert(); //kill alert so preview window can pop up in next call.
                MyDownloader.OpenFileByNameAsync(fileName);
            }

        }

    }
}
