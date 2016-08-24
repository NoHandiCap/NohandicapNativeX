using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
   public class LanguageModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string LanguageName { get; set; }
       
    }
}
