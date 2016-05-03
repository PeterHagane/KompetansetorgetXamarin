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
    class StudentsController : BaseController
    {
        public StudentsController()
        {
            Adress += "v1/students";
        }
        /// <summary>
        /// Inserts the project and its respective children (only Company and CompanyProject) 
        /// into the database.
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Returns true if the project was inserted, returns false if a project with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertStudent(Student student)
        {
            if (CheckIfStudentExist(student.username))
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - InsertStudent: Student not inserted, already exist");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("StudentsController - InsertStudent: Student not inserted");
            //Student did not exist, safe to insert.
            DevicesController dc = new DevicesController();

            lock (DbContext.locker)
            {
                Db.Insert(student);
            }

            if (student.devices != null)
            {
                foreach (Device d in student.devices)
                {
                    dc.InsertDevice(d);
                }
            }

            else
            {
                dc.FixStudentForeignKey(student.username);
            }

            return true;
        }

        /// <summary>
        /// Checks if there already is an entry of that Students primary key
        /// In the database. 
        /// Returns true if exist, false if it doesnt exist
        /// </summary>
        /// <param name="username"></param>
        /// <returns>Returns true if exist, false if it doesnt exist.</returns>
        public bool CheckIfStudentExist(string username)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Student>(username);
                }
                System.Diagnostics.Debug.WriteLine("StudentsController - CheckIfStudentExist: Student Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - CheckIfStudentExist: Student doesnt exists");
                System.Diagnostics.Debug.WriteLine("StudentsController - CheckIfStudentExist: Exception msg: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the student
        /// Warning, student can be null, if thats the case set Page.Authorized = false;
        /// </summary>
        /// <returns></returns>
        public Student GetStudent()
        {
            try
            {
                lock (DbContext.locker)
                {
                    List<Student> students = Db.Query<Student>("Select * from Student");
                    if (students.Count > 1)
                    {
                        // SHOULD NOT BE ALLOWED, IMPLEMENT SOMETHING AGAINST THIS
                        //return null;
                    }
                    Student student;
                    try
                    {
                        student = students[0];
                        List<Device> devices = GetAllDevicesRelatedToStudent(student);
                        //List<StudyGroup> studyGroups = GetAllStudyGroupsRelatedToStudent(student);
                        student.devices = devices;
                        //student.studyGroup = studyGroups;
                    }
                    catch (Exception)
                    {
                        // very ugly hack, if student is null go to login.
                        student = null;
                    }

                    return student;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - GetStudent: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudentsController - GetStudent: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudentsController - GetStudent: End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Gets the student with the specified username
        /// Warning, student can be null, if thats the case set Page.Authorized = false;
        /// </summary>
        /// <returns></returns>
        public Student GetStudent(string username)
        {
            try
            {
                lock (DbContext.locker)
                {
                    Student student = Db.Get<Student>(username);
                    List<Device> devices = GetAllDevicesRelatedToStudent(student);
                    //List<StudyGroup> studyGroups = GetAllStudyGroupsRelatedToStudent(student);
                    student.devices = devices;
                    //student.studyGroup = studyGroups;
                    return student;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - GetStudent: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudentsController - GetStudent: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudentsController - GetStudent: End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Gets a list of all companies that are related to the spesific Project
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public List<Device> GetAllDevicesRelatedToStudent(Student student)
        {
            lock (DbContext.locker)
            {
                return Db.Query<Device>("Select * from Device " +
                                        "Where Device.username = ?", student.username);
            }
        }

        /// <summary>
        /// Gets the students accessToken.
        /// Warning, check for Null
        /// </summary>
        /// <returns></returns>
        public string GetStudentAccessToken()
        {
            Student student = GetStudent();
            string accessToken = null;
            if (student != null)
            {
                accessToken = student.accessToken;
            }
            return accessToken;
        }

        /// <summary>
        /// Gets the student with the specified username
        /// </summary>
        /// <returns></returns>
        public void UpdateStudent(Student student)
        {
            if (!CheckIfStudentExist(student.username))
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: There was no stored record of Student.");
                InsertStudent(student);
            }

            System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: There was a record of project in the database.");
            try
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: Before Updating student.");

                    Db.Update(student);
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: After Updating student.");

                    // Db.InsertOrReplaceWithChildren(project, recursive: true);
                    // Db.UpdateWithChildren(project);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: Student update failed");
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudent: End Of Stack Trace");
            }
        }

        /// <summary>
        /// Gets the student with the specified username
        /// </summary>
        /// <returns></returns>
        public void UpdateStudentWithChilds(Student student)
        {
            if (!CheckIfStudentExist(student.username))
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: There was no stored record of Student.");
                InsertStudent(student);
            }

            System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: There was a record of project in the database.");

            
            
            if (student.devices != null) {
                DevicesController dc = new DevicesController();
                foreach (Device d in student.devices)
                {
                    if (!dc.InsertDevice(d))
                    {
                        System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Device already exists: Calling UpdateDevice.");

                        dc.UpdateDevice(d);
                    }
                }
            }

            /*
            if (student.studyGroup != null)
            {
                
                foreach (StudyGroup sg in student.studyGroup)
                {
                    StudyGroupStudent sgs = new StudyGroupStudent();
                    sgs.StudentUsername = student.username;
                    sgs.StudyGroupId = sg.id;
                    try
                    {
                        lock (DbContext.locker)
                        {
                            System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Inserting StudyGroupStudent.");
                            Db.Insert(sgs);
                            // Db.InsertOrReplaceWithChildren(project, recursive: true);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: StudyGroupStudent Insertion failed");
                        System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Exception msg: " + e.Message);
                        System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Stack Trace: \n" + e.StackTrace);
                        System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: End Of Stack Trace");
                    }
                }
            }
            */
            try
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Before Updating project.");

                    Db.Update(student);
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: After Updating project.");

                    // Db.InsertOrReplaceWithChildren(project, recursive: true);
                    //Db.UpdateWithChildren(project);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Project update failed");
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudentWithChilds: End Of Stack Trace");
            }
        }

        public void DeleteStudent(Student student)
        {
            if (CheckIfStudentExist(student.username))
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - DeleteStudent: Before delete.");
                    Db.Delete<Student>(student.username);
                    System.Diagnostics.Debug.WriteLine("StudentsController - DeleteStudent: After delete.");
                }
            }
        }

        /// <summary>
        /// Gets the Students StudyGroups used for push notifications from the server.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<List<StudyGroup>> GetStudentsStudyGroupFromServer(BaseContentPage page)
        {
            Student student = GetStudent();

            if (student == null)
            {
                page.Authorized = false;
                return null;
            }

            string encodedUsername = Base64Encode(student.username);
            Uri url = new Uri(Adress + "/" + encodedUsername);
            
            System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent uri: " + url.ToString());
            string accessToken = GetStudentAccessToken();

            if (accessToken == null)
            {
                page.Authorized = false;
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
                    page.Authorized = false;
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
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<bool> PostStudentsStudyGroupToServer(BaseContentPage page, List<StudyGroup> studyGroups)
        {           
            Student student = GetStudent();
            string accessToken = GetStudentAccessToken();
            if (student == null || accessToken == null)
            {
                page.Authorized = false;
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
                    page.Authorized = false;
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

            StudyGroupsController sgc = new StudyGroupsController();

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
                        studyGroupList.Add(sgc.GetStudygroup(id));
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

        /// <summary>
        /// Deletes all students from the local database.
        /// </summary>
        public void DeleteAllStudents()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - DeleteAllStudents: Before delete.");
                Db.Execute("delete from " + "Student");
                System.Diagnostics.Debug.WriteLine("StudentsController - DeleteAllStudents: After delete.");
            }
        }

        /// <summary>
        /// Deletes a student and its related devices.
        /// </summary>
        /// <param name="student"></param>
        public void DeleteStudentWithChilds(Student student)
        {
            if (CheckIfStudentExist(student.username))
            {
                DevicesController dc = new DevicesController();
                foreach (Device d in student.devices)
                {
                    dc.DeleteDevice(d);
                }

                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - DeleteStudentWithChilds: Before delete.");
                    Db.Delete<Student>(student.username);
                    System.Diagnostics.Debug.WriteLine("StudentsController - DeleteStudentWithChilds: After delete.");
                }
            }
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
