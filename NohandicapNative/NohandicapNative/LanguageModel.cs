﻿using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
   [JsonObject]
   public class LanguageModel
    {
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string LanguageName { get; set; }

        [JsonProperty(PropertyName = "short")]
        public string ShortName { get; set; }
    }
}
