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
using Android.Graphics;
using Android.Text;

namespace NohandicapNative.Droid.Adapters
{
    public class CardViewAdapter : BaseAdapter<ProductModel>
    {
        private readonly Activity context;
        private readonly List<ProductModel> products;
        SqliteService dbCon;
        public CardViewAdapter(Activity context, List<ProductModel> products)
        {
            this.context = context;
            this.products = products;
            dbCon = Utils.GetDatabaseConnection();
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
            
            var categories=dbCon.GetDataList<CategoryModel>();
            var titleTextView = view.FindViewById<TextView>(Resource.Id.title_text);
            var image = view.FindViewById<ImageView>(Resource.Id.logo_image);
            var adress = view.FindViewById<TextView>(Resource.Id.adress_text);
            var body = view.FindViewById<TextView>(Resource.Id.body_text);
            var photo = view.FindViewById<ImageView>(Resource.Id.image_photo);
            var hours = view.FindViewById<TextView>(Resource.Id.hours_text);
            var booking = view.FindViewById<TextView>(Resource.Id.booking_link);
            var homeLink = view.FindViewById<TextView>(Resource.Id.main_link);
            var mainimage = products[position].ImageCollection.Images;
            if (mainimage.Count != 0)
            {
                
              //  image.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));
                photo.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));
            }
            
            var catImage = categories.FirstOrDefault(x => x.ID == products[position].Categories[0]);
            
            if (catImage != null) {
                image.SetImageDrawable(Utils.GetImage(context, catImage.Icon));
                image.SetBackgroundColor( Color.ParseColor(catImage.Color));
                    }
            image.SetPadding(5, 0, 5, 0);
             titleTextView.Text = products[position].FirmName;
           adress.Text = products[position].Adress;
            body.TextFormatted=Html.FromHtml(products[position].Description);
          if(products[position].OpenTime!="")  hours.Text = products[position].OpenTime;
            if (products[position].BookingPage != "") booking.Text = products[position].BookingPage;
            if (products[position].HomePage !="") homeLink.Text = products[position].HomePage;
            return view;
        }
    }
}
