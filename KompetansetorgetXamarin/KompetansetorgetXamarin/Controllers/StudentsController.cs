using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;
using Newtonsoft.Json;
using System.Web;
using KompetansetorgetXamarin.Controls;
using Xamarin.Forms;
using Device = KompetansetorgetXamarin.Models.Device;

namespace KompetansetorgetXamarin.Controllers
{
    public class StudentsController : BaseController
    {
        public StudentsController()
        {
            Adress += "v1/students";
        }


        /// <summary>
        /// Update the students studygroup.
        /// </summary>
        /// <param name="receiveNotifications"></param>
        /// <param name="receiveProjectNotifications"></param>
        /// <param name="receiveJobNotifications"></param>
        public void UpdateStudentsNotificationsPref(bool? receiveNotifications,
            bool? receiveProjectNotifications, bool? receiveJobNotifications)
        {
            DbStudent db = new DbStudent();
            Student student = db.GetStudent();
            if (receiveNotifications != null)
            {
                student.receiveNotifications = (bool)receiveNotifications;
            }

            if (receiveProjectNotifications != null)
            {
                student.receiveProjectNotifications = (bool)receiveProjectNotifications;
            }

            if (receiveJobNotifications != null)
            {
                student.receiveJobNotifications = (bool)receiveJobNotifications;
            }
            db.UpdateStudent(student);
        }

        /// <summary>
        /// Gets the Students StudyGroups used for push notifications from the server.
        /// </summary>
        /// <returns></returns>
        public async Task<List<StudyGroup>> GetStudentsStudyGroupFromServer()
        {
            DbStudent db = new DbStudent();
            Student student = db.GetStudent();

            if (student == null)
            {
                Authenticater.Authorized = false;
                return null;
            }

            string encodedUsername = Base64Encode(student.username);
            Uri url = new Uri(Adress + "/" + encodedUsername);
            
            System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent uri: " + url.ToString());
            string accessToken = db.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return null;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            string jsonString = null;

            try
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent failed due to lack of Authorization");
                    Authenticater.Authorized = false;
                    return null;
                }
                System.Diagnostics.Debug.WriteLine("UpdateStudyGroupStudent response " + response.StatusCode.ToString());
                jsonString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent: End Of Stack Trace");
                return null;
            }

