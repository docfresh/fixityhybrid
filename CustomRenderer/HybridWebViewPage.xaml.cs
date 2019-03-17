using Xamarin.Forms;

namespace CustomRenderer
{
	public partial class HybridWebViewPage : ContentPage
	{
		public HybridWebViewPage (string theURI)
		{
			InitializeComponent ();
            hybridWebView.Uri = theURI;
			hybridWebView.RegisterAction (data => DisplayAlert ("Alert", "Hello " + data, "OK"));
		}
	}
}
