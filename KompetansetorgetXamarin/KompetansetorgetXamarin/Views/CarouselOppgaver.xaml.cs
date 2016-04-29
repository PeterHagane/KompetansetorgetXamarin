using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using KompetansetorgetXamarin.Controls;

namespace KompetansetorgetXamarin.Views
{
    public partial class CarouselOppgaver : CarouselPage
    {
        string defaultLogo = "http://kompetansetorget.uia.no/extension/kompetansetorget/design/kompetansetorget/images/logo-virksomhet.jpg";
        string uiaLogo = "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter-internt/universitetet-i-agder/18076-2-nor-NO/universitetet-i-agder_width-4.jpg";
        VMOppgaverSettings listInit = new VMOppgaverSettings();
        ObservableCollection<Oppgave> oppgaver = new ObservableCollection<Oppgave>();
        ObservableCollection<fagområdeSetting> fagområder; 


        public CarouselOppgaver()
        {
            InitializeComponent();
            addData();
            OppgaveList.ItemsSource = oppgaver;
            oppgaverSettings.ItemsSource = listInit.oppgaveSettings;
            //oppgaverSettings.ItemsSource = fagområder;
        }

        public void addData()
        {
            oppgaver.Add(new Oppgave("UiA", "Lag en app for kompetansetorget!", uiaLogo));
            oppgaver.Add(new Oppgave("UiA", "Lag en kalenderfunksjon til UiAs studenter", uiaLogo));
            oppgaver.Add(new Oppgave("UiA", "Morseffekter på eggstørrelse hos hummer", uiaLogo));
            oppgaver.Add(new Oppgave("Agder Energi", "Fullstendig interaktiv 3D visualisering av Kraftstasjon (Tungefoss) med innlagte e-læringsressurser og dokumentasjon", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/agder-energi/10593-1-nor-NO/agder-energi_width-12.jpg"));
            oppgaver.Add(new Oppgave("abnc", "Konstruksjon av demo-handelsplattform", defaultLogo));
            oppgaver.Add(new Oppgave("abnc", "Utarbeiding av derivater for internett-handelsplattform", defaultLogo));
            oppgaver.Add(new Oppgave("UiA", "Lag en app for kompetansetorget!", uiaLogo));
            oppgaver.Add(new Oppgave("UiA", "Lag en kalenderfunksjon til UiAs studenter", uiaLogo));
            oppgaver.Add(new Oppgave("UiA", "Morseffekter på eggstørrelse hos hummer", uiaLogo));
            oppgaver.Add(new Oppgave("Agder Energi", "Fullstendig interaktiv 3D visualisering av Kraftstasjon (Tungefoss) med innlagte e-læringsressurser og dokumentasjon", "http://kompetansetorget.uia.no/var/kompetansetorget/storage/images/virksomheter/agder-energi/10593-1-nor-NO/agder-energi_width-12.jpg"));
            oppgaver.Add(new Oppgave("abnc", "Konstruksjon av demo-handelsplattform", defaultLogo));
            oppgaver.Add(new Oppgave("abnc", "Utarbeiding av derivater for internett-handelsplattform", defaultLogo));
        }


        public void getList()
        {
            ObservableCollection<fagområdeSetting> fagområder = listInit.oppgaveSettings;
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