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

namespace NohandicapNative.Droid.Services
{
  public  class Utils
    {
        public const string HOME_TAG= "0";
        public const string MAP_TAG = "1";
        public const string LIST_TAG = "2";
        public const string FAVORITES_TAG = "3";

        public static Android.Graphics.Drawables.Drawable GetImage(Context context, string image)

        { 
            var id = context.Resources.GetIdentifier(image, "drawable", context.PackageName);
            return context.Resources.GetDrawable(id);
        }
    }
}