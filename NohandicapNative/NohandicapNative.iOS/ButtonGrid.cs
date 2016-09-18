using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace NohandicapNative.iOS
{
    public partial class ButtonGrid : UICollectionView
    {
        public UIView ButtonView
        {
            get
            {
                return buttonGridView;
            }
            set
            {
                buttonGridView = value;
            }
        }
        public ButtonGrid (IntPtr handle) : base (handle)
        {
        }
        public static ButtonGrid Create()
        {

            var arr = NSBundle.MainBundle.LoadNib("ButtonGrid", null, null);
            var v = Runtime.GetNSObject<ButtonGrid>(arr.ValueAt(0));
           
            return v;
        }

        public override void AwakeFromNib()
        {
          
            
        }
    }
}