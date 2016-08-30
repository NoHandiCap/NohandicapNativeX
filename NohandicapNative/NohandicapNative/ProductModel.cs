﻿using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
    [JsonObject]
    public class ProductModel
    {
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
        public string Color { get; set; }
   [JsonProperty(PropertyName = "cat"),Ignore]
        public List<string> categoriesList { get; set; }
        //[JsonProperty(PropertyName = "img"), OneToOne(CascadeOperations = CascadeOperation.All)]
        //public ImageModel imageList { get; set; }    
        //[OneToOne(CascadeOperations = CascadeOperation.All)]
        //public ImageModel MainImage { get; set; }
        [JsonProperty(PropertyName = "img"), OneToOne(CascadeOperations = CascadeOperation.All)]
        [JsonConverter(typeof(ImageDataConverter))]
        public ImageJsonModel imageCollection { get; set; }

        [ForeignKey(typeof(ImageJsonModel))]
        public int ImageJsonID { get; set; }

    }
}
