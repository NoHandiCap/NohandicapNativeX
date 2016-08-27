using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace NohandicapNative.iOS
{
    public class TabController : UITabBarController
    {

        UIViewController tab1, tab2, tab3;

        public TabController()
        {
            var items = NohandiLibrary.GetTabs();
            var tabItems = new UIViewController[items.Count];
            for (int i = 0; i < tabItems.Length; i++)
            {
                var tab = items[i];

               var item = new UIViewController();
            item.Title = tab.Title;
            item.TabBarItem = new UITabBarItem(tab.Title, UIImage.FromBundle(tab.Image), 0);
            item.View.BackgroundColor = UIColor.White;
                tabItems[i] = new UINavigationController(item);
                if (i == 0)
                {
                    var flowLayout = new UICollectionViewFlowLayout()
                    {
                        HeaderReferenceSize = new CGSize(100, 100),
                        SectionInset = new UIEdgeInsets(2, 2, 2,2),
                        ScrollDirection = UICollectionViewScrollDirection.Vertical,
                        MinimumInteritemSpacing = 5, // minimum spacing between cells
                        MinimumLineSpacing = 5 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
                    };

                    var col = new ButtonCollectionController(flowLayout);
                    col.CollectionView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
                    col.Title = tab.Title;
                    col.TabBarItem = new UITabBarItem(tab.Title, UIImage.FromBundle(tab.Image), 0);
                    col.View.BackgroundColor = UIColor.White;
                    tabItems[i] = col;
                }
                
            }
           
            //tab1 = new UIViewController();
            //tab1.Title = "Green";
            //tab1.View.BackgroundColor = UIColor.Green;
            //tab1.TabBarItem = new UITabBarItem(UITabBarSystemItem.Favorites, 0);
            //tab2 = new UIViewController();
            //tab2.Title = "Orange";
            //tab2.View.BackgroundColor = UIColor.Orange;
            //tab2.TabBarItem = new UITabBarItem(UITabBarSystemItem.Downloads, 0);
            //tab3 = new UIViewController();
            //tab3.Title = "Red";
            //tab3.View.BackgroundColor = UIColor.Red;
            //tab3.TabBarItem = new UITabBarItem(UITabBarSystemItem.Bookmarks, 0);

            ViewControllers = tabItems;
        }
    }
}
