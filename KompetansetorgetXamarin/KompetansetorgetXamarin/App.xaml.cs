using System;
using Xamarin.Forms;
using KompetansetorgetXamarin.Views;
using System.IO;
using System.Linq;
using KompetansetorgetXamarin.Controllers;
using PCLStorage;
using KompetansetorgetXamarin.CRUD;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using UAuth;

namespace KompetansetorgetXamarin
{
    public partial class App : Application
    {
        private static NavigationPage NavPage;

        public App()
        {
            InitializeComponent();   //must be included in order to initialise global xaml styles   
        }

        protected override void OnStart()
        {
            DbStudent dbStudent = new DbStudent();
            Student student = dbStudent.GetStudent();

            // Comment out the 4 lines under to deactive the GoToLogin at 
            if (student == null || student.accessToken == null)
            {
                Authenticater.Authorized = false;
            }

            NavPage = new NavigationPage(new MainPage());
            //NavPage = new NavigationPage(new LoginPage());
            MainPage = NavPage;
            
            //NavPage.BarBackgroundColor = Color.FromHex("ec7a08");
            //NavPage.BarTextColor = Color.White;

            if (student != null)
            {
                DeleteOutdatedData();
                UpdateAllFilters();
                DbDevice dbDevice = new DbDevice();
                if (dbDevice.GetDevice() != null && !dbDevice.GetDevice().tokenSent)
                {
                    DevicesController dc = new DevicesController();
                    dc.UpdateServersDb();
                }
            }
        }

        private void DeleteOutdatedData()
        {
            DbJob dbJob = new DbJob();
            dbJob.DeleteAllExpiredJobs();
        }

        private void UpdateAllFilters()
        {
            // This is to make sure that the app got the study groups that is used as search filters. 
            DbLocation dbLocation = new DbLocation();
            StudyGroupsController sgc = new StudyGroupsController();
            LocationsController lc = new LocationsController();
            JobTypesController jtc = new JobTypesController();
            CoursesController cc = new CoursesController();
            if (dbLocation.GetAllLocations().Count != 0)
            {

                lc.CompareServerHash();
                sgc.CompareServerHash();
                jtc.CompareServerHash();
                cc.CompareServerHash();
            }
            else
            {
                cc.UpdateCoursesFromServer();
                jtc.UpdateJobTypesFromServer();
                sgc.UpdateStudyGroupsFromServer();
                lc.UpdateLocationsFromServer();
            }
            
        }

         

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        /// <summary>
        /// Activate this method when a login is successful to navigate to a MainPage and remove the 
        /// Former pages for the Navigation.
        /// It will also update all the search filter for the app, and its crucial that they are in this 
        /// method and not just the startup method.
        /// </summary>
        public static void SuccessfulLoginAction()
        {
            // NavPage.Navigation.PopModalAsync();
            DbLocation dbLocation = new DbLocation();
            StudyGroupsController sgc = new StudyGroupsController();
            LocationsController lc = new LocationsController();
            JobTypesController jtc = new JobTypesController();
            CoursesController cc = new CoursesController();
            NavPage.Navigation.InsertPageBefore(new MainPage(), NavPage.Navigation.NavigationStack.First());
            NavPage.Navigation.PopToRootAsync();

            if (dbLocation.GetAllLocations().Count != 0)
            {
                lc.CompareServerHash();
                sgc.CompareServerHash();
                jtc.CompareServerHash();
                cc.CompareServerHash();
            }
            else
            {
                cc.UpdateCoursesFromServer();
                jtc.UpdateJobTypesFromServer();
                sgc.UpdateStudyGroupsFromServer();
                lc.UpdateLocationsFromServer();
            }
        }
    }
}

