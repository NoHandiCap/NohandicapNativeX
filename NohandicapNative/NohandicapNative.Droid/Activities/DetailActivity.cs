using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;
using static System.String;

namespace NohandicapNative.Droid.Activities
{
    [Activity(Label = "DetailActivity", ParentActivity = typeof(MainActivity))]
    public class DetailActivity : AppCompatActivity, IOnMapReadyCallback,View.IOnClickListener
       
    {
        static readonly string TAG = "X:" + typeof(DetailActivity).Name;
        Android.Support.V7.Widget.Toolbar _toolbar;
        List<CategoryModel> _categories;
        UserModel _user;
        ProductDetailModel _product;       
        TextView _descriptionTextView;
        TextView _adressTextView;
        TextView _ortTextView;
        TextView _plzTextView;
        TextView _phoneTextView;
        TextView _emailTextView;
        TextView _linkTextView;
        TextView _bookingTextView;
        TextView _openHoursLabel;
        TextView _openHoursTextView;
        TextView _categoriesTextView;
        GoogleMap _map;
        MapView _mapView;
        FloatingActionButton _fab;
        ViewPager _viewPager;
        SqliteService _conn;
        // MapView _mapView;


        public DetailActivity()
        {
          
            Utils.UpdateConfig(this);
        }

        protected async override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DetailPage);

