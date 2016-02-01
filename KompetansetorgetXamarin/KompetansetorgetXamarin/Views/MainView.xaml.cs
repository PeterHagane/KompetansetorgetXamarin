using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();
        }

        private async void StudentButton_OnClicked(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserverapp.azurewebsites.net/api/students");
            var results = await response.Content.ReadAsAsync<IEnumerable<Student>>();

            var sb = new StringBuilder();
            foreach (var student in results)
            {
                var text = string.Format("FName: {0}, Mail: {1}, UserName {2}", student.FName, student.Mail,
                    student.UserName);
                sb.AppendLine(text);

            }
            TextBox.Text = sb.ToString();
        }
        /*
              private async void FagButton_OnClicked(object sender, EventArgs e)
              {

                  var client = new HttpClient();
                  var response = await client.GetAsync("http://kompetansetorgetserverapp.azurewebsites.net/api/proficiencies1");
                  var results = await response.Content.ReadAsAsync<IEnumerable<Proficiency>>();
              }*/
    }
}