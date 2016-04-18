using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using Xamarin.Forms;

namespace KompetansetorgetXamarin.Models
{
    [Table("Location")]
    public class Location
    {
        [PrimaryKey]
        public string id { get; set; }
        public string name { get; set; }

        [ManyToMany(typeof(LocationJob))]
        public List<Job> Jobs { get; set; }
    }

    public class LocationJob
    {
        [ForeignKey(typeof(Location))]
        public string LocationId { get; set; }

        [ForeignKey(typeof(Job))]
        public string JobUuid { get; set; }

    }
}

