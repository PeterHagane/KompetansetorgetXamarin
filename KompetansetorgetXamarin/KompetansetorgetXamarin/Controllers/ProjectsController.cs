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
    public class ProjectsController
    {
        private DbContext dbContext = DbContext.GetDbContext;
        private SQLiteConnection Db;

        public ProjectsController()
        {
            Db = dbContext.Db;
        }

        public async void GetProjectsFromServer()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects");
            var results = await response.Content.ReadAsAsync<IEnumerable<Project>>();
        }


        /// <summary>
        /// Gets the projects from the server including their respective company name and logo
        /// </summary>
        public async void GetProjectsWithExtraFromServer()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects?fields=cname&fields=clogo");
            var results = await response.Content.ReadAsAsync<IEnumerable<Project>>();

        }

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

        // MÅ SES OVER ER NOE SOM SKURRRER
        /// <summary>
        /// Gets the minimum information about a spesific project to build a proper notification in the notification list
        /// </summary>
        public async void GetProjectNotificationInfoServer(string uuid)
        {
            //http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects/113bff7f-7df5-47cf-ab94-b0a198f24ee1?minnot=true
            string url = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects/" + uuid + "?minnot=true";
            var client = new HttpClient();
            var response = await client.GetAsync("url");
            //if (!response.IsSuccessStatusCode) {}
            var results = await response.Content.ReadAsAsync<IEnumerable<Project>>();

            CompaniesController cc = new CompaniesController();
            foreach (var project in results)
            {
                foreach (Company c in project.companies)
                {
                    cc.InsertCompany(c);
                }
                InsertProject(project);
            }
        }

        public void InsertProject(Project project)
        {
            System.Diagnostics.Debug.WriteLine("ProjectController InsertProject(Project project): initiated");
            try
            {
                var checkIfExist = Db.Get<Project>(project.uuid);
                System.Diagnostics.Debug.WriteLine("ProjectController InsertProject(string uuid): Already exists");
                return;
            }
            catch (Exception e)
            {
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

                // This should perhaps be done above in the other loop, but because of concurrency its in its own loop.
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
            }
        }


        public void InsertProject(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("ProjectController InsertProject(string uuid): initiated");
            try { 
                // If Project already exists it will not be inserted again
                var checkIfExist = Db.Get<Project>(uuid);
                System.Diagnostics.Debug.WriteLine("ProjectController InsertProject(string uuid): Already exists");
                return;
            }
            catch (Exception e)
            {
                //Project did not exist so it will be inserted
                Project p = new Project();
                p.uuid = uuid;
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("ProjectController - InsertProject(string uuid): Under LOCK");
                    Db.Insert(p);
                    //Db.InsertOrReplaceWithChildren(p, recursive: true);
                    System.Diagnostics.Debug.WriteLine("ProjectController - InsertProject(string uuid): After Insert");
                }
            } 
        }

        public async void UpdateProjectFromServer(string uuid)
        {
            System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer(string uuid): initiated");
            string adress = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/projects/" + uuid + "?minnot=true";
            System.Diagnostics.Debug.WriteLine("UpdateProjectFromServer: var url = " + adress);

            Uri url = new Uri(adress);
            var client = new HttpClient();
            System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProjectFromServer: HttpClient created");

            string jsonString = null;
            try { 
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
                
            Project project = DeserializeOneProject(jsonString);
            UpdateProject(project);



                //var project = JsonConvert.DeserializeObject<List<Project>>(jsonString);

                /*
                Cannot deserialize the current JSON object (e.g. { "name":"value"}) 
                into type 'System.Collections.Generic.IEnumerable`1[KompetansetorgetXamarin.Models.Project]'
                because the type requires a JSON array(e.g. [1, 2, 3]) to deserialize correctly.
                To fix this error either change the JSON to a JSON array(e.g. [1, 2, 3]) 
                or change the deserialized type so that it is a normal.NET type 
                (e.g.not a primitive type like integer, not a collection type like an array or List<T>)
                that can be deserialized from a JSON object.JsonObjectAttribute can also be added to the
                type to force it to deserialize from a JSON object. Path 'uuid', line 1, position 8.
                    */

            /*
            CompaniesController cc = new CompaniesController();
            foreach (var project in results)
            {
                foreach (Company c in project.companies)
                {
                    cc.UpdateCompany(c);
                }
                UpdateProject(project);
            } */
        }
        

        public void UpdateProject(Project project)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Checks if project already is stored in database");

                // if exist project will be updated.
                var checkIfExists = Db.Get<Project>(project.uuid);

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
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: Before Updating project.");

                    Db.Update(project);
                    System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: After Updating project.");

                    // Db.InsertOrReplaceWithChildren(project, recursive: true);
                    //Db.UpdateWithChildren(project);
                }

                // This should perhaps be done above in the other loop, but because of concurrency its in its own loop.
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
                        System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: CompanyProject Already exists");

                    }
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - UpdateProject: There was no stored record of Project.");
                // project did not exist so it will be inserted.
                InsertProject(project);


            }
        }

        public List<Company> GetAllCompaniesRelatedToProject(Project project)
        {
            /*
            System.Diagnostics.Debug.WriteLine("ProjectController - GetAllCompaniesRelatedToProject: project.uuid = " +
                                               project.uuid);
            lock (DbContext.locker)
            {
                List<CompanyProject> cps = Db.GetAllWithChildren<CompanyProject>();
                System.Diagnostics.Debug.WriteLine("ProjectController - GetAllCompaniesRelatedToProject: cps.Count = " +
                                                   cps.Count);
                System.Diagnostics.Debug.WriteLine(
                    "ProjectController - GetAllCompaniesRelatedToProject: cps[0].CompanyId = " + cps[0].CompanyId);
                System.Diagnostics.Debug.WriteLine(
                    "ProjectController - GetAllCompaniesRelatedToProject: cps[0].ProjectUuid = " + cps[0].ProjectUuid);
            }
            */
            /*
            lock (DbContext.locker)
            {
                List<Company> companies = Db.GetAllWithChildren<Company>();
                System.Diagnostics.Debug.WriteLine("ProjectController - GetAllCompaniesRelatedToProject: companies[0].id = " + companies[0].id);
            }
            */
            /*
            List<Project> projects;
            lock (DbContext.locker)
            {
                projects = Db.GetAllWithChildren<Project>();
                System.Diagnostics.Debug.WriteLine(
                    "ProjectController - GetAllCompaniesRelatedToProject: projects[0].uuid = " + projects[0].uuid);
                System.Diagnostics.Debug.WriteLine(
                    "ProjectController - GetAllCompaniesRelatedToProject: projects[0].companies.Count = " +
                    projects[0].companies.Count);

            }

            List<Company> companies = new List<Company>();
            foreach (var p in projects)
            {
                if (p.uuid.Equals(project.uuid))
                {
                    System.Diagnostics.Debug.WriteLine(
                        "ProjectController - GetAllCompaniesRelatedToProject: p.companies.Count = " + p.companies.Count);
                    companies = p.companies;
                    if (companies.Count != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("WTFFF");

                       // return companies;
                    }
                }
            }
           // return companies;
           */
        
        
            lock (DbContext.locker)
            {
               return Db.Query<Company>("Select * from Company"
                                         + " inner join CompanyProject on Company.id = CompanyProject.CompanyId"
                                         + " inner join Project on CompanyProject.ProjectUuid = Project.uuid"
                                         + " where Project.uuid = ?", project.uuid);
                /*
                return Db.Query<Company>("Select * from Company"
                                         + " inner join CompanyProject on 'Company.id' == 'CompanyProject.CompanyId'"
                    // + " inner join Project on 'CompanyProject.ProjectUuid' = 'Project.uuid'"
                    //+ " where 'Project.uuid' = " + "'" + project.uuid + "';");
                                         + " WHERE 'CompanyProject.ProjectUuid' == ?", project.uuid);
                                         */
            }
        } 

