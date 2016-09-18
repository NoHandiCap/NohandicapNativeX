using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UIKit;

namespace NohandicapNative.iOS
{
    public class TabController : UITabBarController
    {

        UIViewController tab1, tab2, tab3;

        public TabController()
        {
            var items = NohandicapLibrary.GetTabs();
            var tabItems = new UIViewController[items.Count];
            for (int i = 0; i < tabItems.Length; i++)
            {
                var tab = items[i];

               var item = new UIViewController();
            item.Title = tab.Title;
            item.TabBarItem = new UITabBarItem(tab.Title,ResizeImage(UIImage.FromBundle(tab.Image),30,30), 0);
            item.View.BackgroundColor = UIColor.White;
                tabItems[i] = new UINavigationController(item);
                if (i == 0)
                {               

                    var col = new HomeController();             
                    tabItems[i] = new UINavigationController(col);
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
        public static UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

    }
}
