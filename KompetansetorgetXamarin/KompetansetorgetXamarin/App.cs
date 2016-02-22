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
            MainPage = new NavigationPage(new MainView());
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
            /*
            CheckToken(
                "cvi1LZzRdZ4:APA91bERsfF7kNNMmXV_4qhcwEg7_D5tQCIJhua-QbrGnyIBIsF0K7ovqVcZi9kWRRgheERodLCwbNDwXtNmZWXimZzDbwAPboR3CKcl4OkT6BeHMSSvnpgA9yvgknbqOhEFQjH4eO6Z"); */
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }



        public void CheckToken(string newToken)
        {
            System.Diagnostics.Debug.WriteLine("The new Token is: " + newToken);
            TokenHandler th = new TokenHandler();
            th.CheckToken(newToken);
        }


    }
}
