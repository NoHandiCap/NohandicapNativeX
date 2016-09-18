using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace NohandicapNative.iOS
{
    public partial class HomeView : UIView
    {
     public UIView ButtonView { get
            {
                return buttonView;

            }
            set
            {
                buttonView = value;
            }
        }
        public HomeView():base()
        {

        }
        public HomeView (IntPtr handle) : base (handle)
        {
            
        }
        public static HomeView Create()
        {

            var arr = NSBundle.MainBundle.LoadNib("HomeController", null, null);
            var v = Runtime.GetNSObject<HomeView>(arr.ValueAt(0));

            return v;
        }

        public override void AwakeFromNib()
        {

           
        }
    }
}