using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

using Xamarin.Forms;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.Controls;
using System.Windows.Input;

namespace KompetansetorgetXamarin.Views
{
    public partial class CarouselStillinger : BaseCarouselPage
    {
        VMStillingerSettings LISTINIT = new VMStillingerSettings();
        public ObservableCollection<Job> JOBS = new ObservableCollection<Job>();
        ICommand refreshCommand;
        int currentPage = 0;
       
        string p0title = "Finn stillinger";
        string p1title = "Velg fagområder";
        //public static bool pullList = true;

        public CarouselStillinger()
        {
            InitializeComponent();
            StillingList.SetCarouselStillinger(this);
            //StillingList.IsRefreshing = false;
            this.Title = p0title;
            StillingList.ItemsSource = JOBS;   // oppgave.companies[0].name  .logo
            StillingerSettings.ItemsSource = LISTINIT.stillingerSettings;
            //OppgaverEmner.ItemsSource = LISTINIT.coursesSettings;
            StillingList.IsPullToRefreshEnabled = true;
            
            StillingList.RefreshCommand = RefreshCommand;
            søk.TextChanged += (sender, e) => StillingList.FilterOppgaver(søk.Text);
            søk.SearchButtonPressed += (sender, e) => {
                StillingList.FilterOppgaver(søk.Text);
            };

            //PopupMenu();

        }

        //void PopupMenu()
        //{
        //    StillingList.ItemSelected += (sender, e) =>
        //    {
        //        Varsel d = (Varsel)e.SelectedItem;
        //        var action = DisplayAlert(d.Text, "", "Slett varsel", "Se annonse");
        //        if (action != null)
        //        {
        //            DeleteOrOpen(action, d);
        //        }
        //    };
        //}

        //private async void OpenAdvert(Varsel varsel)
        //{
        //    var url = varsel.Webpage;
        //    var type = varsel.Type;
        //    var WebPage = new WebPage(type, url);
        //    await Navigation.PushAsync(WebPage);  //opens new webpage in browser to given url
        //}

        public ObservableCollection<Job> GetJobs()
        {
            return JOBS;
        }

        void OnClick(object sender, EventArgs e)
        {
            ToolbarItem tbi = (ToolbarItem)sender;
            this.DisplayAlert("Selected!", tbi.Text, "OK");
        }

        async void Sorter_OnTapped(object sender, EventArgs e)
        {
            var action = await DisplayActionSheet("Sorter stillinger", "Avbryt", null, "Nyeste", "Eldste", "Søknadsfrist - Nærmest", "Søknadsfrist - Fjerntliggende");
            if (action != null) { 
                Sort(action);
            }
        }

        void Sort(string action)
        {
            if (action == "Søknadsfrist - Nærmest")
            {
                StillingList.ItemsSource = JOBS
                    .OrderByDescending(x => x.expiryDate);

            }
            else if (action == "Søknadsfrist - Fjerntliggende")
            {
                StillingList.ItemsSource = JOBS
                    .OrderBy(x => x.expiryDate);
            }
            else if (action == "Nyeste")
            {
                StillingList.ItemsSource = JOBS
                    .OrderByDescending(x => x.published);
            }
            else if (action == "Eldste")
            {
                StillingList.ItemsSource = JOBS
                    .OrderBy(x => x.published);
            }
        }

