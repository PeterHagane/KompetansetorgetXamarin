using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.DAL;
using System.Net.Http;
using System.Net.Http.Headers;
using KompetansetorgetXamarin.Utility;
using PCLCrypto;
using Newtonsoft.Json;

namespace KompetansetorgetXamarin.Controllers
{
    public class CoursesController : BaseController
    {
        public CoursesController()
        {
            Adress += "v1/courses";
        }

        /// <summary>
        /// Gets all Courses from the servers REST Api.
        /// </summary>
        public async Task UpdateCoursesFromServer()
        {
            DbCourse db = new DbCourse();
            System.Diagnostics.Debug.WriteLine("CoursesController - UpdateCoursesFromServer: initiated");
            DbStudent dbStudent = new DbStudent();

            string accessToken = dbStudent.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress);
            System.Diagnostics.Debug.WriteLine("CoursesController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Diagnostics.Debug.WriteLine("UpdateCoursesFromServer response " + response.StatusCode.ToString());
                    var newCourses = await response.Content.ReadAsAsync<IEnumerable<Course>>();
                    db.DeleteAllCourses();
                    db.InsertCourses(newCourses);
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("CoursesController - UpdateCoursesFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("CoursesController - UpdateCoursesFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("CoursesController - UpdateCoursesFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("CoursesController - UpdateCoursesFromServer: End Of Stack Trace");
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
            System.Diagnostics.Debug.WriteLine("CoursesController - url " + url.ToString());
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
                        await UpdateCoursesFromServer();
                    }

                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("CoursesController - CompareServerHash: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("CoursesController - CompareServerHash: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("CoursesController - CompareServerHash: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("CoursesController - CompareServerHash: End Of Stack Trace");
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
            DbCourse db = new DbCourse();
            List<Course> courses = db.GetAllCourses();
            StringBuilder sb = new StringBuilder();
            foreach (var c in courses)
            {
                sb.Append(c.id);
            }
            return Hasher.CalculateMd5Hash(sb.ToString());
        }
    }
}
