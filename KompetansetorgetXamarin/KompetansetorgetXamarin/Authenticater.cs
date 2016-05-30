using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using KompetansetorgetXamarin.Controllers;
using KompetansetorgetXamarin.DAL;
using KompetansetorgetXamarin.Models;
using UAuth;
//using Xamarin.Forms;

namespace KompetansetorgetXamarin
{
    public class Authenticater
    {
        public static bool Authorized = true;

        public Authenticater()
        {
            IAuth auth = Auth.auth;

            Authenticate(auth.auth2, AccountStore.Create().FindAccountsForService(AuthProvider.Name));
        }

        /// <summary>
        /// Handles the logic of the authentication with the OAuth2 Authentication provider
        /// </summary>
        /// <param name="auth">The authenticater</param>
        /// <param name="accounts">List of registered accounts for the choosen Service</param>
        void Authenticate(IOAuth2Authenticator auth, List<Account> accounts)
        {
            // Checks if there any accounts stored.
           // if (accounts.Count == 0)
           // {
                // EventHandler: When an auth is completed this will be executed.
                auth.Completed += (sender, eventArgs) =>
                {
                    if (eventArgs.IsAuthenticated)
                    {
                        try
                        {
                            AccountStore.Create().Save(eventArgs.Account, AuthProvider.Name);
                            PerformAuth2TestRequests(eventArgs.Account);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Authenticate: Not Authenticated");
                        return;
                    }
                };
                auth.Error += (sender, eventArgs) =>
                {
                    System.Diagnostics.Debug.WriteLine("Authenticate: Error:" + eventArgs.Message);
                    Exception ex = eventArgs.Exception;
                    for (Exception inner = eventArgs.Exception; inner != null; inner = inner.InnerException)
                    {
                        System.Diagnostics.Debug.WriteLine("Message:" + inner.Message);
                    }
                    return;
                };
                try
                {
                    // This is were the user will meet the OAuth2 providers interface
                    // When the auth is complete the EventHandler under the IF-statement will be run.
                    auth.OAuth2Authenticator(AuthProvider.ClientId, AuthProvider.Scope,
                        new Uri(AuthProvider.AuthorizeUrl), new Uri(AuthProvider.RedirectUrl));
                    // Code in this try block will run even if user is in the auth providers interface.

                }
                catch (Exception ex)
                {
                        System.Diagnostics.Debug.WriteLine("Authenticate: Exception:");
                    for (Exception inner = ex.InnerException; inner != null; inner = inner.InnerException)
                    {
                        System.Diagnostics.Debug.WriteLine("Message:" + inner.Message);
                    }
                    // Denne vil kaste en ny exception?
                    foreach (KeyValuePair<string, string> p in accounts[0].Properties)
                            System.Diagnostics.Debug.WriteLine("Key:" + p.Key + " Value:" + p.Value);
                }
            

            // If there is a registered account the manual login will not be needed and this 
            // and this else statement will be executed instead.
            //else
            //    PerformAuth2TestRequests(accounts[0]);
        }


        /// <summary>
        /// This method is called after authentication is successfull.
        /// Implement functionality that is useful as initial server communication. 
        /// </summary>
        /// <param name="account"></param>
        async void PerformAuth2TestRequests(Account account)
        {
            Authorized = true;
            App.SuccessfulLoginAction();

            try
            {

                /*
                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests before looop");


                foreach (KeyValuePair<string, string> p in account.Properties)
                {
                    System.Diagnostics.Debug.WriteLine("Property: Key:" + p.Key + " Value:" + p.Value);
                }
                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests before looop");

                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests: Url:" + AuthProvider.ApiRequests);
                System.Diagnostics.Debug.WriteLine("Request Url:" + AuthProvider.ApiRequests);
                */
                Uri requestLocalToken = new Uri(AuthProvider.ApiRequests + account.Properties["access_token"]);
                System.Diagnostics.Debug.WriteLine("Requesting local token");
                System.Diagnostics.Debug.WriteLine("Using access_token: " + account.Properties["access_token"]);
                OAuth2Request request1 = new OAuth2Request("GET", requestLocalToken, null, account);
                //OAuth2Request request1 = new OAuth2Request("GET", requestLocalToken, null, null);

                IResponse response1 = await request1.GetResponseAsync();
                System.Diagnostics.Debug.WriteLine("After Response");


                Dictionary<string, string> responseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response1.GetResponseText());


                string localToken = "";
                string username = "";

                if (response1.StatusCode == 200)
                {
                    System.Diagnostics.Debug.WriteLine("Response code from backend: 200");
                    localToken = responseDict["access_token"];
                    username = responseDict["userName"];
                    System.Diagnostics.Debug.WriteLine("username: " + username);
                    System.Diagnostics.Debug.WriteLine("localToken: " + localToken);

                    StudentsController sc = new StudentsController();
                    DbStudent dbStudent = new DbStudent();
                    if (dbStudent.CheckIfStudentExist(username))
                    {
                        System.Diagnostics.Debug.WriteLine("Student did exist");
                        Student student = dbStudent.GetStudent(username);
                        student.accessToken = localToken;
                        dbStudent.UpdateStudent(student);
                        DevicesController dc = new DevicesController();
                        dc.UpdateServersDb();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Student did not exist");
                        dbStudent.DeleteAllStudents();
                        Student student = new Student();
                        student.username = username;
                        student.accessToken = localToken;
                        student.receiveNotifications = true;
                        student.receiveJobNotifications = true;
                        student.receiveProjectNotifications = true;
                        dbStudent.InsertStudent(student);
                        DevicesController dc = new DevicesController();
                        DbDevice dbDevice = new DbDevice();
                        dbDevice.FixStudentForeignKey(username);
                        dc.UpdateServersDb();
                    }
                }
                /*
                string studentEndpoint = "http://kompetansetorgetserver1.azurewebsites.net/api/v1/students";
                Uri testAuthorize = new Uri(studentEndpoint);


                //authorization: bearer b2Dvqzi9Ux_FAjbBYat6PE-LgNGKL_HDBWbnJ3Fb9cwfjaE8NQdqcvC8jwSB5QJUIVRog_gQQPjaRI0DT7ahu7TEpqP28URtPr1LjgaV - liCqgIuTdSHW_NqD3qh - 5shVh - h7TCin7XNHq8GSkGg5qtOlcHeFPSZ4xMwMbw5_1rBfKYJr3w0_D5R9jk0hJPEfJldCTYcawatz7wVfbmz0qKHAkrKxZyaqum6IHJWdczWz5K26RCfZWMwEmK1uLN5

                var client = new HttpClient();
                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests before Setting AuthenticationHeaderValue");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", localToken);
                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests after Setting AuthenticationHeaderValue");
            System.Diagnostics.Debug.WriteLine(client.DefaultRequestHeaders.Authorization.Parameter);


                var response = await client.GetAsync(testAuthorize);


                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests: StatusCode:" + response.StatusCode);
                                                 // + " ResponseUri:" + response.ResponseUri);
                //System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests: Headers:");


                foreach (KeyValuePair<string, string> h in response.Headers)
                {
                    System.Diagnostics.Debug.WriteLine("Header: Key:" + h.Key + " Value:" + h.Value);
                } 
                System.Diagnostics.Debug.WriteLine("Response(" + response.StatusCode);
                string jsonString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(jsonString);
                */
                // TODO Implement relevant GET, PUT or POST Requests
                // Notifies the app that the login was successful and that its safe to shift page.
                Authorized = true;
                App.SuccessfulLoginAction();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: PerformAuth2TestRequests: Message:" + ex.Message);
                foreach (KeyValuePair<string, string> p in account.Properties)
                {
                    System.Diagnostics.Debug.WriteLine("Key:" + p.Key + " Value:" + p.Value);
                }
            }

        }
    }
}   