        void SwipeRight(object sender, EventArgs e)
        {
            ToolbarItem tbi = (ToolbarItem)sender;
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (CurrentPage == this.Children[0])
                {
                    this.CurrentPage = this.Children[1];
                }
                else if (CurrentPage == this.Children[1])
                {
                    this.CurrentPage = this.Children[2];
                }
                else if (CurrentPage == this.Children[2])
                {
                    this.CurrentPage = this.Children[3];
                }
            });
        }

        void SwipeLeft(object sender, EventArgs e)
        {
            ToolbarItem tbi = (ToolbarItem)sender;
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if (CurrentPage == this.Children[3])
                {
                    this.CurrentPage = this.Children[2];
                }
                else if (CurrentPage == this.Children[2])
                {
                    this.CurrentPage = this.Children[1];
                }
                else if (CurrentPage == this.Children[1])
                {
                    this.CurrentPage = this.Children[0];
                }
            });
        }

        /// <summary>
        /// Handles refresh behaviour on button click
        /// </summary>
        void Refresh_OnTapped(object sender, EventArgs e)
        {
            ExcecuteRefreshCommand();
        }

        private ICommand RefreshCommand
        {
            get { return refreshCommand ?? (refreshCommand = new Command(async () => await ExcecuteRefreshCommand())); }
        }

        /// <summary>
        /// Refreshes the list
        /// </summary>
        async Task ExcecuteRefreshCommand()
        {
            //if (pullList == false)
            //{
            //    StillingList.IsRefreshing = false;
            //}
            //else if (pullList == true)
            //{
            LISTINIT.SaveSettings();
            await AddData();

            StillingList.IsRefreshing = false;
            //}
        }

        //Alters title on carouselpage by contentpage
        public new event EventHandler CurrentPageChanged;
        protected override void OnCurrentPageChanged()
        {
            int prevPage = 0;
            EventHandler changed = CurrentPageChanged;
            if (changed != null)
                changed(this, EventArgs.Empty);
            if (CurrentPage == this.Children[0])
            {
                this.Title = p0title;
                LISTINIT.SaveSettings();
            }
            else if (CurrentPage == this.Children[1])
            {
                this.Title = p1title;
            }
            //add in case of more pages
            //else if (CurrentPage == this.Children[2]) {
            //    this.Title = p2title;
            //} else if (CurrentPage == this.Children[3]) {
            //    this.Title = p3title;
            //}
        }


        protected override void OnDisappearing()
        {
            LISTINIT.SaveSettings(); //saves the settings when pressing the up button/leaving the page
        }

        protected override bool OnBackButtonPressed() //behaviour of HARDWARE back button, not the up button.
        {
            //var p0 = this.Children[0];
            //var p1 = this.Children[1];

            if (CurrentPage.SendBackButtonPressed()) return true;

            //if (CurrentPage == p1)
            //{
            //    this.CurrentPage = p0;
            //    listInit.SaveSettings();
            //}
            //else if (CurrentPage == p0)
            //{
            //    return false;
            //}
            //listInit.SaveSettings();
            return true;
        }

        public void getFilter()
        { }

        public void OnMore(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            DisplayAlert("More Context Action", mi.CommandParameter + " more context action", "OK");
        }

        public void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            DisplayAlert("Delete Context Action", mi.CommandParameter + " delete context action", "OK");
        }

        void stillingToggle(object sender, ToggledEventArgs e)
        {
            this.DisplayAlert("Selected!", "Stilling = " + e.Value, "OK");
        }

        private async Task AddData()
        {
            //Dictionary<string, string> filter = new Dictionary<string, string>(); //contains only one item from each group
            //filter.Add("courses", "DAT-304");
            //filter.Add("types", "virksomhet");

            //if (pullList == false)
            //{
            //}
            //else if (pullList == true)
            //{
            JobsController jc = new JobsController();

            IEnumerable<Job> jobs = await jc.GetJobsBasedOnFilter(LISTINIT.GetSettings(), null);

            HashSet<Job> newJobs = new HashSet<Job>(jobs);
            HashSet<Job> oldJobs = new HashSet<Job>(JOBS);
            bool sameJobs = newJobs.SetEquals(oldJobs);

            if (!sameJobs) {
                StillingList.ItemsSource = null;
                JOBS.Clear();
                foreach (Job p in jobs)
                {
                    //JOBS.Clear();
                    JOBS.Add(p);
                }               
                StillingList.ItemsSource = JOBS;
            }

            if (!Authenticater.Authorized)
                {
                    GoToLogin();
                }

            //    pullList = false;
            //}
        }
    }
}
