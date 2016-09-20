using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using NohandicapNative.iOS.Services;

namespace NohandicapNative.iOS.Controllers
{
    public class TabController : UITabBarController
    {

        UIViewController tab1, tab2, tab3,tab4;

        public TabController()
        {
            var background = UIColor.Clear.FromHexString(Library.BackgroundColor);
            var tabItems = NohandicapLibrary.GetTabs();
            tab1 = new UINavigationController(new HomeViewController() { Title = tabItems[0].Title });
        
            tab1.TabBarItem = new UITabBarItem();
            tab1.TabBarItem.Image = UIImage.FromBundle(tabItems[0].Image);
            tab1.TabBarItem.Title = tabItems[0].Title;
            tab1.View.BackgroundColor =background;

            tab2 = new UINavigationController(new MapViewController() { Title = tabItems[1].Title });
            
            tab2.TabBarItem = new UITabBarItem();
            tab2.TabBarItem.Image = UIImage.FromBundle(tabItems[1].Image);
            tab2.TabBarItem.Title = tabItems[1].Title;
            tab2.View.BackgroundColor = background;
       

            tab3 = new UINavigationController(new UIViewController() { Title = tabItems[2].Title });            
            tab3.TabBarItem = new UITabBarItem();
            tab3.TabBarItem.Image = UIImage.FromBundle(tabItems[2].Image);
            tab3.TabBarItem.Title = tabItems[2].Title;
            tab3.View.BackgroundColor = background;


            tab4 = new UINavigationController(new UIViewController() { Title = tabItems[3].Title });
            tab4.TabBarItem = new UITabBarItem();
            tab4.TabBarItem.Image = UIImage.FromBundle(tabItems[3].Image);
            tab4.TabBarItem.Title = tabItems[3].Title;
            tab4.View.BackgroundColor = background;

            var tabs = new UIViewController[] {
                                tab1, tab2, tab3,tab4
                        };

            ViewControllers = tabs;
        }
    }
}