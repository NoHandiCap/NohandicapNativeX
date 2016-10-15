using Android.Graphics;
using Android.Graphics.Drawables;
using Square.Picasso;
using Android.Gms.Maps.Model;


namespace NohandicapNative.Droid.Model
{
    public class PicassoMarker : Java.Lang.Object, ITarget
    {
        Marker marker;
        public PicassoMarker(Marker marker)
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
                Marker marker = ((PicassoMarker)o).marker;
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