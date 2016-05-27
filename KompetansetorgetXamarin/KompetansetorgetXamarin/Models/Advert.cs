using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    public abstract class Advert
    {
        [PrimaryKey]
        public string uuid { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string webpage { get; set; }
        public string linkedInProfile { get; set; }
        public string stepsToApply { get; set; }

        // The Date/Time stored is too big numbers to be supported by SQLite
        // String comparison should be enought
        public long created { get; set; }

        public long published { get; set; }

        public long modified { get; set; }

        [OneToMany]
        public List<Notification> notifications { get; set; }
    }
}
