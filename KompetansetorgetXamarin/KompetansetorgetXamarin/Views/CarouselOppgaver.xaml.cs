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
using KompetansetorgetXamarin.DAL;

namespace KompetansetorgetXamarin.Views
{
    public partial class CarouselOppgaver : BaseCarouselPage
    {
        VMOppgaverSettings listInit = new VMOppgaverSettings();
        //ObservableCollection<Oppgave> oppgaver = new ObservableCollection<Oppgave>();
        public ObservableCollection<Project> oppgaver = new ObservableCollection<Project>();
        /*Dictionary<string, string> filter = new Dictionary<string, string>();*/ //used in AddData;
        ICommand refreshCommand;
        string p0title = "Finn oppgaveforslag";
        string p1title = "Velg fagområder";
        //static public bool pullList = true;
        //string p2title = "Velg fagområder";
        //string p3title = "Velg emne";        
        private bool sortDesc;

        public CarouselOppgaver()
        {
            InitializeComponent();
            sortDesc = true;
            this.Title = p0title;
            
            OppgaveList.SetCarouselOppgaver(this);
            OppgaveList.RefreshCommand = RefreshCommand;
            OppgaveList.IsPullToRefreshEnabled = true;
            

            søk.TextChanged += (sender, e) => OppgaveList.FilterOppgaver(søk.Text);
            søk.SearchButtonPressed += (sender, e) =>
            {
                OppgaveList.FilterOppgaver(søk.Text);
            };
            InitializeSelectItemEventListener();

            UpdateItemSource();
        }


        void InitializeSelectItemEventListener()
        {
            OppgaveList.ItemSelected += async (sender, e) =>
            {
                Project d = (Project)e.SelectedItem;
                //var action = DisplayAlert(d.Text, d.JobTitle, "Slett varsel", "Se annonse");
                OppgaveList.SelectedItem = null;
                OpenAdvert(d);
            };
        }

        private async Task OpenAdvert(Project oppgave)
        {
            var url = oppgave.webpage;
            var type = "project";
            var WebPage = new WebPage(type, url);
            await Navigation.PushAsync(WebPage);  //opens new webpage in browser to given url
        }

        private async Task UpdateItemSource() 
        {
            OppgaveList.IsRefreshing = true;
            oppgaverSettings.ItemsSource = listInit.oppgaveSettings;
            await AddData();
            OppgaveList.ItemsSource = oppgaver;   // oppgave.companies[0].name  .logo
            OppgaveList.IsRefreshing = false;
            //OppgaverEmner.ItemsSource = listInit.coursesSettings;


        }

        public ObservableCollection<Project> GetProjects()
        {
            return oppgaver;
        }

        void Sorter_OnTapped(object sender, EventArgs e)
        {
            sortDesc = !sortDesc;
            Sort();
        }

        void Sort()
        {
            if (sortDesc)
            {
                OppgaveList.ItemsSource = oppgaver
                    .OrderByDescending(x => x.published);
            }
            else
            {
                OppgaveList.ItemsSource = oppgaver
                    .OrderBy(x => x.published);
            }
            //UpdateItemSource();
            OppgaveList.IsRefreshing = false;
        }

        void SaveSettings(object sender, EventArgs e)
        {
            listInit.SaveSettings();
            this.DisplayAlert("Innstillinger lagret!", "Oppdatér for å få inn nye oppgaver.", "OK");
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
            get
            {
                System.Diagnostics.Debug.WriteLine("CarouselOppgaver - RefreshCommand - before refreshCommand ??");
                return refreshCommand ?? (refreshCommand = new Command(async () => await ExcecuteRefreshCommand()));
            }
        }
        /// <summary>
        /// Custom refresh to pull new data from server. If pullList is false(set in SearchListView.cs), it won't download a new list but will instead just do a regular unmodified refresh.
        /// </summary>
        async Task ExcecuteRefreshCommand()
        {
            OppgaveList.IsRefreshing = true;
            await AddData();
            Sort();
            OppgaveList.IsRefreshing = false;
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
            listInit.SaveSettings(); //saves the settings when pressing the up button/leaving the page
        }

        protected override bool OnBackButtonPressed() //behaviour of HARDWARE back button, not the up button.
        {
            listInit.SaveSettings();
            if (CurrentPage.SendBackButtonPressed()) return true;
            return true;
        }

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
            ProjectsController jc = new ProjectsController();

            
            IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter(listInit.GetSettings(), null);
            HashSet<Project> newProjects = new HashSet<Project>(projects);
            HashSet<Project> oldProjects = new HashSet<Project>(oppgaver);
            bool sameProjects = newProjects.SetEquals(oldProjects);

            if (!sameProjects)
            {
                //OppgaveList.ItemsSource = null;
                oppgaver.Clear();
                foreach (Project project in projects)
                {
                    System.Diagnostics.Debug.WriteLine("project.title: " + project.title);
                    oppgaver.Add(project);
                }
                //OppgaveList.ItemsSource = oppgaver;
            }
            if (!Authenticater.Authorized)
            {
                GoToLogin();
            }
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