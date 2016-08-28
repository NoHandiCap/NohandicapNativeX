using Newtonsoft.Json;
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
        public int ID { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string LanguageName { get; set; }

        [JsonProperty(PropertyName = "short")]
        public string ShortName { get; set; }

        [ForeignKey(typeof(ProductModel))]
        public int ProductID { get; set; }
        [ForeignKey(typeof(CategoryModel))]
        public int CategoryID { get; set; }
    }
}
