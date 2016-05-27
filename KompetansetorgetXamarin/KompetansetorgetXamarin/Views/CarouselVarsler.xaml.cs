using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.Controls;
using KompetansetorgetXamarin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using KompetansetorgetXamarin.Utility;

namespace KompetansetorgetXamarin.Views
{

    public partial class CarouselVarsler : BaseCarouselPage
    {
        string defaultLogo = "http://kompetansetorget.uia.no/extension/kompetansetorget/design/kompetansetorget/images/logo-virksomhet.jpg";
        VMStillingerSettings LISTINIT = new VMStillingerSettings();
        ObservableCollection<Varsel> varsler = new ObservableCollection<Varsel>();
        ICommand refreshCommand;
        string p0title = "Dine varsler";
        string p1title = "Velg fagområder";
        public static bool pullList = true;


        public CarouselVarsler()
        {
            InitializeComponent();
            AddData();
            VarselList.ItemsSource = varsler;
            this.Title = p0title;
            StillingerSettings.ItemsSource = LISTINIT.stillingerSettings;
            //OppgaverEmner.ItemsSource = LISTINIT.coursesSettings;
            VarselList.IsPullToRefreshEnabled = true;
            VarselList.IsRefreshing = false;
            VarselList.RefreshCommand = RefreshCommand;
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
            if (pullList == false)
            {
                VarselList.IsRefreshing = false;

            }
            else if (pullList == true)
            {
                LISTINIT.SaveSettings();
                varsler.Clear();
                VarselList.ItemsSource = null;
                AddData();
                VarselList.ItemsSource = varsler;
                VarselList.IsRefreshing = false;
            }
        }

        //Alters title on carouselpage by contentpage
        public new event EventHandler CurrentPageChanged;
        protected override void OnCurrentPageChanged()
        {
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


        public void AddData()
        {
            if (pullList == false)
            {
            }
            else if (pullList == true)
            {
                System.Diagnostics.Debug.WriteLine("ViktorTestView - NotificationsFromDb_OnClicked: Initiated");
                NotificationsController nc = new NotificationsController();
                List<Advert> notifications = nc.GetNotificationList();
                
                System.Diagnostics.Debug.WriteLine(
                    "ViktorTestView - NotificationsFromDb_OnClicked: notifications.Count = " + notifications.Count);



                foreach (var n in notifications)
                {
                    if (n is Job)
                    {
                        Job job = (Job)n;

                        if (job.companies != null && job.companies[0].logo != null)
                        {
                            string logo = job.companies[0].logo;
                            string varselText = "Ny stilling fra " + job.companies[0].name + "!";
                            string published = "Publisert " + DateTimeHandler.MakeDateTimeString(job.published);
                            varsler.Add(new Varsel(varselText, published, logo));
                            // DO spesific Job code
                            //long date = job.expiryDate; // Will work
                            System.Diagnostics.Debug.WriteLine("job.title = " + job.title);
                            System.Diagnostics.Debug.WriteLine("job.companies.logo = " + job.companies[0].logo);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("job.companies = null");
                        }
                        System.Diagnostics.Debug.WriteLine("job.expiryDate = " + job.expiryDate);

                    }
                    else if (n is Project)
                    {
                        // Do spesific Project  code.
                        Project project = (Project)n;
                        if (project.companies != null && project.companies[0].logo != null)
                        {
                            string logo = project.companies[0].logo;
                            string varselText = "Ny oppgave fra " + project.companies[0].name + "!";
                            string published = "Publisert " + DateTimeHandler.MakeDateTimeString(project.published);
                            varsler.Add(new Varsel(varselText, published, logo));

                            System.Diagnostics.Debug.WriteLine("project.title = " + project.title);

                            System.Diagnostics.Debug.WriteLine("project.companies.logo = " + project.companies[0].logo);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("project.companies = null");
                        }
                        System.Diagnostics.Debug.WriteLine("project.companies.logo = " + project.companies[0].logo);
                        System.Diagnostics.Debug.WriteLine("project.published = " + project.published);
                    }
                    if (!Authenticater.Authorized)
                    {
                        GoToLogin();
                    }
                    if (varsler != null)
                    {
                        System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs.Count(): " +
                                                          notifications.Count());
                    }
                    pullList = false;
                }

            }
        }
    }
}
