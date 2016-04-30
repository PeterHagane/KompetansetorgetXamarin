using System;

using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using PCLStorage;
using SQLite;
using SQLite.Net;
using Xamarin.Forms;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;
//using SQLite.Net.Platform;
//using SQLite.Net.Platform.XamarinAndroid;



namespace KompetansetorgetXamarin.DAL
{
    public class DbContext
    {    
        public static object locker = new object();
        // Considering changing to SQLiteAsyncConnection for increased performance
        public SQLiteConnection Db { get; private set; }
        private static DbContext dbContext;


        private DbContext()
        {
            System.Diagnostics.Debug.WriteLine("DbContext instantiated");
            InitDb();

            /*
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbContext - InitDb Failed");
                System.Diagnostics.Debug.WriteLine("DbContext - InitDb Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbContext - InitDb Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbContext - InitDb End Of Stack Trace");
                //Object reference not set to an instance of an object
            }
            */
        }


        public static DbContext GetDbContext
        {
            get
            {
                if (dbContext == null)
                {
                    dbContext = new DbContext();
                    return dbContext;
                }
                System.Diagnostics.Debug.WriteLine("DbContext instance reused ");
                return dbContext;
            }

        }

        private void InitDb()
        {
            System.Diagnostics.Debug.WriteLine("DbContext - InitDb: Start");
            // Gets the platform spesific implementation of ISQLite
            Db = DependencyService.Get<ISQLite>().GetConnection();
            
            /*
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                //Object reference not set to an instance of an object
            }*/

            System.Diagnostics.Debug.WriteLine("DbContext - InitDb: platform spesific implementation of ISQLite gotten");
            Db.CreateTable<Student>(); 
            System.Diagnostics.Debug.WriteLine("Table student created");
            Db.CreateTable<StudyGroup>();
            System.Diagnostics.Debug.WriteLine("Table studygroup created");

            Db.CreateTable<StudyGroupStudent>();
            Db.CreateTable<StudyGroupJob>();
            Db.CreateTable<StudyGroupProject>();
            Db.CreateTable<Company>();
            Db.CreateTable<CompanyJob>();
            Db.CreateTable<CompanyProject>();
            Db.CreateTable<Contact>();
            Db.CreateTable<ContactJob>();
            Db.CreateTable<ContactProject>();
            Db.CreateTable<Course>();
            Db.CreateTable<CourseProject>();
            System.Diagnostics.Debug.WriteLine("Table CourseProject created");

            Db.CreateTable<ApprovedCourse>();
            Db.CreateTable<ApprovedCourseProject>();
            Db.CreateTable<Degree>();
            Db.CreateTable<DegreeProject>();
            Db.CreateTable<Models.Device>();
            System.Diagnostics.Debug.WriteLine("Table Device created");

            Db.CreateTable<Notification>();
            System.Diagnostics.Debug.WriteLine("Table Notification created");

            Db.CreateTable<Job>();
            Db.CreateTable<JobType>();
            Db.CreateTable<JobTypeJob>();
            Db.CreateTable<JobTypeProject>();
            Db.CreateTable<Location>();
            Db.CreateTable<LocationJob>();
            Db.CreateTable<Project>();

            System.Diagnostics.Debug.WriteLine("DbContext - InitDb: Finish All Tables created");
        }

    }

    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }


}





