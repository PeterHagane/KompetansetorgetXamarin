using System;
using Xamarin.Forms;
using KompetansetorgetXamarin.Views;
using System.IO;
using System.Linq;
using KompetansetorgetXamarin.Controllers;
using PCLStorage;
using KompetansetorgetXamarin.CRUD;
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
            StudentsController sc = new StudentsController();
            Student student = sc.GetStudent();

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
                DevicesController dc = new DevicesController();
                if (dc.GetDevice() != null && !dc.GetDevice().tokenSent)
                {
                    dc.UpdateServersDb();
                }
            }
        }

        private void DeleteOutdatedData()
        {
            JobsController jc = new JobsController();
            jc.DeleteAllExpiredJobs();
        }

        private void UpdateAllFilters()
        {
            // This is to make sure that the app got the study groups that is used as search filters. 
            StudyGroupsController sgc = new StudyGroupsController();
            LocationsController lc = new LocationsController();
            JobTypesController jtc = new JobTypesController();
            CoursesController cc = new CoursesController();
            cc.UpdateCoursesFromServer();
            jtc.UpdateJobTypesFromServer();
            sgc.UpdateStudyGroupsFromServer();
            lc.UpdateLocationsFromServer();
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
            StudyGroupsController sgc = new StudyGroupsController();
            LocationsController lc = new LocationsController();
            JobTypesController jtc = new JobTypesController();
            CoursesController cc = new CoursesController();
            NavPage.Navigation.InsertPageBefore(new MainPage(), NavPage.Navigation.NavigationStack.First());
            NavPage.Navigation.PopToRootAsync();
            jtc.UpdateJobTypesFromServer();
            sgc.UpdateStudyGroupsFromServer();
            lc.UpdateLocationsFromServer();
            cc.UpdateCoursesFromServer();

        }
    }
}

