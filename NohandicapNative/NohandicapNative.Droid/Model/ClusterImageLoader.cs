using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Square.Picasso;
using Com.Google.Maps.Android.Clustering;
using Android.Gms.Maps.Model;
using Java.Lang;

namespace NohandicapNative.Droid.Model
{
    public class PicassoMarker : Java.Lang.Object, ITarget
    {
        MarkerOptions marker;
        public PicassoMarker(MarkerOptions marker)
        {
           this.marker = marker;
        }
        public override int GetHashCode()
        {
            return marker.GetHashCode();
        }
        public override bool Equals(Java.Lang.Object o)
        {
            if (typeof(PicassoMarker)==o.GetType()) {
                MarkerOptions marker = ((PicassoMarker)o).marker;
                return marker.Equals(marker);
            } else {
                return false;
            }
        }
        public void OnBitmapFailed(Drawable p0)
        {
          
        }

        public void OnBitmapLoaded(Bitmap p0, Picasso.LoadedFrom p1)
        {
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(p0));
        }

        public void OnPrepareLoad(Drawable p0)
        {
           
        }
    }
}