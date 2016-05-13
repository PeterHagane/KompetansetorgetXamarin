using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.DAL
{
    public class DbJobType : DbBase
    {
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
                System.Diagnostics.Debug.WriteLine("DbJobTypes - DeleteAllJobTypes: Before delete.");
                Db.Execute("delete from " + "JobType");
                System.Diagnostics.Debug.WriteLine("DbJobTypes - DeleteAllJobTypes: After delete.");
            }
        }

        public void UpdateJobTypes(List<JobType> jobTypes)
        {
            foreach (var jobType in jobTypes)
            {
                UpdateJobType(jobType);
            }
        }

        // fix later
        public void UpdateJobType(JobType jobType)
        {
            try
            {
                lock (DbContext.locker)
                {
                    Db.Update(jobType);
                }
            }
            catch { }
        }

        public void InsertJobTypes(IEnumerable<JobType> jobTypes)
        {
            foreach (var jobType in jobTypes)
            {
                jobType.filterChecked = false;
                InsertJobType(jobType);
            }
        }

        public List<JobType> GetJobTypeFilterJob()
        {
            lock (DbContext.locker)
            {
                return Db.Query<JobType>("Select * from JobType"
                                         + " where JobType.type = ?", "job");
            }
        }

        public List<JobType> GetJobTypeFilterProject()
        {
            lock (DbContext.locker)
            {
                return Db.Query<JobType>("Select * from JobType"
                                         + " where JobType.type = ?", "project");
            }
        }

        public List<JobType> GetAllJobTypes()
        {
            lock (DbContext.locker)
            {
                return Db.Query<JobType>("Select * from JobType"
                                         + " ORDER BY JobType.id ASC");
            }
        }
    }
}
