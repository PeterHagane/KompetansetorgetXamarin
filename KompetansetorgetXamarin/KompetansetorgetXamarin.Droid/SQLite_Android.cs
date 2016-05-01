using System;
using KompetansetorgetXamarin.DAL;
using Xamarin.Forms;
using System.IO;
using Android.Util;
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
            Log.Debug("SQLite_Android", "TEST:  SQLite_Android initiated");
            var sqliteFilename = "kompedb.db3";
            string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);

            Log.Debug("SQLite_Android", "TEST:  Before new SQLitePlatformAndroid();");
            // last print
            // Error no implementation for interface method SQLite.Net.Interop.ISQLiteApi::SourceID() 
            // in class SQLite.Net.Platform.XamarinAndroid.SQLiteApiAndroid

            ISQLitePlatform platform = new SQLitePlatformAndroid();
            Log.Debug("SQLite_Android", "TEST:  Before new SQLiteConnection(platform, path);");

            // Create the connection
            var conn = new SQLiteConnection(platform, path);
            // Return the database connection
            Log.Debug("SQLite_Android", "TEST:  Before return");

            return conn;
        }
    }

    /*
            public string SourceID()
        {
			return Marshal.PtrToStringAnsi(SQLiteApiAndroidInternal.sqlite3_sourceid());            
        } 
    */

}