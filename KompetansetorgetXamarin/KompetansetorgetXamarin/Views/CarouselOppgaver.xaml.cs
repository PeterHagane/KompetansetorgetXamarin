using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using XLabs.Forms;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using KompetansetorgetXamarin.Controls;
using System.Windows.Input;
using KompetansetorgetXamarin.Controllers;
using Xamarin.Forms.Xaml;
using System.Collections.Specialized;

namespace KompetansetorgetXamarin.Views
{
    public partial class CarouselOppgaver : BaseCarouselPage
    {
        VMOppgaverSettings listInit = new VMOppgaverSettings();
        //ObservableCollection<Oppgave> oppgaver = new ObservableCollection<Oppgave>();
        public static ObservableCollection<Project> oppgaver = new ObservableCollection<Project>();
        /*Dictionary<string, string> filter = new Dictionary<string, string>();*/ //used in AddData;
        ICommand refreshCommand;
        string p0title = "Finn oppgaveforslag";
        string p1title = "Velg fagområder";
        static public bool pullList = true;
        //string p2title = "Velg fagområder";
        //string p3title = "Velg emne";

        public CarouselOppgaver()
        {
            InitializeComponent();
            this.Title = p0title;
            OppgaveList.IsRefreshing = true;
            AddData();
            OppgaveList.IsRefreshing = false;
            OppgaveList.ItemsSource = oppgaver;   // oppgave.companies[0].name  .logo
            oppgaverSettings.ItemsSource = listInit.oppgaveSettings;
            //OppgaverEmner.ItemsSource = listInit.coursesSettings;
            OppgaveList.IsPullToRefreshEnabled = true;
            OppgaveList.RefreshCommand = RefreshCommand;
            søk.TextChanged += (sender, e) => OppgaveList.FilterOppgaver(søk.Text);
            søk.SearchButtonPressed += (sender, e) =>
            {
                OppgaveList.FilterOppgaver(søk.Text);
            };
        }



        void OnClick(object sender, EventArgs e)
        {
            this.DisplayAlert("Selected!", "Fagområder get", "OK");
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

        void SaveSettings(object sender, EventArgs e)
        {
            listInit.SaveSettings();
            this.DisplayAlert("Innstillinger lagret!", "Oppdatér for å få en ny liste.", "OK");
        }


        //void VelgEmneButton_OnTapped(object sender, EventArgs e)
        //{
        //    this.CurrentPage = this.Children[3];
        //}

        //void VelgFagområdeButton_OnTapped(object sender, EventArgs e)
        //{
        //        this.CurrentPage = this.Children[2];
        //}

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
        /// Custom refresh to pull new data from server. If pullList is false(set in SearchListView.cs), it won't download a new list but will instead just do a regular unmodified refresh.
        /// </summary>
        async Task ExcecuteRefreshCommand()
        {
            //if (pullList == false)
            //{
            //    OppgaveList.IsRefreshing = false;
            //}
            //else if (pullList == true)
            //{
                listInit.SaveSettings();
                oppgaver.Clear();
                OppgaveList.ItemsSource = null;
                AddData();
                OppgaveList.ItemsSource = oppgaver;
                OppgaveList.IsRefreshing = false;
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
                listInit.SaveSettings();  //saves settings when going to p0
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
            listInit.SaveSettings(); //saves the settings when pressing the up button/leaving the page
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
                return false;
            //}
            //listInit.SaveSettings();
            //return true;
        }

        public void getFilter()
        { }

        //public Dictionary<string, string> GetCourse()
        //{
        //    foreach (Course course in listInit.coursesSettings)
        //    {
        //        object itemname = OppgaverEmner.SelectedItem;
        //        string i = itemname.ToString();



        //        if (course.name == i) {
        //            listInit.coursesFilter.Add("courses", course.id);
        //            System.Diagnostics.Debug.WriteLine("ITEMNAME TOSTRING DOES NOT WORK::::: " + i);
        //        }
        //        else
        //        {
        //            System.Diagnostics.Debug.WriteLine("ITEMNAME TOSTRING PRODUCES STRING::::: " + i);
        //        }


        //    }
        //    return listInit.coursesFilter;
        //}


        //void OnSelection(object sender, SelectedItemChangedEventArgs e)
        //{
        //    if (e.SelectedItem == null)
        //    {
        //        return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
        //    }
        //    DisplayAlert("Item Selected", e.SelectedItem.ToString(), "Ok");
        //    //((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
        //}

        private async void AddData()
        {
            oppgaver.Clear();
            //Dictionary<string, string> filter = new Dictionary<string, string>(); //contains only one item from each group
            //filter.Add("courses", "DAT-304");
            //filter.Add("types", "virksomhet");

            //if (pullList == false)
            //{
            //}
            //else if (pullList == true)
            //{
            ProjectsController jc = new ProjectsController();

                IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter(listInit.GetSettings(), null);
                foreach (Project p in projects)
                {
                    //oppgaver.Clear();
                    oppgaver.Add(p);
                }

                if (!Authenticater.Authorized)
                {
                    GoToLogin();
                }
                if (projects != null)
                {
                    System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter: projects.Count(): " +
                                                       projects.Count());
                }
            //    pullList = false;
            //}
        }
    }
}





//TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();


//private static void OnOppgaveSelected(object o, ItemTappedEventArgs e)
//{
//if itemclicked.contains title X,
//var DBoppgave = database.findOppgaveWhereTitleIs(X)
//navigation.oppgavepage(passing data DBoppgave.text, DBoppgave.imageURL)
//goto StillingPage, insert text and imageURL


//}

//oppgaver.Add(new Oppgave("Universitetet i Agder","Lag en app for kompetansetorget!", defaultLogo));
//            oppgaver.Add(new Oppgave("Universitetet i Agder", "Hjelp oss med å lage en kalenderfunksjon for UiAs studenter", defaultLogo));

//private async void GetAllProjectsFromWebApi_OnClicked(object sender, EventArgs e)
//{
//    ProjectsController jc = new ProjectsController();
//    IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter();
//    if (!Authenticater.Authorized)
//    {
//        GoToLogin();
//    }
//    if (projects != null)
//    {
//        System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter: jobs.Count(): " +
//                                           projects.Count());
//    }
//}