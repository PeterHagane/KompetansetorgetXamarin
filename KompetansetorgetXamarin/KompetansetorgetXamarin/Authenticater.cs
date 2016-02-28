using System;
using System.Collections.Generic;
using System.Linq;
using UAuth;
//using Xamarin.Forms;

namespace KompetansetorgetXamarin
{
    public class Authenticater
    {
        private App app;
        public Authenticater(App app)
        {
            this.app = app;
            IAuth auth = Auth.auth;
            Authenticate(auth.auth2, AccountStore.Create().FindAccountsForService(AuthProvider.Name));
        }

        void Authenticate(IOAuth2Authenticator auth, List<Account> accounts)
        {
            if (accounts.Count == 0)
            {
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
                    // Uten en fungerende server blir vi stuck her i mellomtiden.
                    auth.OAuth2Authenticator(AuthProvider.ClientId, AuthProvider.Scope,
                        new Uri(AuthProvider.AuthorizeUrl), new Uri(AuthProvider.RedirectUrl));
                    System.Diagnostics.Debug.WriteLine("IT RUNS IT RUNS");
                    System.Diagnostics.Debug.WriteLine("IT RUNS IT RUNS");
                    System.Diagnostics.Debug.WriteLine("IT RUNS IT RUNS");
                    System.Diagnostics.Debug.WriteLine("IT RUNS IT RUNS");

                    System.Diagnostics.Debug.WriteLine("IT RUNS IT RUNS");

                    // Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack.First());
                    //  Navigation.PopToRootAsync();
                    // Ingenting under her funker som en mellomtids løsning:
                    //Navigation.PopModalAsync();
                    //Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack.First());
                    //Navigation.PushModalAsync(new MainPage());

                    //Navigation.PopToRootAsync();


                }
                catch (Exception ex)
                {
                        System.Diagnostics.Debug.WriteLine("Authenticate: Exception:");
                    for (Exception inner = ex.InnerException; inner != null; inner = inner.InnerException)
                    {
                        System.Diagnostics.Debug.WriteLine("Message:" + inner.Message);
                    }
                    // Denne vil kaste en ny exception
                    foreach (KeyValuePair<string, string> p in accounts[0].Properties)
                            System.Diagnostics.Debug.WriteLine("Key:" + p.Key + " Value:" + p.Value);
                }
            }
            else
                PerformAuth2TestRequests(accounts[0]);
            // TODO: implement error handling. If error is caused by expired token, renew token.
        }

        async void PerformAuth2TestRequests(Account account)
        {
            try
            {
                //await Navigation.PushModalAsync(new ViktorTestView());
                app.SuccessfulLoginAction();
                /*
                foreach (KeyValuePair<string, string> p in account.Properties)
                {
                    System.Diagnostics.Debug.WriteLine("Property: Key:" + p.Key + " Value:" + p.Value);
                }
                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests: Url:" + AuthProvider.ApiRequests);
                System.Diagnostics.Debug.WriteLine("Request Url:" + AuthProvider.ApiRequests);
                OAuth2Request request = new OAuth2Request("GET", new Uri(AuthProvider.ApiRequests), null, account);
                IResponse response = await request.GetResponseAsync();
                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests: StatusCode:" + response.StatusCode +
                                                   " ResponseUri:" + response.ResponseUri);
                System.Diagnostics.Debug.WriteLine("PerformAuth2TestRequests: Headers:");
                foreach (KeyValuePair<string, string> h in response.Headers)
                {
                    System.Diagnostics.Debug.WriteLine("Header: Key:" + h.Key + " Value:" + h.Value);
                }
                System.Diagnostics.Debug.WriteLine("Response(" + response.StatusCode);
                string r = response.GetResponseText();
                System.Diagnostics.Debug.WriteLine(r);
                */
                //await Navigation.PushModalAsync(new ViktorTestView());
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
