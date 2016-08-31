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
using Java.Util.Jar;
using Android.Util;

namespace NohandicapNative.Droid
{
    public class ButtonGridView : GridView
    {
        public ButtonGridView(Context context) : base(context)
        {

        }

        public ButtonGridView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public ButtonGridView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int expandSpec = MeasureSpec.MakeMeasureSpec(int.MaxValue >> 2,
                MeasureSpecMode.AtMost);
            base.OnMeasure(widthMeasureSpec, expandSpec);
        }
    }
}