using NohandicapNative;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
  public  class CategoryModel
    {
        [PrimaryKey, AutoIncrement]
        public virtual int ID { get; set; }
        [ForeignKey(typeof(ProductModel))]
        public int MarkerID { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }       
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public virtual List<LanguagesDbModel> Languages { get; set; }
    }
}
