
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Degree")]
    public class Degree
    {
        [PrimaryKey]
        public string id { get; set; } // e.g bachelor
        public string name { get; set; } // e.g Bachelor

        [ManyToMany(typeof(DegreeProject))]
        public List<Project> Projects { get; set; }
    }

    public class DegreeProject
    {
        [ForeignKey(typeof(Degree))]
        public string DegreeId { get; set; }

        [ForeignKey(typeof(Project))]
        public string ProjectUuid { get; set; }
    }
}
