using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Forms;
using Xamarin.Forms;
using KompetansetorgetXamarin.Controls;

namespace KompetansetorgetXamarin.Views
{
    public partial class WebPage : ContentPage
    {
        private string url;
        private string type;
        private string title;

        public WebPage(string type, string url)
        {
            InitializeComponent();

            WebView.Source = url;
            this.type = type;

            if (type == "job") {
                this.type = "stilling";
                this.title = "Stilling";
                //this.Title = title;

            } else if(type == "project"){
                this.type = "oppgaveforslag";
                this.title = "Oppgaveforslag";
                //this.Title = title;
            }
            
            //https://github.com/XLabs/Xamarin-Forms-Labs/wiki/HybridWebView
            //https://github.com/XLabs/Xamarin-Forms-Labs/wiki/IOC

            //TODO
            //register IJsonSerializer as type of ServiceStack JSON serializer
            //.Register<IJsonSerializer, Services.Serialization.ServiceStackV3.JsonSerializer>()

            //load web view from the web
            //this.hybridWebView.Uri = new Uri("https://github.com/XLabs/Xamarin-Forms-Labs/wiki/HybridWebView/");

            //Permissions:
            //In order for WebView to work, you must make sure that permissions are set for each platform. Note that on some platforms, WebView will work in debug mode, but not when built for release.That is because some permissions, like those for internet access on Android, are set by default by Xamarin Studio when in debug mode.

            //Windows Phone 8.0 – requires ID_CAP_WEBBROWSERCOMPONENT for the control and ID_CAP_NETWORKING for internet access.
            //Windows Phone 8.1 – requires the Internet(Client & Server) capability when displaying network content.
            //Android – requires INTERNET only when displaying content from the network.Local content requires no special permissions.
            //iOS – requires no special permissions.
        }
        string getUrl() {
            return url;
        }

        void LoadingSite(object sender, WebNavigatingEventArgs e)
        {
            this.Title = "Henter " + type + "...";
            //LoadingLabel.IsVisible = true;
        }

        void SiteLoaded(object sender, WebNavigatedEventArgs e)
        {
            this.Title = title;
            //LoadingLabel.IsVisible = false;
        }
    }
}
