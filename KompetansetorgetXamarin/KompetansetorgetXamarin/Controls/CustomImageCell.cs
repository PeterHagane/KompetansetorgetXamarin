using KompetansetorgetXamarin.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

/// <summary>
/// This class defines menuitems when pressing and holding item in varsellist, and associated commands
/// </summary>
namespace KompetansetorgetXamarin.Controls
{
    class CustomImageCell : ViewCell
    {
        //public CustomImageCell() {
        //    var stack = new StackLayout()
        //{
        //    Orientation = StackOrientation.Horizontal,
        //    Padding = new Thickness(30, 0, 30, 5),
        //    Spacing = 0
        //};

        //var NameLabel = new Label();
        //NameLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
        //    NameLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
        //    NameLabel.FontSize = Xamarin.Forms.Device.GetNamedSize(NamedSize.Default, typeof(Label));
        //    NameLabel.LineBreakMode = LineBreakMode.NoWrap;
        //    NameLabel.TextColor = Color.Black;
        //    NameLabel.SetBinding(Label.TextProperty, "Name");

        //    stack.Children.Add(NameLabel);

        //    var ToggleSwitch = new Switch();
        //ToggleSwitch.HorizontalOptions = LayoutOptions.End;
        //    ToggleSwitch.VerticalOptions = LayoutOptions.CenterAndExpand;
        //    ToggleSwitch.SetBinding(Switch.IsToggledProperty, "IsSelected");
        //    stack.Children.Add(ToggleSwitch);







        //    View = stack;


        //}


        //public CustomImageCell() {
        //    var moreAction = new MenuItem { Text = "Se annonse" };
        //    moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
        //    moreAction.Clicked += (sender, e) => {
        //        Varsel v;
        //        var mi = ((MenuItem)sender);
        //        v = (Varsel)mi.CommandParameter;
        //        Debug.WriteLine("More Context Action clicked: " + mi.CommandParameter);
        //    };

        //    var deleteAction = new MenuItem { Text = "Slett", IsDestructive = true }; // red background
        //    deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
        //    deleteAction.Clicked += (sender, e) => { //async
        //        Varsel v;
        //        var mi = ((MenuItem)sender);
        //        v = (Varsel)mi.CommandParameter;
        //        Debug.WriteLine("Delete Context Action clicked: " + mi.CommandParameter);
        //    };
        //    // add to the ViewCell's ContextActions property
        //    ContextActions.Add(moreAction);
        //    ContextActions.Add(deleteAction);
        //}
    }
}
