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
using System.Globalization;
using Android.Gms.Maps.Model;
using NohandicapNative.Droid.Fragments;

namespace NohandicapNative.Droid.Adapters
{
    public class CardViewAdapter : BaseAdapter<ProductMarkerModel>
    {
        string TAG = "X: " + typeof (CardView).Name;
        private readonly BaseFragment baseFragment;
        private readonly List<ProductMarkerModel> products;
        List<CategoryModel> selectedCategory;
        List<CategoryModel> categories;
        int PageNumber = 1;
        bool isFav = false;
        SqliteService conn;
        private MarkerUrlBuilder markerUrlBuilder;

        public CardViewAdapter(BaseFragment context, bool isFav)
        {

            conn = Utils.GetDatabaseConnection();
            baseFragment = context;
            selectedCategory = conn.GetSubSelectedCategory();
            markerUrlBuilder = new MarkerUrlBuilder();
            markerUrlBuilder.LanguageId = baseFragment.CurrentLang.Id;
            markerUrlBuilder.MainCategoryId = baseFragment.SelectedMainCategory.Id;
            markerUrlBuilder.SubCategoriesList = selectedCategory.Select(x => x.Id).ToList();
            categories = conn.GetDataList<CategoryModel>();
            products = new List<ProductMarkerModel>();
            this.isFav = isFav;
            if (!isFav)
            {
                var LatLngBounds = baseFragment.MainActivity.MapPage.LatLngBounds;
                var inBounds = baseFragment.MainActivity.MapPage.ProductsInBounds
                    .Where(x => LatLngBounds.Contains(new LatLng(
                        double.Parse(x.Lat, CultureInfo.InvariantCulture),
                        double.Parse(x.Lng, CultureInfo.InvariantCulture))
                        ));
                products = new List<ProductMarkerModel>(Utils.SortProductsByDistance(inBounds));
            }
            baseFragment.ShowSpinner(products.Count == 0);
            StartLoad();

        }

        private async void StartLoad()
        {
            if (await LoadNextData())
            {
                baseFragment.ShowSpinner(false);
            }
        }

        public override ProductMarkerModel this[int position]
        {
            get { return products[position]; }
        }

        public override int Count
        {
            get { return products.Count; }
        }

        public override long GetItemId(int position)
        {
            return products[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
// view = context.LayoutInflater.Inflate(Resource.Layout.list_item, parent, false);
                view = baseFragment.MainActivity.LayoutInflater.Inflate(Resource.Layout.list_item_first, parent, false);
                view.SetBackgroundColor(Color.White);
            }

            var imageView = view.FindViewById<ImageView>(Resource.Id.mainImageView);
            var title = view.FindViewById<TextView>(Resource.Id.titleTextView);
            var adress = view.FindViewById<TextView>(Resource.Id.adressTextView);
            var positionTextView = view.FindViewById<TextView>(Resource.Id.positionTextView);
            var distanceLayout = view.FindViewById<LinearLayout>(Resource.Id.distanceLayout);
            title.Text = products[position].Name;
            adress.Text = products[position].Address;

            if (baseFragment.MainActivity.CurrentLocation != null
                && NohandicapApplication.CheckIfGPSenabled()
                && !string.IsNullOrEmpty(products[position].Distance))
            {
                positionTextView.Text = products[position].Distance + " km";
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
                    catImage = selectedCategory.FirstOrDefault(x => products[position].Categories.Any(y => y == x.Id));

                }
                if (catImage != null)
                {
                    imageUrl = Utils.RESOURCE_PATH + catImage.Icon;
                    imageView.SetBackgroundColor(Color.ParseColor(catImage.Color));
                }
            }
            Picasso.With(baseFragment.MainActivity).Load(imageUrl).Resize(60, 60).CenterInside().Into(imageView);
            if (products.Count - 5 == position)
            {
                ThreadPool.QueueUserWorkItem(o => LoadNextData());

            }
            return view;
        }

        private async Task<bool> LoadNextData()
        {
            var conn = Utils.GetDatabaseConnection();


            IEnumerable<ProductMarkerModel> newProducts;


            if (isFav)
            {
                var user = conn.GetDataList<UserModel>().FirstOrDefault();
                if (user == null) return true;
                newProducts = await RestApiService.GetFavorites(user.Id, PageNumber);
                newProducts = Utils.SortProductsByDistance(newProducts);
            }
            else
            {
                var latLngBounds = baseFragment.MainActivity.MapPage.LatLngBounds;
                if (latLngBounds != null)
                {
                    markerUrlBuilder.SetBounds(latLngBounds.Southwest.Latitude, latLngBounds.Southwest.Longitude,
                        latLngBounds.Northeast.Latitude, latLngBounds.Northeast.Longitude);
                    markerUrlBuilder.PageNumber = PageNumber + 1;

                }
                var position = baseFragment.MainActivity.CurrentLocation;
                if (position != null)
                {
                    markerUrlBuilder.SetMyLocation(position.Latitude, position.Longitude);
                }
                newProducts = await markerUrlBuilder.LoadDataAsync();
                if (position == null)
                {
                    newProducts = newProducts.OrderBy(x => x.Name);
                }

            }

            PageNumber++;

            foreach (var product in newProducts)
            {
                if (!products.Any(x => x.Id == product.Id))
                {
                    //   baseFragment.MainActivity.CurrentProductsList.Add(product);
                    baseFragment.MainActivity.RunOnUiThread(() =>
                    {
                        products.Add(product);
                        NotifyDataSetChanged();
                    });
                }
            }
            return true;

        }
    }
}
