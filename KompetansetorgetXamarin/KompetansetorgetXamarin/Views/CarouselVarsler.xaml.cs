using KompetansetorgetXamarin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace KompetansetorgetXamarin.Views
{

    public partial class CarouselVarsler : CarouselPage
    {
        static readonly Random rnd = new Random();
        string randomDate = GetRandomDate();
        string defaultLogo = "http://kompetansetorget.uia.no/extension/kompetansetorget/design/kompetansetorget/images/logo-virksomhet.jpg";
        ObservableCollection<Varsel> varsler = new ObservableCollection<Varsel>();
        int currentPage = 0;



        public CarouselVarsler()
        {
            InitializeComponent();
            addData();
            VarselList.ItemsSource = varsler;
        }

        public void addData()
        {
            varsler.Add(new Varsel("Oppgave", "UiA", randomDate, defaultLogo));
            varsler.Add(new Varsel("Stilling", "UiA", randomDate, defaultLogo));
            varsler.Add(new Varsel("Stilling", "PaulaShop", randomDate, defaultLogo));
            varsler.Add(new Varsel("Stilling", "TestBirds", randomDate, defaultLogo));
            varsler.Add(new Varsel("Oppgave", "TravelBirds", randomDate, defaultLogo));
            varsler.Add(new Varsel("Stilling", "Parko AS", randomDate, defaultLogo));
            varsler.Add(new Varsel("Oppgave", "UiA", randomDate, defaultLogo));
            varsler.Add(new Varsel("Oppgave", "UiA", randomDate, defaultLogo));
            varsler.Add(new Varsel("Stilling", "UiA", randomDate, defaultLogo));
        }


        public static string GetRandomDate()
        {
            int range = 365; //number of days  
            DateTime randomDate = DateTime.Today.AddDays(rnd.Next(range));
            var date = Convert.ToString(randomDate); 
            return date;
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
