using System;
using Android.Gms.Maps.Model;
using Com.Google.Maps.Android.Clustering;


namespace NohandicapNative.Droid.Model
{
    public class ClusterItem : Java.Lang.Object, IClusterItem
    {
        public ClusterItem(double lat, double lng)
        {
            Position = new LatLng(lat, lng);
        }

        public ClusterItem(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {

        }

        public LatLng Position
        {
            get;
            set;
        }
        public BitmapDescriptor Icon
        {
            get;
            set;
        }
        public string Url 
        {
            get;
            set;
        }
        public int ProductId
        {
            get;
            set;
        }
    }
}