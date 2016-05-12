using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.DAL
{
    public class DbLocation : DbBase
    {
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

        public void InsertLocations(IEnumerable<Location> locations)
        {
            foreach (var location in locations)
            {
                InsertLocation(location);
            }
        }

        public void UpdateLocations(List<Location> locations)
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
                return Db.Query<Location>("Select * from Location ORDER BY Location.id ASC");
            }
        }

        /// <summary>
        /// Deletes all Location from the local database.
        /// </summary>
        public void DeleteAllLocations()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("DbLocation - DeleteAllLocations: Before delete.");
                Db.Execute("delete from " + "Location");
                System.Diagnostics.Debug.WriteLine("DbLocation - DeleteAllLocations: After delete.");
            }
        }
    }
}
