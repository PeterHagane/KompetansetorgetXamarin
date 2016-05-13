using KompetansetorgetXamarin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Controls
{
    class VMStillingSettings : BaseContentPage
    {
        public ObservableCollection<fagområdeSetting> stillingSettings { get; set; }
    }
}
