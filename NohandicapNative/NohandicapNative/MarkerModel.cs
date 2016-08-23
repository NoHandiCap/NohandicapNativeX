using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public class MarkerModel:BaseModel
    {
  
        public string Title { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
            public string Lat { get; set; }
        public string Lang { get; set; }

        [OneToMany]
        public List<CategoryModel> Categories { get; set; }
    }
}
