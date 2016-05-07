﻿using System;
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
        int currentPage = 0;

        public CarouselOppgaver()
        {
            InitializeComponent();
            addData();
            OppgaveList.ItemsSource = oppgaver;
            oppgaverSettings.ItemsSource = listInit.oppgaveSettings;
            //OnBackButtonPressed();

 
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