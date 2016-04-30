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
    class StudentsController
    {
        private DbContext dbContext = DbContext.GetDbContext;
        private SQLiteConnection Db;

        public StudentsController()
        {
            Db = dbContext.Db;
        }

        /// <summary>
        /// WARNING, this is a hack workaround.
        /// How to use:
        /// 1: Call UpdateStudyGroupStudent before calling this method
        /// 2: Check BaseContentPage.Authorized before 
        /// 3. Call this method, but check for null. 
        /// If this list is null and Authorized is true then communication with server has failed.
        /// </summary>
        public List<StudyGroup> StudyGroupStudentId { get; set; }

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
                    Student student = students[0];
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
        /// Gets the student with the specified username
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
                    List<StudyGroup> studyGroups = GetAllStudyGroupsRelatedToStudent(student);
                    student.devices = devices;
                    student.studyGroup = studyGroups;
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




        /*
        public bool UpdateOAuthToken(string accessToken)
        {
            // IMPLEMENT
            return false;
        }
        */

        public string GetStudentAccessToken()
        {
            return GetStudent().accessToken;
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

        /// <summary>
        /// Creates a list of the Ids of the studygroups used for push notifications
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task UpdateStudyGroupStudent(BaseContentPage page)
        {
            Student student = GetStudent();

            if (student == null)
            {
                return;
            }

            string adress = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/students/";
            string encodedUsername = Base64Encode(student.username);
            Uri url = new Uri(adress + encodedUsername);
            
            System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent uri: " + url.ToString());
            string accessToken = GetStudentAccessToken();

            if (accessToken == null)
            {
                page.Authorized = false;
                return;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            string jsonString = null;

            try
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);
                // doesnt activate even if unauthorized, so fix this if. 21:35
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent failed due to lack of Authorization");
                    // THIS DOES NOT WORK!!
                    page.Authorized = false;
                    return;
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
                return;
            }

            if (jsonString != null)
            {
                StudyGroupStudentId = ExtractStudyGroupId(jsonString);
                //DeleteAllStudyGroupStudent();
                //CreateStudyGroupStudents(student.username, studyGroupIds);
            }
        }

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

        private List<StudyGroup> ExtractStudyGroupId(string jsonString)
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

                        studyGroupList.Add(sgc.GetStudygroup(studyGroupDict["id"].ToString()));
                    }
                    return studyGroupList;
                }
                
            }
            return null;
        }

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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Gets a list of all studyGroups that are related to the student
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public List<StudyGroup> GetAllStudyGroupsRelatedToStudent(Student student)
        {
            lock (DbContext.locker)
            {
                return Db.Query<StudyGroup>("SELECT * FROM StudyGroup" +
                                        " INNER JOIN StudyGroupStudent ON StudyGroup.id = StudyGroupStudent.StudyGroupId" +
                                        " WHERE StudyGroupStudent.StudentUsername = ?", student.username);
            }
        }

        public void DeleteAllStudyGroupStudent()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - DeleteAllStudyGroupStudent: Before delete.");
                Db.Execute("delete from " + "StudyGroupStudent");
                System.Diagnostics.Debug.WriteLine("StudentsController - DeleteAllStudyGroupStudent: After delete.");
            }
        }
    }
}
