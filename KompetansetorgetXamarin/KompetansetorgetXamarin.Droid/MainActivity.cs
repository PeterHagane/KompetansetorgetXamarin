using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common;
using Android.Util;
//using KompetansetorgetXamarin.Views;
using Xamarin.Forms;
using PCLStorage;

namespace KompetansetorgetXamarin.Droid
{
    [Activity(Label = "KompetansetorgetXamarin", Icon = "@drawable/icon", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            // This check get run once every startup
            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                StartService(intent);
            }

        }

        /// <summary>
        /// This method checks that Google Play services are available on the device
        /// Without these available there is no point trying to obtain contact with GCM
        /// </summary>
        /// <returns>
        /// If Google play services available the method retuns true, if not false.
        /// </returns>
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Console.WriteLine(GoogleApiAvailability.Instance.GetErrorString(resultCode)); // SERVICE_MISSING
                }
                else
                {
                    Console.WriteLine("Sorry, this device is not supported");
                    Finish();
                }
                return false;
            }
            else
            {
                Console.WriteLine("Google Play Services is available.");
                return true;
            }
        }
    }
}

