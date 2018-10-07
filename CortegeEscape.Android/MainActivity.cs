using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android;
using System.Threading;

namespace CortegeEscape.Droid
{
    [Activity(Label = "CortegeEscape", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int LocationPermissionsRequestId = 60419;
        private AutoResetEvent locationRequestWaitHandle = new AutoResetEvent(false);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Xamarin.FormsMaps.Init(this, savedInstanceState); // they told me to do so at https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/map

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode != LocationPermissionsRequestId) return;

            SharedProperties.IsLocationPermissionsGranted = grantResults[0] == Permission.Granted;

            locationRequestWaitHandle.Set();
        }

        private void RequestLocationPermissions()
        {
            // a guy here https://stackoverflow.com/questions/48487333/location-permission-for-android-above-6-0-with-xamarin-forms-maps
            // suggests this is a solution for asking access for location
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(
                    this, 
                    new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 
                        LocationPermissionsRequestId);
            }
            else
            {
                SharedProperties.IsLocationPermissionsGranted = true;
                locationRequestWaitHandle.Set();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt > 22)
            {
                RequestLocationPermissions();
                locationRequestWaitHandle.WaitOne();
            }
            else
            {
                SharedProperties.IsLocationPermissionsGranted = true;
            }


        }
    }
}