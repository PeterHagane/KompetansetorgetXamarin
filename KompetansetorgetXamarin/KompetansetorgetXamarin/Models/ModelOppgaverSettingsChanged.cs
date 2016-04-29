using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using Xamarin.Forms;
using KompetansetorgetXamarin.Views;

namespace KompetansetorgetXamarin.Models
{
    class ModelOppgaverSettingsChanged
    {
        static ModelOppgaverSettingsChanged()
        {
        }

        public static void persist(List<ModelSwitchBindings> lights)
        {
            //do something here
        }

        public static ObservableCollection<ModelSwitchBindings> getLights()
        {
            ObservableCollection<ModelSwitchBindings> lights = new ObservableCollection<ModelSwitchBindings>() {
                new ModelSwitchBindings(true, "Administrasjon og ledelse",          Color.Blue, "0"),
                new ModelSwitchBindings(true, "Datateknologi",                      Color.Blue, "1"),
                new ModelSwitchBindings(true, "Helse- og sosialfag",                Color.Blue, "2"),
                new ModelSwitchBindings(true, "Historie, filosofi og religion",     Color.Blue, "3"),
                new ModelSwitchBindings(true, "Idrettsfag",                         Color.Blue, "4"),
                new ModelSwitchBindings(true, "Ingeniør- og teknologiske fag",      Color.Blue, "5"),
                new ModelSwitchBindings(true, "Kunstfag",                           Color.Blue, "6"),
                new ModelSwitchBindings(true, "Lærerutdanning og pedagogikk",       Color.Blue, "7"),
                new ModelSwitchBindings(true, "Medie- og kommunikasjonsfag",        Color.Blue, "8"),
                new ModelSwitchBindings(true, "Musikk",                             Color.Blue, "9"),
                new ModelSwitchBindings(true, "Realfag",                            Color.Blue, "10"),
                new ModelSwitchBindings(true, "Samfunnsfag",                        Color.Blue, "11"),
                new ModelSwitchBindings(true, "Språk og litteratur",                Color.Blue, "12"),
                new ModelSwitchBindings(true, "Uspesifisert",                       Color.Blue, "13"),
                new ModelSwitchBindings(true, "Økonomi og juss",                    Color.Blue, "14"),

            };

            return lights;
        }


    }
}
