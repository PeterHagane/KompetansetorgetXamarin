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
    class SearchOppgaverListView : ListView
    {
        private CarouselOppgaver carouselOppgaver; 
        public SearchOppgaverListView()
        {
           ItemSelected += (s, e) => {
                if (SelectedItem == null)
                    return;
                var selected = (Project)e.SelectedItem;
                SelectedItem = null;
                //Navigation.PushAsync (new thispage (selected));
            };
        }

        public void SetCarouselOppgaver(CarouselOppgaver carouselOppgaver)
        {
            this.carouselOppgaver = carouselOppgaver;
        }

        public void FilterOppgaver(string filter)
        {
            ObservableCollection<Project> oppgaver = carouselOppgaver.GetProjects();
            this.BeginRefresh();

            if (string.IsNullOrWhiteSpace(filter))
            {
                this.ItemsSource = oppgaver;
            }
            else {
                this.ItemsSource = oppgaver
                        //.Where(x => x.companies[0].name.ToLower() //have to choose between one or the other with current linq statement -- how combine two wheres? Needs to be OR 
                        .Where(x => x.title.ToLower()
                        .Contains(filter.ToLower())
                        );

            }

            this.EndRefresh();
        }

    }
}
