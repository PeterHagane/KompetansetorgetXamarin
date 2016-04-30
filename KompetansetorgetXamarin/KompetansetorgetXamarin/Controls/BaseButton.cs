using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    //establishes BaseButton as a child of Button
    public class BaseButton : Button
    {
        #region Padding

        //Adds the padding property to basebutton
        public static BindableProperty PaddingProperty = BindableProperty.Create<BaseButton, Thickness>(d => d.Padding, default(Thickness));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        #endregion Padding

        #region Margin

        //Adds the padding property to basebutton
        public static BindableProperty MarginProperty = BindableProperty.Create<BaseButton, Thickness>(d => d.Margin, default(Thickness));

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        #endregion Margin
    }
}
