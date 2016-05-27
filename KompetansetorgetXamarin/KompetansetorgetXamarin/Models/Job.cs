using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Job")]
    public class Job : Advert
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

        // The Date/Time stored is too big numbers to be supported by SQLite
        // String comparison should be enought
        public long expiryDate { get; set; }

        [ManyToMany(typeof(CompanyJob))]
        public List<Company> companies { get; set; }

        [ManyToMany(typeof(ContactJob))]
        public List<Contact> contacts { get; set; }

        [ManyToMany(typeof(LocationJob))]
        public List<Location> locations { get; set; }

        [ManyToMany(typeof(JobTypeJob))]
        public List<JobType> jobTypes { get; set; }

        [ManyToMany(typeof(StudyGroupJob))]
        public List<StudyGroup> studyGroups { get; set; }
    }
}
