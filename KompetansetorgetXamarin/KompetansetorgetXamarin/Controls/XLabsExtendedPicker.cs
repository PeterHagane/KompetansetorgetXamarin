using System;
using Xamarin.Forms;
using System.Collections;
using System.Reflection;


namespace KompetansetorgetXamarin.Controls
{
    /// <summary>
    /// Extends the Picker Class to add additional features not supported out of the box by Xamarin.
    /// Features Add:
    /// DataBinding
    /// </summary>
    public class XLabsExtendedPicker : Picker
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public XLabsExtendedPicker()
        {
            base.SelectedIndexChanged += OnSelectedIndexChanged;
        }
        /// <summary>
        /// Identifies the <see cref="P:XLabs.Forms.Controls.ExtendedPicker.SelectedItem" /> property.
        /// </summary>
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(XLabsExtendedPicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(XLabsExtendedPicker.OnSelectedItemChanged), null, null, null);
        /// <summary>
        /// Identifies the <see cref="P:XLabs.Forms.Controls.XLabsExtendedPicker.ItemSource" /> property.
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(XLabsExtendedPicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(XLabsExtendedPicker.OnItemsSourceChanged), null, null, null);
        /// <summary>
        /// Identifies the <see cref="P:XLabs.Forms.Controls.XLabsExtendedPicker.DisplayProperty" /> property.
        /// </summary>
        public static readonly BindableProperty DisplayPropertyProperty = BindableProperty.Create("DisplayProperty", typeof(string), typeof(XLabsExtendedPicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(XLabsExtendedPicker.OnDisplayPropertyChanged), null, null, null);

        /// <summary>
        /// Accepts an <see cref="T:System.Collections.IList" /> which is used to populate the picker with data. 
        /// By default the <see cref="System.Object.ToString"/> is displayed.
        /// </summary>
        /// <value>The items source.</value>
        public IList ItemsSource
        {
            get
            {
                return (IList)base.GetValue(XLabsExtendedPicker.ItemsSourceProperty);
            }
            set
            {
                base.SetValue(XLabsExtendedPicker.ItemsSourceProperty, value);
            }
        }
        /// <summary>
        /// Sets the item currently displayed in the UI. 
        /// This works by finding the index of the item in the backing data and then updateing <see cref="P:Xamarin.Forms.Picker.SelectedIndex" />  
        /// so the two object need the same ref. 
        /// </summary>
        /// <value>The selected item.</value>
        public object SelectedItem
        {
            get
            {
                return base.GetValue(XLabsExtendedPicker.SelectedItemProperty);
            }
            set
            {
                base.SetValue(XLabsExtendedPicker.SelectedItemProperty, value);
            }
        }
        /// <summary>
        /// A string used in reflection to identify the property to use as the display property of the object. 
        /// If this is not specified the <see cref="System.Object.ToString"/> is used.
        /// </summary>
        /// <value>The display property.</value>
        public string DisplayProperty
        {
            get
            {
                return (string)base.GetValue(XLabsExtendedPicker.DisplayPropertyProperty);
            }
            set
            {
                base.SetValue(XLabsExtendedPicker.DisplayPropertyProperty, value);
            }
        }


        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedItem = ItemsSource[SelectedIndex];
        }


        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            XLabsExtendedPicker bindablePicker = (XLabsExtendedPicker)bindable;
            bindablePicker.SelectedItem = newValue;
            if (bindablePicker.ItemsSource != null && bindablePicker.SelectedItem != null)
            {
                int count = 0;
                foreach (object obj in bindablePicker.ItemsSource)
                {
                    if (obj == bindablePicker.SelectedItem)
                    {
                        bindablePicker.SelectedIndex = count;
                        break;
                    }
                    count++;
                }
            }
        }
        private static void OnDisplayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {

            XLabsExtendedPicker bindablePicker = (XLabsExtendedPicker)bindable;
            bindablePicker.DisplayProperty = (string)newValue;
            loadItemsAndSetSelected(bindable);

        }
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            XLabsExtendedPicker bindablePicker = (XLabsExtendedPicker)bindable;
            bindablePicker.ItemsSource = (IList)newValue;
            loadItemsAndSetSelected(bindable);
        }
        static void loadItemsAndSetSelected(BindableObject bindable)
        {
            XLabsExtendedPicker bindablePicker = (XLabsExtendedPicker)bindable;
            if (bindablePicker.ItemsSource as IEnumerable != null)
            {
                PropertyInfo propertyInfo = null;
                int count = 0;
                foreach (object obj in (IEnumerable)bindablePicker.ItemsSource)
                {
                    string value = string.Empty;
                    if (bindablePicker.DisplayProperty != null)
                    {
                        if (propertyInfo == null)
                        {
                            propertyInfo = obj.GetType().GetRuntimeProperty(bindablePicker.DisplayProperty);
                            if (propertyInfo == null)
                                throw new Exception(String.Concat(bindablePicker.DisplayProperty, " is not a property of ", obj.GetType().FullName));
                        }
                        value = propertyInfo.GetValue(obj).ToString();
                    }
                    else {
                        value = obj.ToString();
                    }
                    bindablePicker.Items.Add(value);
                    if (bindablePicker.SelectedItem != null)
                    {
                        if (bindablePicker.SelectedItem == obj)
                        {
                            bindablePicker.SelectedIndex = count;
                        }
                    }
                    count++;
                }
            }
        }
    }
}
