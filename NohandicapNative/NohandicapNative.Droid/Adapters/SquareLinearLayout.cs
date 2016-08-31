using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace NohandicapNative.Droid.Adapters
{
  public  class SquareRelativeLayout : RelativeLayout
    {
        public SquareRelativeLayout(Context context) : base(context)
        {
          
        }

        public SquareRelativeLayout(Context context, IAttributeSet attrs): base(context, attrs)
        {
            
        }

        public SquareRelativeLayout(Context context, IAttributeSet attrs, int defStyle): base(context, attrs, defStyle)
        {
            
        }

     
protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
           var widthDesc =MeasureSpec.GetMode(widthMeasureSpec);
            var heightDesc = MeasureSpec.GetMode(heightMeasureSpec);
            int size = 0;
            if (widthDesc == MeasureSpecMode.Unspecified
                    && heightDesc == MeasureSpecMode.Unspecified)
            {
                size = Context.Resources.GetDimensionPixelSize(125); // Use your own default size, for example 125dp
            }
            else if ((widthDesc == MeasureSpecMode.Unspecified || heightDesc == MeasureSpecMode.Unspecified)
                  && !(widthDesc == MeasureSpecMode.Unspecified && heightDesc == MeasureSpecMode.Unspecified))
            {
                //Only one of the dimensions has been specified so we choose the dimension that has a value (in the case of unspecified, the value assigned is 0)
                size = width > height ? width : height;
            }
            else
            {
                //In all other cases both dimensions have been specified so we choose the smaller of the two
                size = width > height ? height : width;
            }
            SetMeasuredDimension(size, size-10);
        }
    }
}