using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        /// Gets a project based on optional filters.
        /// Current implementation supports only 1 key on the filter param!
        /// </summary>
        /// <param name="studyGroups">studyGroups can be a list of numerous studygroups ex: helse, idrettsfag, datateknologi </param>
        /// <param name="sortBy">published - oldest to newest
        ///                      -published - newest to oldest</param>
        /// <param name="filter">A dictionary where key can be: titles (values:title of the project), types (values: virksomhet, faglærer). 
        ///                      Supports only 1 key at this current implementation!</param>
        /// <returns></returns>
        public async Task<IEnumerable<Project>> GetProjectsBasedOnFilter(List<string> studyGroups = null, 
            string sortBy = "", Dictionary<string, string> filter = null)
        {
            // Projects: types, title, sortby=published (oldest to newest), sortby=-published (newest to oldest)
            // Extra for jobs: 
            // jobs: types, Location, og sorterting sortby=expirydate (descending order), sortby=-expirydate (ascending order)
            // api/v1/jobs?locations=vestagder&sortby=published  elste til nyeste
            // api/v1/jobs?locations=vestagder&sortby=-published nyeste til eldste

            //api/v1/projects?studygroups=datateknologi&sortby=published

            string queryParams = "";
            if (studyGroups != null) {
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

            if (filter != null && filter.Count == 1)
            {
                if (string.IsNullOrWhiteSpace(queryParams))
                {
                    queryParams = "?";
                }
                else queryParams += "&";
                string category = filter.Keys.ToArray()[0];
                // removes whitespaces from a potential user typed parameters like title search.
                // And replaces them with +
                string value = filter[category].Replace(" ", "+");
                queryParams += category + "=" + value;
            }

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                if (string.IsNullOrWhiteSpace(queryParams))
                {
                    queryParams = "?";
                }
                else queryParams += "&";
                queryParams += "sortby=" + sortBy;
            }

            Uri url = new Uri(Adress + queryParams);
            System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - adress: " + Adress);
            var client = new HttpClient();
            string jsonString = null;
            try
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter response " + response.StatusCode.ToString());
                jsonString = await response.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectsBasedOnFilter: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectsBasedOnFilter: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectsBasedOnFilter: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectsBasedOnFilter: End Of Stack Trace");
                return null;
                // TODO Implement local db query for cached data.
            }

            IEnumerable<Project> projects = DeserializeMany(jsonString);
            return projects;
        }

        /// <summary>
        /// Updates the Project from the servers REST Api.
        /// 
        /// This implementation also get the minimum data from the related
        /// Companies to build a proper notification list.
        /// </summary>
        /// <param name="uuid"></param>
        public async void UpdateProjectFromServer(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer(string uuid): initiated");
            string adress = Adress + "/" + uuid + "?minnot=true";
            System.Diagnostics.Debug.WriteLine("UpdateProjectFromServer: var url = " + adress);

            Uri url = new Uri(adress);
            var client = new HttpClient();
            System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer: HttpClient created");

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

            CompaniesController cp = new CompaniesController();

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
                        p.companies.Add(company);
                        System.Diagnostics.Debug.WriteLine("DeserializeOneProject: After p.companies.Add(company)");

                    }
                }

                /*
                if (key.Equals("courses"))
                {
                    
                    Same as companies implementation
                    
                }

                if (key.Equals("studyGroups"))
                {
                    
                    Same as companies implementation
                    
                }

                if (key.Equals("approvedCourses"))
                {
                    
                    Same as companies implementation
                    
                }

                if (key.Equals("degrees"))
                {
                    
                    Same as companies implementation
                    
                }

                if (key.Equals("jobTypes"))
                {
                    
                    Same as companies implementation
                    
                }

                */

            }
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
