using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}

