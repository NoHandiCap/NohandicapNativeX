using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace NohandicapNative.iOS
{
    public class ButtonCell : UICollectionViewCell
    {
        UIImageView imageView;
        UILabel titleLabel;
        [Export("initWithFrame:")]
        public ButtonCell(CGRect frame) : base(frame)
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.Orange };

            SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green };

            ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
            ContentView.Layer.BorderWidth = 2.0f;
            ContentView.BackgroundColor = UIColor.Blue;
            ContentView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);

            imageView = new UIImageView(UIImage.FromBundle("eat.png"));
            imageView.Center = ContentView.Center;
            imageView.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);

            titleLabel = new UILabel();
            titleLabel.Center = ContentView.Center;


            ContentView.AddSubview(imageView);
            ContentView.AddSubview(titleLabel);
        }

        public UIImage Image
        {
            set
            {
                imageView.Image = value;
            }
        }
        public string Title
        {
            get { return titleLabel.Text; }
            set
            {
                titleLabel.Text = value;
            }
        }
    }
}
