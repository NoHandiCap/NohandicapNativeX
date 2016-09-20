using CoreGraphics;
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
        UICollectionView buttonGrid;
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
        private void DrawInterface()
        {

            float itemWidth;
            if (InterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                itemWidth = ((float)View.Frame.Size.Width) / 3f - 6;
            }
            else
            {
                itemWidth = ((float)View.Frame.Size.Width) / 5f - 11;
            }
            
            layout = new UICollectionViewFlowLayout
            {
                SectionInset = new UIEdgeInsets(-20, 0, 0, 0),
                MinimumInteritemSpacing = 1,
                MinimumLineSpacing = 4,
                ItemSize = new SizeF(itemWidth, itemWidth),

            };
            var gridHeight = View.Frame.Height * 0.7;
            var categoriesViewHeight = View.Frame.Height * 0.20;
            var imageFrame = new CGRect(4, 2.5, View.Frame.Width * 0.20, 25);
            var nameFram = new CGRect(imageFrame.Width + 4, 0, View.Frame.Width * 0.6, 30);
            var itemCategoryHeight = 30f;
                                
        
            buttonGrid = new UICollectionView(new CGRect(5, View.Frame.Size.Height - gridHeight-20, View.Frame.Width - 10, gridHeight), layout);

            buttonGrid.CollectionViewLayout = layout;
            buttonGrid.RegisterClassForCell(typeof(CustomButtonCell), CustomButtonCell.Key);
            // buttonCollectionView.RegisterNibForCell(CustomButtonCell.Nib, CustomButtonCell.Key);
            buttonGrid.DataSource = new ButtonCollectionDataSource(NohandicapLibrary.GetAdditionalCategory());
            buttonGrid.BackgroundColor = UIColor.Clear;     

          
            var categoriesView = new UIView();
            categoriesView.Frame = new CGRect(5, 80, View.Frame.Width - 10, categoriesViewHeight);
            categoriesView.BackgroundColor = UIColor.Clear;
         
            var category1 = new UIView();
            category1.BackgroundColor = UIColor.White;
            var name1 = new UILabel(nameFram);
            var image1 = new UIImageView(imageFrame);
            name1.Text= "Barierefrei";
            name1.Font = UIFont.SystemFontOfSize(11);
            image1.Image = UIImage.FromBundle("wheelchair1");
            category1.AddSubview(name1);
            category1.AddSubview(image1);
            category1.Frame = new CGRect(0,0, categoriesView.Frame.Size.Width, itemCategoryHeight);
            categoriesView.AddSubview(category1);
            

            var category2 = new UIView();
            var name2 = new UILabel(nameFram);
            var image2 = new UIImageView(imageFrame);
            name2.Text = "Teilweise behinderte";
            name2.Font = UIFont.SystemFontOfSize(11);
            image2.Image = UIImage.FromBundle("wheelchair2");
            category2.AddSubview(name2);
            category2.AddSubview(image2);
            category2.BackgroundColor = UIColor.White;
            category2.Frame = new CGRect(new CGPoint(0, category1.Frame.Y + itemCategoryHeight + 3), new CGSize(categoriesView.Frame.Size.Width, itemCategoryHeight));
            categoriesView.AddSubview(category2);

            var category3 = new UIView();
            var name3 = new UILabel(nameFram);
            var image3 = new UIImageView(imageFrame);
            name3.Text = "Total behinderte";
            name3.Font = UIFont.SystemFontOfSize(11);
            image3.Image = UIImage.FromBundle("wheelchair3");
            category3.AddSubview(name3);
            category3.AddSubview(image3);
            category3.BackgroundColor = UIColor.White;
            category3.Frame = new CGRect(new CGPoint(0, category2.Frame.Y + itemCategoryHeight + 3), new CGSize(categoriesView.Frame.Size.Width,itemCategoryHeight));
            categoriesView.AddSubview(category3);
            // buttonCollectionView.Frame = new CoreGraphics.CGRect(0, View.Frame.Y, buttonCollectionView.Frame.Width, buttonCollectionView.Frame.Height);

            View.AddSubview(buttonGrid);
            View.AddSubview(categoriesView);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            DrawInterface();
           
          
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
            cell.TitleLabel.Text = cat.Name;
            var img = UIImage.FromBundle(cat.Icon);
            cell.iconView.Image = img;
            return cell;

        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return 9;
        }
    }
}