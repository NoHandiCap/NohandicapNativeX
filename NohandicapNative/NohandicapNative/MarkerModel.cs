using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public class MarkerModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        [JsonProperty("properties")]
        public PropertiesModel Properites { get; set; }
     
        [JsonProperty("geometry")]
        public Coordinates Coordinates { get; set;}
        public string Lat { get; set; }
        public string Lang { get; set; }

    }
}
