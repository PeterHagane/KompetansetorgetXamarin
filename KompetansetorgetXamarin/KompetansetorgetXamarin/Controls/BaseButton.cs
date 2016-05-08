using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace KompetansetorgetXamarin.Controls
{
    //establishes BaseButton as a child of XlabsImageButton
    public class BaseButton : XLabsExtendedButton
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

        //Adds the margin property to basebutton
        public static BindableProperty MarginProperty = BindableProperty.Create<BaseButton, Thickness>(d => d.Margin, default(Thickness));

        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        #endregion Margin
    }
}

//Note XlabsImageButton inherits from XLabs' ExtendedButton, you can also apply the following attributes to a BaseButton:
//HorizontalContentAlignment="Start" OR "Center" OR "End"
//VerticalContentAlignment="Start" OR "Center" OR "End"
//This way you can arbitrarily align text within the button