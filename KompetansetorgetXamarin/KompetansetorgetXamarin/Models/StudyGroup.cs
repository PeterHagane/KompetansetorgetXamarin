using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("StudyGroup")]
    public class StudyGroup
    {
        [PrimaryKey]
        public string id { get; set; } // e.g administrasjon
        public string name { get; set; }
        public bool filterChecked { get; set; }

        [ManyToMany(typeof(StudyGroupStudent))]
        public List<Student> Students { get; set; }

        [ManyToMany(typeof(StudyGroupJob))]
        public List<Job> Jobs { get; set; }

        [ManyToMany(typeof(StudyGroupProject))]
        public List<Project> Projects { get; set; }

    }

    public class StudyGroupProject
    {
        [ForeignKey(typeof(Project))]
        public string ProjectUuid { get; set; }

        [ForeignKey(typeof(StudyGroup))]
        public string StudyGroupId { get; set; }
    }

    public class StudyGroupJob
    {
        [ForeignKey(typeof(Job))]
        public string JobUuid { get; set; }

        [ForeignKey(typeof(StudyGroup))]
        public string StudyGroupId { get; set; }
    }

    public class StudyGroupStudent
    {
        [ForeignKey(typeof(Student))]
        public string StudentUsername { get; set; }

        [ForeignKey(typeof(StudyGroup))]
        public string StudyGroupId { get; set; }
    }
}
