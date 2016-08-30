using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Text;
#if __ANDROID__
using NohandicapNative.Droid.Services;
using NohandicapNative.Droid.Services;
#endif
namespace NohandicapNative
{
   public class ImageDataConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ImageJsonModel));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
//                var thumbs = token["thumbs"].ToObject<List<string>>();
//                var images = token["images"].ToObject<List<string>>();
//                ImageJsonModel model = new ImageJsonModel();
//                foreach (var item in thumbs)
//                {
//#if __ANDROID__
//                    ImageModel thumb = new ImageModel();

//                        var imgName = "img" + thumb.ID;
//                        Utils.SaveImageBitmapFromUrl(item, imgName);
               
                    
             
//#else
//            new NotImplementedException("Mehtod not Implement");
//#endif      
//                }
              
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
