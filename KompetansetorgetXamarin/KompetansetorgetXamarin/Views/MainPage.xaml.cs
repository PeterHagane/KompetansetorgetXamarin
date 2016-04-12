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

        private async void ViktorButton_OnClicked(object sender, EventArgs e)
        {
            var ViktorTestView = new ViktorTestView();
            await Navigation.PushAsync(ViktorTestView);
        }

        private async void CarouselButton_OnClicked(object sender, EventArgs e)
        {
            var ListsCarouselPage = new ListsCarouselPage();
            await Navigation.PushAsync(ListsCarouselPage);
        }
    }
}
