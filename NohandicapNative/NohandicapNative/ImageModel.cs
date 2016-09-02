
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
           
        public string LinkImage { get; set; }
        public string LocalImage { get; set; } 


    }
}