/*
            var result = conn.Query<MeasurementInstanceModel>(
    "SELECT * " +
    "FROM MeasurementInstanceModel AS it " +
    "JOIN MeasurementSubjectModel AS sb " +
    "ON it.MeasurementSubjectId == sb.Id " +
    "WHERE sb.Name == ?", avariable);
            */


            /*

            select column1 from table1 
   inner join table2 on table1.column = table2.column
   where table2.columne=0

            select * from Company
             inner join CompanyProject on Company.id = CompanyProject.CompanyId
             inner join Project on CompanyProject.ProjectUuid = Project.uuid
             where Project.uuid = project.uuid

             */
        

        private Project DeserializeOneProject(string jsonString)
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

                if (key.Equals("companies"))
                {
                    // if not true then company already exist and needs to be updated.
                    Company company = new Company();
                    IEnumerable companies = (IEnumerable)dict[key];
                    //`Newtonsoft.Json.Linq.JArray'
                    System.Diagnostics.Debug.WriteLine("companies created");
                    foreach (var comp in companies)
                    {
                        System.Diagnostics.Debug.WriteLine("foreach initiated");

                        Dictionary<string, object> companyDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(comp.ToString());
                        System.Diagnostics.Debug.WriteLine("companyDict created");

                        company.id = companyDict["id"].ToString();

                        if (companyDict.ContainsKey("name"))
                        {
                            company.name = companyDict["name"].ToString();
                        }

                        if (companyDict.ContainsKey("logo"))
                        {
                            company.logo = companyDict["logo"].ToString();
                        }
                        /*
                        if (!cp.InsertCompany(company))
                        {
                            cp.UpdateCompany(company);
                        }*/
                        System.Diagnostics.Debug.WriteLine("DeserializeOneProject: Before p.companies.Add(company)");
                        p.companies.Add(company);
                        System.Diagnostics.Debug.WriteLine("DeserializeOneProject: After p.companies.Add(company)");

                    }
                }
            }
            return p;
            /*
            foreach (var item in dict)
            {
                System.Diagnostics.Debug.WriteLine("key: " + item.Key);
                System.Diagnostics.Debug.WriteLine("value: " + item.Value);
            }
            */
        }

        // return 

        //serialized.Select(u => new Project(u.Uuid));
        /*serialized.Select(p => new Project
        {
            serialized.,
            p.title,
            p.description,
            p.webpage,
            p.linkedInProfile,
            p.expiryDate,
            p.stepsToApply,
            p.created,
            p.published,
            p.modified,
            p.status,
            companies = p.companies.Select(c => new { c.id }),
            courses = p.courses.Select(l => new { l.id }),
            degrees = p.degrees.Select(d => new { d.id }),
            jobTypes = p.jobTypes.Select(jt => new { jt.id }),
            studyGroups = p.studyGroups.Select(st => new { st.id })
        }); */
   

    }
}
