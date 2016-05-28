using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    class SeparatorLine : StackLayout
    {
        public SeparatorLine()
        {
            var stack = new StackLayout();
            stack.HeightRequest = 1;
            stack.WidthRequest = 500;
            stack.BackgroundColor = Color.Black;
        }
    }
}
