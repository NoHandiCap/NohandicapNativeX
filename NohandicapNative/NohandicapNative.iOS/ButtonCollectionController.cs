using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using static CoreText.CTFontFeatureAnnotation;

namespace NohandicapNative.iOS
{
 public   class ButtonCollectionController : UICollectionViewController
    {
        static NSString animalCellId = new NSString("ButtonCell");
        static NSString headerId = new NSString("Header");
        List<TabItem> tabs;

        public ButtonCollectionController(UICollectionViewLayout layout, List<TabItem> tabs) : base(layout)
        {
            this.tabs = tabs;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CollectionView.RegisterClassForCell(typeof(ButtonCell), animalCellId);
            CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header, headerId);


        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return tabs.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var tabCell = (ButtonCell)collectionView.DequeueReusableCell(animalCellId, indexPath);

            var tab = tabs[indexPath.Row];

            tabCell.Image =UIImage.FromBundle(tab.Image);

            return tabCell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var headerView = (Header)collectionView.DequeueReusableSupplementaryView(elementKind, headerId, indexPath);
            headerView.Text = "Supplementary View";
            return headerView;
        }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = UIColor.Yellow;
        }

        public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.CellForItem(indexPath);
            cell.ContentView.BackgroundColor = UIColor.White;
        }

        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        //      public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
        //      {
        //          return false;
        //      }

        // for edit menu
        public override bool ShouldShowMenu(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

     

        // CanBecomeFirstResponder and CanPerform are needed for a custom menu item to appear
        public override bool CanBecomeFirstResponder
        {
            get
            {
                return true;
            }
        }

        /*public override bool CanPerform (Selector action, NSObject withSender)
		{
			if (action == new Selector ("custom"))
				return true;
			else
				return false;
		}*/

        //public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        //{
        //    base.WillRotate(toInterfaceOrientation, duration);

        //    var lineLayout = CollectionView.CollectionViewLayout as LineLayout;
        //    if (lineLayout != null)
        //    {
        //        if ((toInterfaceOrientation == UIInterfaceOrientation.Portrait) || (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown))
        //            lineLayout.SectionInset = new UIEdgeInsets(400, 0, 400, 0);
        //        else
        //            lineLayout.SectionInset = new UIEdgeInsets(220, 0.0f, 200, 0.0f);
        //    }
        //}
    }
    public class Header : UICollectionReusableView
    {
        UILabel label;

        public string Text
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
                SetNeedsDisplay();
            }
        }

        [Export("initWithFrame:")]
        public Header(CGRect frame) : base(frame)
        {
            label = new UILabel() { Frame = new CGRect(0, 0, 300, 50), BackgroundColor = UIColor.Yellow };
            AddSubview(label);
        }
    }
}