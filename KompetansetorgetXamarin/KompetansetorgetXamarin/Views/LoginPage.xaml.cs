﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controls;
using Xamarin.Forms;
using System.Net.Http;

namespace KompetansetorgetXamarin.Views
{
    public partial class LoginPage : BaseContentPage
    {
        public LoginPage()
        {

            InitializeComponent();
        }


        //public void LoginClicked(object sender, EventArgs e)
        //{
        //   string username = "nadia";
        //    string password = "1234";


        //if (username.Equals(usernameEntry.Text) && password.Equals(passwordEntry.Text))
        //{
        //   Resultat.Text = "Velkommen";


        //}
        //else {
        //   Resultat.Text = "Feil, prøv igjen";
        //}
        //}


        //private void GlemtPassord(object sender, EventArgs e)
        //{

        //}

        private void OnBypassLogin(object sender, EventArgs e)
        {
            Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack.First());
            Navigation.PopToRootAsync();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            new Authenticater();
            /*
            Uri uri =
                new Uri(
                    "http://oauthtest101.azurewebsites.net/api/Account/ExternalLogin?provider=Google&response_type=token&client_id=ngAuthApp&redirect_uri=http://oauthtest101.azurewebsites.net");
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var results = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine("results: " + results);
            */
        }

    }
   // http://oauthtest101.azurewebsites.net/api/account/externallogin?provider=Google&response_type=token&client_id=ngAuthApp&redirect_uri=http://oauthtest101.azurewebsites.net/api/Account/ExternalLogin
    //http://oauthtest101.azurewebsites.net/api/Account/ExternalLogin?provider=Google&response_type=token&client_id=Kompetansetorget&redirect_uri=http://oauthtest101.azurewebsites.net
}