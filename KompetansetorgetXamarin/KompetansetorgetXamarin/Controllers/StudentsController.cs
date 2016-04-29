using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;

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
                return false;
            }

            //Project did not exist, safe to insert.
            DevicesController dc = new DevicesController();

            foreach (Device d in student.devices)
            {
                dc.InsertDevice(d);
            }

            lock (DbContext.locker)
            {
                Db.Insert(student);
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
                System.Diagnostics.Debug.WriteLine("StudentsController - CheckIfStudentExist: Student Already exists");
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
                        return null;
                    }
                    Student student = students[0];
                    List<Device> devices = GetAllDevicesRelatedToStudent(student);
                    student.devices = devices;
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
                    student.devices = devices;
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
                                        "Where device.username = ?", student.username);
            }
        }

        /*
        public bool UpdateOAuthToken(string accessToken)
        {
            // IMPLEMENT
            return false;
        }
        */

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
                    //Db.UpdateWithChildren(project);
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



    }
}
