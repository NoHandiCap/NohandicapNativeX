using Newtonsoft.Json;
using SQLite;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public  class PropertiesModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [JsonProperty("NAME")]
        public string Title { get; set; }
        [JsonProperty("BEMERKUNG")]
        public string Description { get; set; }
        [JsonProperty("ADRESSE")]
        public string Adresse { get; set; }
    }
}
