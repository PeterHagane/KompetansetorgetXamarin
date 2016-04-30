using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    public class ApprovedCourse
    {
        [PrimaryKey]
        public string id { get; set; }
        public string name { get; set; }

        [ManyToMany(typeof(ApprovedCourseProject))]
        public List<Project> projects { get; set; }

    }

    public class ApprovedCourseProject
    {
        [ForeignKey(typeof(Course))]
        public string CourseId { get; set; }

        [ForeignKey(typeof(Project))]
        public string ProjectUuid { get; set; }
    }
}
