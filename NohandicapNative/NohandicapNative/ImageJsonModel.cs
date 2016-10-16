using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public  class ImageJsonModel
    {
        public ImageJsonModel()
        {
            Thumbs = new List<string>();
            Images = new List<string>();
        }
       
       [JsonProperty(PropertyName = "thumbs"), TextBlob("ThumbsBlobbed")]      
        public List<string> Thumbs { get; set; }
       [JsonProperty(PropertyName = "images"), TextBlob("ImagesBlobbed")]
        public List<string> Images { get; set; }

        public string ThumbsBlobbed { get; set; } 
        public string ImagesBlobbed { get; set; }

    }
}
