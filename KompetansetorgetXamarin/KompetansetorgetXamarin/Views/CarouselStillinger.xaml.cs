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
        private string sort;
        //public static bool pullList = true;

        public CarouselStillinger()
        {
            InitializeComponent();
            sort = "Nyeste";
            StillingList.SetCarouselStillinger(this);
            //StillingList.IsRefreshing = false;
            this.Title = p0title;
            //OppgaverEmner.ItemsSource = LISTINIT.coursesSettings;
            StillingList.IsPullToRefreshEnabled = true;         
            StillingList.RefreshCommand = RefreshCommand;
            søk.TextChanged += (sender, e) => StillingList.FilterOppgaver(søk.Text);
            søk.SearchButtonPressed += (sender, e) => {
                StillingList.FilterOppgaver(søk.Text);
            };
            InitializeSelectItemEventListener();                      

            UpdateItemSource();
        }

        private async Task UpdateItemSource()
        {
            //OppgaveList.IsRefreshing = false;
            StillingList.IsRefreshing = true;
            StillingerSettings.ItemsSource = LISTINIT.stillingerSettings;
            await AddData();
            StillingList.ItemsSource = JOBS;   // oppgave.companies[0].name  .logo
            //OppgaverEmner.ItemsSource = listInit.coursesSettings;
            StillingList.IsRefreshing = false;
        }

        void InitializeSelectItemEventListener()
        {
            StillingList.ItemSelected += (sender, e) =>
            {
                
                Job d = (Job)e.SelectedItem;
                //var action = DisplayAlert(d.Text, d.JobTitle, "Slett varsel", "Se annonse");
                OpenAdvert(d); 
            };
        }

        private async Task OpenAdvert(Job stilling)
        {
            StillingList.SelectedItem = null;
            var url = stilling.webpage;
            var type = "job";
            var webPage = new WebPage(type, url);
            await Navigation.PushAsync(webPage);  //opens new webpage in browser to given url
        }

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
            string action = await DisplayActionSheet("Sorter etter:", "Avbryt", null, "Nyeste", "Eldste", "Nærmeste søknadsfrist", "Seneste søknadsfrist");
            if (action != null) {
                sort = action;
                Sort(sort);
            }
        }

        void Sort(string sort)
        {
            if (sort == "Tidligste søknadsfrist")
            {
                StillingList.ItemsSource = JOBS
                    .OrderByDescending(x => x.expiryDate);

            }
            else if (sort == "Seneste søknadsfrist")
            {
                StillingList.ItemsSource = JOBS
                    .OrderBy(x => x.expiryDate);
            }
            else if (sort == "Nyeste")
            {
                StillingList.ItemsSource = JOBS
                    .OrderByDescending(x => x.published);
            }
            else if (sort == "Eldste")
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
                //if (CurrentPage == this.Children[3])
                //{
                //    this.CurrentPage = this.Children[2];
                //}
                //else if (CurrentPage == this.Children[2])
                //{
                //    this.CurrentPage = this.Children[1];
                //}
                //else
                if (CurrentPage == this.Children[1])
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
            StillingList.IsRefreshing = true;
            await AddData();
            Sort(sort);
            StillingList.IsRefreshing = false;
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
                ExcecuteRefreshCommand();
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

        //protected override bool OnBackButtonPressed() //behaviour of HARDWARE back button, not the up button.
        //{
        //    //var p0 = this.Children[0];
        //    //var p1 = this.Children[1];

        //    if (CurrentPage.SendBackButtonPressed()) return true;

        //    //if (CurrentPage == p1)
        //    //{
        //    //    this.CurrentPage = p0;
        //    //    listInit.SaveSettings();
        //    //}
        //    //else if (CurrentPage == p0)
        //    //{
        //    //    return false;
        //    //}
        //    //listInit.SaveSettings();
        //    return true;
        //}

        void SaveSettings(object sender, EventArgs e)
        {
            LISTINIT.SaveSettings();
            this.DisplayAlert("Innstillinger lagret!", "Oppdatér for å få nye stillinger", "OK");
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
                Sort(sort);
                //StillingList.ItemsSource = JOBS;
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
