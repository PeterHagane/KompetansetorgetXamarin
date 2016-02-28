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
    public class App : Application
    {
        private static NavigationPage NavPage;


        public App()
        {
            // The root page of your application
            //MainPage = new NavigationPage(new MainPage());  //ViktorTestView();

            NavPage = new NavigationPage(new LoginPage(this));
            //NavPage = new NavigationPage(new MainPage());
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

        public void SuccessfulLoginAction()
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