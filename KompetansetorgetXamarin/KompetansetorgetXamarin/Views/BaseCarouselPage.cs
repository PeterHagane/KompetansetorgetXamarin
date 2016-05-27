using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Views
{
    public class BaseCarouselPage : CarouselPage
    {
        public void GoToLogin()
        {
            new Authenticater();
        }

        /// <summary>
        /// Active to logout.
        /// </summary>
        protected async void Logout(object sender, EventArgs e)
        {
            var logout = await DisplayAlert("Logg ut", "Ønsker du å logge ut?", "Ja", "Nei");
            if (logout == true)
            {
                DbStudent dbStudent = new DbStudent();
                dbStudent.DeleteAllStudents();
                GoToLogin();
            }
        }
    }
}
