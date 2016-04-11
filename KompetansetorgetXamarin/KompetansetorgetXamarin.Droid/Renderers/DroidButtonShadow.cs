using Android.Content;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using KompetansetorgetXamarin.Droid;
using Android.Graphics.Drawables;
using System.ComponentModel;
using KompetansetorgetXamarin.Controls;
using Android.Graphics;

[assembly: ExportRenderer(typeof(KompetansetorgetXamarin.Controls.ButtonShadow), typeof(DroidButtonShadow))]
namespace KompetansetorgetXamarin.Droid
{
    class DroidButtonShadow : VisualElementRenderer<ButtonShadow>
    {
        private GradientDrawable _normal, _pressed;

        protected override void OnElementChanged(ElementChangedEventArgs<ButtonShadow> e)
        {



            ButtonShadow customFram = e.NewElement as ButtonShadow;
            // Create a drawable for the button's normal state
            _normal = new Android.Graphics.Drawables.GradientDrawable();
            _normal.SetColor(customFram.BackgroundColor.ToAndroid());
            _normal.SetCornerRadius(customFram.BorderRadius);
            SetBackgroundDrawable(_normal);
            //SetBackgroundColor(customFram.BackgroundColor.ToAndroid());



            base.OnElementChanged(e);

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

        }
    }
}