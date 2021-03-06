﻿using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace NohandicapNative
{
    [JsonObject]
    public  class ProductMarkerModel
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "prodimgpin")]
        public string ProdimgPin { get; set; }
        [JsonProperty(PropertyName = "adress")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "ort")]
        public string Ort { get; set; }
        [JsonProperty(PropertyName = "plz")]
        public string Plz { get; set; }
        [JsonProperty(PropertyName = "land")]
        public string Country { get; set; }
        [JsonProperty(PropertyName = "prodimg")]
        public string ProdImg { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public string Lat { get; set; }
        [JsonProperty(PropertyName = "lng")]
        public string Lng { get; set; }
        [JsonProperty(PropertyName = "cat"), TextBlob("categoriesBlobbed")]
        public List<int> Categories { get; set; }
        [JsonProperty(PropertyName = "dist")]
        public string Distance { get; set; }

        public string categoriesBlobbed { get; set; }

    }
}
