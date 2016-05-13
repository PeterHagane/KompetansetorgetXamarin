using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{

    /// <summary>
    /// This Class maps a table that represents jobs types like fulltime, part time, apprenticeship etc..
    /// </summary>
    [Table("JobType")]
    public class JobType
    {
        [PrimaryKey]
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool filterChecked { get; set; }

        [ManyToMany(typeof(JobTypeJob))]
        public List<Job> Jobs { get; set; }

        [ManyToMany(typeof(JobTypeProject))]
        public List<Project> Projects { get; set; }

    }

    public class JobTypeJob
    {
        [ForeignKey(typeof(JobType))]
        public string JobTypeId { get; set; }

        [ForeignKey(typeof(Job))]
        public string JobUuid { get; set; }
    }

    public class JobTypeProject
    {
        [ForeignKey(typeof(JobType))]
        public string JobTypeId { get; set; }

        [ForeignKey(typeof(Project))]
        public string ProjectUuid { get; set; }
    }
}