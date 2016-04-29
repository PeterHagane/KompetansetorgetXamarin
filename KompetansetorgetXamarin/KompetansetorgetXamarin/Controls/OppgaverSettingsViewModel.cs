using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.Controls
    //ViewModel
{
    class OppgaverSettingsViewModel
    {
        public static ObservableCollection<ModelSwitchBindings> lights { get; set; }

        static OppgaverSettingsViewModel()
        {
            OppgaverSettingsViewModel.lights = ModelOppgaverSettingsChanged.getLights();
        }
    }
}