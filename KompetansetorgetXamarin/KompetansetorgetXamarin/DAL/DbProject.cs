using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.Models;
using SQLiteNetExtensions.Extensions;

namespace KompetansetorgetXamarin.DAL
{
    public class DbProject : DbBase
    {
        /// <summary>
        /// Gets the project with the spesific uuid. 
        /// If no matching Project is found it returns null.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public Project GetProjectByUuid(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    return Db.GetWithChildren<Project>(uuid);
                    //return Db.Get<Project>(uuid);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbProject - GetProjectByUuid(string uuid): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DbProject - GetProjectByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DbProject - GetProjectByUuid(string uuid): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Inserts the project and its respective children (only Company and CompanyProject) 
        /// into the database.
        /// </summary>
        /// <param name="project"></param>
        /// <returns>Returns true if the project was inserted, returns false if a project with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertProject(Project project)
        {
            System.Diagnostics.Debug.WriteLine("DbProject InsertProject(Project project): initiated");
            if (CheckIfProjectExist(project.uuid))
            {
                return false;
            }

            //Project did not exist, safe to insert.
            DbCompany dbCompany = new DbCompany();

            foreach (Company c in project.companies)
            {
                dbCompany.InsertCompany(c);
            }

            lock (DbContext.locker)
            {
                Db.Insert(project);
                // Db.InsertOrReplaceWithChildren(project, recursive: true);
            }

            // This could perhaps be done in the above foreach loop, 
            // but because of lack of concurrency control in SQLite its done in its own loop.
            foreach (Company c in project.companies)
            {
                CompanyProject cp = new CompanyProject();
                cp.ProjectUuid = project.uuid;
                cp.CompanyId = c.id;
                lock (DbContext.locker)
                {
                    Db.Insert(cp);
                    // Db.InsertOrReplaceWithChildren(project, recursive: true);
                }
            }
            // Project was successfully inserted
            return true;
        }

        /// <summary>
        /// Inserts a new Project with the param as primary key 
        /// </summary>
        /// <param name="uuid">The new Projects primary key</param>
        /// <returns>Returns true if the project was inserted, returns false if a project with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertProject(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("DbProject InsertProject(string uuid): initiated");
            if (CheckIfProjectExist(uuid))
            {
                System.Diagnostics.Debug.WriteLine("DbProject InsertProject(string uuid): Project already exists");
                return false;
            }

