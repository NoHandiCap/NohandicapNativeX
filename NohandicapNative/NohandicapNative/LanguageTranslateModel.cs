
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
   public class LanguagTranslateModel
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        [ForeignKey(typeof(LanguagesDbModel))]
        public int LanguageID { get; set; }
        public string Translate { get; set; }
       
    }
}
