using Android.Content;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using KompetansetorgetXamarin.Droid;
using Android.Graphics.Drawables;
using System.ComponentModel;
using KompetansetorgetXamarin.Controls;
using XLabs.Forms.Extensions;
using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Views;
using XLabs.Forms.Controls;
using XLabs.Enums;
using Color = Xamarin.Forms.Color;
using View = Android.Views.View;



//This is one of two classes that ensure the ability to render custom buttons for android. (Both WP and ios would need their own custom button renderer) 
//This is necessary because the button class used by forms has limited capabilities for customisation
//The other class is the BaseButton class in the forms project


[assembly: ExportRenderer(typeof(KompetansetorgetXamarin.Controls.BaseButton), typeof(DroidBaseButton))]
namespace KompetansetorgetXamarin.Droid
{
    public class DroidBaseButton : ButtonRenderer
    {
        private GradientDrawable _normal, _pressed;
        string buttonGrey = "#FFA6BCC6"; //NOTE: THE TWO FIRST ARE ALPHA
        string buttonWhite = "#FFFFFFFF";
        private static float _density = float.MinValue;

        private BaseButton BaseButton
        {
            get { return (BaseButton)Element; }
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            UpdateAlignment();
            UpdateFont();
            UpdatePadding();
            //UpdateMargin();

            _density = Resources.DisplayMetrics.Density;
            var targetButton = Control;
            if (targetButton != null) targetButton.SetOnTouchListener(TouchListener.Instance.Value);

            if (Element != null && Element.Font != Font.Default && targetButton != null) targetButton.Typeface = Element.Font.ToExtendedTypeface(Context);

            if (Element != null && BaseButton.Source != null) await SetImageSourceAsync(targetButton, BaseButton).ConfigureAwait(false);


            if (Control != null)
            {
                var button = e.NewElement;


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
                var highlight = Android.Graphics.Color.ParseColor(buttonGrey);
                //var highlight = Context.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.ColorActivatedHighlight }).GetColor(0, Android.Graphics.Color.ParseColor(buttonWhite));
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && Control != null)
            {
                Control.Dispose();
            }
        }

