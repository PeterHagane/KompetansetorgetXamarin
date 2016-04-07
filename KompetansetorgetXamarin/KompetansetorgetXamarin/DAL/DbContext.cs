using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using PCLStorage;
using SQLite;
using SQLite.Net;
using Xamarin.Forms;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;
//using SQLite.Net.Platform;
//using SQLite.Net.Platform.XamarinAndroid;



namespace KompetansetorgetXamarin.DAL
{
    public class DbContext
    {
        static object locker = new object();
        private SQLiteConnection database;

        private string dbPath;
        public DbContext()
        {
            /*
                    /// Path.Combine (
                    /// Environment.GetFolderPath (Environment.SpecialFolder.Personal),
                    /// "database.db3");
                    /// Gives the same adress
            string path = FileSystem.Current.LocalStorage.Path;
            string dbName = "kompedb.db3";
            dbPath = path + "\\" + dbName;
            */
        }

        private void InitDb()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<Student>();
            //ISQLitePlatform platform = new SQLitePlatformAndroid();
            //ISQLitePlatform platform = new SQLitePlatformAndroid();


            //var db = new SQLiteConnection(, dbPath);
            //db.CreateTable<Student>();
        }
    }

    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
