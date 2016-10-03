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
using Square.Picasso;
using Android.Gms.Maps.Model;

namespace NohandicapNative.Droid.Model
{
    public class MarkerCallback : Java.Lang.Object, ICallback
    {
        Marker marker = null;
        public MarkerCallback(Marker marker)
        {
            this.marker = marker;
        }    

        public void OnError()
        {
            
        }

        public void OnSuccess()
        {
            if (marker != null && marker.IsInfoWindowShown)
            {
                marker.HideInfoWindow();
                marker.ShowInfoWindow();
            }
        }
    }
}