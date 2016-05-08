using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Views;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            var style = (Style)Application.Current.Resources["basePageStyle"];
            Style = style;
        }

        /// <summary>
        /// Activate this method if Authorization fails and a new login is required.
        /// </summary>
        public void GoToLogin()
        {
            new Authenticater();
        }
    }
}
