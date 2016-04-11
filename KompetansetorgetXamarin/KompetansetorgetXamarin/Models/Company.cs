
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace KompetansetorgetXamarin.Models
{
    [Table("Company")]
    public class Company
    {
        [PrimaryKey]
        public string id { get; set; }
        public string name { get; set; }
        public string adress { get; set; }
        public string url { get; set; }
        public string facebook { get; set; }
        public string linkedIn { get; set; }
        public string description { get; set; }
        public string logo { get; set; }

        // When the object got Cached, to prevent old data
        public DateTime Cached { get; set; }


        [ManyToMany(typeof(CompanyProject), CascadeOperations = CascadeOperation.All, ReadOnly = true)]
        public List<Project> Projects { get; set; }

        [ManyToMany(typeof(CompanyJob))]
        public List<Job> Jobs { get; set; }
    }

    public class CompanyProject
    {
        [ForeignKey(typeof(Company))]
        public string CompanyId { get; set; }

        [ForeignKey(typeof(Project))]
        public string ProjectUuid { get; set; }
    }

    public class CompanyJob
    {
        [ForeignKey(typeof(Company))]
        public string CompanyId { get; set; }

        [ForeignKey(typeof(Job))]
        public string JobUuid { get; set; }
    }
}