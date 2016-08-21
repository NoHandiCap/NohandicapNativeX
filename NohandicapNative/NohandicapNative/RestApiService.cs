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
 public   class RestApiService
    {
       public static async Task<T> GetData<T>(string dataUri, string accessToken = null, string rootName = null)
        {
            var url = "http://data.wien.gv.at/daten/geo?service=WFS&request=GetFeature&version=1.1.0&srsName=EPSG:4326&outputFormat=json&typeName=ogdwien:SPAZIERPUNKTOGD,ogdwien:SPAZIERLINIEOGD";
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
                                                args.ErrorContext.OriginalObject.GetType() == typeof(MarkerModel))
                                            {
                                                args.ErrorContext.Handled = false;
                                            }
                                        }
                                    };
                                    return JsonConvert.DeserializeObject<T>(root, settings);

                                }
                            }
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            throw new Exception("Internal server error received (" + url + "). " + content);
                        }
                        else
                        {
                            throw new Exception("Bad or invalid request received (" + url + "). " + content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // Log.Error("Could not fetch data via GetData (" + url + ").", ex.ToString());
                throw ex;
            }
            return default(T);
        }
       
    }
}
