using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Models
{
    class Varsel
    {
        public Varsel(string varselText, string published, string logo, string uuid, string type, string webpage)
        {
            this.Text = varselText;
            this.Detail = published;
            this.Image = logo;
            this.Uuid = uuid;
            this.Type = type;
            this.Webpage = webpage;
        }

        public string Text { private set; get; }

        public string Detail { private set; get; }

        public string Image { private set; get; }

        public string Uuid { get; set; }

        public string Type { get; set; }

        public string Webpage { get; set; }
    }
}
