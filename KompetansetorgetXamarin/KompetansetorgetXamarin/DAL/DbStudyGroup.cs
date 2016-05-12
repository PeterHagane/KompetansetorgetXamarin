using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;

namespace KompetansetorgetXamarin.DAL
{
    public class DbStudyGroup : DbBase
    {
        /// <summary>
        /// Inserts the studyGroup into the database.
        /// </summary>
        /// <param name="studyGroup"></param>
        /// <returns>Returns true if the studyGroup was inserted, 
        /// </returns>
        public bool InsertStudyGroup(StudyGroup studyGroup)
        {

            lock (DbContext.locker)
            {
                var rowsAffected = Db.Update(studyGroup);
                if (rowsAffected == 0)
                {
                    Db.Insert(studyGroup);
                }
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
                System.Diagnostics.Debug.WriteLine("DbStudyGroup - CheckIfStudyGroupExist: StudyGroup Already exists");
                return true;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DbStudyGroup - CheckIfStudyGroupExist: entry of StudyGroup doesnt exists");
                System.Diagnostics.Debug.WriteLine("DbStudyGroup - CheckIfStudyGroupExist: Exception msg: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes all StudyGroup from the local database.
        /// </summary>
        public void DeleteAllStudyGroups()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("DbStudyGroup - DeleteAllStudyGroups: Before delete.");
                Db.Execute("delete from " + "StudyGroup");
                System.Diagnostics.Debug.WriteLine("DbStudyGroup - DeleteAllStudyGroups: After delete.");
            }
        }

        public void UpdateStudyGroups(List<StudyGroup> studyGroups)
        {
            foreach (var studygroup in studyGroups)
            {
                UpdateStudyGroup(studygroup);
            }
        }

        /// <summary>
        /// Updates a studygroup, if it doesnt already exist a new entry will be inserted into the db.
        /// </summary>
        /// <param name="studyGroup"></param>
        public void UpdateStudyGroup(StudyGroup studyGroup)
        {
            if (CheckIfStudyGroupExist(studyGroup.id))
            {
                System.Diagnostics.Debug.WriteLine("DbStudyGroup - UpdateStudyGroups: updates: " + studyGroup.id);
                lock (DbContext.locker)
                {
                    Db.Update(studyGroup);
                }
            }

            else
            {
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("DbStudyGroup - UpdateStudyGroups: inserts: + " + studyGroup.id);

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

        public void InsertStudyGroupJob(string studygroupId, string jobUuid)
        {
            System.Diagnostics.Debug.WriteLine("StudyGroupJob created");
            StudyGroupJob sgj = new StudyGroupJob();
            sgj.StudyGroupId = studygroupId;
            sgj.JobUuid = jobUuid;
            System.Diagnostics.Debug.WriteLine("StudyGroupJob before insert");

            lock (DbContext.locker)
            {
                var rowsAffected =
                    Db.Query<StudyGroupJob>(
                        "Select * FROM StudyGroupJob WHERE StudyGroupJob.StudyGroupId = ?" +
                        " AND StudyGroupJob.JobUuid = ?", sgj.StudyGroupId, sgj.JobUuid).Count;
                System.Diagnostics.Debug.WriteLine("DeserializeOneJobs: StudyGroupJob rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(sgj);
                }
            }
            System.Diagnostics.Debug.WriteLine("StudyGroupJob after insert");
        }

        public void InsertStudyGroupProject(string studygroupId, string projectUuid)
        {
            System.Diagnostics.Debug.WriteLine("StudyGroupProject created");
            StudyGroupProject sgp = new StudyGroupProject();
            sgp.StudyGroupId = studygroupId;
            sgp.ProjectUuid = projectUuid;
            System.Diagnostics.Debug.WriteLine("StudyGroupProject before insert");

            lock (DbContext.locker)
            {
                var rowsAffected =
                    Db.Query<StudyGroupProject>(
                        "Select * FROM StudyGroupProject WHERE StudyGroupProject.StudyGroupId = ?" +
                        " AND StudyGroupProject.ProjectUuid = ?", sgp.StudyGroupId, sgp.ProjectUuid).Count;
                System.Diagnostics.Debug.WriteLine("Deserialize: StudyGroupProject rowsAffected: " +
                                                   rowsAffected);
                if (rowsAffected == 0)
                {
                    // The item does not exists in the database so safe to insert
                    Db.Insert(sgp);
                }
            }
            System.Diagnostics.Debug.WriteLine("StudyGroupProject after insert");
        }
    }
}
