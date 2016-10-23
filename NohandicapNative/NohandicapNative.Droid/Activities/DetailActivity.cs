using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Util;
using System.Globalization;
using System.Threading.Tasks;
using static Android.Views.View;

namespace NohandicapNative.Droid
{
    [Activity(Label = "DetailActivity", ParentActivity = typeof(MainActivity))]
    public class DetailActivity : AppCompatActivity, IOnMapReadyCallback,IOnClickListener
       
    {
        static readonly string TAG = "X:" + typeof(DetailActivity).Name;
        Android.Support.V7.Widget.Toolbar toolbar;
        List<CategoryModel> categories;
        UserModel user;
        ProductDetailModel product;       
        TextView descriptionTextView;
        TextView adressTextView;
        TextView ortTextView;
        TextView plzTextView;
        TextView phoneTextView;
        TextView emailTextView;
        TextView linkTextView;
        TextView bookingTextView;
        TextView openHoursLabel;
        TextView openHoursTextView;
        TextView categoriesTitleTextView;
        TextView categoriesTextView;
        ImageView mapImageView;
        GoogleMap map;
        MapView mapView;
        FloatingActionButton fab;
        ViewPager viewPager;
        SqliteService conn;
        // MapView mapView;


        public DetailActivity()
        {
            Utils.UpdateConfig(this);
        }
        protected async override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DetailPage);

