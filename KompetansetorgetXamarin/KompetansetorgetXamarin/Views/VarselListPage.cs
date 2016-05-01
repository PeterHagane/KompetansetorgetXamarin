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

            // Lag eksempeldata
            // i fremtiden vil dette kunne være f.eks en arraylist som blir foret varseldata fra en annen metode
            List<DataPassed> varsler = new List<DataPassed>
            {
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 1, 15), Color.Aqua),
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 2, 20), Color.Black),
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 1, 10), Color.Purple),
                new DataPassed("Ny stilling har blitt lagt ut: Utvikler", new DateTime(2016, 2, 5), Color.Red)
            };

            // Lag ListView
            ListView listView = new ListView
            {
                // Datakilden til items i listen
                ItemsSource = varsler,

                // Definer en mal for hvert item i listen, dette er layoutet til innholdet i én celle i listen
                // "Argument of DataTemplate constructor is called for 
                //  each item; it must return a Cell derivative"
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
                        View = new StackLayout //definerer innholdet i cellen som stack layout
                        {
                            Padding = new Thickness(0, 5),
                            Orientation = StackOrientation.Horizontal,
                            Children = //children = nested cellestruktur, samme prinsipp som i XAML
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


            //private async void ListView_OnClicked(object sender, EventArgs e)
            //{
            //var ListItemClicked = new ViktorTestView();
            //await Navigation.PushAsync(ViktorTestView);
            //}

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Dette bygger hele siden, inkluderer layouts som har blitt definert tidligere
            //foreløpige layouts som blir bygd er kun headeren og listviewet med genererte items
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