            //Project did not exist so it will be inserted
            Project p = new Project();
            p.uuid = uuid;
            lock (DbContext.locker)
            {
                Db.Insert(p);
                //Db.InsertOrReplaceWithChildren(p, recursive: true);
                System.Diagnostics.Debug.WriteLine("DbProject - InsertProject(string uuid): Project Inserted");
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
                int rowsAffected = Db.Query<Project>("Select * from Project"
                                 + " where Project.uuid = ?"
                                 + " and Project.modified = ?", uuid, modified).Count;
                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine("DbProject - ExistsInDb: " + "true");
                    return true;
                }
                System.Diagnostics.Debug.WriteLine("DbProject - ExistsInDb: " + "false");
                return false;
            }
        }

        /// <summary>
        /// Updates an entry in the Project table. 
        /// If it doesnt already exist InsertProject will be called.
        /// </summary>
        /// <param name="project"></param>
        public void UpdateProject(Project project)
        {
            if (!CheckIfProjectExist(project.uuid))
            {
                System.Diagnostics.Debug.WriteLine(
                    "DbProject - UpdateProject: There was no stored record of Project.");
                InsertProject(project);
            }

            else
            {
                try
                {
                    lock (DbContext.locker)
                    {
                        System.Diagnostics.Debug.WriteLine("DbProject - UpdateProject: Before Updating project.");
                        Db.Update(project);
                        System.Diagnostics.Debug.WriteLine("DbProject - UpdateProject: After Updating project.");
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("DbProject - UpdateProject: project update failed");
                    System.Diagnostics.Debug.WriteLine("DbProject - UpdateProject: Exception msg: " + e.Message);
                    System.Diagnostics.Debug.WriteLine("DbProject - UpdateProject: Stack Trace: \n" + e.StackTrace);
                    System.Diagnostics.Debug.WriteLine("DbProject - UpdateProject: End Of Stack Trace");
                }
            }
        }

        /// <summary>
        /// Gets a list of all companies that are related to the spesific Project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public List<Company> GetAllCompaniesRelatedToProject(Project project)
        {
            lock (DbContext.locker)
            {
                return Db.Query<Company>("Select * from Company"
                                          + " inner join CompanyProject on Company.id = CompanyProject.CompanyId"
                                          + " inner join Project on CompanyProject.ProjectUuid = Project.uuid"
                                          + " where Project.uuid = ?", project.uuid);
            }
        }

        /// <summary>
        /// Gets a list of all companies that are related to each Project in the list.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public List<Project> GetAllCompaniesRelatedToProjects(List<Project> projects)
        {
            foreach (var project in projects)
            {
                project.companies = GetAllCompaniesRelatedToProject(project);
            }
            return projects;
        }

        /// <summary>
        /// Checks if there already is an entry of that Projects primary key
        /// In the database.
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns>Returns true if exist, false if it doesnt exist.</returns>
        public bool CheckIfProjectExist(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Project>(uuid);
                }
                System.Diagnostics.Debug.WriteLine("DbProject - CheckIfProjectExist(string uuid): Project Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbProject - CheckIfProjectExist(string uuid): entry of Project doesnt exists");
                System.Diagnostics.Debug.WriteLine("DbProject - GetProjectByUuid(string uuid): Exception msg: " + e.Message);
                // System.Diagnostics.Debug.WriteLine("DbProject - GetProjectByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                // System.Diagnostics.Debug.WriteLine("DbProject - GetProjectByUuid(string uuid): End Of Stack Trace");
                return false;
            }
        }

        public void DeleteObsoleteProjects(List<Project> obsoleteProjects)
        {
            if (obsoleteProjects != null && obsoleteProjects.Count > 0)
            {
                foreach (var project in obsoleteProjects)
                {
                    lock (DbContext.locker)
                    {
                        Db.Execute("delete from CompanyProject " +
                                   "where CompanyProject.ProjectUuid = ?", project.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbProject - DeleteObsoleteProjects: after delete from CompanyProject");

                        Db.Execute("delete from StudyGroupProject " +
                                   "where StudyGroupProject.ProjectUuid = ?", project.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbProject - DeleteObsoleteProjects: after delete from StudyGroupProject");

                        Db.Execute("delete from CourseProject " +
                                   "where CourseProject.ProjectUuid = ?", project.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbProject - DeleteObsoleteProjects: after delete from CourseProject");

                        Db.Execute("delete from JobTypeProject " +
                                   "where JobTypeProject.ProjectUuid = ?", project.uuid);
                        System.Diagnostics.Debug.WriteLine(
                            "DbProject - DeleteObsoleteProjects: after delete from JobTypeProject");

                        Db.Execute("delete from Project " +
                                   "where Project.uuid = ?", project.uuid);

                        System.Diagnostics.Debug.WriteLine("DbProject - DeleteObsoleteProjects: after delete from Project");
                    }
                }
            }
        }

        /// <summary>
        ///  Used if the web api is unavailable (not 401)
        /// </summary>
        /// <param name="studyGroups">studyGroups can be a list of numerous studygroups ex: helse, idrettsfag, datateknologi </param>
        /// <param name="filter">A dictionary where key can be: titles (values:title of the job), types (values: faglærer, virksomhet, etc...),
        ///                      courses (values: IS-304, IS-201).
        ///                      </param>
        /// <returns></returns>
        public IEnumerable<Project> GetProjectsFromDbBasedOnFilter(List<string> studyGroups = null, Dictionary<string, string> filter = null, bool checkUiids = false)
        {
            //string query = "";
            StringBuilder query = new StringBuilder();
            if (checkUiids)
            {
                query.Append("SELECT Project.uuid FROM Project");
            }
            else
            {
                query.Append("SELECT * FROM Project");
            }


            if (studyGroups != null && filter == null)
            {
                for (int i = 0; i < studyGroups.Count; i++)
                {
                    if (i == 0)
                    {
                        query.Append(" INNER JOIN StudyGroupProject ON Project.uuid = StudyGroupProject.ProjectUuid "
                                 + "INNER JOIN StudyGroup ON StudyGroupProject.StudyGroupId = StudyGroup.id "
                                 + "WHERE StudyGroup.id = '" + studyGroups[i] + "'");
                    }
                    else
                    {
                        query.Append(" OR StudyGroup.id = '" + studyGroups[i] + "'");
                    }
                }
                query.Append(" ORDER BY Project.published DESC");
                System.Diagnostics.Debug.WriteLine("if (studyGroups != null && filter == null)");
                System.Diagnostics.Debug.WriteLine("query: " + query.ToString());
                lock (DbContext.locker)
                {
                    return Db.Query<Project>(query.ToString());
                }
            }

            if (filter != null && studyGroups == null)
            {
                string joins = "";
                //string whereAnd = "";
                StringBuilder whereAnd = new StringBuilder();
                string prepValue = "";

                foreach (var filterType in filter.Keys.ToArray())
                {
                    string value = filter[filterType];

                    if (filterType == "titles")
                    {
                        prepValue = filter[filterType];

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE Project.title = ?");
                        }
                        else
                        {
                            whereAnd.Append(" AND Project.title = ?");
                        }
                    }

                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeProject ON Project.uuid = JobTypeProject.ProjectUuid"
                               + " INNER JOIN JobType ON JobTypeProject.JobTypeId = JobType.id";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE JobType.id = '" + value + "'");
                        }
                        else
                        {
                            whereAnd.Append(" AND JobType.id = '" + value + "'");
                        }
                    }

                    if (filterType == "courses")
                    {
                        joins += " INNER JOIN CourseProject ON Project.uuid = CourseProject.ProjectUuid"
                               + " INNER JOIN Course ON CourseProject.CourseId = Course.id";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE Course.id = '" + value + "'");
                        }
                        else
                        {
                            whereAnd.Append(" AND Course.id = '" + value + "'");
                        }
                    }
                }

                query.Append(joins);
                query.Append(whereAnd.ToString());
                query.Append(" ORDER BY Project.published DESC");
                System.Diagnostics.Debug.WriteLine("query: " + query.ToString());

                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Project>(query.ToString());
                    }
                }

                lock (DbContext.locker)
                {
                    return Db.Query<Project>(query.ToString(), prepValue);
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
                            whereAnd.Append(" WHERE Project.title = ?");
                        }
                        else
                        {
                            whereAnd.Append(" AND Project.title = ?");
                        }
                    }

                    string value = filter[filterType];
                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeProject ON Project.uuid = JobTypeProject.ProjectUuid"
                               + " INNER JOIN JobType ON JobTypeProject.JobTypeId = JobType.id";
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
                            return Db.Query<Project>(query, value);
                        }
                        */
                    }

                    if (filterType == "courses")
                    {
                        joins += " INNER JOIN CourseProject ON Project.uuid = CourseProject.ProjectUuid"
                               + " INNER JOIN Course ON CourseProject.CourseId = Course.id";
                        //+ " WHERE Course.id = ?";

                        if (string.IsNullOrWhiteSpace(whereAnd.ToString()))
                        {
                            whereAnd.Append(" WHERE Course.id = '" + value + "'");
                        }
                        else
                        {
                            whereAnd.Append(" AND Course.id = '" + value + "'");
                        }

                        /*
                        System.Diagnostics.Debug.WriteLine("if (filterType == \"courses\")");
                        System.Diagnostics.Debug.WriteLine("query before prepstatement insert: " + query);
                        lock (DbContext.locker)
                        {
                            return Db.Query<Project>(query, value);
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
                            joins += " INNER JOIN StudyGroupProject ON Project.uuid = StudyGroupProject.ProjectUuid "
                                     + "INNER JOIN StudyGroup ON StudyGroupProject.StudyGroupId = StudyGroup.id ";

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
                            joins += " INNER JOIN StudyGroupProject ON Project.uuid = StudyGroupProject.ProjectUuid "
                                     + "INNER JOIN StudyGroup ON StudyGroupProject.StudyGroupId = StudyGroup.id ";

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
                query.Append(whereAnd.ToString());
                query.Append(" ORDER BY Project.published DESC");

                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Project>(query.ToString());
                    }
                }

                return Db.Query<Project>(query.ToString(), prepValue);

            }

            System.Diagnostics.Debug.WriteLine("Filter and studyGroups is null");
            System.Diagnostics.Debug.WriteLine("query: " + query);
            // if both studyGroups and filter is null
            query.Append(" ORDER BY Project.published DESC");
            lock (DbContext.locker)
            {
                return Db.Query<Project>(query.ToString());
            }
        }        
    }
}
