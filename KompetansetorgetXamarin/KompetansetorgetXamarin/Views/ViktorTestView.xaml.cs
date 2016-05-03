using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.CRUD;
using KompetansetorgetXamarin.DAL;
using Xamarin.Forms;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.Controls;
using Newtonsoft.Json;
using PCLStorage;
using SQLiteNetExtensions.Extensions;


namespace KompetansetorgetXamarin.Views
{
    public partial class ViktorTestView : BaseContentPage
    {
        public ViktorTestView()
        {
            InitializeComponent();            
        }

        
        private async void StudentButton_OnClicked(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserver1.azurewebsites.net/api/v1/students");
            if (response.IsSuccessStatusCode) { 
                var results = await response.Content.ReadAsAsync<IEnumerable<Student>>();

            
                var sb = new StringBuilder();
                foreach (var student in results)
                {
                    var text = string.Format("Mail: {0}, Username: {1}", student.email, student.username);
                    sb.AppendLine(text);

                }
                TextBox.Text = sb.ToString();
            }
        }
        


        /*
        private void PathButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                TextBox.Text = rootFolder.Path;

             }
             catch (Exception ex)
            {
                TextBox.Text = ex.Message;
            }
        }

         private async void TokenButton_OnClicked(object sender, EventArgs e)
         {
             TokenHandler th = new TokenHandler();
             string token = await th.GetToken();
             TextBox.Text = token;
         }

         private async void DeviceIdButton_OnClicked(object sender, EventArgs e)
         {
             TokenHandler th = new TokenHandler();
             string deviceId = await th.GetDeviceId();
             TextBox.Text = deviceId;
         }
         */
         /*
        private async void GetJobsFromServer_OnClicked(object sender, EventArgs e)
        {
            //JobsController jc = new JobsController();
            // jc.GetJobsFromServer();

            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs");
            var results = await response.Content.ReadAsAsync<IEnumerable<Job>>();

            var sb = new StringBuilder();
            foreach (var job in results)
            {
                var id = new StringBuilder();
                foreach (var c in job.companies)
                {
                    id.AppendLine(c.id);
                }
             
                string[] s = new string[2];
                s[0] = "a";
                s[1] = "b";

                var text = string.Format("title: {0}, published: {1}, id: {2}", job.title, job.published,
                id.ToString());
                //job.companies.Select(c => new { c.id }));
                sb.AppendLine(text);

            }
            TextBox.Text = sb.ToString();

        }
        */
        private async void NotificationsFromDb_OnClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: Initiated");
            NotificationsController nc = new NotificationsController();
            nc.GetNotificationList();
        }

