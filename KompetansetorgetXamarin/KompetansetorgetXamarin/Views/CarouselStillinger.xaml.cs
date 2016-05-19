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
        ObservableCollection<Job> JOBS = new ObservableCollection<Job>();
        ICommand refreshCommand;
        int currentPage = 0;
        string p0title = "Finn stillinger";
        string p1title = "Velg fagområder";

        public CarouselStillinger()
        {
            System.Diagnostics.Debug.WriteLine("FØR INITIALIZECOMPONENT ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::");
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::: ETTER INITIALIZECOMPONENT");
            AddData();
            this.Title = p0title;
            StillingList.ItemsSource = JOBS;   // oppgave.companies[0].name  .logo
            StillingerSettings.ItemsSource = LISTINIT.stillingerSettings;
            //OppgaverEmner.ItemsSource = LISTINIT.coursesSettings;
            StillingList.IsPullToRefreshEnabled = true;
            StillingList.IsRefreshing = false;
            StillingList.RefreshCommand = RefreshCommand;
            
        }

        void OnClick(object sender, EventArgs e)
        {
            ToolbarItem tbi = (ToolbarItem)sender;
            this.DisplayAlert("Selected!", tbi.Text, "OK");
        }

        void Sorter_OnTapped(object sender, EventArgs e)
        {
            this.DisplayAlert("Selected!", "Fagområder get", "OK");
            bool alphabeticallyFirst = false;
            Sort();
        }

        void Sort()
        {

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
            LISTINIT.SaveSettings();
            JOBS.Clear();
            StillingList.ItemsSource = null;
            AddData();
            StillingList.ItemsSource = JOBS;
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


        private async void AddData()
        {
            //Dictionary<string, string> filter = new Dictionary<string, string>(); //contains only one item from each group
            //filter.Add("courses", "DAT-304");
            //filter.Add("types", "virksomhet");


            JobsController jc = new JobsController();

            IEnumerable<Job> jobs = await jc.GetJobsBasedOnFilter(LISTINIT.GetSettings(), null, null);
            
            foreach (Job p in jobs)
            {
                //JOBS.Clear();
                JOBS.Add(p);
            }

            if (!Authenticater.Authorized)
            {
                GoToLogin();
            }
            if (jobs != null)
            {
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs.Count(): " +
                                                   jobs.Count());
            }
        }
    }
}
