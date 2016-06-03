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
using XLabs.Forms;

namespace KompetansetorgetXamarin.Droid
{                                                                           //MainLauncher = true
    [Activity(Label = "Kompetansetorget", Icon = "@drawable/icon", Theme = "@style/CustomActionBarTheme",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : XFormsApplicationDroid
    //upgraded to XForms, old inheritance is global::Xamarin.Forms.Platform.Android.FormsApplicationActivity

    {
        //App app;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            UAuth.Auth.auth = new UAuthImpl.Auth(this);
            //app = new App();
            LoadApplication(new App());
            // This check get run once every startup
            

            if (IsPlayServicesAvailable())
            {
                var intent = new Intent(this, typeof(RegistrationIntentService));
                StartService(intent);
            }

           // SetActionBar();

        }

       // protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
       // {
       //     app.SetNavPageToNotificationList();
       //}

        public void SetActionBar() {
            var upArrow = Resources.GetDrawable(Resource.Drawable.ArrowWhiteLeft);

            ActionBar.SetHomeAsUpIndicator(upArrow);
           
            //var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            //SetActionBar(toolbar);
            //ActionBar.SetIcon(Android.Resource.Color.Transparent);
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

