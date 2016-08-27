using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
  
    public class ProductModel
    {
        [PrimaryKey, AutoIncrement]
        public virtual int ID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
            public string Lat { get; set; }
        public string Lang { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<CategoryModel> Categories { get; set; }
      
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<LanguagesDbModel> Languages { get; set; }
    }
}
