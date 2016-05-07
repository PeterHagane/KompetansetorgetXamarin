
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace KompetansetorgetXamarin.Models
{
    [Table("Device")]
    public class Device
    {
        [PrimaryKey]
        public string id { get; set; }
        public string token { get; set; } // size: (4096)
        public string deviceType { get; set; }
        public bool tokenSent { get; set; } // represent if it has been successfully sent to the server application.



        [ForeignKey(typeof(Student))]
        public string username { get; set; } 
    }
}
