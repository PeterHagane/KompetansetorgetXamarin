using System;
using System.Collections;
using System.Collections.Generic;

using System.Net.Http;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controls;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using Newtonsoft.Json;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;

namespace KompetansetorgetXamarin.Controllers
{
    public class JobsController : BaseController
    {
        public JobsController()
        {
            Adress += "v1/jobs";
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
        /// Returns true if there are any new or modified jobs.
        /// </summary>
        /// <returns></returns>
        private async Task<bool?> CheckServerForNewData(List<string> studyGroups = null, Dictionary<string, string> filter = null)
        {
            //"api/v1/jobs/lastmodifed"
            string queryParams = CreateQueryParams(studyGroups, null, filter);
            string adress = Adress + "/" + "lastmodified" + queryParams;
            System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData - adress: " + adress);
            Uri url = new Uri(adress);
            System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData - url.ToString: " + url.ToString());

            var client = new HttpClient();
            StudentsController sc = new StudentsController();
            string accessToken = sc.GetStudentAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Authenticater.Authorized = false;
                return null;
            }
            System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData - bearer: " + accessToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            string jsonString = null;
            try
            {
                var response = await client.GetAsync(url);
                System.Diagnostics.Debug.WriteLine("CheckServerForNewData response " + response.StatusCode.ToString());
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("JobsController - CheckServerForNewData failed due to lack of Authorization");
                    Authenticater.Authorized = false;
                }

                else if (response.StatusCode == HttpStatusCode.OK) { 
                    //results = await response.Content.ReadAsAsync<IEnumerable<Job>>();
                    jsonString = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: End Of Stack Trace");
                return null;
            }
            if (jsonString != null)
            {
                // using <string, object> instead of <string, string> makes the date be stored in the right format when using .ToString()
                Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

                if (dict.ContainsKey("uuid") && dict.ContainsKey("modified") && dict.ContainsKey("amountOfJobs"))
                {
                    string uuid = dict["uuid"].ToString();
                    DateTime dateTime = (DateTime)dict["modified"];
                    DateTime modified = TrimMilliseconds(dateTime);
                    int amountOfJobs = 0;
                    try
                    {
                        amountOfJobs = Int32.Parse(dict["amountOfJobs"].ToString());
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: await client.GetAsync(\"url\") Failed");
                        System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: Exception msg: " + ex.Message);
                        System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: Stack Trace: \n" + ex.StackTrace);
                        System.Diagnostics.Debug.WriteLine("JobController - CheckServerForNewData: End Of Stack Trace");
                        return null;
                    }
                    bool existInDb = ExistsInDb(uuid, modified);
                    if (!existInDb)
                    {
                        return existInDb;
                    }
                    int localDbCount = GetJobsFromDbBasedOnFilter(studyGroups, filter).Count();
                    System.Diagnostics.Debug.WriteLine("CheckServerForNewData: localDbCount: " + localDbCount + " serverCount: " + amountOfJobs);
                    // if there is a greater amount of jobs on that search filter then the job that exist 
                    // in the database has been inserted throught another search filter
                    if (amountOfJobs > localDbCount)
                    {
                        return !existInDb;
                    }
                    return existInDb;                    
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if there exist an entry in the database matching 
        /// the jobs uuid and modified.
        /// Returns false if not.
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        private bool ExistsInDb(string uuid, DateTime modified)
        {
            lock (DbContext.locker)
            {
                int rowsAffected = Db.Query<Job>("Select * from Job"
                                 + " where Job.uuid = ?"
                                 + " and Job.modified = ?", uuid, modified).Count;
                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine("JobController - ExistsInDb: " + "true");
                    return true;
                }
                System.Diagnostics.Debug.WriteLine("JobController - ExistsInDb: " + "false");
                return false;
            }
        }

        /// <summary>
        /// Updates the Job from the servers REST Api.
        /// 
        /// This implementation also get the minimum data from the related
        /// Companies to build a proper notification list.
        /// </summary>
        /// <param name="uuid"></param>
        public async Task UpdateJobFromServer(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer(string uuid): initiated");
                       //as in minimumInformationForNotifications=true
            string adress = Adress + "/" + uuid + "?minnot=true";
            System.Diagnostics.Debug.WriteLine("UpdateJobFromServer: var url = " + adress);

            Uri url = new Uri(adress);
            var client = new HttpClient();
            System.Diagnostics.Debug.WriteLine("JobController - UpdateJobFromServer: HttpClient created");
            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Authenticater.Authorized = false;
                return;
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
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
        /// Gets a list of all companies that are related to each Job in the list.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public List<Job> GetAllCompaniesRelatedToJobs(List<Job> jobs)
        {
            foreach (var job in jobs)
            {
                job.companies = GetAllCompaniesRelatedToJob(job);
            }
            return jobs;
        }

        public void DeleteOldJobsOnExpiryDate()
        {
            
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

        private string CreateQueryParams(List<string> studyGroups = null,
            string sortBy = "", Dictionary<string, string> filter = null)
        {
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

            if (filter != null)
            {
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - amount of filters: " + filter.Keys.ToArray().Length);
                foreach (var category in filter.Keys.ToArray())
                {
                    if (string.IsNullOrWhiteSpace(queryParams))
                    {
                        queryParams = "?";
                    }
                    else queryParams += "&";
                    // removes whitespaces from a potential user typed parameters like title search.
                    // And replaces them with +
                    string value = filter[category].Replace(" ", "+");
                    queryParams += category + "=" + value;
                }
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (string.IsNullOrWhiteSpace(queryParams))
                {
                    queryParams = "?";
                }
                else queryParams += "&";
                queryParams += "sortby=" + sortBy;
            }
            return queryParams;
        }

        /// <summary>
        /// Gets a job based on optional filters.
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
            //string adress = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/jobs";
            string queryParams = CreateQueryParams(studyGroups, sortBy, filter);
            bool ? dataExist = await CheckServerForNewData(studyGroups, filter);
            System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - dataExist" + dataExist);
            if (dataExist != null)
            {
                if ((bool)dataExist)
                {
                    IEnumerable<Job> filteredJobs = GetJobsFromDbBasedOnFilter(studyGroups, filter);
                    filteredJobs = GetAllCompaniesRelatedToJobs(filteredJobs.ToList());
                    return filteredJobs;
                }
            }

            Uri url = new Uri(Adress + queryParams);
            System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - url: " + url.ToString());

            StudentsController sc = new StudentsController();
            
            string accessToken = sc.GetStudentAccessToken();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Authenticater.Authorized = false;
                return null;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            string jsonString = null;
            IEnumerable<Job> jobs = null;
            try
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter response " + response.StatusCode.ToString());
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent failed due to lack of Authorization");
                    Authenticater.Authorized = false;
                    
                }

                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    jsonString = await response.Content.ReadAsStringAsync();
                    jobs = DeserializeMany(jsonString);                   
                }

                else
                {
                    System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - Using the local database");
                    jobs = GetJobsFromDbBasedOnFilter(studyGroups, filter);
                    jobs = GetAllCompaniesRelatedToJobs(jobs.ToList());
                }
                return jobs;
            }
            catch (Exception e)
            {
                // Hack workaround if mobil data and wifi is turned off
                try
                {
                    jobs = GetJobsFromDbBasedOnFilter(studyGroups, filter);
                    jobs = GetAllCompaniesRelatedToJobs(jobs.ToList());
                    return jobs;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: await client.GetAsync(\"url\") Failed");
                    System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: Exception msg: " + ex.Message);
                    System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: Stack Trace: \n" + ex.StackTrace);
                    System.Diagnostics.Debug.WriteLine("JobsController - GetJobsBasedOnFilter: End Of Stack Trace");
                    System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - Using the local database");
                    return null;
                }
            }
        }

