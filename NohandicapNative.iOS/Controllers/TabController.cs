using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using NohandicapNative.iOS.Services;
using System.Collections.Generic;

namespace NohandicapNative.iOS.Controllers
{
    public class TabController : UITabBarController
    {

        #region private Properties
        UINavigationController tab1, tab2, tab3, tab4;
        #endregion

        #region Tabs Properties
        public HomeViewController HomeTab
        {
            get
            {
                return tab1.ViewControllers[0] as HomeViewController;
            }
            set
            {
                tab1.ViewControllers[0] = value;               
            }
        }
        public MapViewController MapTab
        {
            get
            {
                return tab2.ViewControllers[0] as MapViewController;
            }
            set
            {
                tab2.ViewControllers[0] = value;
                tab2.TabBarItem.Title = "Map";              
            }
        }
        public UIViewController ListTab
        {
            get
            {
                return tab3.ViewControllers[0] as UIViewController;
            }
            set
            {
                tab3.ViewControllers[0] = value;              
            }
        }
        public UIViewController FavTab
        {
            get
            {
                return tab3.ViewControllers[0] as UIViewController;
            }
            set
            {
                tab4.ViewControllers[0] = value;              
            }
        }
        #endregion

        #region ctor
        public TabController()
        {
            var background = UIColor.Clear.FromHexString(Library.BackgroundColor);
            var theme = UIColor.Clear.FromHexString(Library.ThemeColor);
            TabBar.BarTintColor =theme;
            TabBar.TintColor = UIColor.White;
            var logoImg = UIImage.FromBundle("logo_small").ResizeImage(40, 40);
            var logoBtn = new UIButton(UIButtonType.Custom);
            logoBtn.SetBackgroundImage(logoImg, UIControlState.Normal);
            logoBtn.Frame = new CoreGraphics.CGRect(0, 0, logoImg.Size.Width, logoImg.Size.Height);
          
            var tabItems = NohandicapLibrary.GetTabs();
            tab1 = new UINavigationController(new HomeViewController()
            { Title ="Nohandicap"});

            tab2 = new UINavigationController(new MapViewController(new List<ProductDetailModel>())
            { Title = tabItems[1].Title });           

            tab3 = new UINavigationController(new UIViewController()
            { Title = tabItems[2].Title });              

            tab4 = new UINavigationController(new UIViewController()
            { Title = tabItems[3].Title });            

            var tabs = new UINavigationController[] {
                                tab1, tab2, tab3,tab4
                        };
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].NavigationBar.BarTintColor = theme;
                tabs[i].NavigationBar.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = UIColor.White
                };
                tabs[i].TabBarItem = new UITabBarItem();
                tabs[i].TabBarItem.Image = UIImage.FromBundle(tabItems[i].Image);
                tabs[i].TabBarItem.Title = tabItems[i].Title;
                tabs[i].View.BackgroundColor = background;
                    
            }
            ViewControllers = tabs;

        }
        #endregion
    }
}