using System;
using System.IO;
using CustomRenderer;
using CustomRenderer.iOS;
using Foundation;
using ObjCRuntime;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Net;



[assembly: ExportRenderer (typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace CustomRenderer.iOS
{
	public class HybridWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>, IWKScriptMessageHandler
	{
		const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
        WKUserContentController userController;


        protected override void OnElementChanged (ElementChangedEventArgs<HybridWebView> e)
		{
			base.OnElementChanged (e);

			if (Control == null) {
				userController = new WKUserContentController ();
				var script = new WKUserScript (new NSString (JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
				userController.AddUserScript (script);
				userController.AddScriptMessageHandler (this, "invokeAction");

				var config = new WKWebViewConfiguration { UserContentController = userController };
				var webView = new WKWebView (Frame, config);

                webView.Configuration.Preferences.JavaScriptEnabled = true;
                webView.Configuration.Preferences.JavaScriptCanOpenWindowsAutomatically = true; //allow application.navigatye in wisej

                //https://forums.xamarin.com/discussion/9791/set-a-top-margin-for-a-webview-in-ios
                
                //webView.ScrollView.ContentInset = new UIKit.UIEdgeInsets(40, 0,0 , 0);
                //webView.ScrollView.BackgroundColor = UIKit.UIColor.Clear;
                //webView.BackgroundColor = UIKit.UIColor.Clear;

                SetNativeControl(webView);

                webView.NavigationDelegate = new CustomWebViewDelegate();
                //webView.
			}
			if (e.OldElement != null) {
				userController.RemoveAllUserScripts ();
				userController.RemoveScriptMessageHandler ("invokeAction");
				var hybridWebView = e.OldElement as HybridWebView;
				hybridWebView.Cleanup ();

                

            }
            if (e.NewElement != null) {
                //string fileName = Path.Combine (NSBundle.MainBundle.BundlePath, string.Format ("Content/{0}", Element.Uri));
                //Control.LoadRequest (new NSUrlRequest (new NSUrl (fileName, false)));
                //Control.LoadRequest(new NSUrlRequest(new NSUrl("https://fixity.io/?hybrid=1"))); //force load of fixity.io 
                Control.LoadRequest(new NSUrlRequest(new NSUrl(e.NewElement.Uri))); //force load of fixity.io 
            }

            Control.Configuration.Preferences.JavaScriptEnabled = true;
            Control.Configuration.Preferences.JavaScriptCanOpenWindowsAutomatically = true; //allow application.navigatye in wisej
            //Control.Configuration.Preferences.

        }

        public void DidReceiveScriptMessage (WKUserContentController userContentController, WKScriptMessage message)
		{
			Element.InvokeAction (message.Body.ToString ());
		}

        internal class CustomWebViewDelegate : WKNavigationDelegate
        {

            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
               // base.DecidePolicy(webView, navigationAction, decisionHandler);
               
                if (navigationAction.NavigationType == WKNavigationType.LinkActivated) {
                    var url = navigationAction.Request.Url;
                    if (url.AbsoluteUrl.AbsoluteString.StartsWith("https://fixity.io")) //Whatever your test happens to be
                    {
                        decisionHandler(WKNavigationActionPolicy.Allow);
                    }
                    else if (url.AbsoluteUrl.AbsoluteString.StartsWith("mailto:"))
                    {
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        Device.OpenUri(new Uri(url.AbsoluteString)); //note: mail app not installed on simulator, must test with real device.
                       
                    }
                    else
                    {
                        //tel:, sms: , and URLs outside the fixity.io domain
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        //webView.LoadFileUrl()
                        Device.OpenUri(new Uri(url.AbsoluteString));
                    }


                }
                else if (navigationAction.NavigationType == WKNavigationType.Other)
                {
                    var url = navigationAction.Request.Url;
                    if (url.AbsoluteUrl.AbsoluteString.Contains("download.wx"))
                    {
                        //wisej download: cancel navigation and open in new Safari window.
                        //decisionHandler(WKNavigationActionPolicy.Cancel);
                        //Device.OpenUri(new Uri(url.AbsoluteString));

                        //Ideally, this would download the file to the users's home folder and then open it locally.
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                        //Xamarin.Forms.DependencyService.Get<IMessage>().ShortAlert("Downloading...");
                        var fileName = MyDownloader.DownloadAndWriteFile(url.AbsoluteString);
                        //Xamarin.Forms.DependencyService.Get<IMessage>().CancelAlert(); //kill alert so preview window can pop up in next call.
                        MyDownloader.OpenFileByName(fileName);
                    }
                    else
                    {
                        decisionHandler(WKNavigationActionPolicy.Allow);
                    }
                }
                else
                {
                    decisionHandler(WKNavigationActionPolicy.Allow);

                }

            }

        }





        //public override bool ShouldStartLoad(UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
        //{
        //    if (navigationType == UIWebViewNavigationType.LinkClicked)
        //    {
        //        UIApplication.SharedApplication.OpenUrl(request.Url);
        //        return false;
        //    }
        //    return true;
        //}



    }
}