        /// <summary>
        ///  Used if the web api is unavailable (not 401)
        /// </summary>
        /// <param name="studyGroups">studyGroups can be a list of numerous studygroups ex: helse, idrettsfag, datateknologi </param>
        /// <param name="filter">A dictionary where key can be: titles (values:title of the job), types (values: deltid, heltid, etc...),
        ///                      locations (values: vestagder, austagder).
        ///                      </param>
        /// <returns></returns>
        public IEnumerable<Job> GetJobsFromDbBasedOnFilter(List<string> studyGroups = null, Dictionary<string, string> filter = null)
        {
            string query = "SELECT * FROM Job";

            if (studyGroups != null && filter == null)
            {
                for (int i = 0; i < studyGroups.Count; i++)
                {
                    if (i == 0)
                    {
                        query += " INNER JOIN StudyGroupJob ON Job.uuid = StudyGroupJob.JobUuid "
                                 + "INNER JOIN StudyGroup ON StudyGroupJob.StudyGroupId = StudyGroup.id "
                                 + "WHERE StudyGroup.id = '" + studyGroups[i] + "'";
                    }
                    else
                    {
                        query += " OR StudyGroup.id = '" + studyGroups[i] + "'";
                    }
                }

                System.Diagnostics.Debug.WriteLine("if (studyGroups != null && filter == null)");
                System.Diagnostics.Debug.WriteLine("query: " + query); 
                lock (DbContext.locker)
                {
                    return Db.Query<Job>(query);
                }
            }

            if (filter != null && studyGroups == null)
            {
                string joins = "";
                string whereAnd = "";

                string prepValue = "";

                foreach (var filterType in filter.Keys.ToArray())
                {
                    string value = filter[filterType];

                    if (filterType == "titles")
                    {
                        prepValue = filter[filterType];

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Job.title = ?";
                        }
                        else
                        {
                            whereAnd += " AND Job.title = ?";
                        }
                    }

                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeJob ON Job.uuid = JobTypeJob.JobUuid"
                               + " INNER JOIN JobType ON JobTypeJob.JobTypeId = JobType.id";

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE JobType.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND JobType.id = '" + value + "'";
                        }
                    }

