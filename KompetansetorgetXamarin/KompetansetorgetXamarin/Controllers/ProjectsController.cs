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
using KompetansetorgetXamarin.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using PCLCrypto;

namespace KompetansetorgetXamarin.Controllers
{
    public class ProjectsController : BaseController
    {

        public ProjectsController()
        {
            Adress += "v1/projects";
        }

        /// <summary>
        /// Returns true if there are any new or modified projects.
        /// </summary>
        /// <returns></returns>
        private async Task<String> CheckServerForNewData(List<string> studyGroups = null, Dictionary<string, string> filter = null)
        {
            string queryParams = CreateQueryParams(studyGroups, null, filter);
            //"api/v1/jobs/lastmodifed"
            string adress = Adress + "/" + "lastmodified" + queryParams;
            System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData - adress: " + adress);
            Uri url = new Uri(adress);
            System.Diagnostics.Debug.WriteLine("ProjectController - CheckServerForNewData - url.ToString: " + url.ToString());

            var client = new HttpClient();
            DbStudent dbStudent = new DbStudent();
            string accessToken = dbStudent.GetStudentAccessToken();
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

                if (dict.ContainsKey("uuid") && dict.ContainsKey("modified") && dict.ContainsKey("hash") && dict.ContainsKey("amountOfProjects"))
                {
                    string uuid = dict["uuid"].ToString();
                    DateTime dateTime = (DateTime)dict["modified"];
                    long modified = long.Parse(dateTime.ToString("yyyyMMddHHmmss"));
                    string uuids = dict["hash"].ToString();
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

                    DbProject dbProject = new DbProject();
                    bool existInDb = dbProject.ExistsInDb(uuid, modified);
                    if (!existInDb)
                    {
                        return "newData";
                        //return existInDb;
                    }
                    var localProjects = dbProject.GetProjectsFromDbBasedOnFilter(studyGroups, filter, true);
                    int localDbCount = localProjects.Count();

                    StringBuilder sb = new StringBuilder();
                    foreach (var project in localProjects)
                    {
                        sb.Append(project.uuid);
                    }
                    string localUuids = Hasher.CalculateMd5Hash(sb.ToString());

                    // if there is a greater amount of jobs on that search filter then the job that exist 
                    // in the database has been inserted throught another search filter
                    if (uuids != localUuids)
                    {
                        if (amountOfProjects > localDbCount)
                        {
                            return "newData";
                            //return !existInDb;
                        }

                        return "incorrectCache";
                    }
                    return "exists";
                    //return existInDb;
                }
            }
            return null;
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
        /// <param name="sortBy">publish - oldest to newest
        ///                      -publish - newest to oldest
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
            DbProject db = new DbProject();
            //string adress = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects";
            string queryParams = CreateQueryParams(studyGroups, sortBy, filter);

            string instructions = await CheckServerForNewData(studyGroups, filter);
            if (!Authenticater.Authorized)
            {
                return null;
            }
            if (instructions != null)
            {
                System.Diagnostics.Debug.WriteLine("GetJobsBasedOnFilter - instructions" + instructions);
                if (instructions == "exists")
                {
                    IEnumerable<Project> filteredProjects = db.GetProjectsFromDbBasedOnFilter(studyGroups, filter);
                    filteredProjects = db.GetAllCompaniesRelatedToProjects(filteredProjects.ToList());
                    return filteredProjects;
                }
            }

            Uri url = new Uri(Adress + queryParams);
            System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - url: " + url.ToString());

            DbStudent dbStudent = new DbStudent();

            string accessToken = dbStudent.GetStudentAccessToken();

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
                    if (instructions != null && instructions == "incorrectCache")
                    {
                        var cachedProjects = db.GetProjectsFromDbBasedOnFilter(studyGroups, filter);
                        projects = DeserializeMany(jsonString);
                        // Get all jobs from that local dataset that was not in the data set provided by the server
                        // These are manually deleted projects and have to be cleared from cache.
                        // linear search is ok because of small data set
                        var manuallyDeletedProjects = cachedProjects.Where(p => !projects.Any(cp2 => cp2.uuid == p.uuid));
                        db.DeleteObsoleteProjects(manuallyDeletedProjects.ToList());
                    }
                    else
                    {
                        projects = DeserializeMany(jsonString);
                    }
                }

                else
                {
                    System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - Using the local database");
                    projects = db.GetProjectsFromDbBasedOnFilter(studyGroups, filter);
                    projects = db.GetAllCompaniesRelatedToProjects(projects.ToList());
                }
                return projects;
            }
            catch (Exception e)
            {
                // Hack workaround if mobil data and wifi is turned off
                try
                {
                    System.Diagnostics.Debug.WriteLine("GetProjectsBasedOnFilter - Using the local database");
                    projects = db.GetProjectsFromDbBasedOnFilter(studyGroups, filter);
                    projects = db.GetAllCompaniesRelatedToProjects(projects.ToList());
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
            string adress = Adress + "/" + uuid;
            System.Diagnostics.Debug.WriteLine("UpdateProjectFromServer: var url = " + adress);

            DbStudent dbStudent = new DbStudent();
            string accessToken = dbStudent.GetStudentAccessToken();
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
            Deserialize(jsonString);

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
            DbProject db = new DbProject();
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
                    DateTime dateTime = (DateTime)dict[key];
                    p.published = long.Parse(dateTime.ToString("yyyyMMddHHmmss"));
                }

                if (key.Equals("modified"))
                {
                    DateTime dateTime = (DateTime)dict[key];
                    p.modified = long.Parse(dateTime.ToString("yyyyMMddHHmmss"));
                }

                if (key.Equals("companies"))
                {
                    // if not true then company already exist and needs to be updated.
                    CompaniesController cc = new CompaniesController();
                    DbCompany dbCompany = new DbCompany();
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
                        dbCompany.UpdateCompany(company);
                        System.Diagnostics.Debug.WriteLine("Deserialize: After j.companies.Add(company)");
                        string projectUuid = dict["uuid"].ToString();
                        dbCompany.InsertCompanyProject(company.id, projectUuid);

                    }
                }
     
                if (key.Equals("courses"))
                {
                    DbCourse dbCourse = new DbCourse();
                    IEnumerable courses = (IEnumerable)dict[key];
                    //Newtonsoft.Json.Linq.JArray'
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

                        dbCourse.InsertCourse(co);
                        p.courses.Add(co);
                        string projectUuid = dict["uuid"].ToString();
                        dbCourse.InsertCourseProject(co.id, projectUuid);
                    }
                }
                
               if (key.Equals("studyGroups"))
                {
                    DbStudyGroup dbStudyGroup = new DbStudyGroup();
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
                        dbStudyGroup.InsertStudyGroupProject(sg.id, projectUuid);

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
                    DbJobType dbJobType = new DbJobType();
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

                        dbJobType.InsertJobType(jt);
                        System.Diagnostics.Debug.WriteLine("before p.jobTypes.Add(jt);");
                        p.jobTypes.Add(jt);

                        string projectUuid = dict["uuid"].ToString();
                        dbJobType.InsertJobTypeProject(jt.id, projectUuid);
                    }
                }
            }
            db.UpdateProject(p);
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
