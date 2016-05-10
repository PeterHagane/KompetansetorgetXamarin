using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.DAL
{
    public class DbStudent : DbBase
    {
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
                System.Diagnostics.Debug.WriteLine("DbStudent - InsertStudent: Student not inserted, already exist");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("DbStudent - InsertStudent: Student not inserted");
            //Student did not exist, safe to insert.
            DbDevice dbDevice = new DbDevice();

            lock (DbContext.locker)
            {
                Db.Insert(student);
            }

            if (student.devices != null)
            {
                foreach (Device d in student.devices)
                {
                    dbDevice.InsertDevice(d);
                }
            }

            else
            {
                dbDevice.FixStudentForeignKey(student.username);
            }

            return true;
        }

        /// <summary>
        /// Gets the students accessToken.
        /// Warning, check for Null
        /// </summary>
        /// <returns></returns>
        public string GetStudentAccessToken()
        {
            DbStudent db = new DbStudent();
            Student student = db.GetStudent();
            string accessToken = null;
            if (student != null)
            {
                accessToken = student.accessToken;
            }
            return accessToken;
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
                System.Diagnostics.Debug.WriteLine("DbStudent - CheckIfStudentExist: Student Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbStudent - CheckIfStudentExist: Student doesnt exists");
                System.Diagnostics.Debug.WriteLine("DbStudent - CheckIfStudentExist: Exception msg: " + e.Message);
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
                System.Diagnostics.Debug.WriteLine("DbStudent - GetStudent: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbStudent - GetStudent: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbStudent - GetStudent: End Of Stack Trace");
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
                System.Diagnostics.Debug.WriteLine("DbStudent - GetStudent: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbStudent - GetStudent: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbStudent - GetStudent: End Of Stack Trace");
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
        /// Gets the student with the specified username
        /// </summary>
        /// <returns></returns>
        public void UpdateStudent(Student student)
        {
            if (!CheckIfStudentExist(student.username))
            {
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: There was no stored record of Student.");
                InsertStudent(student);
            }

            System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: There was a record of project in the database.");
            try
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: Before Updating student.");

                    Db.Update(student);
                    System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: After Updating student.");

                    // Db.InsertOrReplaceWithChildren(project, recursive: true);
                    // Db.UpdateWithChildren(project);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: Student update failed");
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudent: End Of Stack Trace");
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
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: There was no stored record of Student.");
                InsertStudent(student);
            }
            System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: There was a record of project in the database.");
            if (student.devices != null)
            {
                DbDevice dbDevice = new DbDevice();
                foreach (Device d in student.devices)
                {
                    if (!dbDevice.InsertDevice(d))
                    {
                        System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: Device already exists: Calling UpdateDevice.");

                        dbDevice.UpdateDevice(d);
                    }
                }
            }

            try
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: Before Updating project.");

                    Db.Update(student);
                    System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: After Updating project.");

                    // Db.InsertOrReplaceWithChildren(project, recursive: true);
                    //Db.UpdateWithChildren(project);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: Project update failed");
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbStudent - UpdateStudentWithChilds: End Of Stack Trace");
            }
        }

        public void DeleteStudent(Student student)
        {
            if (CheckIfStudentExist(student.username))
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("DbStudent - DeleteStudent: Before delete.");
                    Db.Delete<Student>(student.username);
                    System.Diagnostics.Debug.WriteLine("DbStudent - DeleteStudent: After delete.");
                }
            }
        }

        /// <summary>
        /// Deletes all students from the local database.
        /// </summary>
        public void DeleteAllStudents()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("DbStudent - DeleteAllStudents: Before delete.");
                Db.Execute("delete from " + "Student");
                System.Diagnostics.Debug.WriteLine("DbStudent - DeleteAllStudents: After delete.");
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
                DbDevice dbDevice = new DbDevice();
                foreach (Device d in student.devices)
                {
                    dbDevice.DeleteDevice(d);
                }

                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("DbStudent - DeleteStudentWithChilds: Before delete.");
                    Db.Delete<Student>(student.username);
                    System.Diagnostics.Debug.WriteLine("DbStudent - DeleteStudentWithChilds: After delete.");
                }
            }
        }
    }
}
