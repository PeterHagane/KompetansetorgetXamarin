using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    public class ButtonShadow : Frame
    {
        public readonly static BindableProperty BorderRadiusProperty = BindableProperty.Create("BorderRadius", typeof(int), typeof(ButtonShadow), 5, BindingMode.OneWay, null, null, null, null, null);
        public int BorderRadius
        {
            get
            {
                return (int)base.GetValue(ButtonShadow.BorderRadiusProperty);
            }
            set
            {
                base.SetValue(ButtonShadow.BorderRadiusProperty, value);
            }
        }

        /// <summary>
        /// The corner radius property.
        /// </summary>
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create("CornerRadius", typeof(double), typeof(ButtonShadow), 0.0);


        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
    }
}



//this class exists so that the custom renderer in droid can consume a custom control instead of applying custom render to all ContentViews