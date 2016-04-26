
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SQLite;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Student")]
    public class Student
    {
        public Student()
        {
           // studyGroups = new HashSet<StudyGroup>();
            //Devices = new List<Device>();
        }
       [PrimaryKey, Column("username")]
        public string username { get; set; }

        public string name { get; set; }

        public string accessToken { get; set; }

        public string email { get; set; }

        [ManyToMany(typeof(StudyGroupStudent))]
        public List<StudyGroup> studyGroup { get; set; }

        [OneToMany]
        public List<Device> devices { get; set; }

    }


}

