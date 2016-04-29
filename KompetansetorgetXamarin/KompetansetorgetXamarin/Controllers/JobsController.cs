using System;
using System.Collections;
using System.Collections.Generic;

using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using Newtonsoft.Json;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;

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

        /// <summary>
        /// Gets the job with the spesific uuid. 
        /// If no matching Job is found it returns null.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Job GetJobByUuid(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    return Db.GetWithChildren<Job>(uuid);
                    //return Db.Get<Job>(uuid);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobController - GetJobByUuid(string uuid): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobController - GetJobByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobController - GetJobByUuid(string uuid): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Inserts the job and its respective children (only Company and CompanyJob) 
        /// into the database.
        /// </summary>
        /// <param name="job"></param>
        /// <returns>Returns true if the job was inserted, returns false if a job with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertJob(Job job)
        {
            System.Diagnostics.Debug.WriteLine("JobController InsertJob(Job job): initiated");
            if (CheckIfJobExist(job.uuid))
            {
                return false;
            }

            //Job did not exist, safe to insert.
            CompaniesController cc = new CompaniesController();

            foreach (Company c in job.companies)
            {
                cc.InsertCompany(c);
            }

            lock (DbContext.locker)
            {
                Db.Insert(job);
                // Db.InsertOrReplaceWithChildren(job, recursive: true);
            }

            // This could perhaps be done in the above foreach loop, 
            // but because of lack of concurrency control in SQLite its done in its own loop.
            foreach (Company c in job.companies)
            {
                CompanyJob cp = new CompanyJob();
                cp.JobUuid = job.uuid;
                cp.CompanyId = c.id;
                lock (DbContext.locker)
                {
                    Db.Insert(cp);
                    // Db.InsertOrReplaceWithChildren(job, recursive: true);
                }
            }
            // Job was successfully inserted
            return true;
        }

        /// <summary>
        /// Inserts a new Job with the param as primary key 
        /// </summary>
        /// <param name="uuid">The new Jobs primary key</param>
        /// <returns>Returns true if the job was inserted, returns false if a job with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertJob(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("JobController InsertJob(string uuid): initiated");
            if (CheckIfJobExist(uuid))
            {
                System.Diagnostics.Debug.WriteLine("JobController InsertJob(string uuid): Job already exists");
                return false;
            }

            //Job did not exist so it will be inserted
            Job j = new Job();
            j.uuid = uuid;
            lock (DbContext.locker)
            {
                Db.Insert(j);
                //Db.InsertOrReplaceWithChildren(j, recursive: true);
                System.Diagnostics.Debug.WriteLine("JobController - InsertJob(string uuid): Job Inserted");
                return true;
            }
        }

        /// <summary>
        /// Updates the Job from the servers REST Api.
        /// 
        /// This implementation also get the minimum data from the related
        /// Companies to build a proper notification list.
        /// </summary>
        /// <param name="uuid"></param>
        public async void UpdateJobFromServer(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer(string uuid): initiated");
            string adress = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs/" + uuid + "?minnot=true";
            System.Diagnostics.Debug.WriteLine("UpdateJobFromServer: var url = " + adress);

            Uri url = new Uri(adress);
            var client = new HttpClient();
            System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer: HttpClient created");

            string jsonString = null;
            try
            {
                var response = await client.GetAsync(url);
                System.Diagnostics.Debug.WriteLine("UpdateJobFromServer response " + response.StatusCode.ToString());
                //results = await response.Content.ReadAsAsync<IEnumerable<Job>>();
                jsonString = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer: End Of Stack Trace");
                return;
            }

            Job job = Deserialize(jsonString);
            UpdateJob(job);

        }



        /// <summary>
        /// Updates an entry in the Job table. 
        /// If it doesnt already exist InsertJob will be called.
        /// </summary>
        /// <param name="job"></param>
        public void UpdateJob(Job job)
        {
            if (!CheckIfJobExist(job.uuid))
            {
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: There was no stored record of Job.");
                InsertJob(job);
            }

            System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: There was a record of job in the database.");

            //Job do exist.
            CompaniesController cc = new CompaniesController();
            foreach (Company c in job.companies)
            {
                if (!cc.InsertCompany(c))
                {
                    System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: company already exists: Calling UpdateCompany.");

                    cc.UpdateCompany(c);
                }
            }
            try
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: Before Updating job.");

                    Db.Update(job);
                    System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: After Updating job.");

                    // Db.InsertOrReplaceWithChildren(job, recursive: true);
                    //Db.UpdateWithChildren(job);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: Job update failed");
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: End Of Stack Trace");
            }


            // This should perhaps be done above in the other loop, but because of lack of concurrency its in its own loop.
            foreach (Company c in job.companies)
            {
                CompanyJob cp = new CompanyJob();
                cp.JobUuid = job.uuid;
                cp.CompanyId = c.id;
                try
                {
                    lock (DbContext.locker)
                    {
                        System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: Inserting CompanyJob.");
                        Db.Insert(cp);
                        // Db.InsertOrReplaceWithChildren(job, recursive: true);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: CompanyJob Insertion failed");
                    System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: Exception msg: " + e.Message);
                    System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: Stack Trace: \n" + e.StackTrace);
                    System.Diagnostics.Debug.WriteLine("JobController - UpdateJob: End Of Stack Trace");
                }
            }


        }

        /// <summary>
        /// Gets a list of all companies that are related to the spesific Job
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public List<Company> GetAllCompaniesRelatedToJob(Job job)
        {
            lock (DbContext.locker)
            {
                return Db.Query<Company>("Select * from Company"
                                          + " inner join CompanyJob on Company.id = CompanyJob.CompanyId"
                                          + " inner join Job on CompanyJob.JobUuid = Job.uuid"
                                          + " where Job.uuid = ?", job.uuid);
            }
        }

        /// <summary>
        /// Checks if there already is an entry of that Jobs primary key
        /// In the database.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns>Returns true if exist, false if it doesnt exist.</returns>
        public bool CheckIfJobExist(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Job>(uuid);
                }
                System.Diagnostics.Debug.WriteLine("JobController - CheckIfJobExist(string uuid): Job Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobController - CheckIfJobExist(string uuid): DB entry of job doesnt exists");
                System.Diagnostics.Debug.WriteLine("JobController - GetJobByUuid(string uuid): Exception msg: " + e.Message);
                // System.Diagnostics.Debug.WriteLine("JobController - GetJobByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                // System.Diagnostics.Debug.WriteLine("JobController - GetJobByUuid(string uuid): End Of Stack Trace");
                return false;
            }
        }

        /// <summary>
        /// Gets a job based on optional filters.
        /// Current implementation supports only 1 key on the filter param!
        /// </summary>
        /// <param name="studyGroups">studyGroups can be a list of numerous studygroups ex: helse, idrettsfag, datateknologi </param>
        /// <param name="sortBy">published - oldest to newest
        ///                      -published - newest to oldest
        ///                      expirydate - descending order
        ///                      -expirydate - ascending order
        /// </param>
        /// <param name="filter">A dictionary where key can be: titles (values:title of the job), types (values: deltid, heltid, etc...),
        ///                      locations (values: vestagder, austagder), . 
        ///                      Supports only 1 key at this current implementation!</param>
        /// <returns></returns>
        public async Task<IEnumerable<Job>> GetJobsBasedOnFilter(List<string> studyGroups = null,
            string sortBy = "", Dictionary<string, string> filter = null)
        {
            string adress = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs";
            string queryParams = "";
            if (studyGroups != null)
            {
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - studyGroups.Count(): " + studyGroups.Count());
                for (int i = 0; i < studyGroups.Count(); i++)
                {
                    if (i == 0)
                    {
                        queryParams = "?studygroups=" + studyGroups[i];
                    }

                    else
                    {
                        queryParams += "&studygroups=" + studyGroups[i];
                    }
                }
            }

            if (filter != null && filter.Count == 1)
            {
                if (string.IsNullOrWhiteSpace(queryParams))
                {
                    queryParams = "?";
                }
                else queryParams += "&";
                string category = filter.Keys.ToArray()[0];
                // removes whitespaces from a potential user typed parameters like title search.
                // And replaces them with +
                string value = filter[category].Replace(" ", "+");
                queryParams += category + "=" + value;
            }

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                if (string.IsNullOrWhiteSpace(queryParams))
                {
                    queryParams = "?";
                }
                else queryParams += "&";
                queryParams += "sortby=" + sortBy;
            }


            adress += queryParams;
            Uri url = new Uri(adress);
            System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - adress: " + adress);
            var client = new HttpClient();
            string jsonString = null;
            try
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter response " + response.StatusCode.ToString());
                jsonString = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: End Of Stack Trace");
                return null;
                // TODO Implement local db query for cached data.
            }

            IEnumerable<Job> jobs = DeserializeMany(jsonString);
            return jobs;
        }

        /// <summary>
        /// Deserializes a json formated string containing multiple Project objects
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private IEnumerable<Job> DeserializeMany(string jsonString)
        {
            System.Diagnostics.Debug.WriteLine("JobsController - DeserializeMany Initialized");

            List<object> serializedJobs =
                JsonConvert.DeserializeObject<List<object>>(jsonString);
            //System.Diagnostics.Debug.WriteLine("ProjectController - jsonString: " + jsonString);

            //List<string> serializedProjects =
            //    JsonConvert.DeserializeObject<List<string>>(jsonString);

            System.Diagnostics.Debug.WriteLine("JobsController - serializedProjects.Count(): " + serializedJobs.Count());

            List<Job> jobs = new List<Job>();
            foreach (var serializedJob in serializedJobs)
            {
                jobs.Add(Deserialize(serializedJob.ToString()));
            }
            return jobs;
        }

        /// <summary>
        /// Deseriliazes a singular Jobs with childrem. 
        /// This method is not fully completed and should be used with caution.
        /// </summary>
        /// <param name="jsonString">Serialized data contain information about job and its children</param>
        /// <returns>A deserialized Jobs object</returns>
        private Job Deserialize(string jsonString)
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            System.Diagnostics.Debug.WriteLine("DeserializeApiData. Printing Key Value:");

            string[] keys = dict.Keys.ToArray();

            Job j = new Job();
            j.companies = new List<Company>();

            CompaniesController cp = new CompaniesController();

            foreach (var key in keys)
            {
                System.Diagnostics.Debug.WriteLine("key: " + key);
                System.Diagnostics.Debug.WriteLine("value: " + dict[key].ToString());
                /*
                if (!key.Equals("companies") || !key.Equals("courses") || !key.Equals("degrees")
                    || !key.Equals("jobTypes") || !key.Equals("studyGroup")) {} */
                if (key.Equals("uuid"))
                {
                    j.uuid = dict[key].ToString();
                }
                if (key.Equals("title"))
                {
                    j.title = dict[key].ToString();
                }
                if (key.Equals("webpage"))
                {
                    j.webpage = dict[key].ToString();
                }

                if (key.Equals("expiryDate"))
                {
                    j.expiryDate = dict[key].ToString();
                }

                if (key.Equals("modified"))
                {
                    j.modified = dict[key].ToString();
                }
                
                if (key.Equals("published"))
                {
                    j.published = dict[key].ToString();
                }

                if (key.Equals("companies"))
                {
                    // if not true then company already exist and needs to be updated.
                    CompaniesController cc = new CompaniesController();
                    IEnumerable companies = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    System.Diagnostics.Debug.WriteLine("companies created");
                    foreach (var comp in companies)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> companyDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(comp.ToString());
                        Company company = cc.DeserializeCompany(companyDict);
                        j.companies.Add(company);
                        System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: After j.companies.Add(company)");

                    }
                }

                /*
                if (key.Equals("courses"))
                {
                    
                    Same as companies implementation
                    
                }

                if (key.Equals("studyGroups"))
                {
                    
                    Same as companies implementation
                    
                }

                if (key.Equals("locations"))
                {
                    
                    Same as companies implementation
                    
                }


                if (key.Equals("jobTypes"))
                {
                    
                    Same as companies implementation
                    
                }

                */

            }
            return j;
        }

    }
}
