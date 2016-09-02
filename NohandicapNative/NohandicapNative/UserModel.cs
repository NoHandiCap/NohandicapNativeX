using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
   public class UserModel
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
      
        [JsonProperty(PropertyName = "name")]
        public string Login { get; set; }
        public List<ProductModel> Fravorites { get; set; }
    }
}
