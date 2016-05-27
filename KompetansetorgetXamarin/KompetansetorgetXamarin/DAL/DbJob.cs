using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.Models;
using SQLiteNetExtensions.Extensions;
using KompetansetorgetXamarin;

namespace KompetansetorgetXamarin.DAL
{
    public class DbJob : DbBase
    {
        /// <summary>
        /// Gets the job with the spesific uuid. 
        /// If no matching Job is found it returns null.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Job GetJobByUuid(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    return Db.GetWithChildren<Job>(uuid);
                    //return Db.Get<Job>(uuid);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbJob - GetJobByUuid(string uuid): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbJob - GetJobByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbJob - GetJobByUuid(string uuid): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Inserts the job and its respective children (only Company and CompanyJob) 
        /// into the database.
        /// </summary>
        /// <param name="job"></param>
        /// <returns>Returns true if the job was inserted, returns false if a job with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertJob(Job job)
        {
            System.Diagnostics.Debug.WriteLine("DbJob InsertJob(Job job): initiated");
            if (CheckIfJobExist(job.uuid))
            {
                return false;
            }

            //Job did not exist, safe to insert.
            DbCompany dbCompany = new DbCompany();

            foreach (Company c in job.companies)
            {
                dbCompany.InsertCompany(c);
            }

            lock (DbContext.locker)
            {
                Db.Insert(job);
                // Db.InsertOrReplaceWithChildren(job, recursive: true);
            }

            // This could perhaps be done in the above foreach loop, 
            // but because of lack of concurrency control in SQLite its done in its own loop.
            foreach (Company c in job.companies)
            {
                CompanyJob cp = new CompanyJob();
                cp.JobUuid = job.uuid;
                cp.CompanyId = c.id;
                lock (DbContext.locker)
                {
                    Db.Insert(cp);
                    // Db.InsertOrReplaceWithChildren(job, recursive: true);
                }
            }
            // Job was successfully inserted
            return true;
        }

        /// <summary>
        /// Inserts a new Job with the param as primary key 
        /// </summary>
        /// <param name="uuid">The new Jobs primary key</param>
        /// <returns>Returns true if the job was inserted, returns false if a job with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertJob(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("DbJob InsertJob(string uuid): initiated");
            if (CheckIfJobExist(uuid))
            {
                System.Diagnostics.Debug.WriteLine("DbJob InsertJob(string uuid): Job already exists");
                return false;
            }

            //Job did not exist so it will be inserted
            Job j = new Job();
            j.uuid = uuid;
            lock (DbContext.locker)
            {
                Db.Insert(j);
                //Db.InsertOrReplaceWithChildren(j, recursive: true);
                System.Diagnostics.Debug.WriteLine("DbJob - InsertJob(string uuid): Job Inserted");
                return true;
            }
        }

