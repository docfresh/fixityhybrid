using Plugin.Connectivity;
using Xamarin.Forms;

namespace CustomRenderer
{
	public class App : Application
	{
		public App ()
		{
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.Application.SetWindowSoftInputModeAdjust(this, Xamarin.Forms.PlatformConfiguration.AndroidSpecific.WindowSoftInputModeAdjust.Resize);


            //MainPage = new HybridWebViewPage("https://fixity.io/?hybrid=1");
            

            if (!CrossConnectivity.Current.IsConnected)
            {
                //MainPage = new HybridWebViewPage("file:///android_asset/Content/offline.html");                
                //MainPage = new HybridWebViewPage("file:///android_asset/Content/offline.html");

                //string fileName = System.IO.Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("Content/{0}", "offline.html"));
                //Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));

                string fileName ="";

                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        string baseurl =  DependencyService.Get<IBaseUrl>().Get();

                        fileName = "file://"+baseurl + "/offline.html";
                        
                        break;
                    case Device.Android:
                        fileName = "file:///android_asset/Content/offline.html";
                        break;
                    case Device.UWP:
                    
                        break;
                }

                MainPage = new HybridWebViewPage(fileName);


            }
            else
            {
                MainPage = new HybridWebViewPage("https://fixity.io/?hybrid=1");
            }

            if (Device.RuntimePlatform.Equals(Device.Android))
            {

            }


            //


        }



        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}


	}
}

