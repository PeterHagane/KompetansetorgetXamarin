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


        private async void TestJobsStudyGroup_OnClicked(object sender, EventArgs e)
        {
            List<string> studyGroups = new List<string>();
            studyGroups.Add("helse");
            studyGroups.Add("datateknologi");
            //LEGG FAGOMRÅDER TIL AKTIVT FILTER HER MED EN SWITCH ELLER LIGNENDE


            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("locations", "vestagder");

            JobsController jc = new JobsController();
            Task<IEnumerable<Job>> jobs = jc.GetJobsBasedOnFilter(studyGroups, "-published", filter);
            IEnumerable<Job> pro = jobs.Result;
            System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: projects.Count(): " +
                                   pro.Count());
        }

        private async void TestGetStudyGroupStudent_OnClicked(object sender, EventArgs e)
        {
            StudentsController sc = new StudentsController();
            await sc.UpdateStudyGroupStudent(this);
            if (!Authorized)
            {
                System.Diagnostics.Debug.WriteLine("TestGetStudyGroupStudent inside If: Authorized:  " + Authorized);
                GoToLogin();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("TestGetStudyGroupStudent inside Else: Authorized:  " + Authorized);
                var list = sc.GetAllStudyGroupsRelatedToStudent(sc.GetStudent());
                System.Diagnostics.Debug.WriteLine("Studygroups.Count: " + list.Count);

                foreach (var sg in list)
                {
                    System.Diagnostics.Debug.WriteLine("id: " + sg.id);
                    System.Diagnostics.Debug.WriteLine("name: " + sg.name);
                }
            }

        }
        
    }
}