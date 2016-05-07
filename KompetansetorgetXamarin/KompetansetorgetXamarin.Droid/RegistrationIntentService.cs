using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using KompetansetorgetXamarin.Controllers;

namespace KompetansetorgetXamarin.Droid
{
    [Service(Exported = false)]
    class RegistrationIntentService : IntentService
    {
        static object locker = new object();

        public RegistrationIntentService() : base("RegistrationIntentService") { }
        
        /// <summary>
        /// Requests a unique token/registration Id for the specific projectId registered
        /// on Google Developers Console
        /// </summary>
        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var projectId = "976073814766";
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(
                        projectId, GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);

                    SendRegistrationToAppServer(token);
                    Subscribe(token);


                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }

        /// <summary>
        /// An implementation of this method can be used to send the token to the server so 
        /// that the token can be registered to a specific user. 
        /// </summary>
        /// <param name="token">The registration id of the app device provided by GCM</param>
        void SendRegistrationToAppServer(string token)
        {
            DevicesController dc = new DevicesController();
            Log.Debug("SendRegistrationToAppServer", "token");
            //TokenHandler th = new TokenHandler();
            string aSerial = Android.OS.Build.Serial;
            //th.CheckToken(token, aSerial);
            dc.InsertOrUpdateDevice(token, aSerial, "android");
        }


        /// <summary>
        /// Subscribes to the topics global, global can be changed to any thing, the
        /// only thing that is important is that server sends the message under the same topics.
        /// </summary>
        /// <param name="token">Registration id of the app device provided by GCM</param>
        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global", null);
        }

    }
}