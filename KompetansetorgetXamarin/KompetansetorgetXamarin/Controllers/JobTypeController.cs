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
using KompetansetorgetXamarin.Utility;
using Newtonsoft.Json;
using PCLCrypto;

namespace KompetansetorgetXamarin.Controllers
{
    public class JobTypesController : BaseController
    {
        public JobTypesController()
        {
            Adress += "/jobTypes";
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
            DbJobType db = new DbJobType();
            System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: initiated");
            DbStudent dbStudent = new DbStudent();
            string accessToken = dbStudent.GetStudentAccessToken();

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
                    db.DeleteAllJobTypes();
                    db.InsertJobTypes(newJobTypes);
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

        private string CreateLocalHash()
        {
            DbJobType db = new DbJobType();
            List<JobType> jobTypes = db.GetAllJobTypes();
            StringBuilder sb = new StringBuilder();
            foreach (var jt in jobTypes)
            {
                sb.Append(Hasher.Base64Decode(jt.id));
            }
            return Hasher.CalculateMd5Hash(sb.ToString());
        }
    }
}
