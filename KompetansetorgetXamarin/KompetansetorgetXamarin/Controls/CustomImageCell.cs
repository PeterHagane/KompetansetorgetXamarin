using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    class CustomImageCell : ImageCell
    {
        public CustomImageCell() {
            var moreAction = new MenuItem { Text = "More" };
            moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            moreAction.Clicked += async (sender, e) => {
                var mi = ((MenuItem)sender);
                Debug.WriteLine("More Context Action clicked: " + mi.CommandParameter);
            };

            var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true }; // red background
            deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            deleteAction.Clicked += async (sender, e) => {
                var mi = ((MenuItem)sender);
                Debug.WriteLine("Delete Context Action clicked: " + mi.CommandParameter);
            };
            // add to the ViewCell's ContextActions property
            ContextActions.Add(moreAction);
            ContextActions.Add(deleteAction);
        }


    }
}
