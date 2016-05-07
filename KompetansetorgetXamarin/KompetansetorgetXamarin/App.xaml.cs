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

            // The root page of your application
            NavPage = new NavigationPage(new LoginPage());
            //NavPage.BarBackgroundColor = Color.FromHex("ec7a08");
            //NavPage.BarTextColor = Color.White;
            MainPage = NavPage;
        }
        
        protected override void OnStart()
        {
            StudentsController sc = new StudentsController();
            if (sc.GetStudent() != null) { 
                DeleteOutdatedData();
                UpdateAllFilters();
                DevicesController dc = new DevicesController();
                if (!dc.GetDevice().tokenSent)
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
        /// Activate this method if Authorization fails and a new login is required.
        /// </summary>
        public static void GoToLogin()
        {
            System.Diagnostics.Debug.WriteLine("App - GoToLogin");
            NavPage = new NavigationPage(new LoginPage());
            NavPage.Navigation.InsertPageBefore(new LoginPage(), NavPage.Navigation.NavigationStack.First());
            NavPage.Navigation.PopToRootAsync();
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

//public static Student Student { get; set; }

/*
public static bool IsLoggedIn
{
    get
    {
        if (Student != null)
        {
            return !string.IsNullOrWhiteSpace(Student.Mail);
        }
        else
        {
            return false;
        }
    }
}
/
public static Action SuccessfulLoginAction
{
    get
    {
        return new Action(() => {
            NavPage.Navigation.PopModalAsync();

            if (IsLoggedIn)
            {
                NavPage.Navigation.InsertPageBefore(new MainPage(), NavPage.Navigation.NavigationStack.First());
                NavPage.Navigation.PopToRootAsync();
            }
        });
    }
}
}
*/
