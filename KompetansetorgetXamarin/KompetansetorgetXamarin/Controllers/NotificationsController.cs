using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;

namespace KompetansetorgetXamarin.Controllers
{
    public class NotificationsController
    {
        private DbContext dbContext = DbContext.GetDbContext;
        private SQLiteConnection Db;

        public NotificationsController()
        {
            Db = dbContext.Db;
        }


        /// <summary>
        /// Get all stored notifications
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notification> GetNotifications()
        {
            lock (DbContext.locker)
            {
                return (from i in Db.Table<Notification>() select i).ToList();
            }
        }

        /// <summary>
        /// Get all stored notifications
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notification> GetNotificationsWithChildren()
        {
            lock (DbContext.locker)
            {
                return Db.GetAllWithChildren<Notification>();
            }
        }
       

        /// <summary>
        /// Get all stored notifications
        /// </summary>
        /// <returns></returns>
        public void InsertNotification(string type, string typeUuid)
        {
            System.Diagnostics.Debug.WriteLine("NotificationsController - InsertNotification: initiated");

            Notification notification = new Notification();
            System.Diagnostics.Debug.WriteLine("NotificationsController - InsertNotification: type: " + type); 
            System.Diagnostics.Debug.WriteLine("NotificationsController - InsertNotification: typeUuid: " + typeUuid);

            if (String.IsNullOrEmpty(typeUuid) || String.IsNullOrEmpty(type))
            {
                return;
            }
            // type is "" so both is false, but the method crashes before end of line.
            if (type.Equals("job"))
            {
                // FIRST insert a new job object matching the uuid as long as it doesn't exist
                // SECOND: insert the notification.
                notification.jobUuid = typeUuid;

                lock (DbContext.locker)
                {
                    Db.Insert(notification);
                }
                //THIRD: async get extra minimum info for the notification list.
            }

            else if (type.Equals("project"))
            {
                // FIRST !!
                // insert a new project object matching the uuid as long as it doesn't exist
                System.Diagnostics.Debug.WriteLine("NotificationsController - InsertNotification: new ProjectsController();");
                ProjectsController pc = new ProjectsController();
                System.Diagnostics.Debug.WriteLine("NotificationsController - InsertNotification: ProjectsController Created");

                pc.InsertProject(typeUuid);

                // SECOND insert the notification.
                notification.projectUuid = typeUuid;
                lock (DbContext.locker)
                {
                    Db.Insert(notification);
                }
                //THIRD: async get extra minimum info for the notification list.
                pc.UpdateProjectFromServer(typeUuid);

            }
            System.Diagnostics.Debug.WriteLine("NotificationsController - InsertNotification: Test: End of method");
        }


    }
}
