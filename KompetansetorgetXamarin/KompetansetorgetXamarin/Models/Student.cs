using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin.Models
{
    public class Proficiency
    {
        public int Id { get; set; }
        public bool Administasjon { get; set; }
        public bool Datateknologi { get; set; }
        public bool Helse { get; set; }
        public bool Historie { get; set; }
        public bool Ingenior { get; set; }
        public bool Idrettsfag { get; set; }
        public bool Kunstfag { get; set; }
        public bool Lerer { get; set; }
        public bool Medie { get; set; }
        public bool Musikk { get; set; }
        public bool Realfag { get; set; }
        public bool Samfunnsfag { get; set; }
        public bool Sprak { get; set; }
        public bool Okonomi { get; set; }
        public bool Uspesifisert { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public int ProficiencyId { get; set; }
        public Proficiency Proficiency { get; set; }
    }
}