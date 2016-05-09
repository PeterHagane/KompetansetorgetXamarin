using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using Newtonsoft.Json;
using PCLCrypto;
using SQLite.Net;

namespace KompetansetorgetXamarin.Controllers
{
    class StudyGroupsController : BaseController
    {
        public StudyGroupsController()
        {
            Adress += "v1/studygroups";
        }

        /// <summary>
        /// Inserts the studyGroup into the database.
        /// </summary>
        /// <param name="studyGroup"></param>
        /// <returns>Returns true if the studyGroup was inserted, 
        /// </returns>
        public bool InsertStudyGroup(StudyGroup studyGroup)
        {

                lock (DbContext.locker)
                {
                    var rowsAffected = Db.Update(studyGroup);
                    if (rowsAffected == 0)
                    {
                        Db.Insert(studyGroup);
                    }
                }
                return true;         
        }

        /// <summary>
        /// Checks if there already is an entry of that Device primary key
        /// In the database.
        /// Returns true if exist, false if it doesnt exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns true if exist, false if it doesnt exist.</returns>
        public bool CheckIfStudyGroupExist(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Device>(id);
                }
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CheckIfStudyGroupExist: StudyGroup Already exists");
                return true;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CheckIfStudyGroupExist: entry of StudyGroup doesnt exists");
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CheckIfStudyGroupExist: Exception msg: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets all StudyGroups from the servers REST Api.
        /// </summary>
        public async Task UpdateStudyGroupsFromServer()
        {
            System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: initiated");
            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress);
            System.Diagnostics.Debug.WriteLine("StudyGroupsController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK) {
                    System.Diagnostics.Debug.WriteLine("GetStudyGroupsFromServer response " + response.StatusCode.ToString());
                    var results = await response.Content.ReadAsAsync<IEnumerable<StudyGroup>>();
                    DeleteAllStudyGroups();

                    foreach (var studygroup in results)
                    {
                        InsertStudyGroup(studygroup);
                    }
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: End Of Stack Trace");
            }
        }


        /// <summary>
        /// Deletes all StudyGroup from the local database.
        /// </summary>
        public void DeleteAllStudyGroups()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - DeleteAllStudyGroups: Before delete.");
                Db.Execute("delete from " + "StudyGroup");
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - DeleteAllStudyGroups: After delete.");
            }
        }

        /// <summary>
        /// Updates a studygroup, if it doesnt already exist a new entry will be inserted into the db.
        /// </summary>
        /// <param name="studyGroup"></param>
        public void UpdateStudyGroups(StudyGroup studyGroup)
        {
            if (CheckIfStudyGroupExist(studyGroup.id)) 
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - UpdateStudyGroups: updates: " + studyGroup.id);
                lock (DbContext.locker)
                {
                    Db.Update(studyGroup);
                }
            }

            else
            { 
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("StudyGroupsController - UpdateStudyGroups: inserts: + " + studyGroup.id);
                    
                    Db.Insert(studyGroup);
                }
            }            
        }

        /// <summary>
        /// Gets a studygroup based in its id
        /// </summary>
        /// <returns></returns>
        public StudyGroup GetStudygroup(string id)
        {
            lock (DbContext.locker)
            {
                return Db.Get<StudyGroup>(id);
            }
        }

        /// <summary>
        /// Returns a List containing all stored StudyGroups
        /// </summary>
        /// <returns></returns>
        public List<StudyGroup> GetAllStudyGroups()
        {
            lock (DbContext.locker)
            {
                return Db.Query<StudyGroup>("Select * from StudyGroup");
            }
        }

        public void InsertStudyGroupJob(string studygroupId, string jobUuid)
        {
            System.Diagnostics.Debug.WriteLine("StudyGroupJob created");
            StudyGroupJob sgj = new StudyGroupJob();
            sgj.StudyGroupId = studygroupId;
            sgj.JobUuid = jobUuid;
            System.Diagnostics.Debug.WriteLine("StudyGroupJob before insert");

            lock (DbContext.locker)
            {
                var rowsAffected =
                    Db.Query<StudyGroupJob>(
                        "Select * FROM StudyGroupJob WHERE StudyGroupJob.StudyGroupId = ?" +
                        " AND StudyGroupJob.JobUuid = ?", sgj.StudyGroupId, sgj.JobUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: StudyGroupJob rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(sgj);
                }
            }
            System.Diagnostics.Debug.WriteLine("StudyGroupJob after insert");
        }

        public void InsertStudyGroupProject(string studygroupId, string projectUuid)
        {
            System.Diagnostics.Debug.WriteLine("StudyGroupProject created");
            StudyGroupProject sgp = new StudyGroupProject();
            sgp.StudyGroupId = studygroupId;
            sgp.ProjectUuid = projectUuid;
            System.Diagnostics.Debug.WriteLine("StudyGroupProject before insert");

            lock (DbContext.locker)
            {
                var rowsAffected =
                    Db.Query<StudyGroupProject>(
                        "Select * FROM StudyGroupProject WHERE StudyGroupProject.StudyGroupId = ?" +
                        " AND StudyGroupProject.ProjectUuid = ?", sgp.StudyGroupId, sgp.ProjectUuid).Count;
                System.Diagnostics.Debug.WriteLine("Deserialize: StudyGroupProject rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(sgp);
                }
            }
            System.Diagnostics.Debug.WriteLine("StudyGroupProject after insert");
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
            System.Diagnostics.Debug.WriteLine("StudyGroupsController - url " + url.ToString());
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
                        await UpdateStudyGroupsFromServer();
                    }

                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CompareServerHash: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CompareServerHash: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CompareServerHash: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CompareServerHash: End Of Stack Trace");
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

        private string CreateLocalHash()
        {
            List<StudyGroup> studyGroups = GetAllStudyGroups();
            StringBuilder sb = new StringBuilder();
            foreach (var sg in studyGroups)
            {
                sb.Append(sg.id);
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
