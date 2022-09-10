using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;

using Android.Gms.Common.Apis;
using Android.Gms.Location;
using System.Threading.Tasks;
using Android;
//using Plugin.FacebookClient;

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
            askForPermission(this);
            //LoadApplication(new App());
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (grantResults[0] == Permission.Granted)
            {
                //Permission granted
                //var snack = Snackbar.Make(layout, "Location permission is available, getting lat/long.", Snackbar.LengthShort);
                //snack.Show();
                LoadApplication(new App());
            }
            else
            {
                //Permission Denied :(
                //Disabling location functionality
                //var snack = Snackbar.Make(layout, "Location permission is denied.", Snackbar.LengthShort);
                //snack.Show();
                    Xamarin.Forms.DependencyService.Get<IMessage>().LongAlert("Fixity may not work without proper permissions");
                    LoadApplication(new App());
                }
                    
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

        public async void askForPermission(MainActivity activity)
        {
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                // We have permission, go ahead and use the camera.
                Android.Util.Log.WriteLine(Android.Util.LogPriority.Debug, "Fixity.io", "Location permission is on");
                LoadApplication(new App());

            }
            else
            {
                // Camera permission is not granted. If necessary display rationale & request.
                //Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new System.String[] { Android.Manifest.Permission.AccessFineLocation }, REQUEST_LOCATION);
                await GetLocationPermissionAsync();
            }

        }

        async Task GetLocationPermissionAsync()
        {

            string[] PermissionsLocation =
              {
                  Manifest.Permission.AccessFineLocation, Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage,Manifest.Permission.WriteExternalStorage
                };

            const int RequestLocationId = 0;
            //Check to see if any permission in our group is available, if one, then all are
            const string permission = Manifest.Permission.AccessFineLocation;
            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                return;
            }

            //need to request permission
            //if (ShouldShowRequestPermissionRationale(permission))
            //{
                //Explain to the user why we need to read the contacts
                //Snackbar.Make(layout, "Location access is required to show coffee shops nearby.", Snackbar.LengthIndefinite)
                        //.SetAction("OK", v => RequestPermissions(PermissionsLocation, RequestLocationId))
                        //.Show();
            //    return;
            //}
            //Finally request permissions with the list of permissions and Id
            RequestPermissions(PermissionsLocation, RequestLocationId);
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