        /*
            NotificationsController nc = new NotificationsController();
            ProjectsController pc = new ProjectsController();
            JobsController jc = new JobsController();

            IEnumerable<Notification> notifications = nc.GetNotifications();

            var sb = new StringBuilder();

            foreach (var n in notifications)
            {
                System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var n.id = " + n.id);
                System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var n.jobUuid = " + n.jobUuid);
                System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var n.projectUuid = " + n.projectUuid);

                if (!string.IsNullOrWhiteSpace(n.jobUuid))
                {

                    Job j = jc.GetJobByUuid(n.jobUuid);
                    List<Company> jCompanies = jc.GetAllCompaniesRelatedToJob(j);

                    System.Diagnostics.Debug.WriteLine(
                    "ViktorTestView - NotificationsFromDb_OnClicked: companies.Count = " + jCompanies.Count());
                    j.companies = jCompanies;
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var n.projectUuid = " + n.projectUuid);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.uuid = " + j.uuid);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.title = " + j.title);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.webpage = " + j.webpage);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: p.companies.Count = " + j.companies.Count);

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.companies[0].name = " + j.companies[0].name);
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.companies[0].logo " + j.companies[0].logo);

                        var text = string.Format("title: {0}, company: {1}, published: {2}, webpage: {3}, logo: {4}, expiryDate: {5}",
                            j.title, j.companies[0].name, j.published,
                            j.webpage, j.companies[0].logo, j.expiryDate);


                        //job.companies.Select(c => new { c.id }));
                        sb.AppendLine(text);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: Exception msg: " + ex.Message);
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: Stack Trace: \n" + ex.StackTrace);
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: End Of Stack Trace");
                    }

                }

                else
                {
                    Project p = pc.GetProjectByUuid(n.projectUuid);
                    List<Company> pCompanies = pc.GetAllCompaniesRelatedToProject(p);
                    System.Diagnostics.Debug.WriteLine(
                        "ViktorTestView - NotificationsFromDb_OnClicked: companies.Count = " + pCompanies.Count());
                    p.companies = pCompanies;
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var n.projectUuid = " + n.projectUuid);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.uuid = " + p.uuid);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.title = " + p.title);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.webpage = " + p.webpage);
                    System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: p.companies.Count = " + p.companies.Count);
        
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.companies[0].name = " + p.companies[0].name);
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: var p.companies[0].logo " + p.companies[0].logo);

                        //var text = string.Format("title: {0}, published: {1}, webpage: {2}", p.title,  p.published, p.webpage);
                        
                        var text = string.Format("title: {0}, company: {1}, published: {2}, webpage: {3}, logo: {4}",
                            p.title, p.companies[0].name, p.published,
                            p.webpage, p.companies[0].logo);
                        //job.companies.Select(c => new { c.id }));
                        
                        sb.AppendLine(text);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: Exception msg: " + ex.Message);
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: Stack Trace: \n" + ex.StackTrace);
                        System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: End Of Stack Trace");
                    }
                } 

             


            }
            TextBox.Text = sb.ToString();
            */

