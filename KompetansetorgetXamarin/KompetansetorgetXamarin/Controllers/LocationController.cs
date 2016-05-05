using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.DAL;

namespace KompetansetorgetXamarin.Controllers
{
    public class LocationsController : BaseController
    {
        public LocationsController()
        {
            Adress += "v1/locations";
        }

        public void InsertLocation(Location location)
        {
            lock (DbContext.locker)
            {
                var rowsAffected = Db.Update(location);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(location);
                }
            }
        }

        public void InsertLocationJob(string locationId, string jobUuid)
        { 
            LocationJob lj = new LocationJob();
            lj.LocationId = locationId;
            lj.JobUuid = jobUuid;
            lock (DbContext.locker)
            {
                var rowsAffected =
                Db.Query<LocationJob>("Select * FROM LocationJob WHERE LocationJob.LocationId = ?" +
                                      " AND LocationJob.JobUuid = ?", lj.LocationId, lj.JobUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: StudyGroupJob rowsAffected: " + rowsAffected);
                 if (rowsAffected == 0)
                 {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(lj);
                 }
            }
        }

        /// <summary>
        /// Gets all Locations from the servers REST Api.
        /// </summary>
        public async Task UpdateLocationsFromServer()
        {
            System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: initiated");
            StudentsController sc = new StudentsController();

            string accessToken = sc.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress);
            System.Diagnostics.Debug.WriteLine("LocationsController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Diagnostics.Debug.WriteLine("UpdateLocationsFromServer response " + response.StatusCode.ToString());
                    var newLocations = await response.Content.ReadAsAsync<IEnumerable<Location>>();
                    DeleteAllLocations();
                    InsertLocations(newLocations);
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: End Of Stack Trace");
            }
        }

        /// <summary>
        /// Deletes all Location from the local database.
        /// </summary>
        private void DeleteAllLocations()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("LocationsController - DeleteAllLocations: Before delete.");
                Db.Execute("delete from " + "Location");
                System.Diagnostics.Debug.WriteLine("LocationsController - DeleteAllLocations: After delete.");
            }
        }

        private void InsertLocations(IEnumerable<Location> locations)
        {
            foreach (var location in locations)
            {
                InsertLocation(location);
            }
        }

        /// <summary>
        /// Returns a List containing all stored Locations
        /// </summary>
        /// <returns></returns>
        public List<Location> GetAllLocations()
        {
            lock (DbContext.locker)
            {
                return Db.Query<Location>("Select * from Location");
            }
        }
    }
}