            if (jsonString != null)
            {
                return ExtractStudyGroupsFromJson(jsonString);
                //DeleteAllStudyGroupStudent();
                //CreateStudyGroupStudents(student.username, studyGroupIds);
            }
            return null;
        }

        /// <summary>
        /// Posts the Students StudyGroups used for push notifications to the server.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PostStudentsStudyGroupToServer(List<StudyGroup> studyGroups)
        {
            DbStudent db = new DbStudent();
            Student student = db.GetStudent();
            string accessToken = db.GetStudentAccessToken();
            if (student == null || accessToken == null)
            {
                Authenticater.Authorized = false;
                return false;
            }           
            string jsonString = "";
            foreach (var studyGroup in studyGroups)
            {
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    jsonString = "{\"StudyGroup\":[{\"id\":\"" + studyGroup.id + "\"}";
                }

                else
                {
                    jsonString += ",{\"id\":\"" + studyGroup.id + "\"}";
                }
            }
            jsonString += "]}";
            // {"StudyGroup":[{"id":"idrettsfag"},{"id":"datateknologi"}]}
            // {"studyGroups":[{"id":"helse"},{"id":"ingeniør"},{"id":"samfunnsfag"}]} 
            System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer jsonString: " + jsonString);

            string encodedUsername = Base64Encode(student.username);
            Uri url = new Uri(Adress + "/" + encodedUsername);
            System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer uri: " + url.ToString());

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);         

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(url, content);
                System.Diagnostics.Debug.WriteLine("PostStudentsStudyGroupToServer response " + response.StatusCode.ToString());

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer failed due to lack of Authorization");
                    Authenticater.Authorized = false;
                }

                // response.StatusCode is either unauthorized or another failed status.
                return false;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: await client.PostAsJsonAsync(url, jsonString) Failed");
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: End Of Stack Trace");
                return false;
            }     
        }

        /// <summary>
        /// Extracts the StudyGroup ids from a json string, then builds a list from those ids by querying the local database. 
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private List<StudyGroup> ExtractStudyGroupsFromJson(string jsonString)
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            System.Diagnostics.Debug.WriteLine("DeserializeApiData. Printing Key Value:");

            string[] keys = dict.Keys.ToArray();
            List<StudyGroup> studyGroupList = new List<StudyGroup>();

            DbStudyGroup dbStudyGroup = new DbStudyGroup();

            foreach (var key in keys)
            {
                System.Diagnostics.Debug.WriteLine("key: " + key);
                System.Diagnostics.Debug.WriteLine("value: " + dict[key].ToString());

                if (key.Equals("studyGroups"))
                {
                    IEnumerable studyGroups = (IEnumerable)dict[key];
                    foreach (var studyGroup in studyGroups)
                    {
                        Dictionary<string, object> studyGroupDict =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(studyGroup.ToString());

                        string id = studyGroupDict["id"].ToString();
                        studyGroupList.Add(dbStudyGroup.GetStudygroup(id));
                    }
                    return studyGroupList;
                }
            }
            return null;
        }

        /// <summary>
        /// Base64Encodes a string, should be used on all query strings that 
        /// can contain special characters not suitable for in a url adress.  
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }



        /*

        [Obsolete("CreateStudyGroupStudents is deprecated, StudyGroupStudent should not be stored on client anymore.")]
        private void CreateStudyGroupStudents(string username, List<string> studyGroupIds)
        {
            foreach (var id in studyGroupIds)
            {
                StudyGroupStudent sgs = new StudyGroupStudent();
                sgs.StudentUsername = username;
                sgs.StudyGroupId = id;

                lock (DbContext.locker)
                {
                    Db.Insert(sgs);
                }
            }
        }
        
        /// <summary>
        /// Gets a list of all studyGroups that are related to the student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [Obsolete("GetAllStudyGroupsRelatedToStudent is deprecated, StudyGroupStudent should not be stored on client anymore.")]
        public List<StudyGroup> GetAllStudyGroupsRelatedToStudent(Student student)
        {
            lock (DbContext.locker)
            {
                return Db.Query<StudyGroup>("SELECT * FROM StudyGroup" +
                                        " INNER JOIN StudyGroupStudent ON StudyGroup.id = StudyGroupStudent.StudyGroupId" +
                                        " WHERE StudyGroupStudent.StudentUsername = ?", student.username);
            }
        }

        [Obsolete("ExtractStudyGroupId is deprecated, StudyGroupStudent should not be stored on client anymore.")]
        public void DeleteAllStudyGroupStudent()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - DeleteAllStudyGroupStudent: Before delete.");
                Db.Execute("delete from " + "StudyGroupStudent");
                System.Diagnostics.Debug.WriteLine("StudentsController - DeleteAllStudyGroupStudent: After delete.");
            }
        }

        */
        /*
        /// <summary>
        /// WARNING, this is a hack workaround.
        /// How to use:
        /// 1: Call UpdateStudyGroupStudent before calling this method
        /// 2: Check BaseContentPage.Authorized before 
        /// 3. Call this method, but check for null. 
        /// If this list is null and Authorized is true then communication with server has failed.
        /// </summary>
        */
        //public List<StudyGroup> StudyGroupStudentId { get; set; }

        /*
        private List<string> ExtractStudyGroupId(string jsonString)
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            System.Diagnostics.Debug.WriteLine("DeserializeApiData. Printing Key Value:");

            string[] keys = dict.Keys.ToArray();
            List<string> idList = new List<string>();

            foreach (var key in keys)
            {
                System.Diagnostics.Debug.WriteLine("key: " + key);
                System.Diagnostics.Debug.WriteLine("value: " + dict[key].ToString());

                if (key.Equals("studyGroups"))
                {
                    IEnumerable studyGroups = (IEnumerable) dict[key];
                    foreach (var studyGroup in studyGroups)
                    {                        
                        Dictionary<string, object> studyGroupDict =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(studyGroup.ToString());
                        idList.Add(studyGroupDict["id"].ToString());
                    }
                    return idList;
                }

            }
            return null;
        }

        */
    }
}
