using System;
using System.Linq.Expressions;
using Xamarin.Forms;
using KompetansetorgetXamarin.Controls;
//using XLabs.Enums;
using XLabs.Forms.Controls;

/// <summary>
/// This class is lifted directly from xlabs for inheritance purposes
/// </summary>


namespace KompetansetorgetXamarin.Controls
{
    public class XLabsImageButton : Button
    {
        /// <summary>
        /// Backing field for the Image property.
        /// </summary>
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            (Expression<Func<XLabsImageButton, ImageSource>>)(w => w.Source),
            null,
            BindingMode.OneWay,
            null,
            (bindable, oldvalue, newvalue) => ((VisualElement)bindable).ToString());

        /// <summary>
        /// Backing field for the Image property.
        /// </summary>
        public static readonly BindableProperty DisabledSourceProperty = BindableProperty.Create(
            (Expression<Func<XLabsImageButton, ImageSource>>)(w => w.DisabledSource),
            null,
            BindingMode.OneWay,
            null,
            (bindable, oldvalue, newvalue) => ((VisualElement)bindable).ToString());

        /// <summary>
        /// Backing field for the image width property.
        /// </summary>
        public static readonly BindableProperty ImageWidthRequestProperty =
            BindableProperty.Create<XLabsImageButton, int>(
                p => p.ImageWidthRequest, default(int));

        /// <summary>
        /// Backing field for the image height property.
        /// </summary>
        public static readonly BindableProperty ImageHeightRequestProperty =
            BindableProperty.Create<XLabsImageButton, int>(
                p => p.ImageHeightRequest, default(int));

        /// <summary>
        /// Backing field for the orientation property.
        /// </summary>
        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create<XLabsImageButton, ImageOrientationOverride>(
                p => p.Orientation, ImageOrientationOverride.ImageToLeft);

        /// <summary>
        /// Backing field for the tint color property.
        /// </summary>
        public static readonly BindableProperty ImageTintColorProperty =
            BindableProperty.Create<XLabsImageButton, Color>(
                p => p.ImageTintColor, Color.Transparent);

        /// <summary>
        /// Backing field for the disbaled tint color property.
        /// </summary>
        public static readonly BindableProperty DisabledImageTintColorProperty =
            BindableProperty.Create<XLabsImageButton, Color>(
                p => p.DisabledImageTintColor, Color.Transparent);

        /// <summary>
        /// Gets or sets the ImageSource to use with the control.
        /// </summary>
        /// <value>
        /// The Source property gets/sets the value of the backing field, SourceProperty.
        /// </value>
        [TypeConverter(typeof(XLabs.Forms.Controls.ImageSourceConverter))]
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ImageSource to use with the control.
        /// </summary>
        /// <value>
        /// The Source property gets/sets the value of the backing field, SourceProperty.
        /// </value>
        [TypeConverter(typeof(XLabs.Forms.Controls.ImageSourceConverter))]
        public ImageSource DisabledSource
        {
            get { return (ImageSource)GetValue(DisabledSourceProperty); }
            set { SetValue(DisabledSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets The orientation of the image relative to the text.
        /// </summary> 
        /// <value>
        /// The Orientation property gets/sets the value of the backing field, OrientationProperty.
        /// </value> 
        public ImageOrientationOverride Orientation
        {
            get { return (ImageOrientationOverride)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the requested height of the image.  If less than or equal to zero than a 
        /// height of 50 will be used.
        /// </summary>
        /// <value>
        /// The ImageHeightRequest property gets/sets the value of the backing field, ImageHeightRequestProperty.
        /// </value> 
        public int ImageHeightRequest
        {
            get { return (int)GetValue(ImageHeightRequestProperty); }
            set { SetValue(ImageHeightRequestProperty, value); }
        }

        /// <summary>
        /// Gets or sets the requested width of the image.  If less than or equal to zero than a 
        /// width of 50 will be used.
        /// </summary>
        /// <value>
        /// The ImageHeightRequest property gets/sets the value of the backing field, ImageHeightRequestProperty.
        /// </value> 
        public int ImageWidthRequest
        {
            get { return (int)GetValue(ImageWidthRequestProperty); }
            set { SetValue(ImageWidthRequestProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tint color of the image 
        /// </summary>
        /// <value>
        /// The ImageTintColor property gets/sets the value of the backing field, ImageTintColorProperty.
        /// </value> 
        public Color ImageTintColor
        {
            get { return (Color)GetValue(ImageTintColorProperty); }
            set { SetValue(ImageTintColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tint color of the image when the button is disabled
        /// </summary>
        /// <value>
        /// The DisabledImageTintColor property gets/sets the value of the backing field, DisabledImageTintColorProperty.
        /// </value> 
        public Color DisabledImageTintColor
        {
            get { return (Color)GetValue(DisabledImageTintColorProperty); }
            set { SetValue(DisabledImageTintColorProperty, value); }
        }
    }
}