        /// <summary>
        /// Sets the image source.
        /// </summary>
        /// <param name="targetButton">The target button.</param>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task"/> for the awaited operation.</returns>
        private async Task SetImageSourceAsync(Android.Widget.Button targetButton, BaseButton model)
        {
            if (targetButton == null || targetButton.Handle == IntPtr.Zero || model == null) return;

            // const int Padding = 10;
            var source = model.IsEnabled ? model.Source : model.DisabledSource ?? model.Source;

            using (var bitmap = await GetBitmapAsync(source).ConfigureAwait(false))
            {
                if (bitmap == null)
                    targetButton.SetCompoundDrawables(null, null, null, null);
                else
                {
                    var drawable = new BitmapDrawable(bitmap);
                    var tintColor = model.IsEnabled ? model.ImageTintColor : model.DisabledImageTintColor;
                    if (tintColor != Color.Transparent)
                    {
                        drawable.SetTint(tintColor.ToAndroid());
                        drawable.SetTintMode(PorterDuff.Mode.SrcIn);
                    }

                    using (var scaledDrawable = GetScaleDrawable(drawable, GetWidth(model.ImageWidthRequest), GetHeight(model.ImageHeightRequest)))
                    {
                        Drawable left = null;
                        Drawable right = null;
                        Drawable top = null;
                        Drawable bottom = null;
                        //System.Diagnostics.Debug.WriteLine($"SetImageSourceAsync intptr{targetButton.Handle}");
                        int padding = 10; // model.Padding
                        targetButton.CompoundDrawablePadding = RequestToPixels(padding);
                        switch (model.Orientation)
                        {
                            case ImageOrientationOverride.ImageToLeft:
                                targetButton.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                                left = scaledDrawable;
                                break;
                            case ImageOrientationOverride.ImageToRight:
                                targetButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                                right = scaledDrawable;
                                break;
                            case ImageOrientationOverride.ImageOnTop:
                                targetButton.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
                                top = scaledDrawable;
                                break;
                            case ImageOrientationOverride.ImageOnBottom:
                                targetButton.Gravity = GravityFlags.Bottom | GravityFlags.CenterHorizontal;
                                bottom = scaledDrawable;
                                break;
                            case ImageOrientationOverride.ImageCentered:
                                targetButton.Gravity = GravityFlags.Center; // | GravityFlags.Fill;
                                top = scaledDrawable;
                                break;
                        }

                        targetButton.SetCompoundDrawables(left, top, right, bottom);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> for the supplied <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="source">The <see cref="ImageSource"/> to get the image for.</param>
        /// <returns>A loaded <see cref="Bitmap"/>.</returns>
        private async Task<Bitmap> GetBitmapAsync(ImageSource source)
        {
            var handler = GetHandler(source);
            var returnValue = (Bitmap)null;

            if (handler != null)
                returnValue = await handler.LoadImageAsync(source, Context).ConfigureAwait(false);

            return returnValue;
        }


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

        private void UpdateFont()
        {
            Control.Typeface = Element.Font.ToExtendedTypeface(Context);
        }

        private void UpdateAlignment()
        {
            var element = this.Element as BaseButton;

            if (element == null || this.Control == null)
            {
                return;
            }

            this.Control.Gravity = element.VerticalContentAlignment.ToDroidVerticalGravity() |
                element.HorizontalContentAlignment.ToDroidHorizontalGravity();
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BaseButton.VerticalContentAlignmentProperty.PropertyName ||
                e.PropertyName == BaseButton.HorizontalContentAlignmentProperty.PropertyName)
            {
                UpdateAlignment();
            }
            else if (e.PropertyName == Button.FontProperty.PropertyName)
            {
                UpdateFont();
            }

            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == BaseButton.SourceProperty.PropertyName ||
    e.PropertyName == BaseButton.DisabledSourceProperty.PropertyName ||
    e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
    e.PropertyName == BaseButton.ImageTintColorProperty.PropertyName ||
    e.PropertyName == BaseButton.DisabledImageTintColorProperty.PropertyName)
            {
                await SetImageSourceAsync(Control, BaseButton).ConfigureAwait(false);
            }

            var button = (Xamarin.Forms.Button)sender;

            if (e.PropertyName == "Padding")
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

        /// <summary>
        /// Returns a <see cref="Drawable"/> with the correct dimensions from an 
        /// Android resource id.
        /// </summary>
        /// <param name="drawable">An android <see cref="Drawable"/>.</param>
        /// <param name="width">The width to scale to.</param>
        /// <param name="height">The height to scale to.</param>
        /// <returns>A scaled <see cref="Drawable"/>.</returns>
        private Drawable GetScaleDrawable(Drawable drawable, int width, int height)
        {
            var returnValue = new ScaleDrawable(drawable, 0, 100, 100).Drawable;

            returnValue.SetBounds(0, 0, RequestToPixels(width), RequestToPixels(height));

            return returnValue;
        }

        /// <summary>
        /// Returns a drawable dimension modified according to the current display DPI.
        /// </summary>
        /// <param name="sizeRequest">The requested size in relative units.</param>
        /// <returns>Size in pixels.</returns>
        public int RequestToPixels(int sizeRequest)
        {
            if (_density == float.MinValue)
            {
                if (Resources.Handle == IntPtr.Zero || Resources.DisplayMetrics.Handle == IntPtr.Zero)
                    _density = 1.0f;
                else
                    _density = Resources.DisplayMetrics.Density;
            }

            return (int)(sizeRequest * _density);
        }


        /// <summary>
        /// The following is taken from ImageButtonRenderer.shared.cs (search for it on XLabs' github)
        /// </summary>

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }

        private int GetWidth(int requestedWidth)
        {
            const int DefaultWidth = 50;
            return requestedWidth <= 0 ? DefaultWidth : requestedWidth;
        }

        private int GetHeight(int requestedHeight)
        {
            const int DefaultHeight = 50;
            return requestedHeight <= 0 ? DefaultHeight : requestedHeight;
        }

    }






    //Hot fix for the layout positioning issue on Android as described in http://forums.xamarin.com/discussion/20608/fix-for-button-layout-bug-on-android
    class TouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        public static readonly Lazy<TouchListener> Instance = new Lazy<TouchListener>(() => new TouchListener());

        /// <summary>
        /// Make TouchListener a singleton.
        /// </summary>
        private TouchListener()
        { }

        public bool OnTouch(View v, MotionEvent e)
        {
            var buttonRenderer = v.Tag as ButtonRenderer;
            if (buttonRenderer != null && e.Action == MotionEventActions.Down) buttonRenderer.Control.Text = buttonRenderer.Element.Text;

            return false;
        }
    }

}