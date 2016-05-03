using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.DAL;


namespace KompetansetorgetXamarin.Controllers
{
    public class CoursesController : BaseController
    {
        public CoursesController()
        {
            Adress += "v1/courses";
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
            cp.CourseId = courseId;
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
    }
}
