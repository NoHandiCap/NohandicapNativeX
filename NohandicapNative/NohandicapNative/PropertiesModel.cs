using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public  class PropertiesModel
    {
        [JsonProperty("NAME")]
        public string Title { get; set; }
        [JsonProperty("BEMERKUNG")]
        public string Description { get; set; }
        [JsonProperty("ADRESSE")]
        public string Adresse { get; set; }
    }
}
