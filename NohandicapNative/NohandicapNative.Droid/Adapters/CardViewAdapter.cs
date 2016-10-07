using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using System.Threading.Tasks;
using Square.Picasso;
using NohandicapNative.Droid.Model;
using Android.Content;

namespace NohandicapNative.Droid.Adapters
{
    public class CardViewAdapter : BaseAdapter<ProductModel>
    {
        string TAG = "X: " + typeof(CardView).Name;
        private readonly Activity context;
        private readonly List<ProductModel> products;
        List<CategoryModel> selectedCategory;
        List<CategoryModel> categories;
        public CardViewAdapter(Activity context, List<ProductModel> products)
        {
            this.context = context;
            this.products = products;
            var dbCon = Utils.GetDatabaseConnection();
            categories = dbCon.GetDataList<CategoryModel>();         
           selectedCategory = dbCon.GetDataList<CategoryModel>().Where(x => x.IsSelected).ToList();
   
            dbCon.Close();
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
            {// view = context.LayoutInflater.Inflate(Resource.Layout.list_item, parent, false);
             view = context.LayoutInflater.Inflate(Resource.Layout.list_item_first, parent, false);
                view.SetBackgroundColor(Color.White);
            }
            var imageView = view.FindViewById<ImageView>(Resource.Id.mainImageView);
            var title = view.FindViewById<TextView>(Resource.Id.titleTextView);
            var adress = view.FindViewById<TextView>(Resource.Id.adressTextView);
            var positionTextView = view.FindViewById<TextView>(Resource.Id.positionTextView);
            var distanceLayout = view.FindViewById<LinearLayout>(Resource.Id.distanceLayout);
            title.Text = products[position].FirmName;
            adress.Text = products[position].Adress;
            if (products[position].DistanceToMyLocation != 0)
            {
                positionTextView.Text = NohandicapLibrary.ConvertMetersToKilometers(products[position].DistanceToMyLocation);
            }
            else
            {
                distanceLayout.Visibility = ViewStates.Gone;
            }
            string imageUrl = "";
            if (!string.IsNullOrEmpty(products[position].MainImageUrl))
            {
                imageUrl = products[position].MainImageUrl;
            }
            else
            {

                CategoryModel catImage;
                if (selectedCategory.Count != 0)
                {
                    catImage = selectedCategory.FirstOrDefault(x => products[position].Categories.Any(y => y == x.ID));
                }
                else
                {
                    catImage = categories.FirstOrDefault(x => products[position].Categories.Any(y => y == x.ID));

                }
                if (catImage != null)
                {
                    imageUrl = ContentResolver.SchemeAndroidResource + "://" + context.PackageName + "/drawable/" + catImage.Icon;
                    imageView.SetBackgroundColor(Color.ParseColor(catImage.Color));
                }
            }          
               Picasso.With(context).Load(imageUrl).Resize(60, 60).Into(imageView);

                return view;
        }
        
    }
}
