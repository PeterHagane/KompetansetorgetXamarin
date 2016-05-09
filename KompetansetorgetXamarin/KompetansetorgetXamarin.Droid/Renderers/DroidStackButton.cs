using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using KompetansetorgetXamarin.Controls;
using KompetansetorgetXamarin.Droid.Renderers;
using Android.Graphics.Drawables;



[assembly: ExportRenderer(typeof(ButtonStack), typeof(DroidStackButton))]

namespace KompetansetorgetXamarin.Droid.Renderers
{
    class DroidStackButton : VisualElementRenderer<StackLayout>
    {
        private GradientDrawable _normal, _pressed;


        protected override void OnElementChanged(ElementChangedEventArgs<StackLayout> e)
        {
            base.OnElementChanged(e);

            this.SetBackgroundDrawable(Resources.GetDrawable(Resource.Drawable.RoundedRectangle));
        }

    }
}