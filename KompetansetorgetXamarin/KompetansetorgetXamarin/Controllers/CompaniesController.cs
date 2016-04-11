using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;

namespace KompetansetorgetXamarin.Controllers
{
    public class CompaniesController
    {
        private DbContext dbContext = DbContext.GetDbContext;
        private SQLiteConnection Db;

        public CompaniesController()
        {
            Db = dbContext.Db;
        }
        public bool InsertCompany(Company company)
        {
            //db.Table<Company>()
            System.Diagnostics.Debug.WriteLine("CompaniesController - InsertCompany: Initiated");

            try
            {
                // if exist company will not be inserted.
                var checkIfExists = Db.Get<Company>(company.id);
                System.Diagnostics.Debug.WriteLine("CompaniesController - InsertCompany: Company already exists");
                return false;
            }

            catch (Exception e)
            {
                // if not exist company will be inserted.
                lock (DbContext.locker)
                {
                    System.Diagnostics.Debug.WriteLine("CompaniesController - InsertCompany: before insert");
                    Db.Insert(company);
                    System.Diagnostics.Debug.WriteLine("CompaniesController - InsertCompany: successfully inserted");
                    return true;
                }
            }
        }

        public void UpdateCompany(Company company)
        {
            try
            {
                // if exist company will be updated.
                var checkIfExists = Db.Get<Project>(company.id);
                lock (DbContext.locker)
                {
                    Db.Update(company);
                }
            }

            catch (Exception e)
            {
                // if not exist company will be inserted.
                lock (DbContext.locker)
                {
                    Db.Insert(company);
                }
            }
        }


    }
}
