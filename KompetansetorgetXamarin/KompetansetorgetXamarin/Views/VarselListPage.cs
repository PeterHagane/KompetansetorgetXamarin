using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace KompetansetorgetXamarin.Views
{
    public partial class VarselListPage : ContentPage
    {
        public VarselListPage()
        {
            Label header = new Label
            {
                Text = "Dine Varsler",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

            // Define some data.
            List<DataPassed> varsler = new List<DataPassed>
            {
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 1, 15), Color.Aqua),
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 2, 20), Color.Black),
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 1, 10), Color.Purple),
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 2, 5), Color.Red)
            };

            // Create the ListView.
            ListView listView = new ListView
            {
                // Source of data items.
                ItemsSource = varsler,

                // Define template for displaying each item.
                // (Argument of DataTemplate constructor is called for 
                //      each item; it must return a Cell derivative.)
                ItemTemplate = new DataTemplate(() =>
                {
                    // Create views with bindings for displaying each property.
                    Label titleLabel = new Label();
                    titleLabel.SetBinding(Label.TextProperty, "Title");

                    Label dateLabel = new Label();
                    dateLabel.SetBinding(Label.TextProperty,
                        new Binding("Date", BindingMode.OneWay,
                            null, null, "Created {0:d}"));

                    BoxView boxView = new BoxView();
                    boxView.SetBinding(BoxView.ColorProperty, "Image");

                    // Return an assembled ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                                {
                                    boxView,
                                    new StackLayout
                                    {
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            titleLabel,
                                            dateLabel
                                        }
                                        }
                                }
                        }
                    };
                })
            };

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    header,
                    listView
                }
            };
        }
    }
}