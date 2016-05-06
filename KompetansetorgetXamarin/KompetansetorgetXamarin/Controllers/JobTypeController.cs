using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.DAL;

namespace KompetansetorgetXamarin.Controllers
{
    public class JobTypesController : BaseController
    {
        public JobTypesController()
        {
            Adress += "v1/jobTypes";
        }

        public void InsertJobType(JobType jobType)
        {
            lock (DbContext.locker)
            {
                var rowsAffected = Db.Update(jobType);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(jobType);
                }
            }
        }

        public void InsertJobTypeJob(string jobTypeId, string jobUuid)
        {
            JobTypeJob jtj = new JobTypeJob();
            jtj.JobTypeId = jobTypeId;
            jtj.JobUuid = jobUuid;

            lock (DbContext.locker)
            {
                var rowsAffected =
                    Db.Query<JobTypeJob>("Select * FROM JobTypeJob WHERE JobTypeJob.JobTypeId = ?" +
                                          " AND JobTypeJob.JobUuid = ?", jtj.JobTypeId, jtj.JobUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: JobTypeJob rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(jtj);
                }
            }
        }

        public void InsertJobTypeProject(string jobTypeId, string projectUuid)
        {
            JobTypeProject jtp = new JobTypeProject();
            jtp.JobTypeId = jobTypeId;
            jtp.ProjectUuid = projectUuid;

            lock (DbContext.locker)
            {
                var rowsAffected = Db.Query<JobTypeProject>("Select * FROM JobTypeProject WHERE JobTypeProject.JobTypeId = ?" +
                                             " AND JobTypeProject.ProjectUuid = ?", jtp.JobTypeId, jtp.ProjectUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneProjects: JobTypeProject rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(jtp);
                }
            }
        }

        /// <summary>
        /// Deletes all JobTypes from the local database.
        /// </summary>
        public void DeleteAllJobTypes()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("JobTypesController - DeleteAllJobTypes: Before delete.");
                Db.Execute("delete from " + "JobType");
                System.Diagnostics.Debug.WriteLine("JobTypesController - DeleteAllJobTypes: After delete.");
            }
        }

        /// <summary>
        /// Gets all StudyGroups from the servers REST Api.
        /// </summary>
        public async Task UpdateJobTypesFromServer()
        {
            System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: initiated");
            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress);
            System.Diagnostics.Debug.WriteLine("JobTypesController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Diagnostics.Debug.WriteLine("GetJobTypesFromServer response " + response.StatusCode.ToString());
                    var newJobTypes = await response.Content.ReadAsAsync<IEnumerable<JobType>>();
                    DeleteAllJobTypes();
                    InsertJobTypes(newJobTypes);


                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("JobTypesController - UpdateJobTypesFromServer: End Of Stack Trace");
            }
        }

        public void InsertJobTypes(IEnumerable<JobType> jobTypes)
        {
            foreach (var jobType in jobTypes)
            {
                InsertJobType(jobType);
            }
        }

        public List<JobType> GetJobTypeFilterJob()
        {
            return Db.Query<JobType>("Select * from JobType"
                                   + " where JobType.type = ?", "job");
        }

        public List<JobType> GetJobTypeFilterProject()
        {
            return Db.Query<JobType>("Select * from JobType"
                                   + " where JobType.type = ?", "project");
        }
    }
}
