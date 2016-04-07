using System;
using KompetansetorgetXamarin.DAL;
using Xamarin.Forms;
using System.IO;
using KompetansetorgetXamarin.Droid;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]
namespace KompetansetorgetXamarin.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android() { }
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "kompedb.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);

            ISQLitePlatform platform = new SQLitePlatformAndroid();
            // Create the connection
            var conn = new SQLiteConnection(platform, path);
            // Return the database connection
            return conn;
        }
    }


}