using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UIKit;

namespace NohandicapNative.iOS.Services
{
  public static class Library
    {
        public static string BackgroundColor = "#FFECB3";
        public static string ThemeColor = "#FF74032C";
        public const string MapsApiKey = "AIzaSyBYePlqzhp-2f4azsWJDhmBFO57gfY6TcI";

        // resize the image (without trying to maintain aspect ratio)
        public static UIImage ResizeImage(this UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
        public static UIImage MaxResizeImage(this UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
            sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
        public static void SetLogoImage(UINavigationItem navItems)
        {
            var logoImg = UIImage.FromBundle("logo_small").ResizeImage(40, 40);
            var logoBtn = new UIButton(UIButtonType.Custom);
            logoBtn.SetBackgroundImage(logoImg, UIControlState.Normal);
            logoBtn.Frame = new CoreGraphics.CGRect(0, 0, logoImg.Size.Width, logoImg.Size.Height);
            navItems.LeftBarButtonItem = new UIBarButtonItem(logoBtn);
            
        }
    }
}
