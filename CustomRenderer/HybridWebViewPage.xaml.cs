﻿using Xamarin.Forms;

namespace CustomRenderer
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage (string theURI)
		{
			InitializeComponent ();
            hybridWebView.Uri = theURI;
			//hybridWebView.RegisterAction (data => DisplayAlert ("Alert", "Hello " + data, "OK"));
			hybridWebView.RegisterAction (data => ExecuteFromBrowser(data));
			

		}

		public void ExecuteFromBrowser(string data)
		{
			if (data == "quit")
			{
				MessagingCenter.Send<object>(this, "QuitApp");
			}

			if (data.StartsWith("fblogin:")) {
				int pos = data.IndexOf(":");
				string email = data.Substring(pos + 1);
				MessagingCenter.Send<object, string>(this, "fbauth", email);
			}
		}



		/// <summary>
		/// intercept the back button and push the message to the MessagingCenter, where it is later handled 
		/// </summary>
		/// <returns></returns>
		protected override bool OnBackButtonPressed()
		{
			MessagingCenter.Send<object>(this, "GoBack");
			return true;
		}

	}
}
