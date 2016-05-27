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
    public class NotificationsController : BaseController
    {

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
        public List<Advert> GetNotificationList()
        {
            DbJob dbJob = new DbJob();
            DbNotification db = new DbNotification();
            JobsController jc = new JobsController();
            DbProject dbProject = new DbProject();
            IEnumerable<Notification> notifications = db.GetNotifications();

            List<Advert> notificationList = new List<Advert>();

            foreach (var n in notifications)
            {
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.id = " + n.id);
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.jobUuid = " + n.jobUuid);
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.projectUuid = " + n.projectUuid);

                if (!string.IsNullOrWhiteSpace(n.jobUuid))
                {

                    Job job = dbJob.GetJobByUuid(n.jobUuid);
                    job.companies = dbJob.GetAllCompaniesRelatedToJob(job);
                    notificationList.Add(job);
                }
                else
                {
                    Project project = dbProject.GetProjectByUuid(n.projectUuid);
                    project.companies = dbProject.GetAllCompaniesRelatedToProject(project);
                    notificationList.Add(project);
                }
            }
            return notificationList.OrderByDescending(a => a.published).ToList();
        }

        public List<object> GetNotificationListJobOnly()
        {
            DbJob dbJob = new DbJob();
            DbNotification db = new DbNotification();
            IEnumerable<Notification> notifications = db.GetNotifications();

            List<object> notificationList = new List<object>();

            foreach (var n in notifications)
            {
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.id = " + n.id);
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.jobUuid = " + n.jobUuid);
                if (!string.IsNullOrWhiteSpace(n.jobUuid))
                {

                    Job job = dbJob.GetJobByUuid(n.jobUuid);
                    job.companies = dbJob.GetAllCompaniesRelatedToJob(job);
                    notificationList.Add(job);
                }
            }
            return notificationList;
        }

        public List<object> GetNotificationListProjectOnly()
        {
            DbNotification db = new DbNotification();
            DbProject dbProject = new DbProject();
            IEnumerable<Notification> notifications = db.GetNotifications();
            List<object> notificationList = new List<object>();
            foreach (var n in notifications)
            {
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.id = " + n.id);
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.jobUuid = " + n.jobUuid);
                System.Diagnostics.Debug.WriteLine("GetNotificationList: var n.projectUuid = " + n.projectUuid);

                if (!string.IsNullOrWhiteSpace(n.projectUuid))
                {
                    Project project = dbProject.GetProjectByUuid(n.projectUuid);
                    project.companies = dbProject.GetAllCompaniesRelatedToProject(project);
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
