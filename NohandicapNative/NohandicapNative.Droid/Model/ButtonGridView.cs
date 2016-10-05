using Android.Content;
using Android.Views;
using Android.Widget;
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