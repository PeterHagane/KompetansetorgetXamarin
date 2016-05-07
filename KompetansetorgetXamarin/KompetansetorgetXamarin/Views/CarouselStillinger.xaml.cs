using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

using Xamarin.Forms;

namespace KompetansetorgetXamarin.Views
{
    public partial class CarouselStillinger : CarouselPage
    {
        string defaultLogo = "http://kompetansetorget.uia.no/extension/kompetansetorget/design/kompetansetorget/images/logo-virksomhet.jpg";
        ObservableCollection<Stilling> stillinger = new ObservableCollection<Stilling>();
        int currentPage = 0;

        public CarouselStillinger()
        {
            InitializeComponent();
            addData();
            StillingList.ItemsSource = stillinger;
        }

        public void addData()
        {
            stillinger.Add(new Stilling("Parko AS", "Butikkmedarbeider deltid og ferie.", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/parko-as/28396-1-nor-NO/parko-as_width-4.jpg"));
            stillinger.Add(new Stilling("Paulashop.no", "Paulashop.no søker sommerhjelp til nettbutikken", defaultLogo));
            stillinger.Add(new Stilling("TravelBird", "Internship Allround Norway", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/travelbird/151481-1-nor-NO/travelbird_width-4.jpg"));
            stillinger.Add(new Stilling("Testbirds", "Test Users for Apps & Websites wanted!", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/testbirds/29460-2-nor-NO/testbirds_width-4.jpg"));
            stillinger.Add(new Stilling("Parko AS", "Butikkmedarbeider deltid og ferie.", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/parko-as/28396-1-nor-NO/parko-as_width-4.jpg"));
            stillinger.Add(new Stilling("Paulashop.no", "Paulashop.no søker sommerhjelp til nettbutikken", defaultLogo));
            stillinger.Add(new Stilling("TravelBird", "Internship Allround Norway", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/travelbird/151481-1-nor-NO/travelbird_width-4.jpg"));
            stillinger.Add(new Stilling("Testbirds", "Test Users for Apps & Websites wanted!", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/testbirds/29460-2-nor-NO/testbirds_width-4.jpg"));
            stillinger.Add(new Stilling("Parko AS", "Butikkmedarbeider deltid og ferie.", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/parko-as/28396-1-nor-NO/parko-as_width-4.jpg"));
            stillinger.Add(new Stilling("Paulashop.no", "Paulashop.no søker sommerhjelp til nettbutikken", defaultLogo));
            stillinger.Add(new Stilling("TravelBird", "Internship Allround Norway", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/travelbird/151481-1-nor-NO/travelbird_width-4.jpg"));
        }

        void OnClick(object sender, EventArgs e)
        {
            ToolbarItem tbi = (ToolbarItem)sender;
            this.DisplayAlert("Selected!", tbi.Text, "OK");
        }

        void SwipeRight(object sender, EventArgs e)
        {
            ToolbarItem tbi = (ToolbarItem)sender;
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (currentPage != 1)
                {
                    currentPage = 1;
                }

                this.CurrentPage = this.Children[currentPage];

            });
        }


        protected override bool OnBackButtonPressed()
        {
            var p0 = this.Children[0];
            var p1 = this.Children[1];

            if (CurrentPage.SendBackButtonPressed()) return true;

            if (CurrentPage == p1)
            {
                this.CurrentPage = p0;
            }
            else if (CurrentPage == p0)
            {
                return false;
            }
            return true;
        }





    }
}
