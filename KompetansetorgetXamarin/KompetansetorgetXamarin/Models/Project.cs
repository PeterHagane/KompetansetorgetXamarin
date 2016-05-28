using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Project")]
    public class Project : Advert
    {
        public Project()
        {
            /*
            companies = new HashSet<Company>();
            contacts = new HashSet<Contact>();
            courses = new HashSet<Course>();
            studyGroups = new HashSet<StudyGroup>();
            jobTypes = new HashSet<JobType>();
            degrees = new HashSet<Degree>(); */
        }
        public string status { get; set; }
        public string tutor { get; set; }

        [ManyToMany(typeof(ContactProject))]
        public List<Contact> contacts { get; set; }

        [ManyToMany(typeof(CompanyProject), CascadeOperations = CascadeOperation.All)]
        public List<Company> companies { get; set; }

        [ManyToMany(typeof(CourseProject))]
        public List<Course> courses { get; set; }

        [ManyToMany(typeof(ApprovedCourseProject))]
        public List<Course> approvedCourses { get; set; }

        [ManyToMany(typeof(DegreeProject))]
        public List<Degree> degrees { get; set; }

        [ManyToMany(typeof(JobTypeProject))]
        public List<JobType> jobTypes { get; set; }

        [ManyToMany(typeof(StudyGroupProject))]
        public List<StudyGroup> studyGroups { get; set; }



    }
}
