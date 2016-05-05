using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Controls;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;

namespace KompetansetorgetXamarin.Controllers
{
    public class ProjectsController : BaseController
    {

        public ProjectsController()
        {
            Adress += "v1/projects";
        }
            
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
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): End Of Stack Trace");
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
            System.Diagnostics.Debug.WriteLine("ProjectController InsertProject(Project project): initiated");
            if (CheckIfProjectExist(project.uuid))
            {
                return false;
            }

            //Project did not exist, safe to insert.
            CompaniesController cc = new CompaniesController();

            foreach (Company c in project.companies)
            {
                cc.InsertCompany(c);
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
            System.Diagnostics.Debug.WriteLine("ProjectController InsertProject(string uuid): initiated");
            if (CheckIfProjectExist(uuid))
            {
                System.Diagnostics.Debug.WriteLine("ProjectController InsertProject(string uuid): Project already exists");
                return false;
            }

            //Project did not exist so it will be inserted
            Project p = new Project();
            p.uuid = uuid;
            lock (DbContext.locker)
            {
                Db.Insert(p);
                //Db.InsertOrReplaceWithChildren(p, recursive: true);
                System.Diagnostics.Debug.WriteLine("ProjectController - InsertProject(string uuid): Project Inserted");
                return true;
            }            
        }

