
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
        [JsonProperty(PropertyName = "thumbs")]
        public List<string> Thumbs { get; set; }
        [JsonProperty(PropertyName = "images")]
        public List<string> Images { get; set; }
        [ForeignKey(typeof(ProductModel))]
        public int ProductID { get; set; }

        public string GetImage()
        {

#if __ANDROID__
				  if (LocalImage == null) {
                var imgName = "img" + ID;
                Utils.SaveImageBitmapFromUrl(LinkImage,imgName);
                return imgName;
            }
      
            else return LocalImage;;
#else
            new NotImplementedException("Mehtod not Implement");
#endif      


            return null;
        }
    }
}
