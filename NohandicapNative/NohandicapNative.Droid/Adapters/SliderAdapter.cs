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
using Android.Support.V4.View;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid.Adapters
{
  public class SliderAdapter : PagerAdapter
    {
        private Context context;
        List<ImageModel> imageList;
        public SliderAdapter(Context context, List<ImageModel> imageList)
        {
            this.context = context;
            this.imageList = imageList;
        }
        public override int Count
        {
            get
            {
                return imageList.Count;
            }
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view == ((ImageView)objectValue);
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            ImageView imageView = new ImageView(context);
            imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            imageView.SetImageBitmap(Utils.GetBitmap(imageList[position].LocalImage));
            ((ViewPager)container).AddView(imageView, 0);
            return imageView;
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object objectValue)
        {
            ((ViewPager)container).RemoveView((ImageView)objectValue);
        }
    }
}