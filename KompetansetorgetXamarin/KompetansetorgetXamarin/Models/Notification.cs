using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    public class Notification
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [ForeignKey(typeof(Project))]
        public string projectUuid { get; set; }

        [ForeignKey(typeof(Job))]
        public string jobUuid { get; set; }

        // PERHAPS IT SHOULD ALSO CONTAIN A DATE FOR WHEN IT GOT STORED?
    }
}
