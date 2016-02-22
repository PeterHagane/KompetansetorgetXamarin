using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using PCLStorage.Exceptions;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KompetansetorgetXamarin.CRUD
{
    /// <summary>
    /// Handles CRUD operations on local files
    /// </summary>
    class Crud
    {

        /// <summary>
        /// Creates a new file with the given filename under the apps local storage
        /// This method will not overwrite an existing file.
        /// </summary>
        /// <param name="filename">The name of the file, including extension type</param>
        public async void CreateNewFile(string filename)
        {
            try
            {
                IFolder path = FileSystem.Current.LocalStorage;
                IFile file = await path.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }


        /// <summary>
        /// Writes to an existing file.
        /// </summary>
        /// <param name="filename">The name of the file, including extension type</param>
        /// <param name="text">The content of the file</param>
        public async void WriteToFile(string filename, string text)
        {
            try
            {
                // example for json
                //  @"{ Token: 'cvi1LZzRdZ4:APA91bERsfF7kNNMmXV_4qhcwEg7_D5tQCIJhua-QbrGnyIBIsF0K7ovqVcZi9kWRRgheERodLCwbNDwXtNmZWXimZzDbwAPboR3CKcl4OkT6BeHMSSvnpgA9yvgknbqOhEFQjH4eO6Z' }";

                IFolder path = FileSystem.Current.LocalStorage;
                IFile file = path.GetFileAsync(Path.Combine(path.Path, filename)).Result;
                await file.WriteAllTextAsync(text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }   
    }

    class TokenHandler
    {
        private Crud crudster = new Crud();

        public async void CheckToken(string newToken)
        {
            IFolder path = FileSystem.Current.LocalStorage;
            string filename = "token.json";
            try
            {
                IFile file = path.GetFileAsync(filename).Result;
                string json = await file.ReadAllTextAsync();
                var jsonToken = JObject.Parse(json);
                string token = (jsonToken["Token"]).ToString();
                if (!token.Equals(newToken))
                {
                    crudster.WriteToFile(filename, BuildTokenJson(newToken));
                    AlertServerOfNewToken(newToken);
                }

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("File containing token doesn't exist");
                crudster.CreateNewFile(filename);
                crudster.WriteToFile(filename, BuildTokenJson(newToken));
            }
        }

        public async Task<string> GetToken()
        {
            IFolder path = FileSystem.Current.LocalStorage;
            string filename = "token.json";
            try
            {
                IFile file = path.GetFileAsync(filename).Result;
                string json = await file.ReadAllTextAsync();
                System.Diagnostics.Debug.WriteLine(json);
                // json = { Token: cvi1LZzRdZ4:APA91bERsfF7kNNMm }
                var jsonToken = JObject.Parse(json);
                string token = (jsonToken["Token"]).ToString();
                // token = error
                System.Diagnostics.Debug.WriteLine("Tokenstring : " + token);
                return token;
            }
            catch (Exception e)
            { 
                return e.Message;
            }
        }

        public string BuildTokenJson(string token)
        {
            return @"{ Token: '" + token + "' }";
        }

        public void AlertServerOfNewToken(string newToken)
        {
            // TODO Contact correct REST API REFERENCE
        }

    }
}
