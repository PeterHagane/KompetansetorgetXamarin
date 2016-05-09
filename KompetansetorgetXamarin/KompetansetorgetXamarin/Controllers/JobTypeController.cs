using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.DAL;
using Newtonsoft.Json;
using PCLCrypto;

namespace KompetansetorgetXamarin.Controllers
{
    public class JobTypesController : BaseController
    {
        public JobTypesController()
        {
            Adress += "v1/jobTypes";
        }

        public void InsertJobType(JobType jobType)
        {
            lock (DbContext.locker)
            {
                var rowsAffected = Db.Update(jobType);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(jobType);
                }
            }
        }

        public void InsertJobTypeJob(string jobTypeId, string jobUuid)
        {
            JobTypeJob jtj = new JobTypeJob();
            jtj.JobTypeId = jobTypeId;
            jtj.JobUuid = jobUuid;

            lock (DbContext.locker)
            {
                var rowsAffected =
                    Db.Query<JobTypeJob>("Select * FROM JobTypeJob WHERE JobTypeJob.JobTypeId = ?" +
                                          " AND JobTypeJob.JobUuid = ?", jtj.JobTypeId, jtj.JobUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: JobTypeJob rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(jtj);
                }
            }
        }

        public void InsertJobTypeProject(string jobTypeId, string projectUuid)
        {
            JobTypeProject jtp = new JobTypeProject();
            jtp.JobTypeId = jobTypeId;
            jtp.ProjectUuid = projectUuid;

            lock (DbContext.locker)
            {
                var rowsAffected = Db.Query<JobTypeProject>("Select * FROM JobTypeProject WHERE JobTypeProject.JobTypeId = ?" +
                                             " AND JobTypeProject.ProjectUuid = ?", jtp.JobTypeId, jtp.ProjectUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneProjects: JobTypeProject rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(jtp);
                }
            }
        }

        /// <summary>
        /// Deletes all JobTypes from the local database.
        /// </summary>
        public void DeleteAllJobTypes()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("JobTypesController - DeleteAllJobTypes: Before delete.");
                Db.Execute("delete from " + "JobType");
                System.Diagnostics.Debug.WriteLine("JobTypesController - DeleteAllJobTypes: After delete.");
            }
        }

        public async Task CompareServerHash()
        {
            StudentsController sc = new StudentsController();
            string accessToken = sc.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress + "/hash");
            System.Diagnostics.Debug.WriteLine("JobTypesController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Diagnostics.Debug.WriteLine("CompareServerHash response " + response.StatusCode.ToString());
                    string json = await response.Content.ReadAsStringAsync();
                    string hash = ExtractServersHash(json);
                    string localHash = CreateLocalHash();
                    if (hash != localHash)
                    {
                        await UpdateJobTypesFromServer();
                    }

                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobTypesController - CompareServerHash: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("JobTypesController - CompareServerHash: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobTypesController - CompareServerHash: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobTypesController - CompareServerHash: End Of Stack Trace");
            }
        }

        private string ExtractServersHash(string json)
        {
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (dict.ContainsKey("hash"))
            {
                string hash = dict["hash"];
                return hash;
            }
            return "";
        }

        /// <summary>
        /// Gets all StudyGroups from the servers REST Api.
        /// </summary>
        public async Task UpdateJobTypesFromServer()
        {
            System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: initiated");
            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress);
            System.Diagnostics.Debug.WriteLine("JobTypesController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Diagnostics.Debug.WriteLine("GetJobTypesFromServer response " + response.StatusCode.ToString());
                    var newJobTypes = await response.Content.ReadAsAsync<IEnumerable<JobType>>();
                    DeleteAllJobTypes();
                    InsertJobTypes(newJobTypes);


                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: End Of Stack Trace");
            }
        }

        public void InsertJobTypes(IEnumerable<JobType> jobTypes)
        {
            foreach (var jobType in jobTypes)
            {
                InsertJobType(jobType);
            }
        }

        public List<JobType> GetJobTypeFilterJob()
        {
            lock (DbContext.locker)
            {
                return Db.Query<JobType>("Select * from JobType"
                                         + " where JobType.type = ?", "job");
            }
        }

        public List<JobType> GetJobTypeFilterProject()
        {
            lock (DbContext.locker)
            {
                return Db.Query<JobType>("Select * from JobType"
                                         + " where JobType.type = ?", "project");
            }
        }

        private List<JobType> GetAllJobTypes()
        {
            lock (DbContext.locker)
            {
                return Db.Query<JobType>("Select * from JobType"
                                         + " ORDER BY JobType.id ASC"); 
            }
        }

        private string CreateLocalHash()
        {
            List<JobType> jobTypes = GetAllJobTypes();
            StringBuilder sb = new StringBuilder();
            foreach (var jt in jobTypes)
            {
                sb.Append(jt.id);
            }
            return CalculateMd5Hash(sb.ToString());
        }

        /// <summary>
        /// Use to create a 128 bit hash
        /// used as part of the cache strategy.
        /// This is not to create a safe encryption, but to create a hash that im
        /// certain that the php backend can replicate.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string CalculateMd5Hash(string input)
        {
            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = hasher.HashData(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
