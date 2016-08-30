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
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [JsonProperty(PropertyName = "thumbs"),OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ImageModel> Thumbs { get; set; }
        [JsonProperty(PropertyName = "images"),OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<ImageModel> Images { get; set; }
        public ImageJsonModel()
        {
            Images = new List<ImageModel>();
            Thumbs = new List<ImageModel>();
        }
    }
}
