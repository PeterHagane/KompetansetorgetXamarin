using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using SQLite.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace KompetansetorgetXamarin.Controllers
{
    public class DevicesController : BaseController
    {
        private string deviceAdress;
        private string studentAdress;
        public DevicesController()
        {
            deviceAdress = Adress + "v1/devices";
            studentAdress = Adress + "v1/students";
        }

        /// <summary>
        /// Inserts the Device
        /// into the database.
        /// </summary>
        /// <param name="student"></param>
        /// <returns>Returns true if the Device was inserted, returns false if a Device with the same 
        ///  primary key already exists in the table.</returns>
        public bool InsertDevice(Device device)
        {
            if (CheckIfDeviceExist(device.id))
            {
                return false;
            }

            lock (DbContext.locker)
            {
                Db.Insert(device);
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
        public bool CheckIfDeviceExist(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    var checkIfExist = Db.Get<Device>(id);
                }
                System.Diagnostics.Debug.WriteLine("DevicesController - CheckIfDeviceExist: Device Already exists");
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DevicesController - CheckIfDeviceExist: Device Already exists");
                System.Diagnostics.Debug.WriteLine("DevicesController - CheckIfDeviceExist: Exception msg: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates a Device, but if it doesnt already exist a new entry will be inserted into the db.
        /// </summary>
        /// <param name="device"></param>
        public void UpdateDevice(Device device)
        {
            if(CheckIfDeviceExist(device.id))
            { 
                lock (DbContext.locker)
                {
                    Db.Update(device);
                }
            }

            else
            {
                lock (DbContext.locker)
                {
                    Db.Insert(device);
                }
            }
        }

        /// <summary>
        /// Gets the Device with the spesific id. 
        /// If no matching Device is found it returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Device GetDevice(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    return Db.Get<Device>(id);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(string id): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(string id): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(string id): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Returns all devices stored in the database. 
        /// If no Devices is found it returns null.
        /// </summary>
        /// <returns></returns>
        public List<Device> GetDevices()
        {
            try
            {
                List<Device> results;
                lock (DbContext.locker)
                {
                    results = Db.Query<Device>("Select * from Device");
                }
                return results;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Returns all devices stored in the database. 
        /// If no Devices is found it returns null.
        /// </summary>
        /// <returns></returns>
        public Device GetDevice()
        {
            try
            {
                List<Device> results;
                lock (DbContext.locker)
                {
                    results = Db.Query<Device>("Select * from Device");
                    try
                    {
                        return results[0];
                    }
                    catch
                    {
                        return null;
                    }
                }
                
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DevicesController - GetDevice(): End Of Stack Trace");
                return null;
            }
        }

        /// <summary>
        /// Deletes the Device with the spesific id. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void DeleteDevice(string id)
        {
            try
            {
                lock (DbContext.locker)
                {
                    Db.Delete<Device>(id);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteDevice(string id): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteDevice(string id): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteDevice(string id): End Of Stack Trace");
            }
        }

        /// <summary>
        /// Deletes the Device from the database
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public void DeleteDevice(Device device)
        {
            try
            {
                lock (DbContext.locker)
                {
                    Db.Delete<Device>(device.id);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteDevice(Device device): Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteDevice(Device device): Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteDevice(Device device): End Of Stack Trace");
            }
        }

        /// <summary>
        /// Sets the foreign key in Device: device.username
        /// </summary>
        /// <returns></returns>
        public List<Device> FixStudentForeignKey(string username)
        {
            List<Device> list = GetDevices();
            foreach (var device in list)
            {
                device.username = username;
                lock (DbContext.locker)
                {
                    Db.Update(device);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gcmToken"></param>
        /// <param name="deviceId"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public async Task InsertOrUpdateDevice(string gcmToken, string deviceId, string deviceType)
        {
            StudentsController sc = new StudentsController();
            Student student = sc.GetStudent();
            Device device = GetDevice();
            if (device == null)
            {
                device = new Device();
                device.id = deviceId;
                device.token = gcmToken;
                device.tokenSent = false;
                device.deviceType = deviceType;

                if (student != null)
                {
                    device.username = student.username;
                }
                InsertDevice(device);
                if (student != null)
                {
                    UpdateServersDb();
                }
                return;
            }

            if (device.token != gcmToken)
            {
                device.token = gcmToken;
                device.tokenSent = false;
                if (student != null)
                {
                    device.username = student.username;
                }
                UpdateDevice(device);
                if (student != null)
                {
                    UpdateServersDb();
                }
                return;
            }

            if (!device.tokenSent && student != null)
            {
                if (device.username != student.username)
                {
                    device.username = student.username;
                    UpdateDevice(device);
                }
                UpdateServersDb();
            }
        }

        public async Task UpdateServersDb()
        {
            StudentsController sc = new StudentsController();
            Student student = sc.GetStudent();
            Device device = GetDevice();
            string accessToken = sc.GetStudentAccessToken();
            if (student == null || accessToken == null || device == null || student.username != device.username)
            {
                Authenticater.Authorized = false;
                return;
            }
            if (device.tokenSent)
            {
                return;
            }
            System.Diagnostics.Debug.WriteLine("DevicesController - UpdateServersDb - bearer: " + accessToken);
            string serializedJson = "{\"Device\":[{\"id\":\"" + device.id + "\"," + "\"token\":\"" + device.token + "\"," +
                                    "\"deviceType\":\"" + device.deviceType + "\"}]}";
            //  {"Device":[{"id":"HT451WM08832","token":"longGCMToken","deviceType":"android"}]}
            System.Diagnostics.Debug.WriteLine("DevicesController - UpdateServersDb serializedJson: " + serializedJson);

            string encodedUsername = Base64Encode(student.username);
                                                //"api/v1/students/{id}"
            string updateAdress = studentAdress + "/" + encodedUsername;
            System.Diagnostics.Debug.WriteLine("DevicesController - UpdateServersDb - adress: " + updateAdress);
            Uri url = new Uri(updateAdress);
            System.Diagnostics.Debug.WriteLine("DevicesController - UpdateServersDb - url.ToString: " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            var content = new StringContent(serializedJson, Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync(url, content);
                System.Diagnostics.Debug.WriteLine("UpdateServersDb response " + response.StatusCode.ToString());

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    device.tokenSent = true;
                    UpdateDevice(device);
                    return;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    System.Diagnostics.Debug.WriteLine("DevicesController - UpdateServersDb failed due to lack of Authorization");
                    Authenticater.Authorized = false;
                }
                // response.StatusCode is either unauthorized or another failed status.
                return;

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: await client.PostAsJsonAsync(url, jsonString) Failed");
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("StudentsController - PostStudentsStudyGroupToServer: End Of Stack Trace");
                return;
            }
        }

        /// <summary>
        /// Deletes all devices from the local database.
        /// </summary>
        public void DeleteAllDevices()
        {
            lock (DbContext.locker)
            {
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteAllDevices: Before delete.");
                Db.Execute("delete from " + "Device");
                System.Diagnostics.Debug.WriteLine("DevicesController - DeleteAllDevices: After delete.");
            }
        }

        /// <summary>
        /// Base64Encodes a string, should be used on all query strings that 
        /// can contain special characters not suitable for in a url adress.  
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
