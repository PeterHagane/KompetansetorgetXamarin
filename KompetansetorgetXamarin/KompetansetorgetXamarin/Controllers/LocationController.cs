using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KompetansetorgetXamarin.Models;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Utility;
using Newtonsoft.Json;
using PCLCrypto;

namespace KompetansetorgetXamarin.Controllers
{
    public class LocationsController : BaseController
    {
        public LocationsController()
        {
            Adress += "/locations";
        }

        public async Task CompareServerHash()
        {
            DbStudent dbStudent = new DbStudent();
            string accessToken = dbStudent.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress + "/hash");
            System.Diagnostics.Debug.WriteLine("LocationsController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Diagnostics.Debug.WriteLine("CheckLocationsServerHash response " + response.StatusCode.ToString());
                    string json = await response.Content.ReadAsStringAsync();
                    string hash = ExtractServersHash(json);
                    string localHash = CreateLocalHash();
                    if (hash != localHash)
                    {
                        await UpdateLocationsFromServer();
                    }
                    
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: End Of Stack Trace");
            }
        }

        private string ExtractServersHash(string json)
        {
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (dict.ContainsKey("hash"))
            {
                string hash = dict["hash"];
                return hash;
            }
            return "";
        }

        /// <summary>
        /// Gets all Locations from the servers REST Api.
        /// </summary>
        public async Task UpdateLocationsFromServer()
        {
            DbLocation db = new DbLocation();
            System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: initiated");
            DbStudent dbStudent = new DbStudent();
            string accessToken = dbStudent.GetStudentAccessToken();

            if (accessToken == null)
            {
                Authenticater.Authorized = false;
                return;
            }

            Uri url = new Uri(Adress);
            System.Diagnostics.Debug.WriteLine("LocationsController - url " + url.ToString());
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
            try
            {
                var response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    System.Diagnostics.Debug.WriteLine("UpdateLocationsFromServer response " + response.StatusCode.ToString());
                    var newLocations = await response.Content.ReadAsAsync<IEnumerable<Location>>();
                    db.DeleteAllLocations();
                    db.InsertLocations(newLocations);
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Authenticater.Authorized = false;
                }
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: await client.GetAsync(\"url\") Failed");
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: Exception msg: " + e.Message);
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: Stack Trace: \n" + e.StackTrace);
                System.Diagnostics.Debug.WriteLine("LocationsController - UpdateLocationsFromServer: End Of Stack Trace");
            }
        }

        private string CreateLocalHash()
        {
            DbLocation db = new DbLocation();
            List<Location> locations = db.GetAllLocations();
            StringBuilder sb = new StringBuilder();
            foreach (var loc in locations)
            {
                sb.Append(Hasher.Base64Decode(loc.id));
            }
            return Hasher.CalculateMd5Hash(sb.ToString());
        }
    }
}

