using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace NohandicapNative.iOS.Services
{
    public static class UIColorExtensions
    {
        public static UIColor FromHexString(this UIColor color, string hexValue)
        {
            var colorString = hexValue.Replace("#", "");
            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIColor.FromRGB(red, green, blue);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIColor.FromRGB(red, green, blue);
                    }
                case 8: // #AARRGGBB
                    {
                        var alpha = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        red = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(6, 2), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB, or #AARRGGBB", hexValue));

            }

        }
    }
}
