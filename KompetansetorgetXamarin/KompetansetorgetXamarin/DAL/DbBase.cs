using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;

namespace KompetansetorgetXamarin.DAL
{
    public abstract class DbBase
    {
        protected DbContext DbContext = DbContext.GetDbContext;
        protected SQLiteConnection Db;

        public DbBase()
        {
            Db = DbContext.Db;
        }
    }
}
