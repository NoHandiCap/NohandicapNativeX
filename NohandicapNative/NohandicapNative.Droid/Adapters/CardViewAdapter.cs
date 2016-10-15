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
using System.Threading;
using System.Collections.ObjectModel;

namespace NohandicapNative.Droid.Adapters
{
    public class CardViewAdapter : BaseAdapter<ProductMarkerModel>
    {
        string TAG = "X: " + typeof(CardView).Name;
        private readonly Activity context;
        private readonly ObservableCollection<ProductMarkerModel> products;
        List<CategoryModel> selectedCategory;
        List<CategoryModel> categories;
        int PageNumber =1;
        public CardViewAdapter(Activity context, List<ProductMarkerModel> products)
        {
            this.context = context;
            this.products = new ObservableCollection<ProductMarkerModel>(products);
            var conn = Utils.GetDatabaseConnection();
            categories = conn.GetDataList<CategoryModel>();
            selectedCategory = conn.GetSubSelectedCategory();
   
            
        }
      

        public override ProductMarkerModel this[int position]
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
            title.Text = products[position].Name;
            adress.Text = products[position].Address;

            if (products[position].Distance != null)
            {
                positionTextView.Text = products[position].Distance;
            }
            else
            {
                distanceLayout.Visibility = ViewStates.Gone;
            }

            string imageUrl = "";
            if (!string.IsNullOrEmpty(products[position].ProdImg))
            {
                imageUrl = products[position].ProdImg;
            }
            else
            {
                //lat.low, long.low, lat.hig
                CategoryModel catImage;
                if (selectedCategory.Count != 0)
                {
                    catImage = selectedCategory.FirstOrDefault(x => products[position].Categories.Any(y => y == x.Id));
                }
                else
                {
                    catImage = categories.FirstOrDefault(x => products[position].Categories.Any(y => y == x.Id));

                }
                if (catImage != null)
                {
                    imageUrl = ContentResolver.SchemeAndroidResource + "://" + context.PackageName + "/drawable/" + catImage.Icon;
                    imageView.SetBackgroundColor(Color.ParseColor(catImage.Color));
                }
            }          
               Picasso.With(context).Load(imageUrl).Resize(60, 60).Into(imageView);
            if (products.Count - 5 == position)
            {
            ThreadPool.QueueUserWorkItem(o => LoadNextData());
               
            }
                return view;
        }
        private async void LoadNextData()
        {
            var conn = Utils.GetDatabaseConnection();
            var selectedSubCategory = conn.GetSubSelectedCategory();
            double lat = NohandicapApplication.MainActivity.CurrentLocation.Latitude;
            double lng = NohandicapApplication.MainActivity.CurrentLocation.Longitude;
            PageNumber++;
            var newProducts= await RestApiService.GetMarkers(NohandicapApplication.SelectedMainCategory, selectedSubCategory, NohandicapApplication.CurrentLang.Id, lat, lng,PageNumber);       
            foreach (var product in newProducts)
            {
                products.Add(product);
                try
                {
                    context.RunOnUiThread(() => {
                    NotifyDataSetChanged();
                    });
                }
                catch(Exception e)
                {

                }
            }
            
        }
    }
}
