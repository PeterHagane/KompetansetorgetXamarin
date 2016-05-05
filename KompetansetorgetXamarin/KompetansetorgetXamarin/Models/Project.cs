using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Project")]
    public class Project
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

        [PrimaryKey]
        public string uuid { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string webpage { get; set; }
        public string linkedInProfile { get; set; }
        public string status { get; set; }
        public string tutor { get; set; }
        public string stepsToApply { get; set; }

        // The Date/Time stored is too big numbers to be supported by SQLite
        // String comparison should be enought
        public long created { get; set; }

        public long published { get; set; }

        public long modified { get; set; }

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

        [OneToMany]
        public List<Notification> notifications { get; set; }
    }
}
