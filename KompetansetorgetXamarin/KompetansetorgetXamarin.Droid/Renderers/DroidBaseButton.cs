using Android.Content;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using KompetansetorgetXamarin.Droid;
using Android.Graphics.Drawables;
using System.ComponentModel;
using KompetansetorgetXamarin.Controls;


//This is one of two classes that renders custom buttons for android. (Both WP and ios would need their own custom button renderer) 
//This is necessary because the button class used by forms has limited capabilities for customisation


[assembly: ExportRenderer(typeof(KompetansetorgetXamarin.Controls.BaseButton), typeof(DroidBaseButton))]
namespace KompetansetorgetXamarin.Droid
{
    public class DroidBaseButton : ButtonRenderer
    {
        private GradientDrawable _normal, _pressed;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            UpdatePadding();
            //UpdateMargin();

            if (Control != null)
            {
                var button = e.NewElement;
                var buttonGrey = "#FFA6BCC6"; //NOTE: THE TWO FIRST ARE ALPHA
                var buttonWhite = "#FFFFFFFF";

                // Create a drawable for the button's normal state
                _normal = new Android.Graphics.Drawables.GradientDrawable();

                if (button.BackgroundColor.R == -1.0 && button.BackgroundColor.G == -1.0 && button.BackgroundColor.B == -1.0)
                    _normal.SetColor(Android.Graphics.Color.ParseColor(buttonWhite));  
                else
                    _normal.SetColor(button.BackgroundColor.ToAndroid());

                _normal.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                _normal.SetCornerRadius(button.BorderRadius);


                // Create a drawable for the button's pressed state
                _pressed = new Android.Graphics.Drawables.GradientDrawable();
                var highlight = Context.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.ColorActivatedHighlight }).GetColor(0, Android.Graphics.Color.ParseColor(buttonGrey));
                _pressed.SetColor(highlight);
                _pressed.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                _pressed.SetCornerRadius(button.BorderRadius);

                // Add the drawables to a state list and assign the state list to the button
                var sld = new StateListDrawable();
                sld.AddState(new int[] { Android.Resource.Attribute.StatePressed }, _pressed);
                sld.AddState(new int[] { }, _normal);
                Control.SetBackgroundDrawable(sld);
            }
        }


        //private void UpdateMargin()
        //{
        //    var element = this.Element as BaseButton;
        //    if (element != null)
        //    {
                
        //        Control.SetMargin(
        //            (int)element.Margin.Left,
        //            (int)element.Margin.Top,
        //            (int)element.Margin.Right,
        //            (int)element.Margin.Bottom
        //        );
        //    }
        //}

        private void UpdatePadding()
        {
            var element = this.Element as BaseButton;
            if (element != null)
            {

                Control.SetPadding(
                    (int)element.Padding.Left,
                    (int)element.Padding.Top,
                    (int)element.Padding.Right,
                    (int)element.Padding.Bottom
                );
            }
        }


        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var button = (Xamarin.Forms.Button)sender;

            if (e.PropertyName == "Margin")
            {
                UpdatePadding();
            }

            if (_normal != null && _pressed != null)
            {
                if (e.PropertyName == "BorderRadius")
                {
                    _normal.SetCornerRadius(button.BorderRadius);
                    _pressed.SetCornerRadius(button.BorderRadius);
                }
                if (e.PropertyName == "BorderWidth" || e.PropertyName == "BorderColor")
                {
                    _normal.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                    _pressed.SetStroke((int)button.BorderWidth, button.BorderColor.ToAndroid());
                }
            }
        }
    }
}