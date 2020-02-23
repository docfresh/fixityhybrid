using CustomRenderer;
using CustomRenderer.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Webkit;
using Android.App;
using System;
using Java.IO;
using Android.Provider;


[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace CustomRenderer.Droid
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        const string JavascriptFunction = "function invokeCSharpAction(data){jsBridge.invokeAction(data);}";
        Context _context;
        Activity mContext;
        private static int FILECHOOSER_RESULTCODE = 1;


        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
            this.mContext = context as Activity;
        }


        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new Android.Webkit.WebView(mContext);
                webView.Settings.JavaScriptEnabled = true;
                webView.SetWebViewClient(new JavascriptWebViewClient($"javascript: {JavascriptFunction}"));
                SetNativeControl(webView);

                MessagingCenter.Subscribe<object>(this, "GoBack", (sender) => {
                    //if (webView.CanGoBack() == true)
                    //{
                    //    webView.GoBack();
                    //}
                    //webView.EvaluateJavascript("App.HitFormBackButtonIfPossible();", new BackButtonAttemptCallback()); //handle the back button behavior we want in WiseJ.
                    webView.EvaluateJavascript("App.HitFormBackButtonIfPossible(function(result) { });", new BackButtonAttemptCallback()); //handle the back button behavior we want in WiseJ.
                    
                    
                    //webView.EvaluateJavascript("eval(3);", new BackButtonAttemptCallback()); //handle the back button behavior we want in WiseJ.

                });

                MessagingCenter.Subscribe<object>(this, "QuitApp", (sender) => {

                    Java.Lang.JavaSystem.Exit(0);//quit
                });

            }
            if (e.OldElement != null)
            {
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                Control.AddJavascriptInterface(new JSBridge(this), "jsBridge");
                //Control.LoadUrl($"file:///android_asset/Content/{Element.Uri}");
                Control.LoadUrl($"{Element.Uri}");
            }

            Control.ClearCache(false);
            Control.Settings.JavaScriptEnabled = true;
            Control.Settings.SetGeolocationEnabled(true);
            Control.Settings.DomStorageEnabled = true;
            //set permissions true to debug application.navigate bug
            //Control.Settings.AllowContentAccess = true;
            //Control.Settings.AllowFileAccess = true;
            //Control.Settings.AllowUniversalAccessFromFileURLs = true;
            //Control.Settings.AllowFileAccessFromFileURLs = true;
            Control.Settings.JavaScriptCanOpenWindowsAutomatically = true; //needed to allow WiseJ to call Application.Navigate() because it sets location.href. may also need target = _system
            Control.Settings.CacheMode = CacheModes.Default;
            //Control.Settings.


            var chromeClient = new FileChooserWebChromeClient((uploadMsg, acceptType, capture) => {
                MainActivity.UploadMessage = uploadMsg;

                // Create MyAppFolder at SD card for saving our images
                File imageStorageDir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(
                        Android.OS.Environment.DirectoryPictures), "MyAppFolder");

                if (!imageStorageDir.Exists())
                {
                    imageStorageDir.Mkdirs();
                }

                // Create camera captured image file path and name, add ticks to make it unique 
                var file = new File(imageStorageDir + File.Separator + "IMG_"
                    + DateTime.Now.Ticks + ".jpg");

                MainActivity.mCapturedImageURI = Android.Net.Uri.FromFile(file);

                // Create camera capture image intent and add it to the chooser
                var captureIntent = new Intent(MediaStore.ActionImageCapture);
                captureIntent.PutExtra(MediaStore.ExtraOutput, MainActivity.mCapturedImageURI);

                var i = new Intent(Intent.ActionGetContent);
                i.AddCategory(Intent.CategoryOpenable);
                i.SetType("image/*");

                var chooserIntent = Intent.CreateChooser(i, "Choose image");
                chooserIntent.PutExtra(Intent.ExtraInitialIntents, new Intent[] { captureIntent });

                mContext.StartActivityForResult(chooserIntent, FILECHOOSER_RESULTCODE);
            });

            Control.SetWebChromeClient(chromeClient);
            //Control.SetWebChromeClient(new MyWebClient(mContext));
        }

        public class MyWebClient : WebChromeClient
        {
            Activity mContext;
            public MyWebClient(Activity context)
            {
                this.mContext = context;
            }

            public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback geocallback)
            {
                geocallback.Invoke(origin, true, false);
            }

            public override void OnPermissionRequest(PermissionRequest request)
            {
                request.Grant(request.GetResources());

                //mContext.RunOnUiThread(() => {
                //    request.Grant(request.GetResources());
                //});
            }
        }


        public class FileChooserWebChromeClient : WebChromeClient
        {
            private Action<IValueCallback, Java.Lang.String, Java.Lang.String> _callback;

            public FileChooserWebChromeClient(Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback)
            {
                _callback = callback;
            }

            // For Android < 5.0
            [Java.Interop.Export]
            public void openFileChooser(IValueCallback uploadMsg, Java.Lang.String acceptType, Java.Lang.String capture)
            {
                _callback(uploadMsg, acceptType, capture);
            }

            // For Android > 5.0
            public override Boolean OnShowFileChooser(Android.Webkit.WebView webView, IValueCallback uploadMsg, WebChromeClient.FileChooserParams fileChooserParams)
            {
                try
                {
                    _callback(uploadMsg, null, null);
                }
                catch (Exception ex)
                {
                    var str =ex.Message;
                }
                return true;
            }

            //allow geo-location on Chrome
            public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback geocallback)
            {
                geocallback.Invoke(origin, true, false);
            }




        }
    }
}

