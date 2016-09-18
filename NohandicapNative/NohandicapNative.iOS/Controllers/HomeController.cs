using CoreGraphics;
using Foundation;
using System;
using System.Drawing;
using UIKit;


namespace NohandicapNative.iOS
{
    public partial class HomeController : UIViewController
    {
        
        UICollectionViewFlowLayout layout;
        UICollectionView collectionView;

        CustomButtonViewFlowLayout collectionButtonView;
        public HomeController()
        {
           
        }
        public HomeController (IntPtr handle) : base (handle)
        {

        }
        public override void LoadView()
        {
            base.LoadView();


            var layout = new UICollectionViewFlowLayout
            {
                SectionInset = new UIEdgeInsets(20, 5, 5, 5),
                MinimumInteritemSpacing = 5,
                MinimumLineSpacing = 5,
                ItemSize = new SizeF(100, 100)
            };

            collectionView = new UICollectionView(UIScreen.MainScreen.Bounds, layout);
            collectionView.ContentSize = View.Frame.Size;
            collectionView.RegisterClassForCell(typeof(ButtonCollectionViewCell), "buttonCell");
            collectionView.DataSource = new HomeControllerDataSource();
            var home = ButtonGrid.Create();

            View = home;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


          
        }
        
    }
    public class HomeControllerDataSource : UICollectionViewDataSource
    {
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("buttonCell", indexPath) as ButtonCollectionViewCell;
            cell.Frame = new CoreGraphics.CGRect(0, 0, 100, 100);


            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return 9;
        }
        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 9;
        }
    }
}