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
using Android.Support.V7.Widget;
using Android.Graphics.Drawables;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid.Adapters
{
    public class CardViewAdapter : BaseAdapter<ProductModel>
    {
        private readonly Activity context;
        private readonly List<ProductModel> products;

        public CardViewAdapter(Activity context, List<ProductModel> products)
        {
            this.context = context;
            this.products = products;
        }

        public override ProductModel this[int position]
        {
            get
            {
                return products[position];
            }
        }

        public override int Count
        {
            get
            {
                return products.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.list_item, parent, false);
            }

            var titleTextView = view.FindViewById<TextView>(Resource.Id.cardViewText);
            var image = view.FindViewById<ImageView>(Resource.Id.imageView);

            var mainimage = products[position].ImageCollection.Images;
            if (mainimage.Count != 0)
                image.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));
            image.LayoutParameters.Height = 100;
            image.LayoutParameters.Width = 100;
            image.SetPadding(5, 0, 5, 0);
             titleTextView.Text = products[position].FirmName;
            return view;
        }
    }
}
