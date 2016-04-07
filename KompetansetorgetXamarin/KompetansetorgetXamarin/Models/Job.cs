using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Job")]
    public class Job
    {
        public Job()
        {
            /*
            companies = new HashSet<Company>();
            contacts = new HashSet<Contact>();
            locations = new HashSet<Location>();
            studyGroups = new HashSet<StudyGroup>();
            jobTypes = new HashSet<JobType>();
            */
        }

        [PrimaryKey]
        public string uuid { get; set; }

        public string title { get; set; }
        public string description { get; set; }
        public string webpage { get; set; }
        public string linkedInProfile { get; set; }

        public string stepsToApply { get; set; }

        // The Date/Time stored is too big numbers to be supported by SQLite
        // String comparison should be enought
        public string expiryDate { get; set; }

        public string created { get; set; }

        public string published { get; set; }

        public string modified { get; set; }

        // When the object got Cached, to prevent storing old data
        public DateTime Cached { get; set; }

        [ManyToMany(typeof(CompanyJob))]
        public List<Company> Companies { get; set; }

        [ManyToMany(typeof(ContactJob))]
        public List<Contact> Contacts { get; set; }

        [ManyToMany(typeof(LocationJob))]
        public List<Location> Locations { get; set; }

        [ManyToMany(typeof(JobTypeJob))]
        public List<JobType> JobTypes { get; set; }

        [ManyToMany(typeof(StudyGroupJob))]
        public List<StudyGroup> StudyGroups { get; set; }
    }
}
