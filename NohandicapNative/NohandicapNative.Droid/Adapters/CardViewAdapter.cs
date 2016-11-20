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
using NohandicapNative.Droid.Activities;
using NohandicapNative.Droid.Fragments;

namespace NohandicapNative.Droid.Adapters
{
    public class CardViewAdapter : BaseAdapter<ProductMarkerModel>
    {
        string TAG = "X: " + typeof (CardView).Name;
        private readonly BaseFragment _baseFragment;
        private readonly List<ProductMarkerModel> _products;
        List<CategoryModel> selectedCategory;
        List<CategoryModel> categories;
        int _pageNumber = 1;
        bool _isFav = false;
        SqliteService conn;
        private MarkerUrlBuilder _markerUrlBuilder;

        public CardViewAdapter(BaseFragment context, bool isFav)
        {

            conn = Utils.GetDatabaseConnection();
            _baseFragment = context;
            selectedCategory = conn.GetSubSelectedCategory();
            _markerUrlBuilder = new MarkerUrlBuilder
            {
                LanguageId = _baseFragment.CurrentLang.Id,
                MainCategoryId = BaseFragment.SelectedMainCategory.Id,
                SubCategoriesList = selectedCategory.Select(x => x.Id).ToList()
            };
            categories = conn.GetDataList<CategoryModel>();
            _products = new List<ProductMarkerModel>();
            this._isFav = isFav;
            if (!isFav)
            {
                var latLngBounds = _baseFragment.MainActivity.MapPage.LatLngBounds;
                var inBounds = _baseFragment.MainActivity.MapPage.ProductsInBounds
                    .Where(x => latLngBounds.Contains(new LatLng(
                        double.Parse(x.Lat, CultureInfo.InvariantCulture),
                        double.Parse(x.Lng, CultureInfo.InvariantCulture))
                        ));
                _products = new List<ProductMarkerModel>(Utils.SortProductsByDistance(inBounds));
            }
            _baseFragment.ShowSpinner(_products.Count == 0);
            StartLoad();

        }

        private async void StartLoad()
        {
            if (await LoadNextData())
            {
                _baseFragment.ShowSpinner(false);
            }
        }

        public override ProductMarkerModel this[int position] => _products[position];

        public override int Count => _products.Count;

        public override long GetItemId(int position) => _products[position].Id;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
// view = context.LayoutInflater.Inflate(Resource.Layout.list_item, parent, false);
                view = _baseFragment.MainActivity.LayoutInflater.Inflate(Resource.Layout.list_item_first, parent, false);
                view.SetBackgroundColor(Color.White);
            }

            var imageView = view.FindViewById<ImageView>(Resource.Id.mainImageView);
            var title = view.FindViewById<TextView>(Resource.Id.titleTextView);
            var adress = view.FindViewById<TextView>(Resource.Id.adressTextView);
            var positionTextView = view.FindViewById<TextView>(Resource.Id.positionTextView);
            var distanceLayout = view.FindViewById<LinearLayout>(Resource.Id.distanceLayout);
            title.Text = _products[position].Name;
            adress.Text = _products[position].Address;

            if (_baseFragment.MainActivity.CurrentLocation != null
                && NohandicapApplication.CheckIfGPSenabled()
                && !string.IsNullOrEmpty(_products[position].Distance))
            {
                positionTextView.Text = _products[position].Distance + " km";
            }
            else
            {
                distanceLayout.Visibility = ViewStates.Gone;
            }

            string imageUrl = "";
            if (!string.IsNullOrEmpty(_products[position].ProdImg))
            {
                imageUrl = _products[position].ProdImg;
            }
            else
            {
                //lat.low, long.low, lat.hig
                CategoryModel catImage;
                if (selectedCategory.Count != 0)
                {
                    catImage = selectedCategory.FirstOrDefault(x => _products[position].Categories.Any(y => y == x.Id));
                }
                else
                {
                    catImage = selectedCategory.FirstOrDefault(x => _products[position].Categories.Any(y => y == x.Id));

                }
                if (catImage != null)
                {
                    imageUrl = Utils.RESOURCE_PATH + catImage.Icon;
                    imageView.SetBackgroundColor(Color.ParseColor(catImage.Color));
                }
            }
            Picasso.With(_baseFragment.MainActivity).Load(imageUrl).Resize(60, 60).CenterInside().Into(imageView);
            if (_products.Count - 5 == position)
            {
                ThreadPool.QueueUserWorkItem(async o =>await LoadNextData());

            }
            return view;
        }

        private async Task<bool> LoadNextData()
        {
            var conn = Utils.GetDatabaseConnection();


            IEnumerable<ProductMarkerModel> newProducts;


            if (_isFav)
            {
                var user = conn.GetDataList<UserModel>().FirstOrDefault();
                if (user == null) return true;
                newProducts = await RestApiService.GetFavorites(user.Id, _pageNumber);
                newProducts = Utils.SortProductsByDistance(newProducts);
            }
            else
            {
                var latLngBounds = _baseFragment.MainActivity.MapPage.LatLngBounds;
                if (latLngBounds != null)
                {
                    if (_baseFragment.MainActivity.MapPage.ProductsInBounds.Count > 50)
                    {
                        _pageNumber++;
                    }
                    _markerUrlBuilder.SetBounds(latLngBounds.Southwest.Latitude, latLngBounds.Southwest.Longitude,
                        latLngBounds.Northeast.Latitude, latLngBounds.Northeast.Longitude);
                    _markerUrlBuilder.PageNumber = _pageNumber;

                }
                var position = _baseFragment.MainActivity.CurrentLocation;
                if (position != null)
                {
                    _markerUrlBuilder.SetMyLocation(position.Latitude, position.Longitude);
                }
                newProducts = await _markerUrlBuilder.LoadDataAsync();
                if (position == null)
                {
                    newProducts = newProducts.OrderBy(x => x.Name);
                }

            }

            _pageNumber++;

            foreach (var product in newProducts)
            {
                if (_products.All(x => x.Id != product.Id))
                {
                    //   _baseFragment.MainActivity.CurrentProductsList.Add(product);
                    _baseFragment.MainActivity.RunOnUiThread(() =>
                    {
                        _products.Add(product);
                        NotifyDataSetChanged();
                    });
                }
            }
            return true;

        }
    }
}
