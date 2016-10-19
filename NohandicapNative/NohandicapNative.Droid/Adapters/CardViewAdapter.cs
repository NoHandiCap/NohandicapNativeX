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
        bool isFav = false;
        SqliteService conn;
        public CardViewAdapter(Activity context,bool isFav)
        {
            conn = Utils.GetDatabaseConnection();
            this.context = context;
            selectedCategory = conn.GetSubSelectedCategory();
            var filtredProducts = NohandicapApplication.MainActivity.CurrentProductsList.Where(x => x.Categories.Any(y => selectedCategory.Any(z => z.Id == y))).ToList();
            products = new ObservableCollection<ProductMarkerModel>(filtredProducts);
            categories = conn.GetDataList<CategoryModel>();
            this.isFav = isFav;
            if (isFav)
            {
                products = new ObservableCollection<ProductMarkerModel>();
                LoadNextData();
            }
            LoadNextData();
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

            return products[position].Id;
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
                if (selectedCategory.Count !=0)
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
            var position = NohandicapApplication.MainActivity.CurrentLocation;
            string lat = "";
            string lng = "";
            if (position != null)
            {
                lat = position.Latitude.ToString();
                lng = position.Longitude.ToString();
            }
            List<ProductMarkerModel> newProducts;

           
            if (isFav)
            {
                var user = conn.GetDataList<UserModel>().FirstOrDefault();
                if (user == null) return;
                newProducts = await RestApiService.GetFavorites(user.Id, PageNumber);               
            }
            else
            {
                newProducts = await RestApiService.GetMarkers(NohandicapApplication.SelectedMainCategory, selectedSubCategory, NohandicapApplication.CurrentLang.Id, lat, lng, PageNumber);             
            }
            PageNumber++;
            foreach (var product in newProducts)
            {
                if (!products.Any(x=>x.Id==product.Id))
                {
                    products.Add(product);
                    NohandicapApplication.MainActivity.CurrentProductsList.Add(product);

                    context.RunOnUiThread(() =>
                    {
                        NotifyDataSetChanged();
                    });
                }
            }

        }
    }
}
