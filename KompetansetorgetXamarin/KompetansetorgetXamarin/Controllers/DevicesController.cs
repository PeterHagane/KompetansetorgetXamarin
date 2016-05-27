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
using KompetansetorgetXamarin.Utility;

namespace KompetansetorgetXamarin.Controllers
{
    public class DevicesController : BaseController
    {
        private string deviceAdress;
        private string studentAdress;
        public DevicesController()
        {
            deviceAdress = Adress + "/devices";
            studentAdress = Adress + "/students";
        }


        /// <summary>
        /// Inserts or update the device with the given fields.
        /// </summary>
        /// <param name="gcmToken"></param>
        /// <param name="deviceId"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public async Task InsertOrUpdateDevice(string gcmToken, string deviceId, string deviceType)
        {
            DbDevice db = new DbDevice();
            DbStudent dbStudent = new DbStudent();
            Student student = dbStudent.GetStudent();
            Device device = db.GetDevice();
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
                db.InsertDevice(device);
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
                db.UpdateDevice(device);
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
                    db.UpdateDevice(device);
                }
                UpdateServersDb();
            }
        }

        public async Task UpdateServersDb()
        {
            DbDevice db = new DbDevice();
            DbStudent dbStudent = new DbStudent();
            Student student = dbStudent.GetStudent();
            Device device = db.GetDevice();
            string accessToken = dbStudent.GetStudentAccessToken();
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

            string encodedUsername = Hasher.Base64Encode(student.username);
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
                    db.UpdateDevice(device);
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
    }
}
