using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;

namespace KompetansetorgetXamarin.Droid
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    class MyInstanceIDListenerService : InstanceIDListenerService
    {

        /// <summary>
        /// Can be activated if there is a need for a new token. It will also repond to token refresh
        /// requests from GCM
        /// </summary>
        public override void OnTokenRefresh()
        {
            var intent = new Intent(this, typeof(RegistrationIntentService));
            StartService(intent);
        }
    }
}