using Android.Content;
using Android.Gms.Maps.Model;
using Com.Google.Maps.Android.Clustering.View;
using Android.Gms.Maps;
using Com.Google.Maps.Android.Clustering;

namespace NohandicapNative.Droid.Model
{
   public class ClusterIconRendered:DefaultClusterRenderer
    {
        public ClusterIconRendered(Context context, GoogleMap map,
                             ClusterManager clusterManager) : base(context, map, clusterManager)
        {

        }
        protected override void OnBeforeClusterItemRendered(Java.Lang.Object p0, MarkerOptions p1)
        {
            var cluster = (ClusterItem)p0;
            p1.SetIcon(cluster.Icon);
            p1.SetTitle(cluster.ProductId.ToString());
            base.OnBeforeClusterItemRendered(p0, p1);
        }

    }
}