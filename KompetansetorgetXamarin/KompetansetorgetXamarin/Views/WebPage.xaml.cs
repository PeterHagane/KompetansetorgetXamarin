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
    public partial class WebPage : BaseContentPage
    {
        public WebPage()
        {
            InitializeComponent();
            //https://github.com/XLabs/Xamarin-Forms-Labs/wiki/HybridWebView
            //https://github.com/XLabs/Xamarin-Forms-Labs/wiki/IOC
            
            //TODO
            //register IJsonSerializer as type of ServiceStack JSON serializer
            //.Register<IJsonSerializer, Services.Serialization.ServiceStackV3.JsonSerializer>()
            
            //load web view from the web
            //this.hybridWebView.Uri = new Uri("https://github.com/XLabs/Xamarin-Forms-Labs/wiki/HybridWebView/");
        }
    }
}
