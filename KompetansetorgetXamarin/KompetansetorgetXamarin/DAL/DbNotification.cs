using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.Models;
using SQLiteNetExtensions.Extensions;

namespace KompetansetorgetXamarin.DAL
{
    public class DbNotification : DbBase
    {
        /// <summary>
        /// Get all stored notifications
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Notification> GetNotifications()
        {
            lock (DbContext.locker)
            { // ORDER BY Job.published DESC
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
        /// Inserts a notification into the database.
        /// </summary>
        /// <param name="type">This param can either be job or project, and will determine what kind of notification it is.</param>
        /// <param name="typeUuid">the foreign key of type</param>
        public async Task InsertNotification(string type, string typeUuid)
        {
            System.Diagnostics.Debug.WriteLine("DbNotification - InsertNotification: initiated");

            Notification notification = new Notification();
            System.Diagnostics.Debug.WriteLine("DbNotification - InsertNotification: type: " + type);
            System.Diagnostics.Debug.WriteLine("DbNotification - InsertNotification: typeUuid: " + typeUuid);

            if (String.IsNullOrEmpty(typeUuid) || String.IsNullOrEmpty(type))
            {
                return;
            }

            if (type.Equals("job"))
            {
                // FIRST !!
                // insert a new job object matching the uuid as long as it doesn't exist
                System.Diagnostics.Debug.WriteLine("DbNotification - InsertNotification: new JobsController();");
                DbJob dbJob = new DbJob();
                JobsController jc = new JobsController();

                System.Diagnostics.Debug.WriteLine(
                    "DbNotification - InsertNotification: JobsController Created");

                dbJob.InsertJob(typeUuid);

                // SECOND insert the notification.
                notification.jobUuid = typeUuid;
                lock (DbContext.locker)
                {
                    Db.Insert(notification);
                }
                //THIRD: async get extra minimum info for the notification list.
                jc.UpdateJobFromServer(typeUuid);

            }

            else if (type.Equals("project"))
            {
                // FIRST !!
                // insert a new project object matching the uuid as long as it doesn't exist
                System.Diagnostics.Debug.WriteLine(
                    "DbNotification - InsertNotification: new ProjectsController();");
                ProjectsController pc = new ProjectsController();
                DbProject dbProject = new DbProject();
                System.Diagnostics.Debug.WriteLine(
                    "DbNotification - InsertNotification: ProjectsController Created");

                dbProject.InsertProject(typeUuid);

                // SECOND insert the notification.
                notification.projectUuid = typeUuid;
                lock (DbContext.locker)
                {
                    Db.Insert(notification);
                }
                //THIRD: async get extra minimum info for the notification list.
                pc.UpdateProjectFromServer(typeUuid);
            }
            System.Diagnostics.Debug.WriteLine("DbNotification - InsertNotification: Test: End of method");
        }

        /// <summary>
        /// Delete a Notification based on projectUuid, which is a foreign key to a row in Project.
        /// This method will not effect the row in the Project table.
        /// </summary>
        /// <param name="uuid"></param>
        public void DeleteNotificationBasedOnProject(string uuid)
        {
            lock (DbContext.locker)
            {
                Db.Execute("DELETE FROM Notification " +
                                       "WHERE Notification.projectUuid = ?", uuid);
            }
        }

        /// <summary>
        /// Delete a Notification based on jobUuid, which is a foreign key to a row in Job.
        /// This method will not effect the row in the Job table.
        /// </summary>
        /// <param name="uuid"></param>
        public void DeleteNotificationBasedOnJob(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("DbNotification - DeleteNotificationBasedOnJob: Trying to delete notification based on jobUuid: " + uuid);
            lock (DbContext.locker)
            {
                Db.Execute("DELETE FROM Notification " +
                           "WHERE Notification.jobUuid = ?", uuid);
            }
        }

        /// <summary>
        /// Delete all Notification.
        /// This method will not effect the row in the Job table.
        /// </summary>
        public void DeleteAllNotifications()
        {
            System.Diagnostics.Debug.WriteLine("DbNotification - DeleteNotificationBasedOnJob: Trying to delete all notifications");
            lock (DbContext.locker)
            {
                Db.Execute("DELETE FROM Notification");
            }
        }
    }
}
