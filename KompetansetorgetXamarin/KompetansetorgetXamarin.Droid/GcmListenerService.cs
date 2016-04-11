using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using KompetansetorgetXamarin.Controllers;
using XamarinTest.Droid;

namespace KompetansetorgetXamarin.Droid
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {

        /// <summary>
        /// Receives a Message from GCM. The method exctracts a message from the Bundle data,
        /// and calls SendNotification
        /// </summary>
        public override async void OnMessageReceived(string from, Bundle data)
        {
            var message = data.GetString("message");
            Log.Debug("MyGcmListenerService", "From:    " + from);
            Log.Debug("MyGcmListenerService", "Message: " + message);
            SendNotification(message);

            var type = data.GetString("type");
            var uuid = data.GetString("uuid");
            Log.Debug("MyGcmListenerService", "type: " + type);
            Log.Debug("MyGcmListenerService", "uuid: " + uuid);
            // type = job or project          
            NotificationsController nc = new NotificationsController();
            Log.Debug("MyGcmListenerService", "After New NotificationController, but before use of method.");
            nc.InsertNotification(type, uuid);
        }

        /// <summary>
        /// This method builds a local notifaction and lauches it to the user with NotificationManager.
        /// </summary>
        /// <param name="message">The message in the Notification</param>
        void SendNotification(string message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentTitle("GCM Message") // can probably be extracted from the Bundle the same way as message.
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate | NotificationDefaults.Lights)
                //.SetLights(16737792, 300, 100)
                .SetContentIntent(pendingIntent);

            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}