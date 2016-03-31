using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            var style = (Style)Application.Current.Resources["basePageStyle"];
            Style = style;
        }
    }
}
