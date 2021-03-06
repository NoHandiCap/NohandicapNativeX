﻿using Newtonsoft.Json;
using NohandicapNative;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public  class CategoryModel
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        public int LangId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "gruppe")]
        public int Group { get; set; }
        [JsonProperty(PropertyName = "reihe")]
        public int Sort { get; set; }
       
        public string Color { get; set; }
        public string Icon { get; set; }
        public string Marker { get; set; }

        public bool IsSelected { get; set; }
       
    }
}
