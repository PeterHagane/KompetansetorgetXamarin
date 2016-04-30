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
        public bool Authorized = true;
        public BaseContentPage()
        {
            var style = (Style)Application.Current.Resources["basePageStyle"];
            Style = style;
        }

        /// <summary>
        /// If an authorization fails (401) activate this method to go back to login.
        /// </summary>
        public void GoToLogin()
        {
            Navigation.InsertPageBefore(new LoginPage(), Navigation.NavigationStack.First());
            Navigation.PopToRootAsync();
        }
    }
}
