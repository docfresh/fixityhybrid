using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace CustomRenderer.Droid
{
    public class FileChooserWebChromeClient : WebChromeClient
    {
        private Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback;

        public FileChooserWebChromeClient(Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback)
        {
            callback = callback;
        }

        // For Android < 5.0
        [Java.Interop.Export]
        public void openFileChooser(IValueCallback uploadMsg, Java.Lang.String acceptType, Java.Lang.String capture)
        {
            callback(uploadMsg, acceptType, capture);
        }

        // For Android > 5.0
        public override Boolean OnShowFileChooser(Android.Webkit.WebView webView, IValueCallback uploadMsg, WebChromeClient.FileChooserParams fileChooserParams)
        {
            try
            {
                callback(uploadMsg, null, null);
            }
            catch (Exception)
            {
                //Log.
            }
            return true;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/12648099/making-facebook-login-work-with-an-android-webview
        /// </summary>
        /// <param name="view"></param>
        /// <param name="isDialog"></param>
        /// <param name="isUserGesture"></param>
        /// <param name="resultMsg"></param>
        /// <returns></returns>
        //public override bool OnCreateWindow(WebView view, bool isDialog, bool isUserGesture, Message resultMsg)
        //{
        //    WebView mWebviewPop = new WebView(Android.App.Application.Context);
        //    mWebviewPop.SetVerticalScrollbarOverlay(false);
        //    mWebviewPop.SetHorizontalScrollbarOverlay(false);
        //    mWebviewPop.SetWebViewClient(new WebViewClient());
        //    mWebviewPop.Settings.JavaScriptEnabled = true;
        //    mWebviewPop.Settings.SavePassword = true;
        //    mWebviewPop.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            
        //    mContainer.addView(mWebviewPop);
        //    WebView.WebViewTransport transport = (WebView.WebViewTransport)resultMsg.obj;
        //    transport.setWebView(mWebviewPop);
        //    resultMsg.sendToTarget();

        //    return true;

        //    'return base.OnCreateWindow(view, isDialog, isUserGesture, resultMsg);
        //}
    }

}