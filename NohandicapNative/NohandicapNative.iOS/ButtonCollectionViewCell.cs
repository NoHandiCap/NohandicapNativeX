using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace NohandicapNative.iOS
{
    public partial class ButtonCollectionViewCell : UICollectionViewCell
    {
        UIImageView imageView;
        UILabel title;
        public ButtonCollectionViewCell (IntPtr handle) : base (handle)
        {
        }
        [Export("initWithFrame:")]
        public ButtonCollectionViewCell(CGRect frame) : base(frame)
        {
            BackgroundView = new UIView { BackgroundColor = UIColor.Orange };

            SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green };

            ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
            ContentView.Layer.BorderWidth = 2.0f;
            ContentView.BackgroundColor = UIColor.White;
            ContentView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);

            imageView = new UIImageView(UIImage.FromBundle("ic_map.png"));
            imageView.Center = ContentView.Center;
            imageView.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
            ContentView.AddSubview(imageView);

            title = new UILabel();
            title.Center = ContentView.Center;
            title.TextColor = UIColor.Black;
            title.Center = ContentView.Center;
            title.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
            ContentView.AddSubview(title);
        }
        [Export("custom")]
        void Custom()
        {
            // Put all your custom menu behavior code here
            Console.WriteLine("custom in the cell");
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
           
         
        }

    }
   
}