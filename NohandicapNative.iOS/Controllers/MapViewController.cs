using CoreGraphics;
using Foundation;
using Google.Maps;
using NohandicapNative.iOS.Services;
using System;
using System.Collections.Generic;
using UIKit;

namespace NohandicapNative.iOS
{
    public partial class MapViewController : UIViewController
    {
        MapView mapView;
        List<ProductDetailModel> productsToShow;
        public MapViewController(List<ProductDetailModel> productsToShow):base()
        {
            this.productsToShow = productsToShow;
        }
        public MapViewController (IntPtr handle) : base (handle)
        {
        }
        public void SetDataProduct(List<ProductDetailModel> productsToShow)
        {
            this.productsToShow = productsToShow;

        }
        public override void LoadView()
        {
            base.LoadView();
            var camera = CameraPosition.FromCamera(latitude: 37.79,
                                            longitude: -122.40,
                                            zoom: 6);
            mapView = MapView.FromCamera(CGRect.Empty, camera);
            mapView.MyLocationEnabled = true;
            View = mapView;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Library.SetLogoImage(NavigationItem);

        }
    }
}