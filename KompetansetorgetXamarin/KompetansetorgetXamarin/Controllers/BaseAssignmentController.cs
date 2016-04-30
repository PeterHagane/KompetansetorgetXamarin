﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;

namespace KompetansetorgetXamarin.Controllers
{
    public abstract class BaseAssignmentController<T>
    {
        protected DbContext dbContext = DbContext.GetDbContext;
        protected SQLiteConnection Db;

        protected string ModelName;

        public BaseAssignmentController()
        {
            Db = dbContext.Db;
        }




        /*
        public T GetProjectByUuid(string uuid)
        {
            try
            {
                lock (DbContext.locker)
                {
                    return Db.GetWithChildren<T>(uuid);
                    //return Db.Get<Project>(uuid);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("ProjectController - GetProjectByUuid(string uuid): End Of Stack Trace");
                return default(T);
            }
        }
        */
    }
}
