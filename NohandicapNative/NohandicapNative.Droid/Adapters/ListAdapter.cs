using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

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
            //var image = view.FindViewById<ImageView>(Resource.Id.image_item);
            //var title= view.FindViewById<TextView>(Resource.Id.title_item);
            //var description = view.FindViewById<TextView>(Resource.Id.description_item);
            //title.Text = items[position].FirmName;
            //description.Text = items[position].Description;
            //var mainimage = items[position].ImageCollection.Images;
            //if (mainimage.Count != 0)
            //   image.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));   
            //image.LayoutParameters.Height = 100;
            //image.LayoutParameters.Width = 100;
            //image.SetPadding(5, 0, 5, 0);
            return view; 
                
        }
    }
}