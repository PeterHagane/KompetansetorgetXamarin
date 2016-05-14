using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.Utility;

namespace KompetansetorgetXamarin.DAL
{
    public class DbCourse : DbBase
    {

    /// <summary>
    /// Deletes all Course from the local database.
    /// </summary>
    public void DeleteAllCourses()
    {
        lock (DbContext.locker)
        {
            System.Diagnostics.Debug.WriteLine("DbCourse - DeleteAllCourses: Before delete.");
            Db.Execute("delete from " + "Course");
            System.Diagnostics.Debug.WriteLine("DbCourse - DeleteAllCourses: After delete.");
        }
    }

    public void InsertCourses(IEnumerable<Course> courses)
    {
        foreach (var course in courses)
        {
            course.filterChecked = false;
            course.id = Hasher.Base64Encode(course.id);
            InsertCourse(course);
        }
    }


    /// <summary>
    /// Returns a List containing all stored Locations
    /// </summary>
    /// <returns></returns>
    public List<Course> GetAllCourses()
    {
        lock (DbContext.locker)
        {
            return Db.Query<Course>("Select * from Course");
        }
    }
        public void InsertCourse(Course course)
        {
            lock (DbContext.locker)
            {
                var rowsAffected = Db.Update(course);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(course);
                }
            }
        }

        public void InsertCourseProject(string courseId, string projectUuid)
        {
            CourseProject cp = new CourseProject();
            cp.CourseId = Hasher.Base64Encode(courseId);
            cp.ProjectUuid = projectUuid;

            lock (DbContext.locker)
            {
                var rowsAffected =
                    Db.Query<CourseProject>("Select * FROM CourseProject WHERE CourseProject.CourseId = ?" +
                                            " AND CourseProject.ProjectUuid = ?", cp.CourseId, cp.ProjectUuid).Count;
                System.Diagnostics.Debug.WriteLine("Deserialize: CourseProject rowsAffected: " +
                                                    rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(cp);
                }
            }
        }

        public void UpdateCourses(List<Course> courses)
        {
            foreach (var course in courses)
            {
                UpdateCourse(course);
            } 
        }

        public void UpdateCourse(Course course)
        {
            InsertCourse(course);
        }
    }
}
