using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Models
{
    class Varsel
    {
        public Varsel(string newtype, string bedrift, string expiry, string image)
        {
            if (newtype == "Stilling")
            {
                this.Text = "Ny stilling fra " + bedrift + "!";
            }
            else if (newtype == "Oppgave")
            {
                this.Text = "Ny oppgave fra " + bedrift + "!";
            }

            this.Detail = "Frist: " + expiry;
            this.Image = image;
        }

        public string Text { private set; get; }

        public string Detail { private set; get; }

        public string Image { private set; get; }
    }
}
