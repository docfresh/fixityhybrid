using Android.App;
using Android.Content;
using Android.Net;
using Android.Webkit;
using System;
using System.IO;
using System.Net;

namespace CustomRenderer.Droid
{
    public class JavascriptWebViewClient : WebViewClient
    {
        string _javascript;

        public JavascriptWebViewClient(string javascript)
        {
            _javascript = javascript;
        }

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            view.EvaluateJavascript(_javascript, null);
        }


        //Decision method for when & how to override URLs in the webview.       
        [Obsolete("ShouldOverrideUrlLoading is deprecated in Android API 24 and above. See https://stackoverflow.com/questions/36484074/is-shouldoverrideurlloading-really-deprecated-what-can-i-use-instead")]
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            //Handles external links, to load them outside of the webview , in the system default browser.
            //Click on a georeferenced link
            if (url != null && url.Contains("download.wx"))
            {
                //Xamarin.Forms.Device.OpenUri(new Uri(url.AbsoluteString));
                //DownloadWiseJFile(view, url);
                var strFilename = DownloadAndWriteFile(view, url);
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    // Do things the API >= 24 way
                    OpenLocalPdf(view, strFilename, view.Context);
                }
                else
                {
                    // Do things the old way
                    OpenLocalPdfClassic(view, strFilename);
                }
                
                return true;
            }
            else if (url != null && url.StartsWith("https://fixity.io"))
            {
                //var geoUri = Android.Net.Uri.Parse(url);
                //var mapIntent = new Intent(Intent.ActionView, geoUri);
                //view.Context.StartActivity(mapIntent);
                //return true;

                //Click on a generic link
                //view.LoadUrl(url);
                //return true;

                return false; //The correct way to continue loading a given URL is to simply return false, without calling WebView.loadUrl(String).
            }
            else if (url!= null && (url.StartsWith("mailto:") || url.StartsWith("tel:") || url.StartsWith("sms:") || url.StartsWith("geo:")))
            {
                var geoUri = Android.Net.Uri.Parse(url);
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                view.Context.StartActivity(mapIntent);
                return true; //abort loading in the webview.
            }
            else if (url!= null && url.Contains("facebook.com") && (url.Contains("dialog/oauth?") || url.Contains("login.php")))
            {
                //do a facebook login or say that we cant.
                Xamarin.Forms.DependencyService.Get<IMessage>().LongAlert("Facebook login does not work yet on hybrid app.");
                return true;  //abort loading in the webview.
                //return false;//give embedded oauth a chance.
            }
            else if (url != null && url.Contains("facebook.com")) 
            {
                //facebook share dialog.
                //Xamarin.Forms.DependencyService.Get<IMessage>().ShortAlert("Facebook login not yet working on hybrid app.");
                //return false;// return true;  //abort loading in the webview.

                var fbUri = Android.Net.Uri.Parse(url);
                var fbIntent = new Intent(Intent.ActionView, fbUri);
                //fbIntent.AddFlags(ActivityFlags.)
                view.Context.StartActivity(fbIntent);
                return true; //abort loading in the webview.

            }
            else
            {
                var geoUri = Android.Net.Uri.Parse(url);
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                view.Context.StartActivity(mapIntent);
                return true; //abort loading in the webview.
            }


            //Click on a generic link
            //view.LoadUrl(url);
            return false;
        }

        //Open a filename directly from the web. Not for Wisej download.wx links.
        public void OpenWebPDFfile(WebView view, string url)
        {            
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse(url), "application/pdf");
            try
            {
                view.Context.StartActivity(intent);
            } catch
            {
                //user does not have a pdf viewer installed

            }
            //return true
        }

        public void DownloadWiseJFile(WebView view, string url)
        {
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse(url), "*/*");
            try
            {
                view.Context.StartActivity(intent);
            }
            catch
            {
                //user does not have a pdf viewer installed

            }
            //return true
        }

        //Download file with WebClient, parse filename from header, write to downloads directory (not async yet!)
        public string DownloadAndWriteFile(WebView view, string url)
        {

            Xamarin.Forms.DependencyService.Get<IMessage>().ShortAlert("Downloading...");

            //var pathFile = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var pathFile = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var absolutePath = pathFile.AbsolutePath;
            var pathToNewFolder = absolutePath + "/Fixity";           
            Directory.CreateDirectory(pathToNewFolder);


            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Completed);
                var folder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Fixity";

                //DOWNLOAD THE FILE
                //webClient.DownloadFileAsync(new System.Uri(url), folder + "file.pdf");
                //webClient.DownloadFile(new System.Uri(url), pathToNewFolder + "/file.pdf");
                //OpenPdf(view, pathToNewFolder + "/file.pdf");

                //DOWNLOAD when we dont know the original filename: https://stackoverflow.com/questions/20492355/get-original-filename-when-downloading-with-webclient
                var data = webClient.DownloadData(new System.Uri(url));
                string fileName = "";
                // Try to extract the filename from the Content-Disposition header
                if (!String.IsNullOrEmpty(webClient.ResponseHeaders["Content-Disposition"]))
                {
                    fileName = webClient.ResponseHeaders["Content-Disposition"].Substring(webClient.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", "");
                }

                if (fileName == "") { fileName = "issue.pdf"; }
                fileName = System.IO.Path.Combine(pathToNewFolder, fileName);

                using (var fileOutputStream = new Java.IO.FileOutputStream(fileName))
                {
                    fileOutputStream.Write(data);
                }

                return fileName;

                //OpenPdf(view, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                return "";
            }
        }

        private void Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.Error.Message);
        }


        //opens a local pdf file in native viewer, for API 23 and lower
        public void OpenLocalPdfClassic(WebView view, string filePath)
        {
            Android.Net.Uri uri = Android.Net.Uri.Parse("file:///" + filePath);
            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(uri, "application/pdf");
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            try
            {
                view.Context.StartActivity(intent);
            }
            catch (System.Exception ex)
            {
                //Toast.MakeText(Xamarin.Forms.Forms.Context, "No Application Available to View PDF", ToastLength.Short).Show();
                Console.WriteLine("error: " + ex.Message);
            }
        }


        //open local file, API 24 and above
        public void OpenLocalPdf(WebView view, string filePath, Context context)
        {



            //Android.Net.Uri uri = Android.Net.Uri.Parse("file:///" + filePath);
            //Android.Net.Uri uri = Android.Net.Uri.FromFile()


            // Android.Net.Uri pdfPath = Android.Net.Uri.FromFile(new Java.IO.File(filePath));
            Android.Net.Uri pdfPath = Android.Support.V4.Content.FileProvider.GetUriForFile(context, "com.fresh.fixityio.fileprovider", new Java.IO.File(filePath));


            Intent intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(pdfPath, "application/pdf");
            intent.SetFlags(ActivityFlags.ClearWhenTaskReset | ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);

            try
            {
                view.Context.StartActivity(intent);
            }
            catch (System.Exception ex)
            {
                //Toast.MakeText(Xamarin.Forms.Forms.Context, "No Application Available to View PDF", ToastLength.Short).Show();
                Console.WriteLine("error: " + ex.Message);
            }


        }


    }



}
