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

        /// <summary>
        /// Gets all Courses from the servers REST Api.
        /// </summary>
        public async Task UpdateCoursesFromServer()
        {
            System.Diagnostics.Debug.WriteLine("CoursesController - UpdateCoursesFromServer: initiated");
            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

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
                    DeleteAllCourses();
                    InsertCourses(newCourses);
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

        /// <summary>
        /// Deletes all Course from the local database.
        /// </summary>
        private void DeleteAllCourses()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("CoursesController - DeleteAllCourses: Before delete.");
                Db.Execute("delete from " + "Course");
                System.Diagnostics.Debug.WriteLine("CoursesController - DeleteAllCourses: After delete.");
            }
        }

        private void InsertCourses(IEnumerable<Course> courses)
        {
            foreach (var course in courses)
            {
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
    }
}
