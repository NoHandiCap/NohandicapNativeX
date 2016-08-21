using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public class Coordinates
    {
        [JsonProperty("coordinates")]
        public object coordinates { get; set; }
    }
    [JsonObject]
  public  class GeometryModel
    {
       
    }
}
