using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Models
{
    class Varsel
    {
        public Varsel(string varselText, string published, string logo)
        {
            this.Text = varselText;
            this.Detail = published;
            this.Image = logo;
        }

        public string Text { private set; get; }

        public string Detail { private set; get; }

        public string Image { private set; get; }
    }
}
