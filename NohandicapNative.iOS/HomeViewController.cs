using Foundation;
using NohandicapNative;
using NohandicapNative.iOS.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UIKit;

namespace NohandicapNative.iOS
{
    public partial class HomeViewController : UIViewController
    {
        UICollectionViewFlowLayout layout;
 
        public HomeViewController()
        {
           
        }
        public HomeViewController (IntPtr handle) : base (handle)
        {
        }
        public override void LoadView()
        {
            base.LoadView();

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
          
            float itemWidth = ((float)View.Frame.Size.Width) / 3f-8;
            layout = new UICollectionViewFlowLayout
            {
                SectionInset = new UIEdgeInsets(0, 0, 0, 0),
                MinimumInteritemSpacing = 2,
                MinimumLineSpacing = 2,               
                ItemSize = new SizeF(itemWidth, itemWidth)
            };
            buttonCollectionView.CollectionViewLayout = layout;
            buttonCollectionView.ContentSize = View.Frame.Size;
            buttonCollectionView.RegisterClassForCell(typeof(CustomButtonCell), CustomButtonCell.Key);
           // buttonCollectionView.RegisterNibForCell(CustomButtonCell.Nib, CustomButtonCell.Key);
            buttonCollectionView.DataSource = new ButtonCollectionDataSource(NohandicapLibrary.GetAdditionalCategory());
        }
    }
    public class ButtonCollectionDataSource : UICollectionViewDataSource
    {
        List<CategoryModel> tabs;
        public ButtonCollectionDataSource(List<CategoryModel> tabs)
        {
            this.tabs = tabs;
        }
       
        
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(CustomButtonCell.Key,indexPath)as CustomButtonCell;
            var cat = tabs[indexPath.Row]; 
            cell.BackgroundColor = UIColor.Clear.FromHexString(cat.Color);
            cell.Bind(UIImage.FromBundle(cat.Icon),cat.Name);
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return 9;
        }
    }
}