using System;
using Xamarin.Forms;
using KompetansetorgetXamarin.Views;
using System.IO;
using System.Linq;
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
            IFolder path = FileSystem.Current.LocalStorage;
            string filename = "token.json";
            try
            {
                IFile file = path.GetFileAsync(filename).Result;
                System.Diagnostics.Debug.WriteLine("File containing token exists");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                System.Diagnostics.Debug.WriteLine("File containing token doesn't exist");
                Crud crudster = new Crud();
                crudster.CreateNewFile(filename);
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
        /// Former pages for the Navigation Stack
        /// </summary>
        public static void SuccessfulLoginAction()
        {
            // NavPage.Navigation.PopModalAsync();
            NavPage.Navigation.InsertPageBefore(new MainPage(), NavPage.Navigation.NavigationStack.First());
            NavPage.Navigation.PopToRootAsync();
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
