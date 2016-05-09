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
using KompetansetorgetXamarin.Utility;
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
        /// Gets all StudyGroups from the servers REST Api.
        /// </summary>
        public async Task UpdateStudyGroupsFromServer()
        {
            DbStudyGroup db = new DbStudyGroup();
            System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: initiated");
            DbStudent dbStudent = new DbStudent();
            string accessToken = dbStudent.GetStudentAccessToken();

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
                    db.DeleteAllStudyGroups();

                    foreach (var studygroup in results)
                    {
                        db.InsertStudyGroup(studygroup);
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

        public async Task CompareServerHash()
        {
            DbStudent dbStudent = new DbStudent();
            string accessToken = dbStudent.GetStudentAccessToken();

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
            DbStudyGroup db = new DbStudyGroup();
            List<StudyGroup> studyGroups = db.GetAllStudyGroups();
            StringBuilder sb = new StringBuilder();
            foreach (var sg in studyGroups)
            {
                sb.Append(sg.id);
            }
            return Hasher.CalculateMd5Hash(sb.ToString());
        }
    }
}
