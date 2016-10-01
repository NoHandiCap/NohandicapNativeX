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
using Android.Util;
using System.Threading.Tasks;
using Android.Graphics;

namespace NohandicapNative.Droid.Adapters
{
  public class SliderAdapter : PagerAdapter
    {
        string TAG = "X: " + typeof(SliderAdapter).Name;

        private Context context;
       ProductModel product;

        public SliderAdapter(Context context, ProductModel product)
        {
            this.context = context;
            this.product = product;
           
        }
        public override int Count
        {
            get
            {
                return product.ImageCollection.Images.Count;
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
           
            var img = product.ImageCollection.Images[position];
          
            try
            {
                if (string.IsNullOrWhiteSpace(img.LocalImage))
                {
                    LoadImageAsync(imageView, img);
                }
                else
                {
                    imageView.SetImageBitmap(Utils.GetBitmap(img.LocalImage));
                    
                }
                
            }
            catch(Exception e)
            {
                Log.Error(TAG, e.Message);
            }
           ((ViewPager)container).AddView(imageView, 0);
            return imageView;
        }
       public async void LoadImageAsync(ImageView imageView,ImageModel img)
        {
            var dbCon = Utils.GetDatabaseConnection();
            imageView.SetBackgroundResource(Resource.Drawable.placeholder);          
                string filename = "none";
                Uri uri = new Uri(img.LinkImage);
                filename = System.IO.Path.GetFileName(uri.LocalPath);
                var bitmap =await Utils.SaveImageBitmapFromUrl(img.LinkImage, filename);
                img.LocalImage = filename;
                dbCon.InsertUpdateProduct(product);
                imageView.SetImageBitmap(bitmap);
            dbCon.Close();
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object objectValue)
        {
            ((ViewPager)container).RemoveView((ImageView)objectValue);
            GC.Collect();
        }
    }
}