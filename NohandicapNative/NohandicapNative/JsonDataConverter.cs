using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
            if (objectType == typeof(ImageJsonModel))
            {
                JToken token = JToken.Load(reader);
                ImageJsonModel model = new ImageJsonModel();
#if __ANDROID__
            if (token.Type == JTokenType.Object)
            {
                var thumbs = token["thumbs"].ToObject<List<string>>();
                var images = token["images"].ToObject<List<string>>();
             
                model.Thumbs = new List<string>();
                model.Images = new List<string>();

                var conn = Utils.GetDatabaseConnection();
                if (thumbs != null)
                {
                    foreach (var item in thumbs)
                    {                     
                        model.Thumbs.Add(item);

                    }
                }
                if (images != null)
                {

                    foreach (var item in images)
                    {                        
                        model.Images.Add(item);
                    }
                }


                


            }
#endif
                return model;
            }
            else
            {
                JToken token = JToken.Load(reader);             
                var urlImage= token.ToString();
                if (!string.IsNullOrEmpty(urlImage)) {                   
                    return urlImage;
                }
                else
                {
                    return null;
                }
               
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
