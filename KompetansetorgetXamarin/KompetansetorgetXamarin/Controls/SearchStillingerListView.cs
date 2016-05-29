using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Controls
{
    class SearchStillingerListView : ListView
    {
        private CarouselStillinger carouselStillinger;
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

        public void SetCarouselStillinger(CarouselStillinger carouselStillinger)
        {
            this.carouselStillinger = carouselStillinger;
        }

        public void FilterOppgaver(string filter)
        {
            ObservableCollection<Job> jobs = carouselStillinger.GetJobs();
            this.BeginRefresh();

            if (string.IsNullOrWhiteSpace(filter))
            {
                this.ItemsSource = jobs;
            }
            else {
                this.ItemsSource = jobs
                        //.Where(x => x.companies[0].name.ToLower() :::: have to choose between one or the other with current linq statement -- how combine two wheres? Needs to be OR 
                        .Where(x => x.title.ToLower()
                        .Contains(filter.ToLower())
                        );

            }

            this.EndRefresh();
        }

    }
}