                    if (filterType == "locations")
                    {
                        joins += " INNER JOIN LocationJob ON Job.uuid = LocationJob.JobUuid"
                               + " INNER JOIN Location ON LocationJob.LocationId = Location.id";

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Location.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND Location.id = '" + value + "'";
                        }
                    }
                }

                query += joins + whereAnd;
                System.Diagnostics.Debug.WriteLine("query: " + query);

                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Job>(query);
                    }
                }
        
                lock (DbContext.locker)
                {
                    return Db.Query<Job>(query, prepValue);
                }                
            }

            if (filter != null && studyGroups != null)
            {
                string joins = "";
                string whereAnd = "";
                string prepValue = "";

                foreach (var filterType in filter.Keys.ToArray())
                {
                    if (filterType == "titles")
                    {
                        prepValue = filter[filterType];

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Job.title = ?";
                        }
                        else
                        {
                            whereAnd += " AND Job.title = ?";
                        }
                    }

                    string value = filter[filterType];
                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeJob ON Job.uuid = JobTypeJob.JobUuid"
                               + " INNER JOIN JobType ON JobTypeJob.JobTypeId = JobType.id";
                        // + " WHERE JobType.id = ?";

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE JobType.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND JobType.id = '" + value + "'";
                        }

                        /*
                        System.Diagnostics.Debug.WriteLine("if (filterType == \"types\")");
                        System.Diagnostics.Debug.WriteLine("query before prepstatement insert:" + query);
                        lock (DbContext.locker)
                        {
                            return Db.Query<Job>(query, value);
                        }
                        */
                    }

                    if (filterType == "locations")
                    {
                        joins += " INNER JOIN LocationJob ON Job.uuid = LocationJob.JobUuid"
                               + " INNER JOIN Location ON LocationJob.LocationId = Location.id";
                        //+ " WHERE Location.id = ?";

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Location.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND Location.id = '" + value + "'";
                        }

                        /*
                        System.Diagnostics.Debug.WriteLine("if (filterType == \"locations\")");
                        System.Diagnostics.Debug.WriteLine("query before prepstatement insert: " + query);
                        lock (DbContext.locker)
                        {
                            return Db.Query<Job>(query, value);
                        }
                        */
                    }
                }

                for (int i = 0; i < studyGroups.Count; i++)
                {
                    if (i == 0)
                    {
                        if (studyGroups.Count > 1)
                        {
                            joins += " INNER JOIN StudyGroupJob ON Job.uuid = StudyGroupJob.JobUuid "
                                     + "INNER JOIN StudyGroup ON StudyGroupJob.StudyGroupId = StudyGroup.id ";

                            if (string.IsNullOrWhiteSpace(whereAnd))
                            {
                                whereAnd += " WHERE (StudyGroup.id = '" + studyGroups[i] + "'";
                            }
                            else
                            {
                                whereAnd += " AND (StudyGroup.id = '" + studyGroups[i] + "'";
                            }
                            //+ "WHERE (StudyGroup.id = '" + studyGroups[i] + "'";
                        }
                        else
                        {
                            joins += " INNER JOIN StudyGroupJob ON Job.uuid = StudyGroupJob.JobUuid "
                                     + "INNER JOIN StudyGroup ON StudyGroupJob.StudyGroupId = StudyGroup.id ";

                            if (string.IsNullOrWhiteSpace(whereAnd))
                            {
                                whereAnd += " WHERE StudyGroup.id = '" + studyGroups[i] + "'";
                            }

                            else
                            {
                                whereAnd = " AND StudyGroup.id = '" + studyGroups[i] + "'";
                            }
                            
                        }
                    }

                    else if (i != 0 && i + 1 == studyGroups.Count)
                    {
                        whereAnd += " OR StudyGroup.id = '" + studyGroups[i] + "')";
                    }
                    else
                    {
                        whereAnd += " OR StudyGroup.id = '" + studyGroups[i] + "'";
                    }
                }

                System.Diagnostics.Debug.WriteLine("if(filter != null && studyGroups != null)");
                System.Diagnostics.Debug.WriteLine("query: " + query + joins + whereAnd);
                System.Diagnostics.Debug.WriteLine("full query: " + query + joins + whereAnd);


                query += joins + whereAnd;


                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Job>(query);
                    }
                }
                
                return Db.Query<Job>(query, prepValue);
                
            }

            System.Diagnostics.Debug.WriteLine("Filter and studyGroups is null");
            System.Diagnostics.Debug.WriteLine("query: " + query);
            // if both studyGroups and filter is null
            lock (DbContext.locker)
            {
                return Db.Query<Job>(query);
            }          
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

            System.Diagnostics.Debug.WriteLine("JobsController - serializedJobs.Count(): " + serializedJobs.Count());

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
            j.jobTypes = new List<JobType>();
            j.locations = new List<Location>();
            j.studyGroups = new List<StudyGroup>();           

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

                if (key.Equals("description"))
                {
                    j.description = dict[key].ToString();
                }
                
                if (key.Equals("webpage"))
                {
                    j.webpage = dict[key].ToString();
                }

                if (key.Equals("expiryDate"))
                {
                    DateTime dateTime = (DateTime)dict[key];
                    j.expiryDate = TrimMilliseconds(dateTime);
                }

                if (key.Equals("modified"))
                {
                    DateTime dateTime = (DateTime)dict[key];
                    j.modified = TrimMilliseconds(dateTime);
                }
            
                
                if (key.Equals("published"))
                {
                    DateTime dateTime = (DateTime)dict[key];
                    j.published = TrimMilliseconds(dateTime);
                }
        

                if (key.Equals("companies"))
                {
                    CompaniesController cc = new CompaniesController();
                    IEnumerable companies = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    System.Diagnostics.Debug.WriteLine("companies created");
                    foreach (var comp in companies)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> companyDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(comp.ToString());
                        Company company = cc.DeserializeCompany(companyDict);
                        System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: company.id: " + company.id);
                        j.companies.Add(company);
                        cc.UpdateCompany(company);
                        System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: After j.companies.Add(company)");
                        string jobUuid = dict["uuid"].ToString();
                        cc.InsertCompanyJob(company.id, jobUuid);
                    }
                }

                if (key.Equals("studyGroups"))
                {
                    IEnumerable studyGroups = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    System.Diagnostics.Debug.WriteLine("studyGroups created");
                    StudyGroupsController sgc = new StudyGroupsController();
                    foreach (var studyGroup in studyGroups)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> studyGroupDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(studyGroup.ToString());
                        
                        StudyGroup sg = new StudyGroup();
                        if (studyGroupDict.ContainsKey("id"))
                        {
                            sg.id = studyGroupDict["id"].ToString();
                        }

                        if (studyGroupDict.ContainsKey("name"))
                        {
                            sg.name = studyGroupDict["name"].ToString();
                        }

                        j.studyGroups.Add(sg);

                        string jobUuid = dict["uuid"].ToString();
                        sgc.InsertStudyGroupJob(sg.id, jobUuid);

                    }
                }

                if (key.Equals("locations"))
                {
                    IEnumerable locations = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    LocationsController lc = new LocationsController();
                    System.Diagnostics.Debug.WriteLine("location created");
                    foreach (var location in locations)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> locationDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(location.ToString());
                        Location loc = new Location();
                        if (locationDict.ContainsKey("id"))
                        {
                            
                            loc.id = locationDict["id"].ToString();
                            System.Diagnostics.Debug.WriteLine("location id: " + loc.id);
                        }

                        if (locationDict.ContainsKey("name"))
                        {
                            loc.name = locationDict["name"].ToString();
                        }

                        lc.InsertLocation(loc);
                        j.locations.Add(loc);
                        string jobUuid = dict["uuid"].ToString();
                        lc.InsertLocationJob(loc.id, jobUuid);
                    }
                }


                if (key.Equals("jobTypes"))
                {
                    IEnumerable jobTypes = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    JobTypesController jtc = new JobTypesController();
                    System.Diagnostics.Debug.WriteLine("jobTypes created");
                    foreach (var jobType in jobTypes)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> jtDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jobType.ToString());

                        JobType jt = new JobType();
                        if (jtDict.ContainsKey("id"))
                        {
                            jt.id = jtDict["id"].ToString();
                        }

                        if (jtDict.ContainsKey("name"))
                        {
                            jt.name = jtDict["name"].ToString();
                        }

                        jtc.InsertJobType(jt);

                        System.Diagnostics.Debug.WriteLine("before j.jobTypes.Add(jt);");
                        j.jobTypes.Add(jt);

                        string jobUuid = dict["uuid"].ToString();
                        jtc.InsertJobTypeJob(jt.id, jobUuid);
                    }
                }
            }
            UpdateJob(j);
            return j;
        }

    }
}


