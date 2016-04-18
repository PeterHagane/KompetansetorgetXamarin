using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controls;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Views
{
    public partial class MainPage : BaseContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void VarselButton_OnClicked(object sender, EventArgs e)
        {
            var CarouselVarsler = new CarouselVarsler();
            await Navigation.PushAsync(CarouselVarsler);
        }

        private async void StillingButton_OnClicked(object sender, EventArgs e)
        {
            var CarouselStillinger = new CarouselStillinger();
            await Navigation.PushAsync(CarouselStillinger);
        }

        private async void OppgaveButton_OnClicked(object sender, EventArgs e)
        {
            var CarouselOppgaver = new CarouselOppgaver();
            await Navigation.PushAsync(CarouselOppgaver);
        }

        private async void ViktorButton_OnClicked(object sender, EventArgs e)
        {
            var viktorsTest = new ViktorTestView();
            await Navigation.PushAsync(viktorsTest);
        }
    }
}
