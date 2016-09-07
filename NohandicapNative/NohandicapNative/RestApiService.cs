using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NohandicapNative
{
 public class RestApiService
    {
        public static async Task<T> GetDataFromUrl<T>(string dataUri, string rootName = "result",bool readBack=true)
        {
            var url = dataUri;
            using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) })
            {
                //// Set OAuth authentication header
                //if (!string.IsNullOrEmpty(accessToken))
                //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {

                    string content = null;
                    if (response != null && response.Content != null)
                        content = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK ||
                        response.StatusCode == HttpStatusCode.Created)
                    {
                        if (content.Length > 0&&readBack)
                        {
                            if (rootName == null)
                            {
                                return Deserializedata<T>(content);
                            }
                            else
                            {

                                var v = Deserializedata<T>(content);
                                return v;

                            }
                        }
                        else
                        {
                            return default(T);
                        }
                    }
                    else
                    {
                        return default(T);
                    }

                }
            }
        }
           
        
        public static T Deserializedata<T>(string content,  string rootName = "result")
        {
            try
            {
                var root = JObject.Parse(content).SelectToken(rootName);

                if (root == null || root.ToString() == "[]") return default(T);
                var settings = new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        if (object.Equals(args.ErrorContext.Member, "test") &&
                            args.ErrorContext.OriginalObject.GetType() == typeof(T))
                        {
                            args.ErrorContext.Handled = false;
                        }
                    }
                };
                var s = root.ToString().Replace("\n", "");
                var v = JsonConvert.DeserializeObject<T>(s, settings);
                return v;
            }catch(Exception e)
            {
                
                return default(T);
            }

        }
        public static async Task<UserModel> Login(string email,string password)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    reqparm.Add("user", email);
                    reqparm.Add("pwd", password);
                    byte[] responsebytes = client.UploadValues(NohandiLibrary.LINK_LOGIN, "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    var user = Deserializedata<UserModel>(responsebody, rootName: "user");
                    var fav = Deserializedata<List<int>>(responsebody, rootName: "favorites");
                    if (user != null && fav != null)
                        user.Fravorites = fav;
                    return user;
                }
            }catch(Exception e)
            {
                return null;
            }
            
        }
       
    }
}
