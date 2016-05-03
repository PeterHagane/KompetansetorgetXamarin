﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;

namespace KompetansetorgetXamarin.Controllers
{
    class StudyGroupsController : BaseController
    {
        public StudyGroupsController()
        {
            Adress += "v1/studygroups";
        }

        /// <summary>
        /// Inserts the project and its respective children (only Company and CompanyProject) 
        /// into the database.
        /// </summary>
        /// <param name="studyGroup"></param>
        /// <returns>Returns true if the project was inserted, returns false if a project with the same 
        ///  uuid (primary key) already exists in the table.</returns>
        public bool InsertStudyGroup(StudyGroup studyGroup)
        {
            if (CheckIfStudyGroupExist(studyGroup.id))
            {
                return false;
            }

            lock (DbContext.locker)
            {
                Db.Insert(studyGroup);
            }
            return true;
        }

        /// <summary>
        /// Checks if there already is an entry of that Device primary key
        /// In the database.
        /// Returns true if exist, false if it doesnt exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns true if exist, false if it doesnt exist.</returns>
        public bool CheckIfStudyGroupExist(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Device>(id);
                }
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CheckIfStudyGroupExist: StudyGroup Already exists");
                return true;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CheckIfStudyGroupExist: entry of StudyGroup doesnt exists");
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - CheckIfStudyGroupExist: Exception msg: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets all StudyGroups from the servers REST Api.
        /// </summary>
        public async Task GetStudyGroupsFromServer()
        {
            System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: initiated");
            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress);
            System.Diagnostics.Debug.WriteLine("StudyGroupsController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK) {
                    System.Diagnostics.Debug.WriteLine("GetStudyGroupsFromServer response " + response.StatusCode.ToString());
                    var results = await response.Content.ReadAsAsync<IEnumerable<StudyGroup>>();
                    foreach (var studygroup in results)
                    {
                        UpdateStudyGroups(studygroup);
                    }
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - GetStudyGroupsFromServer: End Of Stack Trace");
            }
        }

        /// <summary>
        /// Updates a studygroup, if it doesnt already exist a new entry will be inserted into the db.
        /// </summary>
        /// <param name="studyGroup"></param>
        public void UpdateStudyGroups(StudyGroup studyGroup)
        {
            if (CheckIfStudyGroupExist(studyGroup.id)) 
            {
                System.Diagnostics.Debug.WriteLine("StudyGroupsController - UpdateStudyGroups: updates: " + studyGroup.id);
                lock (DbContext.locker)
                {
                    Db.Update(studyGroup);
                }
            }

            else
            { 
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("StudyGroupsController - UpdateStudyGroups: inserts: + " + studyGroup.id);
                    
                    Db.Insert(studyGroup);
                }
            }            
        }

        /// <summary>
        /// Gets a studygroup based in its id
        /// </summary>
        /// <returns></returns>
        public StudyGroup GetStudygroup(string id)
        {
            lock (DbContext.locker)
            {
                return Db.Get<StudyGroup>(id);
            }
        }

        /// <summary>
        /// Returns a List containing all stored StudyGroups
        /// </summary>
        /// <returns></returns>
        public List<StudyGroup> GetAllStudyGroups()
        {
            lock (DbContext.locker)
            {
                return Db.Query<StudyGroup>("Select * from StudyGroup");
            }
        }
    }
}
