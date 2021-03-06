﻿using System;
using NohandicapNative.iOS.Services;
using Foundation;
using UIKit;
using System.Drawing;
using CoreGraphics;

namespace NohandicapNative.iOS
{
	public partial class CustomButtonCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString("CustomButtonCell");
		public static readonly UINib Nib;
       public UILabel TitleLabel
        {
            get;set;
        }
        public UIImageView iconView { get; set; }
		static CustomButtonCell()
		{
			Nib = UINib.FromName("CustomButtonCell", NSBundle.MainBundle);
		}
  
       
        [Export("initWithFrame:")]
        public CustomButtonCell(CGRect frame) : base (frame)
        {
            var iconSize = new CGSize(50, 50);
            iconView = new UIImageView(new CoreGraphics.CGRect(new CGPoint(0,0), iconSize));
            iconView.Center = ContentView.Center;
            iconView.Frame = new CGRect(iconView.Frame.X, iconView.Frame.Y-5, iconView.Frame.Width, iconView.Frame.Height);
            TitleLabel = new UILabel();
            TitleLabel.Font = UIFont.SystemFontOfSize(14);
            TitleLabel.TextColor = UIColor.White;
            TitleLabel.TextAlignment = UITextAlignment.Center;
            TitleLabel.Frame = new CGRect(0,iconView.Frame.Y+iconSize.Height/2+20, frame.Size.Width, 35);
            ContentView.AddSubview(TitleLabel);
            ContentView.AddSubview(iconView);  

        }    

        protected CustomButtonCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
            
		}
	}
}
