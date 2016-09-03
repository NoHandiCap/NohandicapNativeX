using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
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

        [JsonProperty(PropertyName = "username")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "mail")]
        public string E_mail { get; set; }
        [TextBlob("FavoritesBlobbed")]
        public List<int> Fravorites { get; set; }

       public string FavoritesBlobbed { get; set; }
    }
}
