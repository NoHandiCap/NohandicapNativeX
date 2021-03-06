﻿using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;


namespace NohandicapNative
{
    [JsonObject]
    public class ProductDetailModel
    {
        public ProductDetailModel()
        {
            ImageCollection = new ImageJsonModel();
            Categories = new List<int>();
        }
        [PrimaryKey]
        [JsonProperty(PropertyName = "id")]
        public int ID { get; set; }
        public int LangID { get; set; }

        [JsonProperty(PropertyName = "firmenname")]
        public string FirmName { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "openinghours")]
        public string OpenTime { get; set; }
        [JsonProperty(PropertyName = "homepage")]
        public string HomePage { get; set; }
        [JsonProperty(PropertyName = "bookingcom")]
        public string BookingPage { get; set; }
        [JsonProperty(PropertyName = "telefon")]
        public string Telefon { get; set; }
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        [JsonProperty(PropertyName = "maincategoryid")]
        public int MainCategoryID { get; set; }
 
        [JsonProperty(PropertyName = "prodimg")]
        public string MainImageUrl { get; set; }
        
        [JsonProperty(PropertyName = "adress")]
        public string Adress { get; set; }
        [JsonProperty(PropertyName = "ort")]
        public string Ort { get; set; }
        [JsonProperty(PropertyName = "plz")]
        public string Plz { get; set; }
        [JsonProperty(PropertyName = "land")]
        public string Land { get; set; }
        [JsonProperty(PropertyName = "gpslat")]
        public string Lat { get; set; }
        [JsonProperty(PropertyName = "gpslon")]
        public string Long { get; set; }        

        [JsonProperty(PropertyName = "prodimgpin")]
        public string ProductMarkerImg { get; set; }   

        [JsonProperty(PropertyName = "cat"), TextBlob("categoriesBlobbed")]
        public List<int> Categories { get; set; }  
        [field: NonSerialized]
        private ImageJsonModel _imageCollection;
        [JsonProperty(PropertyName = "img"), JsonConverter(typeof(ImageDataConverter)), TextBlob("imagesBlobbed")]
        public ImageJsonModel ImageCollection
        {
            get
            {
                return _imageCollection;
            }
            set
            {
                _imageCollection = value;              
            }
        }

    //    [ForeignKey(typeof(ImageJsonModel))]
   //     public int ImageJsonID { get; set; }
        public string categoriesBlobbed { get; set; }
        public string imagesBlobbed { get; set; }

        public string productMarkerImgBlobbed { get; set; }
        public string productFirmLogoBlobbed { get; set; }


        [Ignore, JsonIgnore]
        public float DistanceToMyLocation { get; set; }

    }
}
