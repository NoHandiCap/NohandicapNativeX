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
        public string Id { get; set; }
        [JsonProperty(PropertyName = "fullname")]
        public string Vname { get; set; }  
        public string Nname { get; set; }       
        public string Phone { get; set; } 
        [JsonProperty(PropertyName = "username")]
        public string Login { get; set; }  
        public string Sex { get; set; }             
        public string Password { get; set; }
        public string FbId { get; set; }
        [JsonProperty(PropertyName = "mail")]
        public string Email { get; set; }
        [TextBlob("FavoritesBlobbed")]
        public List<int> Favorites { get; set; }
       public string FavoritesBlobbed { get; set; }//for database
    }
}
