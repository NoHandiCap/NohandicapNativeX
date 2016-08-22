using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public class MarkerModel
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        [JsonProperty("id"), Ignore]
        public string Id { get; set; }
        public string Title { get { return Properties.Title; }  }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Description { get { return Properties.Description; } }
    
        [ForeignKey(typeof(PropertiesModel))]
        public int PropertiesID { get; set; }
        [JsonProperty("properties"),OneToOne]
        public PropertiesModel Properties { get; set; }
     
        [JsonProperty("geometry"), Ignore]
        public Coordinates Coordinates { get; set;}
        public string Lat { get; set; }
        public string Lang { get; set; }

    }
}
