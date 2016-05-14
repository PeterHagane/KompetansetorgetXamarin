using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.Utility;
using Newtonsoft.Json;
using SQLite.Net;

namespace KompetansetorgetXamarin.Controllers
{
    public class CompaniesController
    {
        /// <summary>
        /// Deserializes Company from web api data.
        /// 
        /// IMPORTANT: Only values used for notification list will work at this implementation.
        /// </summary>
        /// <param name="companyDict"></param>
        /// <returns></returns>
        public Company DeserializeCompany(Dictionary<string, object> companyDict)
        {
            Company company = new Company();
            System.Diagnostics.Debug.WriteLine("companyDict created");

            company.id = companyDict["id"].ToString();
            company.id = Hasher.Base64Encode(company.id);

            if (companyDict.ContainsKey("name"))
            {
                company.name = companyDict["name"].ToString();
            }

            if (companyDict.ContainsKey("modified"))
            {
                company.name = companyDict["modified"].ToString();
            }

            if (companyDict.ContainsKey("logo"))
            {
                company.logo = companyDict["logo"].ToString();
            }

            return company;
        } 
    }
}
