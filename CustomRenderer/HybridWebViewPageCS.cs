using System;

using Xamarin.Forms;

namespace CustomRenderer
{
	public class HybridWebViewPageCS : ContentPage
	{
		public HybridWebViewPageCS (string theUri)
		{
			var hybridWebView = new HybridWebView {
                //Uri = "https://fixity.io/?hybrid=1",
                Uri = theUri, 
                //Uri = "https://mobilehtml5.org/ts/?id=23",
                //Uri = "https://freshsoftware.com/camera.html",
                HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			hybridWebView.RegisterAction (data => DisplayAlert ("Alert", "Hello " + data, "OK"));

			Padding = new Thickness (0, 2, 0, 0);
			Content = hybridWebView;
		}

        //https://stackoverflow.com/questions/30657344/how-to-handle-cancel-back-navigation-in-xamarin-forms


	}
}
