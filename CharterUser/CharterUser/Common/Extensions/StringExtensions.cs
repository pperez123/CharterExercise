using System;
using UIKit;

namespace CharterUser.Common.Extensions
{
    public static class StringExtensions
    {
        public static UIColor ColorFromHexString(this string hexValue, float alpha = 1.0f)
        {
            var colorString = hexValue.Replace("#", "");
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            else if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }
            float red, green, blue;
            red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
            green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
            blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
            return UIColor.FromRGBA(red, green, blue, alpha);
        }
    }
}