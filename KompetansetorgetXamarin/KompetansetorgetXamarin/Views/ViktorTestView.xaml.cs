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
using SQLite.Net;
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
            if (response.IsSuccessStatusCode)
            {
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
            List<object> notifications = nc.GetNotificationList();
            System.Diagnostics.Debug.WriteLine(
                "ViktorTestView - NotificationsFromDb_OnClicked: notifications.Count = " + notifications.Count);

            foreach (var n in notifications)
            {
                if (n is Job)
                {
                    // DO spesific Job code 
                    Job job = (Job) n;
                    //long date = job.expiryDate; // Will work
                    System.Diagnostics.Debug.WriteLine("job.title = " + job.title);
                    if (job.companies != null && job.companies[0].logo != null)
                    {
                        System.Diagnostics.Debug.WriteLine("job.companies.logo = " + job.companies[0].logo);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("job.companies = null");
                    }
                    System.Diagnostics.Debug.WriteLine("job.expiryDate = " + job.expiryDate);

                }
                else if (n is Project)
                {
                    // Do spesific Project  code.
                    Project project = (Project) n;
                    System.Diagnostics.Debug.WriteLine("project.title = " + project.title);
                    if (project.companies != null && project.companies[0].logo != null)
                    {
                        System.Diagnostics.Debug.WriteLine("project.companies.logo = " + project.companies[0].logo);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("project.companies = null");
                    }
                    System.Diagnostics.Debug.WriteLine("project.companies.logo = " + project.companies[0].logo);
                    System.Diagnostics.Debug.WriteLine("project.published = " + project.published);
                }
            }
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
            IEnumerable<Job> jobs = await jc.GetJobsBasedOnFilter();
            if (!Authenticater.Authorized)
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
            IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter();
            if (!Authenticater.Authorized)
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
            IEnumerable<Job> jobs = await jc.GetJobsBasedOnFilter(studyGroups, "-published", filter);

            if (!Authenticater.Authorized)
            {
                GoToLogin();
            }
            if (jobs != null)
            {
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
            List<StudyGroup> list = await sc.GetStudentsStudyGroupFromServer();
            // checks if still Authorized 
            if (!Authenticater.Authorized)
            {
                System.Diagnostics.Debug.WriteLine("TestGetStudyGroupStudent inside If: Authorized:  " +
                                                   Authenticater.Authorized);
                GoToLogin();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("TestGetStudyGroupStudent inside Else: Authorized:  " +
                                                   Authenticater.Authorized);
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
            else
            {
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
            else
            {
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
            else
            {
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
            else
            {
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
            /*
            StudyGroupsController sgc = new StudyGroupsController();
            sgc.UpdateStudyGroupsFromServer(); */
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
            IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter(studyGroups, "-published", filter);

            if (!Authenticater.Authorized)
            {
                GoToLogin();
            }
            if (projects != null)
            {
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

            else
            {
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
            else
            {
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
            else
            {
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
            else
            {
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
            bool success = await sc.PostStudentsStudyGroupToServer(studyGroups);

            System.Diagnostics.Debug.WriteLine(
                "TestPostStudentsStudyGroupToServer: PostStudentsStudyGroupToServer success: " + success);
        }

        private async void GetAllFilters(object sender, EventArgs e)
        {
            LocationsController lc = new LocationsController();
            CoursesController cc = new CoursesController();
            StudyGroupsController sgc = new StudyGroupsController();
            JobTypesController jtc = new JobTypesController();
            List<Location> locationsFilter = lc.GetAllLocations();
            List<Course> coursesFilter = cc.GetAllCourses();
            List<StudyGroup> studyGroupsFilter = sgc.GetAllStudyGroups();
            List<JobType> jobTypesJobFilter = jtc.GetJobTypeFilterJob();
            List<JobType> jobTypesProjectFilter = jtc.GetJobTypeFilterProject();

            System.Diagnostics.Debug.WriteLine("GetAllFilters: locationsFilter.Count: " + locationsFilter.Count);
            System.Diagnostics.Debug.WriteLine("GetAllFilters: coursesFilter.Count: " + coursesFilter.Count);
            System.Diagnostics.Debug.WriteLine("GetAllFilters: studyGroupsFilter.Count: " + studyGroupsFilter.Count);
            System.Diagnostics.Debug.WriteLine("GetAllFilters: jobTypesJobFilter.Count: " + jobTypesJobFilter.Count);
            System.Diagnostics.Debug.WriteLine("GetAllFilters: jobTypesProjectFilter.Count: " +
                                               jobTypesProjectFilter.Count);
        }

        /// <summary>
        /// This test method require that you have not logged in and got no authorization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TestDeleteExpiredJobs(object sender, EventArgs e)
        {
            JobsController jc = new JobsController();

            DbContext DbContext = DbContext.GetDbContext;
            SQLiteConnection Db = DbContext.Db;

            DateTime yesterday = DateTime.Now.AddDays(-1);
            long n = long.Parse(yesterday.ToString("yyyyMMddHHmmss"));

            string testUuid = "colemak";
            Job job = new Job()
            {
                uuid = testUuid,
                expiryDate = n
            };

            string companyId = "Ikea";
            Company comp = new Company()
            {
                id = companyId
            };

            string locationId = "sverige";
            Location loc = new Location()
            {
                id = locationId
            };

            string sgId = "dykking";
            StudyGroup sg = new StudyGroup()
            {
                id = sgId
            };

            StudyGroupJob sgj = new StudyGroupJob()
            {
                StudyGroupId = sgId,
                JobUuid = testUuid
            };

            LocationJob lj = new LocationJob()
            {
                LocationId = locationId,
                JobUuid = testUuid
            };

            CompanyJob cj = new CompanyJob()
            {
                CompanyId = companyId,
                JobUuid = testUuid
            };

            string jtId = "10aarErfaringEcma6";
            JobType jt = new JobType()
            {
                id = jtId
            };

            JobTypeJob jtj = new JobTypeJob()
            {
                JobUuid = testUuid,
                JobTypeId = jtId
            };

            Db.Insert(comp);
            Db.Insert(job);
            Db.Insert(loc);
            Db.Insert(sg);
            Db.Insert(sgj);
            Db.Insert(lj);
            Db.Insert(cj);
            Db.Insert(jt);
            Db.Insert(jtj);

            Job j = Db.Get<Job>(testUuid);
            System.Diagnostics.Debug.WriteLine("j.expiryDate: " + j.expiryDate);
            System.Diagnostics.Debug.WriteLine("StudyGroup.Count: " +
                                               Db.Query<StudyGroup>("Select * from StudyGroup").Count());
            System.Diagnostics.Debug.WriteLine("Job.Count: " +
                                               Db.Query<Job>("Select * from Job").Count());
            System.Diagnostics.Debug.WriteLine("JobType.Count: " +
                                               Db.Query<JobType>("Select * from JobType").Count());
            System.Diagnostics.Debug.WriteLine("Location.Count: " +
                                               Db.Query<Location>("Select * from Location").Count());
            System.Diagnostics.Debug.WriteLine("Company.Count: " +
                                               Db.Query<Company>("Select * from Company").Count());

            System.Diagnostics.Debug.WriteLine("CompanyJob.Count: " +
                                               Db.Query<CompanyJob>("Select * from CompanyJob").Count());
            System.Diagnostics.Debug.WriteLine("JobTypeJob.Count: " +
                                               Db.Query<JobTypeJob>("Select * from JobTypeJob").Count());
            System.Diagnostics.Debug.WriteLine("LocationJob.Count: " +
                                               Db.Query<LocationJob>("Select * from LocationJob").Count());
            System.Diagnostics.Debug.WriteLine("StudyGroupJob.Count: " +
                                               Db.Query<StudyGroupJob>("Select * from StudyGroupJob").Count());

            System.Diagnostics.Debug.WriteLine("Time for delete");
            jc.DeleteAllExpiredJobs();
            System.Diagnostics.Debug.WriteLine("Job.Count: " +
                                               Db.Query<Job>("Select * from Job").Count());
            System.Diagnostics.Debug.WriteLine("CompanyJob.Count: " +
                                               Db.Query<CompanyJob>("Select * from CompanyJob").Count());
            System.Diagnostics.Debug.WriteLine("JobTypeJob.Count: " +
                                               Db.Query<JobTypeJob>("Select * from JobTypeJob").Count());
            System.Diagnostics.Debug.WriteLine("LocationJob.Count: " +
                                               Db.Query<LocationJob>("Select * from LocationJob").Count());
            System.Diagnostics.Debug.WriteLine("StudyGroupJob.Count: " +
                                               Db.Query<StudyGroupJob>("Select * from StudyGroupJob").Count());
            //CompanyJobs, StudyGroupJob, LocationJob og JobTypeJob.

        }

        private async void TestNotificationRegister(object sender, EventArgs e)
        {
            DevicesController dc = new DevicesController();
            dc.UpdateServersDb();
        }

        private async void TestTurnOffNotifications(object sender, EventArgs e)
        {
            StudentsController sc = new StudentsController();
            sc.UpdateStudentsNotificationsPref(false, true, true);
        }

        private async void TestTurnOffProjectNotification(object sender, EventArgs e)
        {
            StudentsController sc = new StudentsController();
            sc.UpdateStudentsNotificationsPref(true, false, true);
        }

        private async void TestTurnOffJobNotification(object sender, EventArgs e)
        {
            StudentsController sc = new StudentsController();
            sc.UpdateStudentsNotificationsPref(true, true, false);
        }

        private async void TestInsertJob(object sender, EventArgs e)
        {
            DbContext DbContext = DbContext.GetDbContext;
            SQLiteConnection Db = DbContext.Db;

            System.Diagnostics.Debug.WriteLine("Before insert Job.Count: " +
                                               Db.Query<Job>("Select * from Job").Count());

            JobsController jc = new JobsController();
            DateTime yesterday = DateTime.Now.AddDays(-1);
            long n = long.Parse(yesterday.ToString("yyyyMMddHHmmss"));

            string testUuid = "colemak";
            Job job = new Job()
            {
                uuid = testUuid,
                expiryDate = n
            };

            string companyId = "Ikea";
            Company comp = new Company()
            {
                id = companyId
            };

            string locationId = "sverige";
            Location loc = new Location()
            {
                id = locationId
            };

            string sgId = "dykking";
            StudyGroup sg = new StudyGroup()
            {
                id = sgId
            };

            StudyGroupJob sgj = new StudyGroupJob()
            {
                StudyGroupId = sgId,
                JobUuid = testUuid
            };

            LocationJob lj = new LocationJob()
            {
                LocationId = locationId,
                JobUuid = testUuid
            };

            CompanyJob cj = new CompanyJob()
            {
                CompanyId = companyId,
                JobUuid = testUuid
            };

            string jtId = "10aarErfaringEcma6";
            JobType jt = new JobType()
            {
                id = jtId
            };

            JobTypeJob jtj = new JobTypeJob()
            {
                JobUuid = testUuid,
                JobTypeId = jtId
            };

            // try catch on tables that will not be affected by a job delete
            try
            {
                Db.Insert(comp);
            }
            catch
            {
            }
            Db.Insert(job);
            try
            {
                Db.Insert(loc);
            }
            catch
            {
            }
            try
            {
                Db.Insert(sg);
            }
            catch
            {
            }
            Db.Insert(sgj);
            Db.Insert(lj);
            Db.Insert(cj);
            try
            {
                Db.Insert(jt);
            }
            catch
            {
            }
            Db.Insert(jtj);

            System.Diagnostics.Debug.WriteLine("After insert: Job.Count: " +
                                               Db.Query<Job>("Select * from Job").Count());

        }

        private async void TestInsertProject(object sender, EventArgs e)
        {
            DbContext DbContext = DbContext.GetDbContext;
            SQLiteConnection Db = DbContext.Db;

            System.Diagnostics.Debug.WriteLine("Before insert Project.Count: " +
                                               Db.Query<Project>("Select * from Project").Count());

            ProjectsController jc = new ProjectsController();


            string testUuid = "colemak";
            Project project = new Project()
            {
                uuid = testUuid,
            };

            string companyId = "Ikea";
            Company comp = new Company()
            {
                id = companyId
            };

            string courseId = "sverige";
            Course course = new Course()
            {
                id = courseId
            };

            string sgId = "dykking";
            StudyGroup sg = new StudyGroup()
            {
                id = sgId
            };

            StudyGroupProject sgj = new StudyGroupProject()
            {
                StudyGroupId = sgId,
                ProjectUuid = testUuid
            };

            CourseProject lj = new CourseProject()
            {
                CourseId = courseId,
                ProjectUuid = testUuid
            };

            CompanyProject cp = new CompanyProject()
            {
                CompanyId = companyId,
                ProjectUuid = testUuid
            };

            string jtId = "10aarErfaringEcma6";
            JobType jt = new JobType()
            {
                id = jtId
            };

            JobTypeProject jtp = new JobTypeProject()
            {
                ProjectUuid = testUuid,
                JobTypeId = jtId
            };

            // try catch on tables that will not be affected by a job delete
            try
            {
                Db.Insert(comp);
            }
            catch
            {
            }
            Db.Insert(project);
            try
            {
                Db.Insert(course);
            }
            catch
            {
            }
            try
            {
                Db.Insert(sg);
            }
            catch
            {
            }
            Db.Insert(sgj);
            Db.Insert(lj);
            Db.Insert(cp);
            try
            {
                Db.Insert(jt);
            }
            catch
            {
            }
            Db.Insert(jtp);

            System.Diagnostics.Debug.WriteLine("After insert: Project.Count: " +
                                               Db.Query<Project>("Select * from Project").Count());
        }

        private async void TestPresentableDateTime(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now.AddDays(-1);
            long n = long.Parse(today.ToString("yyyyMMddHHmmss"));
            string date = DateTimeHandler.MakeDateString(n);
            string dateTime = DateTimeHandler.MakeDateTimeString(n);
            System.Diagnostics.Debug.WriteLine(n);
            System.Diagnostics.Debug.WriteLine("date: " + date);
            System.Diagnostics.Debug.WriteLine("dateTime: " + dateTime);
        }
    }
}