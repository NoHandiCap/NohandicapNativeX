using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
public class LanguagesDbModel
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }

        [ForeignKey(typeof(MarkerModel))]
        public int MarkerID { get; set; }

        [ForeignKey(typeof(CategoryModel))]
        public int CategoryID { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public LanguageModel Language { get; set; }
        [ForeignKey(typeof(LanguageModel))]
        public int LanguageID { get; set; }


        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public TranslateModel Translate { get; set; }
        [ForeignKey(typeof(TranslateModel))]
        public int TranslateID { get; set; }
    }
}
