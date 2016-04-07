//
//  RoundedBoxRenderer_Droid.cs
//  Created by Alexey Kinev on 26 Apr 2015.
//
//    Licensed under The MIT License (MIT)
//    http://opensource.org/licenses/MIT
//
//    Copyright (c) 2015 Alexey Kinev <alexey.rudy@gmail.com>
//
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

using KompetansetorgetXamarin.Droid;
using Android.Graphics.Drawables;
using KompetansetorgetXamarin.Controls;


[assembly: ExportRenderer(typeof(RoundBox), typeof(DroidRoundBox))]

namespace KompetansetorgetXamarin.Droid
{
    public class DroidRoundBox : BoxRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            SetWillNotDraw(false);

            Invalidate();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == RoundBox.CornerRadiusProperty.PropertyName)
            {
                Invalidate();
            }
        }

        public override void Draw(Canvas canvas)
        {
            var box = Element as RoundBox;
            var rect = new Rect();
            var paint = new Paint()
            {
                Color = box.BackgroundColor.ToAndroid(),
                AntiAlias = true,
            };

            GetDrawingRect(rect);

            var radius = (float)(rect.Width() / box.Width * box.CornerRadius);

            canvas.DrawRoundRect(new RectF(rect), radius, radius, paint);
        }
    }
}