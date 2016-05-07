using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using KompetansetorgetXamarin.Droid;

//using XamarinTest.Droid;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;
using Notification = Android.App.Notification;

namespace KompetansetorgetXamarin.Droid
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class MyGcmListenerService : GcmListenerService
    {
        private NotificationsController nc;
        public MyGcmListenerService()
        {
            Log.Debug("MyGcmListenerService", "Before creation of NotificationsController");
            nc = new NotificationsController();
            Log.Debug("MyGcmListenerService", "After creation of NotificationsController");
        }

        /// <summary>
        /// Receives a Message from GCM. The method exctracts a message from the Bundle data,
        /// and calls SendNotification.
        /// 
        /// If the students have enabled receiveNotifications, but
        /// disabled receiveProjectNotifications and receiveJobNotifications
        /// The student can still receive more general messages sent as push notifications
        /// however these notifications will only be displayed once and won't be displayed in the
        /// notification list.
        /// </summary>
        public override async void OnMessageReceived(string from, Bundle data)
        {
            StudentsController sc = new StudentsController();
            var message = data.GetString("message");
            Log.Debug("MyGcmListenerService", "From:    " + from);
            Log.Debug("MyGcmListenerService", "Message: " + message);
            var type = data.GetString("type");
            var uuid = data.GetString("uuid");
            Student student = sc.GetStudent();
            if (student != null && student.receiveNotifications)
            {
                if (type == "project")
                {    
                    if (student.receiveProjectNotifications)  
                    { 
                        SendNotification(message);
                        Log.Debug("MyGcmListenerService", "type: " + type);
                        Log.Debug("MyGcmListenerService", "uuid: " + uuid);
                        // type = job or project          
            
                        Log.Debug("MyGcmListenerService", "After New NotificationController, but before use of method.");
                        nc.InsertNotification(type, uuid);         
                    }
                }
                else if (type == "job")
                {
                    if (student.receiveJobNotifications)
                    {
                        SendNotification(message);
                        Log.Debug("MyGcmListenerService", "type: " + type);
                        Log.Debug("MyGcmListenerService", "uuid: " + uuid);
                        // type = job or project          

                        Log.Debug("MyGcmListenerService", "After New NotificationController, but before use of method.");
                        nc.InsertNotification(type, uuid);
                    }
                }
                else
                {
                    SendNotification(message);
                }
            }
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
                .SetContentTitle("Kompetansetorget") // can probably be extracted from the Bundle the same way as message.
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