        /// <summary>
        /// Returns true if there exist an entry in the database matching 
        /// the jobs uuid and modified.
        /// Returns false if not.
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        public bool ExistsInDb(string uuid, long modified)
        {
            lock (DbContext.locker)
            {
                int rowsAffected = Db.Query<Job>("Select * from Job"
                                 + " where Job.uuid = ?"
                                 + " and Job.modified = ?", uuid, modified).Count;
                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine("DbJob - ExistsInDb: " + "true");
                    return true;
                }
                System.Diagnostics.Debug.WriteLine("DbJob - ExistsInDb: " + "false");
                return false;
            }
        }


        /// <summary>
        /// Updates an entry in the Job table. 
        /// If it doesnt already exist InsertJob will be called.
        /// </summary>
        /// <param name="job"></param>
        public void UpdateJob(Job job)
        {
            if (!CheckIfJobExist(job.uuid))
            {
                System.Diagnostics.Debug.WriteLine("DbJob - UpdateJob: There was no stored record of Job.");
                InsertJob(job);
            }

            else
            {
                try
                {
                    lock (DbContext.locker)
                    {
                        System.Diagnostics.Debug.WriteLine("DbJob - UpdateJob: Before Updating job.");
                        Db.Update(job);
                        System.Diagnostics.Debug.WriteLine("DbJob - UpdateJob: After Updating job.");
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("DbJob - UpdateJob: Job update failed");
                    System.Diagnostics.Debug.WriteLine("DbJob - UpdateJob: Exception msg: " + e.Message);
                    System.Diagnostics.Debug.WriteLine("DbJob - UpdateJob: Stack Trace: \n" + e.StackTrace);
                    System.Diagnostics.Debug.WriteLine("DbJob - UpdateJob: End Of Stack Trace");
                }
            }
        }

        /// <summary>
        /// Gets a list of all companies that are related to the spesific Job
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public List<Company> GetAllCompaniesRelatedToJob(Job job)
        {
            lock (DbContext.locker)
            {
                return Db.Query<Company>("Select * from Company"
                                          + " inner join CompanyJob on Company.id = CompanyJob.CompanyId"
                                          + " inner join Job on CompanyJob.JobUuid = Job.uuid"
                                          + " where Job.uuid = ?", job.uuid);
            }
        }

        /// <summary>
        /// Gets a list of all companies that are related to each Job in the list.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public List<Job> GetAllCompaniesRelatedToJobs(List<Job> jobs)
        {
            foreach (var job in jobs)
            {
                job.companies = GetAllCompaniesRelatedToJob(job);
            }
            return jobs;
        }

        public void DeleteObsoleteJobs(List<Job> obsoleteJobs)
        {
            if (obsoleteJobs != null && obsoleteJobs.Count > 0)
            {
                foreach (var job in obsoleteJobs)
                {
                    lock (DbContext.locker)
                    {
                        Db.Execute("delete from CompanyJob " +
                                   "where CompanyJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbJob - DeleteObsoleteJobs: after delete from CompanyJob");

                        Db.Execute("delete from StudyGroupJob " +
                                   "where StudyGroupJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbJob - DeleteObsoleteJobs: after delete from StudyGroupJob");

                        Db.Execute("delete from LocationJob " +
                                   "where LocationJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbJob - DeleteObsoleteJobs: after delete from LocationJob");

                        Db.Execute("delete from JobTypeJob " +
                                   "where JobTypeJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbJob - DeleteObsoleteJobs: after delete from JobTypeJob");

                        Db.Execute("delete from Job " +
                                   "where Job.uuid = ?", job.uuid);

                        System.Diagnostics.Debug.WriteLine("DbJob - DeleteObsoleteJobs: after delete from Job");
                    }
                }
            }
        }

        public void DeleteAllExpiredJobs()
        {
            DateTime today = DateTimeHandler.TrimMilliseconds(DateTime.Today);
            long todayNumber = long.Parse(today.ToString("yyyyMMddHHmmss"));
            List<Job> jobs;
            lock (DbContext.locker)
            {
                jobs = Db.Query<Job>("Select * from Job "
                                         + "where Job.expiryDate < ?", todayNumber);
            }
            System.Diagnostics.Debug.WriteLine("DbJob - DeleteAllExpiredJobs: jobs.Count: " + jobs.Count);

            if (jobs != null && jobs.Count > 0)
            {
                foreach (var job in jobs)
                {
                    lock (DbContext.locker)
                    {
                        Db.Execute("delete from CompanyJob " +
                        "where CompanyJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine("DbJob - DeleteAllExpiredJobs: after delete from CompanyJob");

                        Db.Execute("delete from StudyGroupJob " +
                        "where StudyGroupJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine("DbJob - DeleteAllExpiredJobs: after delete from StudyGroupJob");

                        Db.Execute("delete from LocationJob " +
                        "where LocationJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine("DbJob - DeleteAllExpiredJobs: after delete from LocationJob");

                        Db.Execute("delete from JobTypeJob " +
                        "where JobTypeJob.JobUuid = ?", job.uuid);
                        System.Diagnostics.Debug.WriteLine("DbJob - DeleteAllExpiredJobs: after delete from JobTypeJob");

                    }
                }
                lock (DbContext.locker)
                {
                    Db.Execute("delete from Job " +
                               "where Job.expiryDate < ?", today);
                }
                System.Diagnostics.Debug.WriteLine("DbJob - DeleteAllExpiredJobs: after delete from Job");
            }
        }

        /// <summary>
        /// Checks if there already is an entry of that Jobs primary key
        /// In the database.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns>Returns true if exist, false if it doesnt exist.</returns>
        public bool CheckIfJobExist(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Job>(uuid);
                }
                System.Diagnostics.Debug.WriteLine("DbJob - CheckIfJobExist(string uuid): Job Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbJob - CheckIfJobExist(string uuid): DB entry of job doesnt exists");
                System.Diagnostics.Debug.WriteLine("DbJob - GetJobByUuid(string uuid): Exception msg: " + e.Message);
                // System.Diagnostics.Debug.WriteLine("DbJob - GetJobByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                // System.Diagnostics.Debug.WriteLine("DbJob - GetJobByUuid(string uuid): End Of Stack Trace");
                return false;
            }
        }

        /// <summary>
        ///  Used if the web api is unavailable (not 401)
        /// </summary>
        /// <param name="studyGroups">studyGroups can be a list of numerous studygroups ex: helse, idrettsfag, datateknologi </param>
        /// <param name="filter">A dictionary where key can be: titles (values:title of the job), types (values: deltid, heltid, etc...),
        ///                      locations (values: vestagder, austagder).
        ///                      </param>
        /// <returns></returns>
        public IEnumerable<Job> GetJobsFromDbBasedOnFilter(List<string> studyGroups = null, Dictionary<string, string> filter = null, bool checkUiids = false)
        {
            StringBuilder query = new StringBuilder();
            if (checkUiids)
            {
                query.Append("SELECT Job.uuid FROM Job");
            }
            else
            {
                query.Append("SELECT * FROM Job");
            }


            if (studyGroups != null && filter == null)
            {
                for (int i = 0; i < studyGroups.Count; i++)
                {
                    if (i == 0)
                    {
                        query.Append(" INNER JOIN StudyGroupJob ON Job.uuid = StudyGroupJob.JobUuid "
                                 + "INNER JOIN StudyGroup ON StudyGroupJob.StudyGroupId = StudyGroup.id "
                                 + "WHERE StudyGroup.id = '" + studyGroups[i] + "'");
                    }
                    else
                    {
                        query.Append(" OR StudyGroup.id = '" + studyGroups[i] + "'");
                    }
                }
                query.Append(" ORDER BY Job.published DESC");
                System.Diagnostics.Debug.WriteLine("if (studyGroups != null && filter == null)");
                System.Diagnostics.Debug.WriteLine("query: " + query);
                lock (DbContext.locker)
                {
                    return Db.Query<Job>(query.ToString());
                }
            }

            if (filter != null && studyGroups == null)
            {
                string joins = "";
                StringBuilder whereAnd = new StringBuilder();

                string prepValue = "";

                foreach (var filterType in filter.Keys.ToArray())
                {
                    string value = filter[filterType];

                    if (filterType == "titles")
                    {
                        prepValue = "%" + filter[filterType] + "%";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE Job.title = ?");
                        }
                        else
                        {
                            whereAnd.Append(" AND Job.title = ?");
                        }
                    }

                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeJob ON Job.uuid = JobTypeJob.JobUuid"
                               + " INNER JOIN JobType ON JobTypeJob.JobTypeId = JobType.id";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE JobType.id = '" + value + "'");
                        }
                        else
                        {
                            whereAnd.Append(" AND JobType.id = '" + value + "'");
                        }
                    }

                    if (filterType == "locations")
                    {
                        joins += " INNER JOIN LocationJob ON Job.uuid = LocationJob.JobUuid"
                               + " INNER JOIN Location ON LocationJob.LocationId = Location.id";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE Location.id = '" + value + "'");
                        }
                        else
                        {
                            whereAnd.Append(" AND Location.id = '" + value + "'");
                        }
                    }
                }

                query.Append(joins);
                query.Append(whereAnd.ToString());
                System.Diagnostics.Debug.WriteLine("query: " + query);
                query.Append(" ORDER BY Job.published DESC");
                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Job>(query.ToString());
                    }
                }

                lock (DbContext.locker)
                {
                    return Db.Query<Job>(query.ToString(), prepValue);
                }
            }

            if (filter != null && studyGroups != null)
            {
                string joins = "";
                StringBuilder whereAnd = new StringBuilder();
                string prepValue = "";

                foreach (var filterType in filter.Keys.ToArray())
                {
                    if (filterType == "titles")
                    {
                        prepValue = filter[filterType];

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE Job.title = ?");
                        }
                        else
                        {
                            whereAnd.Append(" AND Job.title = ?");
                        }
                    }

                    string value = filter[filterType];
                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeJob ON Job.uuid = JobTypeJob.JobUuid"
                               + " INNER JOIN JobType ON JobTypeJob.JobTypeId = JobType.id";
                        // + " WHERE JobType.id = ?";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE JobType.id = '" + value + "'");
                        }
                        else
                        {
                            whereAnd.Append(" AND JobType.id = '" + value + "'");
                        }

                        /*
                        System.Diagnostics.Debug.WriteLine("if (filterType == \"types\")");
                        System.Diagnostics.Debug.WriteLine("query before prepstatement insert:" + query);
                        lock (DbContext.locker)
                        {
                            return Db.Query<Job>(query, value);
                        }
                        */
                    }

                    if (filterType == "locations")
                    {
                        joins += " INNER JOIN LocationJob ON Job.uuid = LocationJob.JobUuid"
                               + " INNER JOIN Location ON LocationJob.LocationId = Location.id";
                        //+ " WHERE Location.id = ?";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE Location.id = '" + value + "'");
                        }
                        else
                        {
                            whereAnd.Append(" AND Location.id = '" + value + "'");
                        }

                        /*
                        System.Diagnostics.Debug.WriteLine("if (filterType == \"locations\")");
                        System.Diagnostics.Debug.WriteLine("query before prepstatement insert: " + query);
                        lock (DbContext.locker)
                        {
                            return Db.Query<Job>(query, value);
                        }
                        */
                    }
                }

                for (int i = 0; i < studyGroups.Count; i++)
                {
                    if (i == 0)
                    {
                        if (studyGroups.Count > 1)
                        {
                            joins += " INNER JOIN StudyGroupJob ON Job.uuid = StudyGroupJob.JobUuid "
                                     + "INNER JOIN StudyGroup ON StudyGroupJob.StudyGroupId = StudyGroup.id ";

                            if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                            {
                                whereAnd.Append(" WHERE (StudyGroup.id = '" + studyGroups[i] + "'");
                            }
                            else
                            {
                                whereAnd.Append(" AND (StudyGroup.id = '" + studyGroups[i] + "'");
                            }
                            //+ "WHERE (StudyGroup.id = '" + studyGroups[i] + "'";
                        }
                        else
                        {
                            joins += " INNER JOIN StudyGroupJob ON Job.uuid = StudyGroupJob.JobUuid "
                                     + "INNER JOIN StudyGroup ON StudyGroupJob.StudyGroupId = StudyGroup.id ";

                            if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                            {
                                whereAnd.Append(" WHERE StudyGroup.id = '" + studyGroups[i] + "'");
                            }

                            else
                            {
                                whereAnd.Append(" AND StudyGroup.id = '" + studyGroups[i] + "'");
                            }

                        }
                    }

                    else if (i != 0 && i + 1 == studyGroups.Count)
                    {
                        whereAnd.Append(" OR StudyGroup.id = '" + studyGroups[i] + "')");
                    }
                    else
                    {
                        whereAnd.Append(" OR StudyGroup.id = '" + studyGroups[i] + "'");
                    }
                }

                System.Diagnostics.Debug.WriteLine("if(filter != null && studyGroups != null)");
                System.Diagnostics.Debug.WriteLine("query: " + query + joins + whereAnd);
                System.Diagnostics.Debug.WriteLine("full query: " + query + joins + whereAnd);


                query.Append(joins);
                query.Append(whereAnd);
                query.Append(" ORDER BY Job.published DESC");

                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Job>(query.ToString());
                    }
                }

                lock (DbContext.locker)
                {
                    return Db.Query<Job>(query.ToString(), prepValue);
                }

            }

            System.Diagnostics.Debug.WriteLine("Filter and studyGroups is null");
            System.Diagnostics.Debug.WriteLine("query: " + query);
            // if both studyGroups and filter is null
            query.Append(" ORDER BY Job.published DESC");
            lock (DbContext.locker)
            {
                return Db.Query<Job>(query.ToString());
            }
        }
    }
}
