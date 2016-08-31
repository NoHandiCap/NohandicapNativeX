
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
#if __ANDROID__
using NohandicapNative.Droid.Services;
#endif
namespace NohandicapNative
{
    [JsonObject]
    public class ImageModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }      
        public string LinkImage { get; set; }
        public string LocalImage { get; set; }

        [ForeignKey(typeof(ImageJsonModel))]
        public int ImageJsonID { get; set; }
        
    }
}
