using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.Utility;

namespace KompetansetorgetXamarin.Tests
{
    [TestFixture]
    public class TestClass
    {
       // var platformServicesProperty = typeof(Device).GetProperty("PlatformServices", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        //platformServicesProperty.SetValue(null, new PlatformServicesMock());

        private string path = "com.kompetansetorget.xamarin.apk";
        private AndroidApp app;

        [SetUp]
        public void Setup()
        {
            app = ConfigureApp.Android.ApkFile(path).StartApp();
        }

        [Test]
        public void TestMethod()
        {
            app.Repl();
            
        }

        [Test]
        public void TestMd5Hasher()
        {
            string toBeHashed =
                "hashthisstring231pleasehashthisstringpleasehas4241hthisstringplea3123sehashthisstringplease";
            string md5FromExternalSource = "daa5084fe8ea9cbb2f9ff1cfac9f479e";
            string hashed = Hasher.CalculateMd5Hash(toBeHashed);
            Assert.AreEqual(hashed, md5FromExternalSource.ToUpper());
        }

        [Test]
        public void TestPresentableDateTime()
        {
            DateTime today = new DateTime(2016, 04, 12);
            long n = long.Parse(today.ToString("yyyyMMddHHmmss"));
            string date = DateTimeHandler.MakeDateString(n);
            string dateTime = DateTimeHandler.MakeDateTimeString(n);
            Assert.AreEqual(date, "12.04.16");
        }
    }
	/*
    public class PlatformServicesMock : IPlatformServices
    {
        void IPlatformServices.BeginInvokeOnMainThread(Action action)
        {
            throw new NotImplementedException();
        }
        ITimer IPlatformServices.CreateTimer(Action<object> callback)
        {
            throw new NotImplementedException();
        }
        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, int dueTime, int period)
        {
            throw new NotImplementedException();
        }
        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, long dueTime, long period)
        {
            throw new NotImplementedException();
        }
        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
        {
            throw new NotImplementedException();
        }
        ITimer IPlatformServices.CreateTimer(Action<object> callback, object state, uint dueTime, uint period)
        {
            throw new NotImplementedException();
        }
        Assembly[] IPlatformServices.GetAssemblies()
        {
            return new Assembly[0];
        }
        Task<Stream> IPlatformServices.GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        IIsolatedStorageFile IPlatformServices.GetUserStoreForApplication()
        {
            throw new NotImplementedException();
        }
        void IPlatformServices.OpenUriAction(Uri uri)
        {
            throw new NotImplementedException();
        }
        void IPlatformServices.StartTimer(TimeSpan interval, Func<bool> callback)
        {
            throw new NotImplementedException();
        }

        bool IPlatformServices.IsInvokeRequired
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }*/
}

