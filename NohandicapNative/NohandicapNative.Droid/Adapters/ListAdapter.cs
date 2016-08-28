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
using Java.Lang;
using Android.Graphics.Drawables;
using Android.Content.Res;
using NohandicapNative.Droid.Services;
using Android.Support.V4.App;
using Android.Graphics;

namespace NohandicapNative.Droid.Adapters
{
   public class ListAdapter : BaseAdapter
    {
       private List<ProductModel> items;
        Activity context;
        private FragmentActivity activity;

       public ListAdapter(Activity context,List<ProductModel> items)
        {
            this.context = context;
            this.items = items;
        }

       

        public override int Count
        {
            get
            {
             return   items.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return items[position].ID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? context.LayoutInflater.Inflate(
        Resource.Layout.list_item, parent, false);
            var image = view.FindViewById<ImageView>(Resource.Id.image_item);
            var title= view.FindViewById<TextView>(Resource.Id.title_item);
            var description = view.FindViewById<TextView>(Resource.Id.description_item);
            title.Text = items[position].FirmName;
            description.Text = items[position].Description;
            //image.SetImageDrawable(new BitmapDrawable(view.Resources, Utils.GetBitmap(items[position].MainImage.GetImage())));
           image.SetImageDrawable(Utils.GetImage(context,items[position].MainImage.GetImage()));

            image.SetBackgroundColor(Color.ParseColor("#7B1FA2"));
            image.LayoutParameters.Height = 100;
            image.LayoutParameters.Width = 100;
            image.SetPadding(5, 0, 5, 0);
            return view; 
                
        }
    }
}