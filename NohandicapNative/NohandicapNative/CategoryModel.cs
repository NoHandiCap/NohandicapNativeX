using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NohandicapNative
{
  public  class CategoryModel:BaseModel
    {
        [ForeignKey(typeof(MarkerModel))]
        public int MarkerID { get; set; }
        public string Color { get; set; }
    }
}