        /// <summary>
        /// Returns true if there are any new or modified projects.
        /// </summary>
        /// <returns></returns>
        private async Task<bool?> CheckServerForNewData(List<string> studyGroups = null, Dictionary<string, string> filter = null)
        {
            string queryParams = CreateQueryParams(studyGroups, null, filter);
            //"api/v1/jobs/lastmodifed"
            string adress = Adress + "/" + "lastmodified" + queryParams;
            System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData - adress: " + adress);
            Uri url = new Uri(adress);
            System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData - url.ToString: " + url.ToString());

            var client = new HttpClient();
            StudentsController sc = new StudentsController();
            string accessToken = sc.GetStudentAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Authenticater.Authorized = false;
                return null;
            }
            System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData - bearer: " + accessToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            string jsonString = null;
            try
            {
                var response = await client.GetAsync(url);
                System.Diagnostics.Debug.WriteLine("CheckServerForNewData response " + response.StatusCode.ToString());
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData failed due to lack of Authorization");
                    Authenticater.Authorized = false;
                }

                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    //results = await response.Content.ReadAsAsync<IEnumerable<Job>>();
                    jsonString = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: End Of Stack Trace");
                return null;
            }
            if (jsonString != null)
            {
                // using <string, object> instead of <string, string> makes the date be stored in the right format when using .ToString()
                Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

                if (dict.ContainsKey("uuid") && dict.ContainsKey("modified") && dict.ContainsKey("amountOfProjects"))
                {
                    string uuid = dict["uuid"].ToString();
                    string modified = dict["modified"].ToString();
                    int amountOfProjects = 0;
                    try
                    {
                        amountOfProjects = Int32.Parse(dict["amountOfProjects"].ToString());
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: await client.GetAsync(\"url\") Failed");
                        System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: Exception msg: " + ex.Message);
                        System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: Stack Trace: \n" + ex.StackTrace);
                        System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData: End Of Stack Trace");
                        return null;
                    }
                    bool existInDb = ExistsInDb(uuid, modified);
                    if (!existInDb)
                    {
                        return existInDb;
                    }
                    int localDbCount = GetProjectsFromDbBasedOnFilter(studyGroups, filter).Count();
                    System.Diagnostics.Debug.WriteLine("CheckServerForNewData: localDbCount: " + localDbCount + " serverCount: " + amountOfProjects);
                    // if there is a greater amount of jobs on that search filter then the job that exist 
                    // in the database has been inserted throught another search filter
                    if (amountOfProjects > localDbCount)
                    {
                        return !existInDb;
                    }
                    return existInDb;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if there exist an entry in the database matching 
        /// the jobs uuid and modified.
        /// Returns false if not.
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="modified"></param>
        /// <returns></returns>
        private bool ExistsInDb(string uuid, string modified)
        {
            lock (DbContext.locker)
            {
                int rowsAffected = Db.Query<Project>("Select * from Project"
                                 + " where Project.uuid = ?"
                                 + " and Project.modified = ?", uuid, modified).Count;
                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine("ProjectController - ExistsInDb: " + "true");
                    return true;
                }
                System.Diagnostics.Debug.WriteLine("ProjectController - ExistsInDb: " + "false");
                return false;
            }
        }

        private string CreateQueryParams(List<string> studyGroups = null,
            string sortBy = "", Dictionary<string, string> filter = null)
        {
            string queryParams = "";
            if (studyGroups != null)
            {
                System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - studyGroups.Count(): " + studyGroups.Count());
                for (int i = 0; i < studyGroups.Count(); i++)
                {
                    if (i == 0)
                    {
                        queryParams = "?studygroups=" + studyGroups[i];
                    }

                    else
                    {
                        queryParams += "&studygroups=" + studyGroups[i];
                    }
                }
            }

            if (filter != null)
            {
                System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - amount of filters: " + filter.Keys.ToArray().Length);
                foreach (var category in filter.Keys.ToArray())
                {
                    if (string.IsNullOrWhiteSpace(queryParams))
                    {
                        queryParams = "?";
                    }
                    else queryParams += "&";
                    // removes whitespaces from a potential user typed parameters like title search.
                    // And replaces them with +
                    string value = filter[category].Replace(" ", "+");
                    queryParams += category + "=" + value;
                }
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (string.IsNullOrWhiteSpace(queryParams))
                {
                    queryParams = "?";
                }
                else queryParams += "&";
                queryParams += "sortby=" + sortBy;
            }
            return queryParams;
        }

        /// <summary>
        /// Gets a project based on optional filters.
        /// </summary>
        /// <param name="studyGroups">studyGroups can be a list of numerous studygroups ex: helse, idrettsfag, datateknologi </param>
        /// <param name="sortBy">published - oldest to newest
        ///                      -published - newest to oldest
        ///                      expirydate - descending order
        ///                      -expirydate - ascending order
        /// </param>
        /// <param name="filter">A dictionary where key can be: titles (values:title of the project), types (values: virksomhet, faglærer, etc...),
        ///                      courses (values: "IS-304" "DAT-304" osv). 
        ///                      Supports only 1 key at this current implementation!</param>
        /// <returns></returns>
        public async Task<IEnumerable<Project>> GetProjectsBasedOnFilter(List<string> studyGroups = null,
            string sortBy = "", Dictionary<string, string> filter = null)
        {
            //string adress = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects";
            string queryParams = CreateQueryParams(studyGroups, sortBy, filter);

            bool? dataExist = await CheckServerForNewData(studyGroups, filter);
            System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - dataExist" + dataExist);
            if (dataExist != null)
            {
                if ((bool)dataExist)
                {
                    IEnumerable<Project> filteredProjects = GetProjectsFromDbBasedOnFilter(studyGroups, filter);
                    filteredProjects = GetAllCompaniesRelatedToProjects(filteredProjects.ToList());
                    return filteredProjects;
                }
            }

            Uri url = new Uri(Adress + queryParams);
            System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - url: " + url.ToString());

            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Authenticater.Authorized = false;
                return null;
            }


            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            string jsonString = null;
            IEnumerable<Project> projects = null;
            try
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter response " + response.StatusCode.ToString());

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("StudentsController - UpdateStudyGroupStudent failed due to lack of Authorization");
                    Authenticater.Authorized = false;
                }

                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    jsonString = await response.Content.ReadAsStringAsync();
                    projects = DeserializeMany(jsonString);
                }

                else
                {
                    System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - Using the local database");
                    projects = GetProjectsFromDbBasedOnFilter(studyGroups, filter);
                    projects = GetAllCompaniesRelatedToProjects(projects.ToList());
                }
                return projects;
            }
            catch (Exception e)
            {
                // Hack workaround if mobil data and wifi is turned off
                try
                {
                    System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - Using the local database");
                    projects = GetProjectsFromDbBasedOnFilter(studyGroups, filter);
                    projects = GetAllCompaniesRelatedToProjects(projects.ToList());
                    return projects;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ProjectsController - GetProjectsBasedOnFilter: await client.GetAsync(\"url\") Failed");
                    System.Diagnostics.Debug.WriteLine("ProjectsController - GetProjectsBasedOnFilter: Exception msg: " + e.Message);
                    System.Diagnostics.Debug.WriteLine("ProjectsController - GetProjectsBasedOnFilter: Stack Trace: \n" + e.StackTrace);
                    System.Diagnostics.Debug.WriteLine("ProjectsController - GetProjectsBasedOnFilter: End Of Stack Trace");
                    return null;
                }
            }
        }

        /// <summary>
        /// Updates the Project from the servers REST Api.
        /// 
        /// This implementation also get the minimum data from the related
        /// Companies to build a proper notification list.
        /// </summary>
        /// <param name="uuid"></param>
        public async Task UpdateProjectFromServer(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer(string uuid): initiated");
                            //as in minimumInformationForNotifications=true
            string adress = Adress + "/" + uuid + "?minnot=true";
            System.Diagnostics.Debug.WriteLine("UpdateProjectFromServer: var url = " + adress);

            StudentsController sc = new StudentsController();
            string accessToken = sc.GetStudentAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(adress);
            var client = new HttpClient();
            System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer: HttpClient created");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            string jsonString = null;
            try
            {
                var response = await client.GetAsync(url);
                System.Diagnostics.Debug.WriteLine("UpdateProjectFromServer response " + response.StatusCode.ToString());
                //results = await response.Content.ReadAsAsync<IEnumerable<Project>>();
                jsonString = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer: End Of Stack Trace");
                return;
            }
            Project project = Deserialize(jsonString);
            UpdateProject(project);

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
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: There was no stored record of Project.");
                InsertProject(project);
            }
     
            System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: There was a record of project in the database.");

            //Project do exist.
            CompaniesController cc = new CompaniesController();
            foreach (Company c in project.companies)
            {
                if (!cc.InsertCompany(c))
                {
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: company already exists: Calling UpdateCompany.");

                    cc.UpdateCompany(c);
                }
            }

            try
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Before Updating project.");

                    Db.Update(project);
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: After Updating project.");

                    // Db.InsertOrReplaceWithChildren(project, recursive: true);
                    //Db.UpdateWithChildren(project);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Project update failed");
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: End Of Stack Trace");
            }


            // This should perhaps be done above in the other loop, but because of lack of concurrency its in its own loop.
            foreach (Company c in project.companies)
            {
                CompanyProject cp = new CompanyProject();
                cp.ProjectUuid = project.uuid;
                cp.CompanyId = c.id;
                try
                {
                    lock (DbContext.locker)
                    {
                        System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Inserting CompanyProject.");
                        Db.Insert(cp);
                        // Db.InsertOrReplaceWithChildren(project, recursive: true);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: CompanyProject Insertion failed");
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Exception msg: " + e.Message);
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Stack Trace: \n" + e.StackTrace);
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: End Of Stack Trace");
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
                System.Diagnostics.Debug.WriteLine("ProjectController - CheckIfProjectExist(string uuid): Project Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - CheckIfProjectExist(string uuid): entry of Project doesnt exists");
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): Exception msg: " + e.Message);
                // System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                // System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): End Of Stack Trace");
                return false;
            }
        }

        /// <summary>
        /// Deserializes a json formated string containing multiple Project objects
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private IEnumerable<Project> DeserializeMany(string jsonString)
        {
            System.Diagnostics.Debug.WriteLine("ProjectController - DeserializeMany Initialized");

            List<object> serializedProjects =
                JsonConvert.DeserializeObject<List<object>>(jsonString);
            //System.Diagnostics.Debug.WriteLine("ProjectController - jsonString: " + jsonString);
             
            //List<string> serializedProjects =
            //    JsonConvert.DeserializeObject<List<string>>(jsonString);

            System.Diagnostics.Debug.WriteLine("ProjectController - serializedProjects.Count(): " + serializedProjects.Count());

            List<Project> projects = new List<Project>();
            foreach (var serializedProject in serializedProjects)
            {
                projects.Add(Deserialize(serializedProject.ToString()));
            }
            return projects;
        }

        /// <summary>
        ///  Used if the web api is unavailable (not 401)
        /// </summary>
        /// <param name="studyGroups">studyGroups can be a list of numerous studygroups ex: helse, idrettsfag, datateknologi </param>
        /// <param name="filter">A dictionary where key can be: titles (values:title of the job), types (values: faglærer, virksomhet, etc...),
        ///                      courses (values: IS-304, IS-201).
        ///                      </param>
        /// <returns></returns>
        public IEnumerable<Project> GetProjectsFromDbBasedOnFilter(List<string> studyGroups = null, Dictionary<string, string> filter = null)
        {
            string query = "SELECT * FROM Project";

            if (studyGroups != null && filter == null)
            {
                for (int i = 0; i < studyGroups.Count; i++)
                {
                    if (i == 0)
                    {
                        query += " INNER JOIN StudyGroupProject ON Project.uuid = StudyGroupProject.ProjectUuid "
                                 + "INNER JOIN StudyGroup ON StudyGroupProject.StudyGroupId = StudyGroup.id "
                                 + "WHERE StudyGroup.id = '" + studyGroups[i] + "'";
                    }
                    else
                    {
                        query += " OR StudyGroup.id = '" + studyGroups[i] + "'";
                    }
                }

                System.Diagnostics.Debug.WriteLine("if (studyGroups != null && filter == null)");
                System.Diagnostics.Debug.WriteLine("query: " + query);
                lock (DbContext.locker)
                {
                    return Db.Query<Project>(query);
                }
            }

            if (filter != null && studyGroups == null)
            {
                string joins = "";
                string whereAnd = "";

                string prepValue = "";

                foreach (var filterType in filter.Keys.ToArray())
                {
                    string value = filter[filterType];

                    if (filterType == "titles")
                    {
                        prepValue = filter[filterType];

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Project.title = ?";
                        }
                        else
                        {
                            whereAnd += " AND Project.title = ?";
                        }
                    }

                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeProject ON Project.uuid = JobTypeProject.ProjectUuid"
                               + " INNER JOIN JobType ON JobTypeProject.JobTypeId = JobType.id";

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE JobType.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND JobType.id = '" + value + "'";
                        }
                    }

                    if (filterType == "courses")
                    {
                        joins += " INNER JOIN CourseProject ON Project.uuid = CourseProject.ProjectUuid"
                               + " INNER JOIN Course ON CourseProject.CourseId = Course.id";

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Course.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND Course.id = '" + value + "'";
                        }
                    }
                }

                query += joins + whereAnd;
                System.Diagnostics.Debug.WriteLine("query: " + query);

                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Project>(query);
                    }
                }

                lock (DbContext.locker)
                {
                    return Db.Query<Project>(query, prepValue);
                }
            }

            if (filter != null && studyGroups != null)
            {

                string joins = "";
                string whereAnd = "";
                string prepValue = "";

                foreach (var filterType in filter.Keys.ToArray())
                {
                    if (filterType == "titles")
                    {
                        prepValue = filter[filterType];

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Project.title = ?";
                        }
                        else
                        {
                            whereAnd += " AND Project.title = ?";
                        }
                    }

                    string value = filter[filterType];
                    if (filterType == "types")
                    {
                        joins += " INNER JOIN JobTypeProject ON Project.uuid = JobTypeProject.ProjectUuid"
                               + " INNER JOIN JobType ON JobTypeProject.JobTypeId = JobType.id";
                        // + " WHERE JobType.id = ?";

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE JobType.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND JobType.id = '" + value + "'";
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

                        if (string.IsNullOrWhiteSpace(whereAnd))
                        {
                            whereAnd += " WHERE Course.id = '" + value + "'";
                        }
                        else
                        {
                            whereAnd += " AND Course.id = '" + value + "'";
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

                            if (string.IsNullOrWhiteSpace(whereAnd))
                            {
                                whereAnd += " WHERE (StudyGroup.id = '" + studyGroups[i] + "'";
                            }
                            else
                            {
                                whereAnd += " AND (StudyGroup.id = '" + studyGroups[i] + "'";
                            }
                            //+ "WHERE (StudyGroup.id = '" + studyGroups[i] + "'";
                        }
                        else
                        {
                            joins += " INNER JOIN StudyGroupProject ON Project.uuid = StudyGroupProject.ProjectUuid "
                                     + "INNER JOIN StudyGroup ON StudyGroupProject.StudyGroupId = StudyGroup.id ";

                            if (string.IsNullOrWhiteSpace(whereAnd))
                            {
                                whereAnd += " WHERE StudyGroup.id = '" + studyGroups[i] + "'";
                            }

                            else
                            {
                                whereAnd = " AND StudyGroup.id = '" + studyGroups[i] + "'";
                            }

                        }
                    }

                    else if (i != 0 && i + 1 == studyGroups.Count)
                    {
                        whereAnd += " OR StudyGroup.id = '" + studyGroups[i] + "')";
                    }
                    else
                    {
                        whereAnd += " OR StudyGroup.id = '" + studyGroups[i] + "'";
                    }
                }

                System.Diagnostics.Debug.WriteLine("if(filter != null && studyGroups != null)");
                System.Diagnostics.Debug.WriteLine("query: " + query + joins + whereAnd);
                System.Diagnostics.Debug.WriteLine("full query: " + query + joins + whereAnd);


                query += joins + whereAnd;


                if (string.IsNullOrWhiteSpace(prepValue))
                {
                    lock (DbContext.locker)
                    {
                        return Db.Query<Project>(query);
                    }
                }

                return Db.Query<Project>(query, prepValue);

            }

            System.Diagnostics.Debug.WriteLine("Filter and studyGroups is null");
            System.Diagnostics.Debug.WriteLine("query: " + query);
            // if both studyGroups and filter is null
            lock (DbContext.locker)
            {
                return Db.Query<Project>(query);
            }
        }

        /// <summary>
        /// Deserializes a singular Project with childrem. 
        /// This method is not fully completed and should be used with caution.
        /// </summary>
        /// <param name="jsonString">Serialized data contain information about project and its children</param>
        /// <returns>A deserialized Project object</returns>
        private Project Deserialize(string jsonString)
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            System.Diagnostics.Debug.WriteLine("DeserializeApiData. Printing Key Value:");

            string[] keys = dict.Keys.ToArray();

            Project p = new Project();
            p.companies = new List<Company>();
            p.courses = new List<Course>();
            p.studyGroups = new List<StudyGroup>();
            p.jobTypes = new List<JobType>();

            foreach (var key in keys)
            {
                System.Diagnostics.Debug.WriteLine("key: " + key);
                System.Diagnostics.Debug.WriteLine("value: " + dict[key].ToString());
                /*
                if (!key.Equals("companies") || !key.Equals("courses") || !key.Equals("degrees")
                    || !key.Equals("jobTypes") || !key.Equals("studyGroup")) {} */
                if (key.Equals("uuid"))
                {
                    p.uuid = dict[key].ToString();
                }
                if (key.Equals("title"))
                {
                    p.title = dict[key].ToString();
                }

                if (key.Equals("description"))
                {
                    p.description = dict[key].ToString();
                }

                if (key.Equals("webpage"))
                {
                    p.webpage = dict[key].ToString();
                }
                if (key.Equals("published"))
                {
                    p.published = dict[key].ToString();
                }

                if (key.Equals("modified"))
                {
                    p.modified = dict[key].ToString();
                }

                if (key.Equals("companies"))
                {
                    // if not true then company already exist and needs to be updated.
                    CompaniesController cc = new CompaniesController();
                    IEnumerable companies = (IEnumerable)dict[key];
                    //`Newtonsoft.Json.Linq.JArray'
                    System.Diagnostics.Debug.WriteLine("companies created");
                    foreach (var comp in companies)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> companyDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(comp.ToString());
                        Company company = cc.DeserializeCompany(companyDict);
                        System.Diagnostics.Debug.WriteLine("Deserialize: company.id: " + company.id);
                        p.companies.Add(company);
                        cc.UpdateCompany(company);
                        System.Diagnostics.Debug.WriteLine("Deserialize: After j.companies.Add(company)");
                        string projectUuid = dict["uuid"].ToString();
                        cc.InsertCompanyProject(company.id, projectUuid);

                    }
                }
     
                if (key.Equals("courses"))
                {

                    IEnumerable courses = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    CoursesController cc = new CoursesController();
                    System.Diagnostics.Debug.WriteLine("location created");
                    foreach (var course in courses)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> courseDict =
                            JsonConvert.DeserializeObject<Dictionary<string, object>>(course.ToString());

                        Course co = new Course();
                        if (courseDict.ContainsKey("id"))
                        {

                            co.id = courseDict["id"].ToString();
                            System.Diagnostics.Debug.WriteLine("Course id: " + co.id);
                        }

                        if (courseDict.ContainsKey("name"))
                        {
                            co.name = courseDict["name"].ToString();
                        }

                        cc.InsertCourse(co);
                        p.courses.Add(co);
                        string projectUuid = dict["uuid"].ToString();
                        cc.InsertCourseProject(co.id, projectUuid);
                    }
                }
                
               if (key.Equals("studyGroups"))
                {
                    StudyGroupsController sgc = new StudyGroupsController();
                    IEnumerable studyGroups = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    System.Diagnostics.Debug.WriteLine("studyGroups created");
                    foreach (var studyGroup in studyGroups)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> studyGroupDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(studyGroup.ToString());

                        
                        StudyGroup sg = new StudyGroup();
                        if (studyGroupDict.ContainsKey("id"))
                        {
                            sg.id = studyGroupDict["id"].ToString();
                        }

                        if (studyGroupDict.ContainsKey("name"))
                        {
                            sg.name = studyGroupDict["name"].ToString();
                        }

                        p.studyGroups.Add(sg);

                        string projectUuid = dict["uuid"].ToString();
                        sgc.InsertStudyGroupProject(sg.id, projectUuid);

                    }
                }
                /*
                if (key.Equals("approvedCourses"))
                {
                    
                    Same as companies implementation
                    
                }

                if (key.Equals("degrees"))
                {
                    
                    Same as companies implementation
                    
                }
                */
                if (key.Equals("jobTypes"))
                {
                    JobTypesController jtc = new JobTypesController();
                    IEnumerable jobTypes = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
                    System.Diagnostics.Debug.WriteLine("jobTypes created");
                    foreach (var jobType in jobTypes)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");
                        Dictionary<string, object> jtDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jobType.ToString());

                        JobType jt = new JobType();
                        if (jtDict.ContainsKey("id"))
                        {
                            jt.id = jtDict["id"].ToString();
                        }

                        if (jtDict.ContainsKey("name"))
                        {
                            jt.name = jtDict["name"].ToString();
                        }

                        jtc.InsertJobType(jt);
                        System.Diagnostics.Debug.WriteLine("before p.jobTypes.Add(jt);");
                        p.jobTypes.Add(jt);

                        string projectUuid = dict["uuid"].ToString();
                        jtc.InsertJobTypeProject(jt.id, projectUuid);
                    }
                }
            }
            UpdateProject(p);
            return p;
        }


        /*
        public async void GetProjectsFromServer()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects");
            var results = await response.Content.ReadAsAsync<IEnumerable<Project>>();
        }
        */

    }
}
