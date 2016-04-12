using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Models
{
        public class Oppgave
        {
            public Oppgave(string text, string detail, string image)
            {
                this.Text = text;
                this.Detail = detail;
                this.Image = image;
            }

            public string Text { private set; get; }

            public string Detail { private set; get; }

            public string Image { private set; get; }
        };
    
}
