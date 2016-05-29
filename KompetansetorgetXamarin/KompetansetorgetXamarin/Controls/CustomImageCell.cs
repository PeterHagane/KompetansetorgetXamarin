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
    class CustomImageCell : ImageCell
    {
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