        private async void GetAllJobsFromWebApi_OnClicked(object sender, EventArgs e)
        {
            JobsController jc = new JobsController();
            IEnumerable<Job> jobs = await jc.GetJobsBasedOnFilter(this);
            if (!Authorized)
            {
                GoToLogin();
            }
            if (jobs != null)
            {
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs.Count(): " +
                                   jobs.Count());
            }
        }

        private async void GetAllProjectsFromWebApi_OnClicked(object sender, EventArgs e)
        {
            ProjectsController jc = new ProjectsController();
            IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter(this);
            if (!Authorized)
            {
                GoToLogin();
            }
            if (projects != null)
            {
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs.Count(): " +
                                   projects.Count());
            }
        }

        private async void TestJobsFilterWeb_OnClicked(object sender, EventArgs e)
        {
            List<string> studyGroups = new List<string>();
            studyGroups.Add("helse");
            studyGroups.Add("datateknologi");
            //LEGG FAGOMRÅDER TIL AKTIVT FILTER HER MED EN SWITCH ELLER LIGNENDE


            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("locations", "vestagder");
            filter.Add("types", "heltid");

            JobsController jc = new JobsController();
            IEnumerable<Job> jobs = await jc.GetJobsBasedOnFilter(this, studyGroups, "-published", filter);

            if (!Authorized)
            {
                GoToLogin();
            }
            if (jobs != null) { 
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs.Count(): " +
                                   jobs.Count());
                foreach (var job in jobs)
                {
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].id);
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].name);
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].logo);
                }
            }
        }

        private async void TestGetStudyGroupStudent_OnClicked(object sender, EventArgs e)
        {
            StudentsController sc = new StudentsController();
            List<StudyGroup> list = await sc.GetStudentsStudyGroupFromServer(this);
            // checks if still Authorized 
            if (!Authorized)
            {
                System.Diagnostics.Debug.WriteLine("TestGetStudyGroupStudent inside If: Authorized:  " + Authorized);
                GoToLogin();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("TestGetStudyGroupStudent inside Else: Authorized:  " + Authorized);
                if (list == null)
                {
                    System.Diagnostics.Debug.WriteLine("Server communication failed");
                } 

                System.Diagnostics.Debug.WriteLine("Studygroups.Count: " + list.Count);

                foreach (var sg in list)
                {
                    System.Diagnostics.Debug.WriteLine("id: " + sg.id);
                    System.Diagnostics.Debug.WriteLine("name: " + sg.name);
                }
            }
        }

        /// <summary>
        /// Peter ikke se på denne metoden, den er kun for å teste databasen :D
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TestJobsFilterDb_OnClicked(object sender, EventArgs e)
        {
            List<string> studyGroups = new List<string>();
            studyGroups.Add("helse");
            studyGroups.Add("datateknologi");

            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("locations", "vestagder");
            filter.Add("types", "heltid");


            JobsController jc = new JobsController();
            var jobs = jc.GetJobsFromDbBasedOnFilter(null, filter);

            if (jobs == null)
            {
                System.Diagnostics.Debug.WriteLine("TestJobsFilterDb:  was null aka failed!");
            }
            else { 
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs.Count(): " +
                                       jobs.Count());

                foreach (var job in jobs)
                {
                    System.Diagnostics.Debug.WriteLine("Jobs title: " + job.title);
                    if (job.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].id);

                    }
            }
            }

            var jobs2 = jc.GetJobsFromDbBasedOnFilter(studyGroups);
            
            if (jobs2 == null)
            {
                System.Diagnostics.Debug.WriteLine("TestJobsFilterDb:  was null aka failed!");
            }
            else { 
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs2.Count(): " +
                                       jobs2.Count());

                foreach (var job in jobs2)
                {
                    System.Diagnostics.Debug.WriteLine("Jobs2 title: " + job.title);
                    if (job.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].id);
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].name);
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].logo);
                    }
                }
            }

            var jobs3 = jc.GetJobsFromDbBasedOnFilter(studyGroups, filter);

            if (jobs3 == null)
            {
                System.Diagnostics.Debug.WriteLine("TestJobsFilterDb:  was null aka failed!");
            }
            else { 
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs3.Count(): " +
                                       jobs3.Count());

                foreach (var job in jobs3)
                {
                    System.Diagnostics.Debug.WriteLine("Jobs3 title: " + job.title);
                    if (job.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].id);

                    }
                }
            }

            Dictionary<string, string> filter2 = new Dictionary<string, string>();
            filter2.Add("titles", "Database ansvarlig");

            var jobs4 = jc.GetJobsFromDbBasedOnFilter(studyGroups, filter2);

            if (jobs4 == null)
            {
                System.Diagnostics.Debug.WriteLine("TestJobsFilterDb:  was null aka failed!");
            }
            else { 
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs4.Count(): " +
                                       jobs4.Count());

                foreach (var job in jobs4)
                {
                    System.Diagnostics.Debug.WriteLine("Jobs4 title: " + job.title);
                    if (job.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + job.companies[0].id);

                    }
                }
            }
        }


        private async void TestProjectsFilter_OnClicked(object sender, EventArgs e)
        {
            List<string> studyGroups = new List<string>();
            studyGroups.Add("helse");
            studyGroups.Add("datateknologi");
            //LEGG FAGOMRÅDER TIL AKTIVT FILTER HER MED EN SWITCH ELLER LIGNENDE


            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("courses", "DAT-304");
            filter.Add("types", "virksomhet");

            ProjectsController jc = new ProjectsController();
            IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter(this, studyGroups, "-published", filter);

            if (!Authorized)
            {
                GoToLogin();
            }
            if (projects != null) { 
                System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter: projects.Count(): " +
                                    projects.Count());
                foreach (var project in projects)
                {
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].id);
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].name);
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].logo);
                }
            }
        }

        /// <summary>
        /// Peter ikke se på denne metoden, den er kun for å teste databasen :D
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TestProjectsFilterDb_OnClicked(object sender, EventArgs e)
        {
            List<string> studyGroups = new List<string>();
            studyGroups.Add("helse");
            studyGroups.Add("datateknologi");
            //LEGG FAGOMRÅDER TIL AKTIVT FILTER HER MED EN SWITCH ELLER LIGNENDE


            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("courses", "DAT-304");
            filter.Add("types", "virksomhet");

            ProjectsController pc = new ProjectsController();
            var projects = pc.GetProjectsFromDbBasedOnFilter(null, filter);

            if (projects == null)
            {
                System.Diagnostics.Debug.WriteLine("TestProjectsFilterDb:  was null aka failed!");
            }

            else { 
                System.Diagnostics.Debug.WriteLine("TestProjectsFilterDb: projects.Count(): " +
                                       projects.Count());

                foreach (var project in projects)
                {
                    System.Diagnostics.Debug.WriteLine("projects title: " + project.title);
                    if (project.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].id);

                    }
                }
            }

            var projects2 = pc.GetProjectsFromDbBasedOnFilter(studyGroups);

            if (projects2 == null)
            {
                System.Diagnostics.Debug.WriteLine("TestProjectsFilterDb:  was null aka failed!");
            }
            else { 
                System.Diagnostics.Debug.WriteLine("GetProjectsFromDbBasedOnFilter: projects2.Count(): " +
                                       projects2.Count());

                foreach (var project in projects2)
                {
                    System.Diagnostics.Debug.WriteLine("project2 title: " + project.title);
                    if (project.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].id);
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].name);
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].logo);

                    }
                }
            }

            var projects3 = pc.GetProjectsFromDbBasedOnFilter(studyGroups, filter);

            if (projects3 == null)
            {
                System.Diagnostics.Debug.WriteLine("TestProjectsFilterDb:  was null aka failed!");
            }
            else { 
                System.Diagnostics.Debug.WriteLine("GetProjectsFromDbBasedOnFilter: projects3.Count(): " +
                                       projects3.Count());

                foreach (var project in projects3)
                {
                    System.Diagnostics.Debug.WriteLine("projects3 title: " + project.title);
                    if (project.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].id);

                    }
                }
            }
            
            Dictionary<string, string> filter2 = new Dictionary<string, string>();
            filter2.Add("titles", "Strømavleser vha gammel mobil");

            var projects4 = pc.GetProjectsFromDbBasedOnFilter(studyGroups, filter2);

            if (projects4 == null)
            {
                System.Diagnostics.Debug.WriteLine("TestProjectsFilterDb:  was null aka failed!");
            }
            else { 
                System.Diagnostics.Debug.WriteLine("GetProjectsFromDbBasedOnFilter: projects4.Count(): " +
                                       projects4.Count());

                foreach (var project in projects4)
                {
                    System.Diagnostics.Debug.WriteLine("projects4 title: " + project.title);
                    if (project.companies != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].id);

                    }
                }
            }
        }

        private async void TestPostStudentsStudyGroupToServer(object sender, EventArgs e)
        {
            List<StudyGroup> studyGroups = new List<StudyGroup>();
            StudyGroup idrettsfag = new StudyGroup();
            idrettsfag.id = "idrettsfag";
            StudyGroup datateknologi = new StudyGroup();
            datateknologi.id = "datateknologi";
            studyGroups.Add(idrettsfag);
            studyGroups.Add(datateknologi);
            StudyGroup samfunnsfag = new StudyGroup();
            samfunnsfag.id = "samfunnsfag";
            StudyGroup realfag = new StudyGroup();
            realfag.id = "realfag";
            //studyGroups.Add(samfunnsfag);
            //studyGroups.Add(realfag);




            StudentsController sc = new StudentsController();
            bool success = await sc.PostStudentsStudyGroupToServer(this, studyGroups);

            System.Diagnostics.Debug.WriteLine(
                "TestPostStudentsStudyGroupToServer: PostStudentsStudyGroupToServer success: " + success);
        }
    }
}