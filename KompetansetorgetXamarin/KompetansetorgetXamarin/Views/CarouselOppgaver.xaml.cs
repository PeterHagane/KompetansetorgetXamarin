using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using KompetansetorgetXamarin.Controls;
using System.Windows.Input;
using KompetansetorgetXamarin.Controllers;

namespace KompetansetorgetXamarin.Views
{
    public partial class CarouselOppgaver : BaseCarouselPage
    {
        string defaultLogo = "http://kompetansetorget.uia.no/extension/kompetansetorget/design/kompetansetorget/images/logo-virksomhet.jpg";
        string uiaLogo = "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter-internt/universitetet-i-agder/18076-2-nor-NO/universitetet-i-agder_width-4.jpg";
        VMOppgaverSettings listInit = new VMOppgaverSettings();
        //ObservableCollection<Oppgave> oppgaver = new ObservableCollection<Oppgave>();
        ObservableCollection<Project> oppgaver = new ObservableCollection<Project>();
        ObservableCollection<fagområdeSetting> fagområder; 
        int currentPage = 0;
        ICommand refreshCommand;

        public CarouselOppgaver()
        {
            InitializeComponent();
            addData();
            OppgaveList.ItemsSource = oppgaver;   // oppgave.companies[0].name  .logo
            oppgaverSettings.ItemsSource = listInit.oppgaveSettings;
            OppgaveList.IsPullToRefreshEnabled = true;
            OppgaveList.IsRefreshing = false;
            OppgaveList.RefreshCommand = RefreshCommand;
            //OnBackButtonPressed();
            //oppgaverSettings.ItemsSource = fagområder;
        }

        public void addData()
        {
            TestProjectsFilter_OnClicked();
        }

        public void getList()
        {
            ObservableCollection<fagområdeSetting> fagområder = listInit.oppgaveSettings;
        }

        void OnClick(object sender, EventArgs e)
        {
            listInit.GetAllFilters();
            this.DisplayAlert("Selected!", "Fagområder get", "OK");
        }

        private ICommand RefreshCommand {
            get { return refreshCommand ?? (refreshCommand = new Command(async () => await ExcecuteRefreshCommand()));}
        }
            
            
        async Task ExcecuteRefreshCommand() {
            OppgaveList.ItemsSource = null;
            await Task.Delay(1000);
            addData();
            OppgaveList.IsRefreshing = false;
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

        private async void TestProjectsFilter_OnClicked()
        {
            //checkedstudygroups must be uuid
            List<string> checkedStudyGroups = listInit.GetSettings();
            
            //LEGG FAGOMRÅDER TIL AKTIVT FILTER HER MED EN SWITCH ELLER LIGNENDE


            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("courses", "DAT-304");
            filter.Add("types", "virksomhet");

            ProjectsController jc = new ProjectsController();
            IEnumerable<Project> projects = await jc.GetProjectsBasedOnFilter(checkedStudyGroups, null , filter);
            foreach(Project p in projects)
            {
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
                foreach (var project in projects)
                {
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].id);
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].name);
                    System.Diagnostics.Debug.WriteLine("Companies is not null: " + project.companies[0].logo);
                }
            }
        }

        //oppgaver.Add(new Oppgave("UiA", "Lag en app for kompetansetorget!", uiaLogo));
        //oppgaver.Add(new Oppgave("UiA", "Lag en kalenderfunksjon til UiAs studenter", uiaLogo));
        //oppgaver.Add(new Oppgave("UiA", "Morseffekter på eggstørrelse hos hummer", uiaLogo));
        //oppgaver.Add(new Oppgave("Agder Energi", "Fullstendig interaktiv 3D visualisering av Kraftstasjon (Tungefoss) med innlagte e-læringsressurser og dokumentasjon", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/agder-energi/10593-1-nor-NO/agder-energi_width-12.jpg"));
        //oppgaver.Add(new Oppgave("abnc", "Konstruksjon av demo-handelsplattform", defaultLogo));
        //oppgaver.Add(new Oppgave("abnc", "Utarbeiding av derivater for internett-handelsplattform", defaultLogo));
        //oppgaver.Add(new Oppgave("UiA", "Lag en app for kompetansetorget!", uiaLogo));
        //oppgaver.Add(new Oppgave("UiA", "Lag en kalenderfunksjon til UiAs studenter", uiaLogo));
        //oppgaver.Add(new Oppgave("UiA", "Morseffekter på eggstørrelse hos hummer", uiaLogo));
        //oppgaver.Add(new Oppgave("Agder Energi", "Fullstendig interaktiv 3D visualisering av Kraftstasjon (Tungefoss) med innlagte e-læringsressurser og dokumentasjon", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/agder-energi/10593-1-nor-NO/agder-energi_width-12.jpg"));
        //oppgaver.Add(new Oppgave("abnc", "Konstruksjon av demo-handelsplattform", defaultLogo));
        //oppgaver.Add(new Oppgave("abnc", "Utarbeiding av derivater for internett-handelsplattform", defaultLogo));




        //Xamarin.Forms.Device.BeginInvokeOnMainThread(async() =>
        //    {


        //        if (CurrentPage == p1)
        //        {
        //            this.CurrentPage = p0;
        //        }
        //        else {

        //        }
        //        return base.OnBackButtonPressed();
        //    });

        //if (result) await this.Navigation.PopAsync();
        //CurrentPage.Navigation.InsertPageBefore(this.Children[0], CurrentPage.Navigation.NavigationStack.First());
        //var result = await this.DisplayAlert("Alert!", "Do you really want to exit?", "Yes", "No");

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