            conn = Utils.GetDatabaseConnection();
                Window.DecorView.SetBackgroundColor(Color.White);
            mapView = FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(bundle);
            mapView.OnResume();
            mapView.GetMapAsync(this);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                descriptionTextView = (TextView)FindViewById(Resource.Id.descriptionTextView);
                adressTextView = (TextView)FindViewById(Resource.Id.adressTextView);
            ortTextView = (TextView)FindViewById(Resource.Id.ortTextView);
            plzTextView = (TextView)FindViewById(Resource.Id.plzTextView);
            phoneTextView = (TextView)FindViewById(Resource.Id.phoneTextView);
                emailTextView = (TextView)FindViewById(Resource.Id.emailTextView);
                linkTextView = (TextView)FindViewById(Resource.Id.linkTextView);
                bookingTextView = (TextView)FindViewById(Resource.Id.bookingTextView);
                phoneTextView = (TextView)FindViewById(Resource.Id.phoneTextView);
                categoriesTextView = (TextView)FindViewById(Resource.Id.categoriesTextView);
            openHoursLabel = (TextView)FindViewById(Resource.Id.openHoursLabel);
            openHoursTextView = (TextView)FindViewById(Resource.Id.openHoursTextView);
                viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
                SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.themeColor)));
            SupportActionBar.Title ="";
            fab.SetOnClickListener(this);
                await LoadProduct();
             



        }
        private async Task LoadProduct()
        {
            try
            {
                var productId = Intent.GetIntExtra(Utils.PRODUCT_ID, -1);
                if (NohandicapApplication.IsInternetConnection)
                {
                    product = await RestApiService.GetProductDetail(productId, NohandicapApplication.CurrentLang.Id);
                }
                else
                {
                    product = conn.GetDataList<ProductDetailModel>(x => x.ID == productId).FirstOrDefault();
                }
                SupportActionBar.Title = product.FirmName;

                if (product.ImageCollection.Images == null || product.ImageCollection.Images.Count == 0)
                {
                    viewPager.Visibility = ViewStates.Gone;
                }
                else
                {
                    SliderAdapter adapter = new SliderAdapter(this, product);
                    viewPager.Adapter = adapter;
                }
                categories = conn.GetDataList<CategoryModel>();
                Drawable icon = Utils.GetImage(this, categories.FirstOrDefault(x => x.Id == product.Categories[0]).Icon);
                SupportActionBar.SetIcon(Utils.SetDrawableSize(this, icon, 70, 70));

                descriptionTextView.TextFormatted = Html.FromHtml(product.Description);
                adressTextView.Text = product.Adress;
                ortTextView.Text = product.Ort;
                plzTextView.Text = product.Plz;
                phoneTextView.Text = product.Telefon.Replace(" ", "");
                emailTextView.Text = product.Email;
                linkTextView.Text = product.HomePage;

                if (!string.IsNullOrEmpty(product.BookingPage)) { 
                    string bookingLink = string.Format("<a href='{0}'> "+Resource.String.bookingcom+ " </a>", product.BookingPage);
                    bookingTextView.TextFormatted = Html.FromHtml(bookingLink);
                    bookingTextView.Visibility = ViewStates.Visible;
                }
                if (!string.IsNullOrEmpty(product.OpenTime))
                    openHoursTextView.TextFormatted = Html.FromHtml(product.OpenTime);
                else
                    openHoursLabel.Visibility = ViewStates.Gone;

                string bulledList = "";
                product.Categories.ForEach(x =>
                {
                    bulledList += "&#8226;" + categories.FirstOrDefault(y => y.Id == x).Name + "<br/>";
                });
                categoriesTextView.TextFormatted = Html.FromHtml(bulledList);
                user = conn.GetDataList<UserModel>().FirstOrDefault();
                if (user != null)
                {
                    if (user.Favorites == null)
                    {
                        user.Favorites = new List<int>();
                    }
                    var userFavorites = user.Favorites;
                    if (userFavorites.Any(x => x == product.ID))
                    {
                        fab.SetImageResource(Resource.Drawable.filled_star);
                        fab.Selected = true;
                    }

                }
                RunOnUiThread(async () =>
                {
                    HideEmptyTextView();
                    var options = new MarkerOptions().SetPosition(new LatLng(double.Parse(product.Lat, CultureInfo.InvariantCulture), double.Parse(product.Long, CultureInfo.InvariantCulture))).SetTitle(product.FirmName);
                    if (string.IsNullOrEmpty(product.ProductMarkerImg))
                    {

                        var cat = categories.FirstOrDefault(y => y.Id == product.Categories[0]).Marker;
                        var drawImage = Utils.SetDrawableSize(this, Utils.GetImage(this, cat), 70, 80);
                        var bitmap = Utils.convertDrawableToBitmap(drawImage);
                        options.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
                    }
                    else
                    {
                        var markerImg = await Utils.LoadBitmapAsync(product.ProductMarkerImg);
                        options.SetIcon(BitmapDescriptorFactory.FromBitmap(Bitmap.CreateScaledBitmap(markerImg, markerImg.Width + 20, markerImg.Height + 20, true)));
                    }
                    if (map == null) return;
                    map.AddMarker(options);
                    CameraPosition cameraPosition = new CameraPosition.Builder().Target(options.Position).Zoom(14.0f).Build();
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    map.MoveCamera(cameraUpdate);
                });
                conn.InsertUpdateProduct(product);
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
            CheckTextView(adressTextView);
            CheckTextView(ortTextView);
            CheckTextView(plzTextView);
            CheckTextView(phoneTextView);
            CheckTextView(emailTextView);
            CheckTextView(linkTextView);
            CheckTextView(bookingTextView);
            CheckTextView(openHoursTextView);          
        }
        private static void CheckTextView(TextView textView)
        {
            if (string.IsNullOrEmpty(textView.Text))
            {
                textView.Visibility = ViewStates.Gone;
            }
            else
            {
                textView.Visibility = ViewStates.Visible;
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
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


        public async void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                map = googleMap;
                map.UiSettings.ScrollGesturesEnabled = false;               
                View toolbar = ((View)mapView.FindViewById(int.Parse("1")).
            Parent).FindViewById(int.Parse("4"));

                // and next place it, for example, on bottom right (as Google Maps app)
                RelativeLayout.LayoutParams rlp = (RelativeLayout.LayoutParams)toolbar.LayoutParameters;
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
        private void GoogleMap_MapClick(object sender, object e)
        {
            Intent intent = new Intent();
            intent.PutExtra(Utils.PRODUCT_ID, product.ID);
            SetResult(Result.Ok, intent);
            Finish();
        }

        public void OnClick(View v)
        {
            if (user != null)
            {
               
                if (!fab.Selected)
                {
                    fab.SetImageResource(Resource.Drawable.filled_star);
                    fab.Selected = true;
                    user.Favorites.Add(product.ID);
                    conn.InsertUpdateProduct(user);
                    var url = String.Format(NohandicapLibrary.LINK_SAVEFAV, user.Id, product.ID);
                    RestApiService.GetDataFromUrl<UserModel>(url, readBack: false);
                }
                else
                {
                    fab.SetImageResource(Resource.Drawable.empty_star);
                    fab.Selected = false;
                    user.Favorites.Remove(product.ID);
                    conn.InsertUpdateProduct(user);
                    NohandicapApplication.MainActivity.Favorites.ReloadData();
                    var url = String.Format(NohandicapLibrary.LINK_DELFAV, user.Id, product.ID);
                    RestApiService.GetDataFromUrl<UserModel>(url, readBack: false);
                }
                
            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.please_login), ToastLength.Short).Show();
            }
        }
    }

}