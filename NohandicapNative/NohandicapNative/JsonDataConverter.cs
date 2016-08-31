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
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                var thumbs = token["thumbs"].ToObject<List<string>>();
                var images = token["images"].ToObject<List<string>>();
                ImageJsonModel model = new ImageJsonModel();
                List<ImageModel> thumbsList = new List<ImageModel>();
                List<ImageModel> imgList = new List<ImageModel>();

#if __ANDROID__
                var dbCon = Utils.GetDatabaseConnection();

                foreach (var item in thumbs)
                {
                    try
                    {
                        string filename="none";
                        Uri uri = new Uri(item);                        
                        filename = System.IO.Path.GetFileName(uri.LocalPath);
                                            
               
                        ImageModel thumb = new ImageModel();
                        Utils.SaveImageBitmapFromUrl(item, filename);
                        thumb.LocalImage = filename;
                        thumb.LinkImage = item;
                       dbCon.InsertUpdateProduct(thumb);

                        thumbsList.Add(thumb);
                    }catch(Exception e) {
                        var s = e.Message;
                    }
                }           

                foreach (var item in thumbs)
                {
                    try {
                        string filename = "none";
                        Uri uri = new Uri(item);                       
                            filename = System.IO.Path.GetFileName(uri.LocalPath);
                                          
                    ImageModel img = new ImageModel();                 
                    Utils.SaveImageBitmapFromUrl(item, filename);
                    img.LocalImage = filename;
                    img.LinkImage = item;
                    dbCon.InsertUpdateProduct(img);
                        imgList.Add(img);
                    }
                    catch (Exception e) {
                        var s = e.Message;
                    }
                }
               

           //     dbCon.InsertUpdateProduct(model);
            ///    thumbsList.ForEach(x => x.ImageJsonID = model.ID);
              //  imgList.ForEach(x => x.ImageJsonID = model.ID);
               // dbCon.InsertUpdateProductList(thumbsList);
               // dbCon.InsertUpdateProductList(imgList);
              //  var v = dbCon.GetDataList<ImageJsonModel>();
                return model;
#else
            new NotImplementedException("Mehtod not Implement");
#endif

            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
