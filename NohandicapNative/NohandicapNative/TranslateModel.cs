
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
   public class TranslateModel
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }      
        public string Translate { get; set; }
      
    }
}
