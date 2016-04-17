using System;
using System.Collections;
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
        /// Inserts a notification into the database.
        /// </summary>
        /// <param name="type">This param can either be job or project, and will determine what kind of notification it is.</param>
        /// <param name="typeUuid">the foreign key of type</param>
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

            if (type.Equals("job"))
            {
                // FIRST !!
                // insert a new job object matching the uuid as long as it doesn't exist
                System.Diagnostics.Debug.WriteLine("NotificationsController - InsertNotification: new JobsController();");
                JobsController jc = new JobsController();
                System.Diagnostics.Debug.WriteLine(
                    "NotificationsController - InsertNotification: JobsController Created");

                jc.InsertJob(typeUuid);

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
                    "NotificationsController - InsertNotification: new ProjectsController();");
                ProjectsController pc = new ProjectsController();
                System.Diagnostics.Debug.WriteLine(
                    "NotificationsController - InsertNotification: ProjectsController Created");

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
            System.Diagnostics.Debug.WriteLine("NotificationsController - DeleteNotificationBasedOnJob: Trying to delete notification based on jobUuid: " + uuid);
            lock (DbContext.locker)
            {
                Db.Execute("DELETE FROM Notification " +
                           "WHERE Notification.jobUuid = ?", uuid);
            }
        }

        /// <summary>
        /// Gets all notifications stored in a collection as an 'object'.
        /// These notifications must NOT be mistaken as the Model class Notification.
        /// The objects can be of either Job or Project
        /// 
        /// Examples on how to use this list:
        /// List<object> notifications = GetNotificationList();
        /// for (n in notifications) {
        ///     if (n is Job) {
        ///         // DO spesific Job code 
        ///         Job job = (Job)n;    
        ///         string date = job.expiryDate; // Will work
        ///     }
        ///     else if (n is Project) {
        ///         // Do spesific Project  code.
        ///         Project p = (Project)n;
        ///     }  
        /// }
        /// </summary>
        /// <returns>A list of objects suitable for to be dislayed to the user as notifications</returns>
        public IEnumerable<object> GetNotificationList()
        {
            JobsController jc = new JobsController();
            ProjectsController pc = new ProjectsController();
            IEnumerable<Notification> notifications = GetNotifications();

            List<object> notificationList = new List<object>();

            foreach (var n in notifications)
            {
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.id = " + n.id);
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.jobUuid = " + n.jobUuid);
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.projectUuid = " + n.projectUuid);

                if (!string.IsNullOrWhiteSpace(n.jobUuid))
                {

                    Job job = jc.GetJobByUuid(n.jobUuid);
                    job.companies = jc.GetAllCompaniesRelatedToJob(job);
                    notificationList.Add(job);
                }
                else
                {
                    Project project = pc.GetProjectByUuid(n.projectUuid);
                    project.companies = pc.GetAllCompaniesRelatedToProject(project);
                    notificationList.Add(project);
                }
            }
            return notificationList;
        }

        /*
            // 
            // TESTING AT 'is' (instanceof) PÅ EN KLASSE AV OBJECT funker til notification list 

            int j = 0;
            int p = 0;
            string jUuid = "";
            foreach (var no in notificationList)
            {

                if (no is Job)
                {
                    j++;
                    Job jo = (Job) no;
                    System.Diagnostics.Debug.WriteLine("Number of jobs: " + j);
                    System.Diagnostics.Debug.WriteLine(jo.uuid);
                    jUuid = jo.uuid;

                }

                else if (no is Project)
                {
                    p++;
                    Project pr = (Project) no;
                    System.Diagnostics.Debug.WriteLine("Number of projects: " + p);
                    System.Diagnostics.Debug.WriteLine(pr.uuid);


                }
            }
            System.Diagnostics.Debug.WriteLine("Totalt amounts of notifications: " + Db.Table<Notification>().Count());
            System.Diagnostics.Debug.WriteLine("Totalt amounts of jobs: " + Db.Table<Job>().Count());
            DeleteNotificationBasedOnJob(jUuid);
            System.Diagnostics.Debug.WriteLine("Totalt amounts of notifications: " + Db.Table<Notification>().Count());
            System.Diagnostics.Debug.WriteLine("Totalt amounts of jobs: " + Db.Table<Job>().Count());


            return notificationList;

        
        }
        */
    }
}
