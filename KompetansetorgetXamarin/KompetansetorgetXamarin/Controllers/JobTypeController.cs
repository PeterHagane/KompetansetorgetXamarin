using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
