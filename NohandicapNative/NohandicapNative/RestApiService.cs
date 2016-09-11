﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NohandicapNative
{
    public class RestApiService
    {
        public static async Task<string> GetStringContent(string dataUri, string rootName = "result")
        {
            var url = dataUri;
            var httpClient = new HttpClient() ;
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(10000));
            try
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(url, cts.Token))
                {
                    string content = null;
                    if (response != null && response.Content != null)
                        content = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == HttpStatusCode.OK ||
                        response.StatusCode == HttpStatusCode.Created)
                    {
                        if (content.Length > 0)
                        {
                       
                            return content;
                        }


                    }
                    return null;
                }
            }
            catch (WebException ex)
            {
                return null;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == cts.Token)
                {
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }


        public static async Task<T> GetDataFromUrl<T>(string dataUri, string rootName = "result", bool readBack = true)
        {
            var content = await GetStringContent(dataUri, rootName);
            if (content != null)
            {
                var v = Deserializedata<T>(content);
                return v;
            }
            else
            {
                return default(T);
            }
        }




        public static T Deserializedata<T>(string content, string rootName = "result")
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
            }
            catch (Exception e)
            {

                return default(T);
            }

        }
        public static async Task<UserModel> Login(string email, string password)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    reqparm.Add("user", email);
                    reqparm.Add("pwd", password);
                    byte[] responsebytes = client.UploadValues(NohandicapLibrary.LINK_LOGIN, "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    var user = Deserializedata<UserModel>(responsebody, rootName: "user");
                    var fav = Deserializedata<List<int>>(responsebody, rootName: "favorites");
                    if (user != null && fav != null)
                        user.Fravorites = fav;
                    return user;
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public static async Task<bool> SignUp(UserModel user)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var reqparm = new System.Collections.Specialized.NameValueCollection();
                    reqparm.Add("vname", user.Vname);
                    reqparm.Add("nname", user.Nname);
                    reqparm.Add("email", user.Email);
                    reqparm.Add("mobil", user.Phone);
                    reqparm.Add("uname", user.Login);
                    reqparm.Add("pwd", user.Password);
                    reqparm.Add("pwd2", user.Password);
                    reqparm.Add("geschlecht", user.Sex);
                    byte[] responsebytes = client.UploadValues(NohandicapLibrary.LINK_SIGN_UP, "POST", reqparm);
                    string responsebody = Encoding.UTF8.GetString(responsebytes);
                    var root = JObject.Parse(responsebody).SelectToken("status").ToString();
                    var code = JsonConvert.DeserializeObject<int>(root);
                    if (code == 1) return true;
                    else
                        return false;

                }
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static async Task<Dictionary<string,string>> CheckUpdate(SqliteService dbCon,string langID, Dictionary<string, string> lastUpdate)
        {
            bool prod = false;
            bool cat = false;
            bool lang = false;
            var result = await GetStringContent(NohandicapLibrary.LINK_GET_UPDATE);
            if (result == null)
            {
                return null;
            }
            var token = JObject.Parse(result).SelectToken("result");
            var productTable= token.SelectToken("prod").ToString();
            var categoryTable = token.SelectToken("cat").ToString();
            var langTable = token.SelectToken("lang").ToString();

            if (productTable !=lastUpdate[NohandicapLibrary.PRODUCT_TABLE])
            {
             prod = await dbCon.SynchronizeDataBase(langID, NohandicapLibrary.PRODUCT_TABLE);
            }
            
            if (categoryTable != lastUpdate[NohandicapLibrary.CATEGORY_TABLE])
            {
            cat=  await  dbCon.SynchronizeDataBase(langID, NohandicapLibrary.CATEGORY_TABLE);
            }
            if (langTable != lastUpdate[NohandicapLibrary.LANGUAGE_TABLE])
            {
             lang=  await dbCon.SynchronizeDataBase(langID, NohandicapLibrary.LANGUAGE_TABLE);
            }
            if (lang || prod || cat)
            {
                Dictionary<string, string> updateList = new Dictionary<string, string>();
                updateList.Add(NohandicapLibrary.PRODUCT_TABLE, productTable);
                updateList.Add(NohandicapLibrary.CATEGORY_TABLE, categoryTable);
                updateList.Add(NohandicapLibrary.LANGUAGE_TABLE, langTable);
                return updateList;
            }
            else
            {
                return null;
            }
   
        }

    }
}
