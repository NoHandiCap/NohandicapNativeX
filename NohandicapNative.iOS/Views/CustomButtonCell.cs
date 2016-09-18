using System;
using NohandicapNative.iOS.Services;
using Foundation;
using UIKit;
using System.Drawing;

namespace NohandicapNative.iOS
{
	public partial class CustomButtonCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString("CustomButtonCell");
		public static readonly UINib Nib;
        public string Title;
        public UIImage img;
		static CustomButtonCell()
		{
			Nib = UINib.FromName("CustomButtonCell", NSBundle.MainBundle);
		}
        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();
          
        }
        public void Bind(UIImage icon, string title)
        {


            img = icon;
            title = title; 
          



        }
        [Export("initWithFrame:")]
        public CustomButtonCell(System.Drawing.RectangleF frame) : base (frame)
        {
            UILabel title= new UILabel();
            title.Text = Title;
            title.Font = UIFont.SystemFontOfSize(12);
            title.TextColor = UIColor.White;
            title.Center = new CoreGraphics.CGPoint(Bounds.Width / 2 - 10, -Bounds.Height+2);
            UIImageView icon = new UIImageView(new CoreGraphics.CGRect(0, 0, 30, 30));         
            icon.Center = new CoreGraphics.CGPoint(Bounds.Width / 2, Bounds.Height / 2 + title.Bounds.Height);
            ContentView.AddSubview(icon);
            ContentView.AddSubview(title);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
           
        }
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
         

        }

        protected CustomButtonCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
            
		}
	}
}
