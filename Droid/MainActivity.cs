using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;

using Android.Gms.Common.Apis;
using Android.Gms.Location;


namespace CustomRenderer.Droid
{
    [Activity(Label = "Fixity.io", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static IValueCallback UploadMessage;
        private static int FILECHOOSER_RESULTCODE = 1;
        public static Android.Net.Uri mCapturedImageURI;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            turnOnGps(this);
            LoadApplication(new App());
        }


        //This procedure calls 
        //not sure if im calling this right. 
        //1) is it OK to pass in mainactivity like this?
        //2) any way to get a callback after they enable? (ie, to reload webview)
        //3) any way to delay the initial load of the webview until they hit the OK button to load fixity?
        //https://forums.xamarin.com/discussion/118189/gps-location-enable-in-xamarin-forms
        //https://stackoverflow.com/questions/33251373/turn-on-location-services-without-navigating-to-settings-page
        //https://stackoverflow.com/questions/43138788/ask-user-to-turn-on-location/43139125
        //https://forums.xamarin.com/discussion/140325/how-to-check-every-time-for-gps-connectivity (code based on this)
        public async void turnOnGps(MainActivity activity)
        {
            try
            {
                //MainActivity activity = Xamarin.Forms.Context as MainActivity;
                GoogleApiClient googleApiClient = new GoogleApiClient.Builder(activity)
                    .AddApi(LocationServices.API).Build();
                googleApiClient.Connect();
                LocationRequest locationRequest = LocationRequest.Create();
                locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
                locationRequest.SetInterval(10000);
                locationRequest.SetFastestInterval(10000 / 2);

                LocationSettingsRequest.Builder
                        locationSettingsRequestBuilder = new LocationSettingsRequest.Builder()
                        .AddLocationRequest(locationRequest);
                locationSettingsRequestBuilder.SetAlwaysShow(false);
                LocationSettingsResult locationSettingsResult = await LocationServices.SettingsApi.CheckLocationSettingsAsync(
                    googleApiClient, locationSettingsRequestBuilder.Build());

                if (locationSettingsResult.Status.StatusCode == LocationSettingsStatusCodes.ResolutionRequired)
                {
                    locationSettingsResult.Status.StartResolutionForResult(activity, 0);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Xamarin.Forms.DependencyService.Get<IMessage>().LongAlert(ex.Message); //show error
            }
        }



        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            if (requestCode == FILECHOOSER_RESULTCODE)
            {
                if (null == UploadMessage)
                    return;
                Java.Lang.Object result = data == null ? mCapturedImageURI : data.Data;

                UploadMessage.OnReceiveValue(new Android.Net.Uri[] { (Android.Net.Uri)result });
                UploadMessage = null;
            }
            else
                base.OnActivityResult(requestCode, resultCode, data);
        }


    }
}

