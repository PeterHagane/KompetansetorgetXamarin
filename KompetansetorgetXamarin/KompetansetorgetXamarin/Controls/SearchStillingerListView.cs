using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    class SearchStillingerListView : ListView
    {
        public SearchStillingerListView()
        {
            ItemSelected += (s, e) => {
                if (SelectedItem == null)
                    return;
                var selected = (Job)e.SelectedItem;
                SelectedItem = null;
                //Navigation.PushAsync (new CampusLocationPage (selected));
            };
        }


        public void FilterOppgaver(string filter)
        {
            CarouselStillinger.pullList = false; //don't pull a new list when refreshing
            this.BeginRefresh();

            if (string.IsNullOrWhiteSpace(filter))
            {
                this.ItemsSource = CarouselStillinger.JOBS;
            }
            else {
                this.ItemsSource = CarouselStillinger.JOBS
                        //.Where(x => x.companies[0].name.ToLower() :::: have to choose between one or the other with current linq statement -- how combine two wheres? Needs to be OR 
                        .Where(x => x.title.ToLower()
                        .Contains(filter.ToLower())
                        );

            }

            this.EndRefresh();
            CarouselStillinger.pullList = true;
        }

    }
}
