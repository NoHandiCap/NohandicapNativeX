using Android.Graphics;
using Android.Graphics.Drawables;
using Square.Picasso;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid.Model
{
    public class PicassoMarker : Java.Lang.Object, ITarget
    {
        Marker _marker;       
        public PicassoMarker(Marker marker)
        {
           this._marker = marker;           
        }
        public override int GetHashCode()
        {
            return _marker.GetHashCode();
        }
        public override bool Equals(Java.Lang.Object o)
        {
            if (typeof(PicassoMarker)==o.GetType()) {
                Marker marker = ((PicassoMarker)o)._marker;
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
            _marker.SetIcon(BitmapDescriptorFactory.FromBitmap(p0));
            _marker.Visible = true;
        }

        public void OnPrepareLoad(Drawable p0)
        {
           
        }
    }
}