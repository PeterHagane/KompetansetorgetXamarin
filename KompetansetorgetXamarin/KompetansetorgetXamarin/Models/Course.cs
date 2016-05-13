using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Course")]
    public class Course
    {
        [PrimaryKey]
        public string id { get; set; }
        public string name { get; set; }
        public bool filterChecked { get; set; }

        [ManyToMany(typeof(CourseProject))]
        public List<Project> Projects { get; set; }

    }

    public class CourseProject
    {
        [ForeignKey(typeof(Course))]
        public string CourseId { get; set; }

        [ForeignKey(typeof(Project))]
        public string ProjectUuid { get; set; }
    }
}
