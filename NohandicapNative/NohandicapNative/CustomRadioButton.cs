using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NohandicapNative.Droid.Model
{
  public  class CustomRadioButton
    {
        public string ResourceImage { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public CustomRadioButton()
        {

        }
    }
}