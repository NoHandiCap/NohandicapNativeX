using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace NohandicapNative.Droid.Adapters
{
    public class CustomInfoWindowAdapter : Java.Lang.Object, Android.Gms.Maps.GoogleMap.IInfoWindowAdapter
    {
        private LayoutInflater layoutInflater = null;

        public CustomInfoWindowAdapter(LayoutInflater inflater)
        {
            layoutInflater = inflater;
        }
        public View GetInfoContents(Marker marker)
        {
            var view = layoutInflater.Inflate(Resource.Layout.infoWindow, null);
            return view;
        }

        public View GetInfoWindow(Marker marker)
        {
            return null;
        }
    }
}