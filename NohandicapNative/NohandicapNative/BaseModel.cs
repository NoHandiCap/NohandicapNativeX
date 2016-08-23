using SQLite;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
  public  class BaseModel
    {
        [PrimaryKey,AutoIncrement]
        public virtual int ID { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public virtual List<LanguagesDbModel> Languages { get; set; }
    }
}
