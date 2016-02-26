using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace KompetansetorgetXamarin.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void VarselButton_OnClicked(object sender, EventArgs e)
        {
            var VarselListPage = new VarselListPage();
            await Navigation.PushAsync(VarselListPage);
        }

        private async void ViktorButton_OnClicked(object sender, EventArgs e)
        {
            var ViktorTestView = new ViktorTestView();
            await Navigation.PushAsync(ViktorTestView);
        }

    }
}
