using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Foundation;
using UIKit;

namespace CustomRenderer.iOS
{
    class MyDownloader
    {
        //Download file with WebClient, parse filename from header, write to downloads directory (not async yet!)
        static public string DownloadAndWriteFile(string url)
        {

            //Xamarin.Forms.DependencyService.Get<IMessage>().ShortAlert("Downloading...");//this stops the previewpanel from showing... comment out.

            var pathFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var absolutePath = pathFile;
            var pathToNewFolder = absolutePath;// + "/Fixity";
            Directory.CreateDirectory(pathToNewFolder);


            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Completed);
                //var folder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Fixity";

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

                File.WriteAllBytes(fileName, data);

                //using (var fileOutputStream = new FileOutputStream(fileName))
                //{
                //    fileOutputStream.Write(data);
                //}

                return fileName;

                //OpenPdf(view, fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                return "";
            }
        }

        static private void Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine("ERROR: " + e.Error.Message);
        }

        static public void OpenFileByName(string fileName)
        {
            try
            {
                //var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //var filePath = Path.Combine(documentsPath, fileName);
                var filePath = fileName;


                //Device.OpenUri(new Uri(filePath));
                var PreviewController = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(filePath));
                PreviewController.Delegate = new UIDocumentInteractionControllerDelegateClass(UIApplication.SharedApplication.KeyWindow.RootViewController);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    PreviewController.PresentPreview(true);
                });


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }


}