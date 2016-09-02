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
       public static async Task<T> GetData<T>(string dataUri, string accessToken = null, string rootName = "result")
        {
            var url = dataUri;
            try
            {
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
                            if (content.Length > 0)
                            {
                                if(rootName==null)
                                return JsonConvert.DeserializeObject<T>(content);
                                else
                                {
                                    var root = JObject.Parse(content).SelectToken(rootName).ToString();
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
                                    var s = root.Replace("\n", "");
                                    var v= JsonConvert.DeserializeObject<T>(root.Replace("\n",""), settings);
                                    return v;

                                }
                            }
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                          //  throw new Exception("Internal server error received (" + url + "). " + content);
                        }
                        else
                        {
                           // throw new Exception("Bad or invalid request received (" + url + "). " + content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log.Error("Could not fetch data via GetData (" + url + ").", ex.ToString());
                return default(T);
            }
            return default(T);
        }
        public static async Task<string> Login(string email,string password)
        {
            using (WebClient client = new WebClient())
            {
                var reqparm = new System.Collections.Specialized.NameValueCollection();
                reqparm.Add("user", email);
                reqparm.Add("pwd", password);
                byte[] responsebytes = client.UploadValues(NohandiLibrary.LINK_LOGIN, "POST", reqparm);
                string responsebody = Encoding.UTF8.GetString(responsebytes);
                return responsebody;
            }
            return null;
        }
       
    }
}