            _conn = Utils.GetDatabaseConnection();
                Window.DecorView.SetBackgroundColor(Color.White);
            _mapView = FindViewById<MapView>(Resource.Id.map);
            _mapView.OnCreate(bundle);
            _mapView.OnResume();
            _mapView.GetMapAsync(this);

            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                _descriptionTextView = (TextView)FindViewById(Resource.Id.descriptionTextView);
                _adressTextView = (TextView)FindViewById(Resource.Id.adressTextView);
            _ortTextView = (TextView)FindViewById(Resource.Id.ortTextView);
            _plzTextView = (TextView)FindViewById(Resource.Id.plzTextView);
            _phoneTextView = (TextView)FindViewById(Resource.Id.phoneTextView);
                _emailTextView = (TextView)FindViewById(Resource.Id.emailTextView);
                _linkTextView = (TextView)FindViewById(Resource.Id.linkTextView);
                _bookingTextView = (TextView)FindViewById(Resource.Id.bookingTextView);
                _phoneTextView = (TextView)FindViewById(Resource.Id.phoneTextView);
                _categoriesTextView = (TextView)FindViewById(Resource.Id.categoriesTextView);
            _openHoursLabel = (TextView)FindViewById(Resource.Id.openHoursLabel);
            _openHoursTextView = (TextView)FindViewById(Resource.Id.openHoursTextView);
                _viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
                SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.themeColor)));
            SupportActionBar.Title ="";
            _fab.SetOnClickListener(this);
                await LoadProduct();
             



        }
        private async Task LoadProduct()
        {
            try
            {
                var productId = Intent.GetIntExtra(Utils.PRODUCT_ID, -1);
                if (NohandicapApplication.IsInternetConnection)
                {
                    _product = await RestApiService.GetProductDetail(productId, NohandicapApplication.CurrentLang.Id);
                }
                else
                {
                    _product = _conn.GetDataList<ProductDetailModel>(x => x.ID == productId).FirstOrDefault();
                }
                if (_product != null)
                {
                    SupportActionBar.Title = _product.FirmName;

                    if (_product.ImageCollection.Images == null || _product.ImageCollection.Images.Count == 0)
                    {
                        _viewPager.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        SliderAdapter adapter = new SliderAdapter(this, _product);
                        _viewPager.Adapter = adapter;
                    }
                    _categories = _conn.GetDataList<CategoryModel>();
                    var category = _categories.FirstOrDefault(x => x.Id == _product.Categories[0]);
                    if (category != null)
                    {
                        Drawable icon = Utils.GetImage(this, category.Icon);
                        SupportActionBar.SetIcon(Utils.SetDrawableSize(this, icon, 70, 70));
                    }

                    _descriptionTextView.TextFormatted = Html.FromHtml(_product.Description);
                    _adressTextView.Text = _product.Adress;
                    _ortTextView.Text = _product.Ort;
                    _plzTextView.Text = _product.Plz;
                    _phoneTextView.Text = _product.Telefon.Replace(" ", "");
                    _emailTextView.Text = _product.Email;
                    _linkTextView.Text = _product.HomePage;

                    if (!IsNullOrEmpty(_product.BookingPage)) { 
                        string bookingLink = Format("<a href='{0}'> "+Resource.String.bookingcom+ " </a>", _product.BookingPage);
                        _bookingTextView.TextFormatted = Html.FromHtml(bookingLink);
                        _bookingTextView.Visibility = ViewStates.Visible;
                    }
                    if (!IsNullOrEmpty(_product.OpenTime))
                        _openHoursTextView.TextFormatted = Html.FromHtml(_product.OpenTime);
                    else
                        _openHoursLabel.Visibility = ViewStates.Gone;

                    string bulledList = "";
                    _product.Categories.ForEach(x =>
                    {
                        bulledList += "&#8226;" + _categories.FirstOrDefault(y => y.Id == x).Name + "<br/>";
                    });
                    _categoriesTextView.TextFormatted = Html.FromHtml(bulledList);
                    _user = _conn.GetDataList<UserModel>().FirstOrDefault();
                    if (_user != null)
                    {
                        if (_user.Favorites == null)
                        {
                            _user.Favorites = new List<int>();
                        }
                        var userFavorites = _user.Favorites;
                        if (userFavorites.Any(x => x == _product.ID))
                        {
                            _fab.SetImageResource(Resource.Drawable.filled_star);
                            _fab.Selected = true;
                        }

                    }
                    RunOnUiThread(async () =>
                    {
                        HideEmptyTextView();
                        var options = new MarkerOptions().SetPosition(new LatLng(double.Parse(_product.Lat, CultureInfo.InvariantCulture), double.Parse(_product.Long, CultureInfo.InvariantCulture))).SetTitle(_product.FirmName);
                        if (IsNullOrEmpty(_product.ProductMarkerImg))
                        {
                            var cat = _categories.FirstOrDefault(y => y.Id == _product.Categories[0]).Marker;
                            var drawImage = Utils.SetDrawableSize(this, Utils.GetImage(this, cat), 70, 80);
                            var bitmap = Utils.convertDrawableToBitmap(drawImage);
                            options.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
                        }
                        else
                        {
                            var markerImg = await Utils.LoadBitmapAsync(_product.ProductMarkerImg);
                            options.SetIcon(BitmapDescriptorFactory.FromBitmap(Bitmap.CreateScaledBitmap(markerImg, markerImg.Width + 20, markerImg.Height + 20, true)));
                        }
                        if (_map == null) return;
                        _map.AddMarker(options);
                        var cameraPosition = new CameraPosition.Builder().Target(options.Position).Zoom(14.0f).Build();
                        var cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                        _map.MoveCamera(cameraUpdate);
                    });
                    _conn.InsertUpdateProduct(_product);
                }
            }
            catch (Exception e)
            {

#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

            }
        }
      private void HideEmptyTextView()
        {           
            CheckTextView(_adressTextView);
            CheckTextView(_ortTextView);
            CheckTextView(_plzTextView);
            CheckTextView(_phoneTextView);
            CheckTextView(_emailTextView);
            CheckTextView(_linkTextView);
            CheckTextView(_bookingTextView);
            CheckTextView(_openHoursTextView);          
        }
        private static void CheckTextView(TextView textView)
        {
            textView.Visibility = IsNullOrEmpty(textView.Text) ? ViewStates.Gone : ViewStates.Visible;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.detail_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    GC.Collect();
                    break;           
            }
            return true;
        }


        public void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                _map = googleMap;
                _map.UiSettings.ScrollGesturesEnabled = false;     
                var toolbar = ((View)_mapView.FindViewById(1).Parent).FindViewById(4);

                // and next place it, for example, on bottom right (as Google Maps app)
                var rlp = (RelativeLayout.LayoutParams)toolbar.LayoutParameters;
                // position on right bottom
                rlp.AddRule(LayoutRules.AlignParentTop, 0);
                rlp.AddRule(LayoutRules.AlignParentBottom, (int)LayoutRules.True);
                rlp.AddRule(LayoutRules.AlignParentLeft, (int)LayoutRules.True);
                rlp.SetMargins(100, 0, 0, 30);

            }
            catch (Exception e)
            {

#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                Log.Error(TAG, "OnMapReady: " + e.Message + " " + e.StackTrace);
            }
        }
       
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GC.Collect();
        }
    

        public async void OnClick(View v)
        {
            if (_user != null)
            {
               
                if (!_fab.Selected)
                {
                    _fab.SetImageResource(Resource.Drawable.filled_star);
                    _fab.Selected = true;
                    _user.Favorites.Add(_product.ID);
                    _conn.InsertUpdateProduct(_user);
                    var url = Format(NohandicapLibrary.LINK_SAVEFAV, _user.Id, _product.ID);
               await     RestApiService.GetDataFromUrl<UserModel>(url, readBack: false);
                }
                else
                {
                    _fab.SetImageResource(Resource.Drawable.empty_star);
                    _fab.Selected = false;
                    _user.Favorites.Remove(_product.ID);
                    _conn.InsertUpdateProduct(_user);
                    var url = Format(NohandicapLibrary.LINK_DELFAV, _user.Id, _product.ID);
                  await RestApiService.GetDataFromUrl<UserModel>(url, readBack: false);
                }
                
            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.please_login), ToastLength.Short).Show();
            }
        }
    }

}