using CoreGraphics;
using Foundation;
using NohandicapNative;
using NohandicapNative.iOS.Controllers;
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
        #region private properties
        UICollectionViewFlowLayout layout;
        UICollectionView buttonGrid;
        UIView[] categotiesArray;
        UIImageView[] categotiesImageArray;
        UILabel[] categotiesNameArray;
        #endregion

        #region ctor
        public HomeViewController()
        {

        }
        public HomeViewController(IntPtr handle) : base(handle)
        {
        }
        #endregion

        #region override methods  
      
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            DrawInterface();


        }
        #endregion

        #region private methods
        private void SelectCategory (int index)
        {
            for (int i = 0; i < categotiesNameArray.Length; i++)
            {
                if (index == i)
                {
                    categotiesNameArray[i].Font = UIFont.BoldSystemFontOfSize(categotiesNameArray[i].Font.PointSize);
                }
                else
                {
                    categotiesNameArray[i].Font = UIFont.SystemFontOfSize(categotiesNameArray[i].Font.PointSize);

                }
            }
        }
        private void DrawInterface()
        {

            Library.SetLogoImage(NavigationItem);
            float itemWidth;
            if (InterfaceOrientation == UIInterfaceOrientation.Portrait)
            {
                itemWidth = ((float)View.Frame.Size.Width) / 3f - 10;
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
            var itemCategoryHeight = (categoriesViewHeight - 10) / 3;
            var imageFrame = new CGRect(4, 7, View.Frame.Width * 0.20, itemCategoryHeight - 14);
            var nameFram = new CGRect(imageFrame.Width + 4, 0, View.Frame.Width * 0.6, itemCategoryHeight - 4);
            var fontSize = UIFont.SystemFontOfSize(16);


            buttonGrid = new UICollectionView(new CGRect(10, View.Frame.Size.Height - gridHeight, View.Frame.Width - 20, gridHeight), layout);

            buttonGrid.CollectionViewLayout = layout;
            buttonGrid.RegisterClassForCell(typeof(CustomButtonCell), CustomButtonCell.Key);
            // buttonCollectionView.RegisterNibForCell(CustomButtonCell.Nib, CustomButtonCell.Key);
            var additionalCategories = NohandicapLibrary.GetAdditionalCategory();
            buttonGrid.DataSource = new ButtonCollectionDataSource(additionalCategories);
            buttonGrid.BackgroundColor = UIColor.Clear;
            buttonGrid.Delegate = new ButtonCollectionDelegate(additionalCategories);

            var categoriesView = new UIView();
            categoriesView.Frame = new CGRect(10, 80, View.Frame.Width - 20, categoriesViewHeight);
            categoriesView.BackgroundColor = UIColor.Clear;

            var category1 = new UIView();
            category1.BackgroundColor = UIColor.White;
            var name1 = new UILabel(nameFram);
            var image1 = new UIImageView(imageFrame);
            name1.Text = "Barierefrei";
            name1.Font = fontSize;
            image1.Image = UIImage.FromBundle("wheelchair1");
            category1.AddSubview(name1);
            category1.AddSubview(image1);
            category1.Frame = new CGRect(0, 0, categoriesView.Frame.Size.Width, itemCategoryHeight);
            category1.AddGestureRecognizer(new UITapGestureRecognizer(() => {
                SelectCategory(0);
            }));
            categoriesView.AddSubview(category1);


            var category2 = new UIView();
            var name2 = new UILabel(nameFram);
            var image2 = new UIImageView(imageFrame);
            name2.Text = "Teilweise behinderte";
            name2.Font = fontSize;
            image2.Image = UIImage.FromBundle("wheelchair2");
            category2.AddSubview(name2);
            category2.AddSubview(image2);
            category2.BackgroundColor = UIColor.White;
            category2.Frame = new CGRect(new CGPoint(0, category1.Frame.Y + itemCategoryHeight + 5), new CGSize(categoriesView.Frame.Size.Width, itemCategoryHeight));
            category2.AddGestureRecognizer(new UITapGestureRecognizer(() => {
                SelectCategory(1);
            }));
            categoriesView.AddSubview(category2);

            var category3 = new UIView();
            var name3 = new UILabel(nameFram);
            var image3 = new UIImageView(imageFrame);
            name3.Text = "Total behinderte";
            name3.Font = fontSize;
            image3.Image = UIImage.FromBundle("wheelchair3");
            category3.AddSubview(name3);
            category3.AddSubview(image3);
            category3.BackgroundColor = UIColor.White;
            category3.Frame = new CGRect(new CGPoint(0, category2.Frame.Y + itemCategoryHeight + 5), new CGSize(categoriesView.Frame.Size.Width, itemCategoryHeight));
            category3.AddGestureRecognizer(new UITapGestureRecognizer(() => {
                SelectCategory(2);
            }));
            categoriesView.AddSubview(category3);
            // buttonCollectionView.Frame = new CoreGraphics.CGRect(0, View.Frame.Y, buttonCollectionView.Frame.Width, buttonCollectionView.Frame.Height);
            categotiesArray = new UIView[] { category1, category2, category3 };
            categotiesImageArray = new UIImageView[] { image1, image2, image3 };
            categotiesNameArray = new UILabel[] { name1, name2, name3 };
            View.AddSubview(buttonGrid);
            View.AddSubview(categoriesView);
        }
        #endregion
    }

    public class ButtonCollectionDataSource : UICollectionViewDataSource
    {
        #region private properties
        List<CategoryModel> tabs;
        #endregion

        #region ctor
        public ButtonCollectionDataSource(List<CategoryModel> tabs)
        {
            this.tabs = tabs;
        }

        #endregion

        #region override methods  
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
        #endregion

    }
    public class ButtonCollectionDelegate : UICollectionViewDelegate
    {
        public delegate void OnItemSelected(UICollectionView tableView, NSIndexPath indexPath);

        #region private properties
        private readonly OnItemSelected _onItemSelected;
        List<CategoryModel> _categoties;

        TabController _tabController = ((AppDelegate)(UIApplication.SharedApplication.Delegate)).TabController;
        #endregion

        #region ctor
        public ButtonCollectionDelegate(List<CategoryModel> categoties)
        {
            _categoties = categoties;
        }
        #endregion

        #region override methods  
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
           
        }

        public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
        {
            _tabController.SelectedIndex = 1;
            _tabController.MapTab.Title = _categoties[indexPath.Row].Name;
        }
        #endregion
    }
}