using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;

namespace KompetansetorgetXamarin.Controllers
{
    public class JobsController
    {
        private DbContext dbContext = DbContext.GetDbContext;
        private SQLiteConnection Db;

        public JobsController()
        {
            Db = dbContext.Db;
        }
        public async void GetJobsFromServer()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs");
            var results = await response.Content.ReadAsAsync<IEnumerable<Job>>();
        }


        /// <summary>
        /// Gets the jobs from the server including their respective company name and logo
        /// </summary>
        public async void GetJobsWithExtraFromServer()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs?fields=cname&fields=clogo");
            var results = await response.Content.ReadAsAsync<IEnumerable<Job>>();
        }

        /// <summary>
        /// Gets the minimum information about a spesific job to build a proper notification in the notification list
        /// </summary>
        public async void GetJobNotificationInfoServer(string uuid)
        {
            //http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs/113bff7f-7df5-47cf-ab94-b0a198f24ee1?minnot=true
            string url = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs/" + uuid + "?minnot=true";
            var client = new HttpClient();
            var response = await client.GetAsync("url");
            var results = await response.Content.ReadAsAsync<IEnumerable<Job>>();
        }

        public Job GetJobByUuid(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    return Db.Get<Job>(uuid);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobsController - GetJobByUuid(string uuid): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobsController - GetJobByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobsController - GetJobByUuid(string uuid): End Of Stack Trace");
                return null;
            }
        }
    }
}
