using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
public class LanguagesDbModel
    {
        public int ID { get; set; }
       
        [ForeignKey(typeof(BaseModel))]
        public int BaseID { get; set; }
        public string LanguageName { get; set; }
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<LanguagTranslateModel> Translates { get; set; }
    }
}
