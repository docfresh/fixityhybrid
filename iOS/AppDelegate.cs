using Foundation;
//using KeyboardOverlap.Forms.Plugin.iOSUnified;
using UIKit;

namespace CustomRenderer.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			LoadApplication (new App ());

            //KeyboardOverlapRenderer.Init(KeyboardOverlapRenderer.OverlapType.Collapse);


            return base.FinishedLaunching (app, options);
		}
	}
}

