using System;
using KompetansetorgetXamarin.Models;
using SQLite.Net;

namespace KompetansetorgetXamarin.DAL
{
    public class DbCompany : DbBase
    { 
        /// <summary>
        /// Inserts the CompanyProject if it doesnt already exist
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="projectUuid"></param>
        public void InsertCompanyProject(string companyId, string projectUuid)
        {
            CompanyProject cp = new CompanyProject();
            cp.CompanyId = companyId;
            cp.ProjectUuid = projectUuid;
            lock (DbContext.locker)
            {

                //System.Diagnostics.Debug.WriteLine("Deserialize: query: " + query);
                var rowsAffected = Db.Query<CompanyProject>("Select * FROM CompanyProject WHERE CompanyProject.CompanyId = ?" +
                               " AND CompanyProject.ProjectUuid = ?", companyId, projectUuid).Count;
                System.Diagnostics.Debug.WriteLine("Deserialize: CompanyProject rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(cp);
                }
            }
        }

        /// <summary>
        /// Inserts the CompanyJob if it doesnt already exist
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="jobUuid"></param>
        public void InsertCompanyJob(string companyId, string jobUuid)
        {
            CompanyJob cj = new CompanyJob();
            cj.CompanyId = companyId;
            cj.JobUuid = jobUuid;

            lock (DbContext.locker)
            {

                //System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: query: " + query);
                var rowsAffected = Db.Query<CompanyJob>("Select * FROM CompanyJob WHERE CompanyJob.CompanyId = ?" +
                               " AND CompanyJob.JobUuid = ?", cj.CompanyId, cj.JobUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: CompanyJobs rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(cj);
                }
            }
        }

        /// <summary>
        /// Inserts a Company into the database
        /// </summary>
        /// <param name="company"></param>
        /// <returns>Returns true if successful, false if an entry with the same id (pk) already exists</returns>
        public bool InsertCompany(Company company)
        {
            //db.Table<Company>()
            System.Diagnostics.Debug.WriteLine("DbCompany - InsertCompany: Initiated");

            try
            {
                // if exist company will not be inserted.
                lock (DbContext.locker)
                {
                    var checkIfExists = Db.Get<Company>(company.id);
                }
                System.Diagnostics.Debug.WriteLine("DbCompany - InsertCompany: Company already exists");
                return false;
            }

            catch (Exception e)
            {
                // if not exist company will be inserted.
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("DbCompany - InsertCompany: before insert");
                    Db.Insert(company);
                    System.Diagnostics.Debug.WriteLine("DbCompany - InsertCompany: successfully inserted");
                    return true;
                }
            }
        }

        /// <summary>
        /// Updates a Company, but if it doesnt already exist a new entry will be inserted into the db.
        /// </summary>
        /// <param name="company"></param>
        public void UpdateCompany(Company company)
        {
            try
            {
                // if exist company will be updated.

                lock (DbContext.locker)
                {
                    var checkIfExists = Db.Get<Company>(company.id);
                    Db.Update(company);
                }
            }

            catch (Exception e)
            {
                // if not exist company will be inserted.
                lock (DbContext.locker)
                {
                    Db.Insert(company);
                }
            }
        }

    }
}