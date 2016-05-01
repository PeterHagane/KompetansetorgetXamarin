using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KompetansetorgetXamarin
{
    public class DataPassed
    {
        public DataPassed(string Title, DateTime Date, Color Image)
        {
            this.Date = Date;
            this.Title = Title;
            this.Image = Image;
        }

        public string Title { private set; get; }

        public DateTime Date { private set; get; }

        public Color Image { private set; get; }

    }
}