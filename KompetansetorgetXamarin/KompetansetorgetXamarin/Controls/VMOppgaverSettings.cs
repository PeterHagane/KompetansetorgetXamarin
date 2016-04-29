using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.Controls
{
    class VMOppgaverSettings
    {
        public ObservableCollection<fagområdeSetting> oppgaveSettings { get; set; }

        public VMOppgaverSettings() {

            if (oppgaveSettings == null) { 
            oppgaveSettings = new ObservableCollection<fagområdeSetting>
            {
                new fagområdeSetting("Administrasjon og ledelse",           true),
                new fagområdeSetting("Datateknologi",                       true),
                new fagområdeSetting("Helse- og sosialfag",                 true),
                new fagområdeSetting("Historie, filosofi og religion",      true),
                new fagområdeSetting("Idrettsfag",                          true),
                new fagområdeSetting("Ingeniør- og teknologiske fag",       true),
                new fagområdeSetting("Kunstfag",                            true),
                new fagområdeSetting("Lærerutdanning og pedagogikk",        true),
                new fagområdeSetting("Medie- og kommunikasjonsfag",         true),
                new fagområdeSetting("Musikk",                              true),
                new fagområdeSetting("Realfag",                             true),
                new fagområdeSetting("Samfunnsfag",                         true),
                new fagområdeSetting("Språk og litteratur",                 true),
                new fagområdeSetting("Uspesifisert",                        true),
                new fagområdeSetting("Økonomi og juss",                     true),
            };
                getOppgaveSettings();

            }
            else{
                getOppgaveSettings();
            }


            foreach (var fagområdeSetting in oppgaveSettings)
            {
                fagområdeSetting.OnToggled += ToggleSelection;
            }

        }

        public static ObservableCollection<fagområdeSetting> getOppgaveSettings() {
            //return oppgaveSettings;
            return null;
        }


        void ToggleSelection(object sender, EventArgs e)
        {
            var fagområdeSetting = sender as fagområdeSetting;
            System.Diagnostics.Debug.WriteLine("{0} has been toggled to {1}", fagområdeSetting.Name, fagområdeSetting.IsSelected);
        }
    }
}