/* Kan kanskje brukes for tester
                var rowsLocation =
                    Db.Query<Location>("Select * FROM Location WHERE Location.id = ?", "vestagder")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query Location Count: " + rowsLocation);

                var rowsLocationJob =
                    Db.Query<LocationJob>("Select * FROM LocationJob WHERE LocationJob.LocationId = ?", "vestagder")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query LocationJob Count: " + rowsLocationJob);

                var rowsLocationJob2 =
                    Db.Query<LocationJob>("Select * FROM LocationJob WHERE LocationJob.JobUuid = ?", "09706b08-78b9-42a5-88f9-ba0a45705432")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query LocationJob on JobUuid Count: " + rowsLocationJob2);

                var rowsStudyGroup =
                     Db.Query<StudyGroup>("Select * FROM StudyGroup WHERE StudyGroup.id = ?", "helse")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query StudyGroup Count: " + rowsStudyGroup);

                var rowsStudyGroupJob =
                    Db.Query<StudyGroupJob>("Select * FROM StudyGroupJob WHERE StudyGroupJob.StudyGroupId = ?", "helse")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query StudyGroupJob on helse Count: " + rowsStudyGroupJob);

                var rowsStudyGroupJob3 =
                    Db.Query<StudyGroupJob>("Select * FROM StudyGroupJob WHERE StudyGroupJob.StudyGroupId = ?", "datateknologi")
                    .Count;
                System.Diagnostics.Debug.WriteLine("Query StudyGroupJob on datateknologi Count: " + rowsStudyGroupJob3);

                var rowsStudyGroupJob4 =
                    Db.Query<StudyGroupJob>("Select * FROM StudyGroupJob WHERE (StudyGroupJob.StudyGroupId = ? OR StudyGroupJob.StudyGroupId = ?)", "datateknologi", "helse")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query StudyGroupJob on datateknologi or helse Count: " + rowsStudyGroupJob4);

                var rowsStudyGroupJob2 =
                    Db.Query<StudyGroupJob>("Select * FROM StudyGroupJob WHERE StudyGroupJob.JobUuid = ?", "09706b08-78b9-42a5-88f9-ba0a45705432")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query StudyGroupJob on JobUuid Count: " + rowsStudyGroupJob2);

                var rowsInner1 =
                    Db.Query<Job>("Select * FROM Job INNER JOIN LocationJob WHERE LocationJob.JobUuid = Job.uuid")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query rowsInner1 Count: " + rowsInner1);

                var rowsInner2 =
                    Db.Query<Job>("Select * FROM Job INNER JOIN LocationJob WHERE LocationJob.JobUuid = Job.uuid AND LocationJob.LocationId = ?", "vestagder")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query rowsInner2 Count: " + rowsInner2);

                var rowsJobs =
                    Db.Query<Job>("Select * FROM Job")
                        .Count;
                System.Diagnostics.Debug.WriteLine("Query rowsInner2 Count: " + rowsJobs);

                var rowsJob =
                    Db.Query<Job>(query)
                    .Count;
                System.Diagnostics.Debug.WriteLine("Query Job without LastAnd, value count: " + rowsJob);

                */
