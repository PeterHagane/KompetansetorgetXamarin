
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Contact")]
    public class Contact
    {
        [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public string phone { get; set; }
        public string email { get; set; }

        [ManyToMany(typeof(ContactJob))]
        public List<Job> Jobs { get; set; }

        [ManyToMany(typeof(ContactProject))]
        public List<Project> Projects { get; set; }

    }

    public class ContactJob
    {
        [ForeignKey(typeof(Contact))]
        public int ContactId { get; set; }

        [ForeignKey(typeof(Job))]
        public string JobUuid { get; set; }
    }

    public class ContactProject
    {
        [ForeignKey(typeof(Contact))]
        public int ContactId { get; set; }

        [ForeignKey(typeof(Project))]
        public string ProjectUuid { get; set; }

    }

}