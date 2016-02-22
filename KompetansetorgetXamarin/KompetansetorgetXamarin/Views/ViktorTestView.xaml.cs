using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using KompetansetorgetXamarin.CRUD;
using Xamarin.Forms;
using KompetansetorgetXamarin.Models;
using PCLStorage;

namespace KompetansetorgetXamarin.Views
{
    public partial class ViktorTestView : ContentPage
    {
        public ViktorTestView()
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
    }
}