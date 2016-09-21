using Foundation;
using NohandicapNative.iOS.Services;
using System;
using System.Collections.Generic;
using UIKit;

namespace NohandicapNative.iOS
{
    public partial class MapViewController : UIViewController
    {
        List<ProductModel> productsToShow;
        public MapViewController(List<ProductModel> productsToShow):base()
        {
            this.productsToShow = productsToShow;
        }
        public MapViewController (IntPtr handle) : base (handle)
        {
        }
        public void SetDataProduct(List<ProductModel> productsToShow)
        {
            this.productsToShow = productsToShow;

        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Library.SetLogoImage(NavigationItem);
        }
    }
}