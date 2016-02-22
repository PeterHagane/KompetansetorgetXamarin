using System;
using Xamarin.Forms;
using KompetansetorgetXamarin.Views;
using System.IO;
using PCLStorage;
using KompetansetorgetXamarin.CRUD;

namespace KompetansetorgetXamarin
{
    public class App : Application
    {
        
        public App()
        {
            // The root page of your application
            MainPage = new ViktorTestView();
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
    }
}
