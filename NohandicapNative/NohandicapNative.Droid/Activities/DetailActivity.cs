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
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using Android.Support.V4.View;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Gms.Maps;
using Android.Gms.Common;
using Android.Gms.Maps.Model;
using Android.Util;
using System.Globalization;

namespace NohandicapNative.Droid
{
    [Activity(Label = "DetailActivity", ParentActivity = typeof(MainActivity))]
    public class DetailActivity : AppCompatActivity, IOnMapReadyCallback
       
    {
        static readonly string TAG = "X:" + typeof(DetailActivity).Name;
        Android.Support.V7.Widget.Toolbar toolbar;
        List<CategoryModel> categories;

        ProductModel product;
        SqliteService dbCon;
        TextView descriptionTextView;
        TextView adressTextView;
        TextView phoneTextView;
        TextView categoriesTitleTextView;
        TextView categoriesTextView;
        ImageView mapImageView;

       // MapView mapView;
        GoogleMap map;
        MapFragment mapFragment;
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.DetailPage);
            dbCon = Utils.GetDatabaseConnection();
            try
            {


                mapFragment = FragmentManager.FindFragmentById(Resource.Id.map) as MapFragment;

                mapFragment.GetMapAsync(this);
               
                toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
                descriptionTextView = (TextView)FindViewById(Resource.Id.descriptionTextView);
                adressTextView = (TextView)FindViewById(Resource.Id.adressTextView);
                phoneTextView = (TextView)FindViewById(Resource.Id.phoneTextView);
                categoriesTextView = (TextView)FindViewById(Resource.Id.categoriesTextView);         
                SetSupportActionBar(toolbar);
                SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.detailBarColor)));
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);                
                var productId = Intent.GetIntExtra(Utils.PRODUCT_ID, -1);
                product = dbCon.GetDataList<ProductModel>().FirstOrDefault(x => x.ID == productId);
                SupportActionBar.Title = product.FirmName;
                var viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
                if (product.ImageCollection.Images.Count == 0)
                {
                    viewPager.Visibility = ViewStates.Gone;
                }
                else
                {
                    SliderAdapter adapter = new SliderAdapter(this, product);
                    viewPager.Adapter = adapter;
                }
                categories = dbCon.GetDataList<CategoryModel>();
                var icon = Utils.GetImage(this, categories.FirstOrDefault(x => x.ID == product.Categories[0]).Icon);
                SupportActionBar.SetIcon(Utils.SetDrawableSize(this, icon, 70, 70));
                descriptionTextView.TextFormatted = Html.FromHtml(product.Description);
                adressTextView.Text = product.Adress;
                phoneTextView.Text = product.Telefon;
                string bulledList = "";
                product.Categories.ForEach(x =>
                {
                    bulledList += "&#8226;" + categories.FirstOrDefault(y => y.ID == x).Name + "<br/>";
                });
                categoriesTextView.TextFormatted = Html.FromHtml(bulledList);
                var user = dbCon.GetDataList<UserModel>().FirstOrDefault();
                if (user != null)
                {
                    var userFavorites = user.Fravorites;
                    if (userFavorites.Any(x => x == product.ID))
                    {
                        fab.SetImageResource(Resource.Drawable.filled_star);
                        fab.Selected = true;
                    }
                }
                fab.Click += (s, e) =>
                {
                    if (user != null)
                    {
                        if (!fab.Selected)
                        {
                            fab.SetImageResource(Resource.Drawable.filled_star);
                            fab.Selected = true;
                            user.Fravorites.Add(product.ID);
                            dbCon.InsertUpdateProduct(user);
                            var url = String.Format(NohandicapLibrary.LINK_SAVEFAV, user.ID, product.ID);
                            RestApiService.GetDataFromUrl<UserModel>(url, readBack: false);
                        }
                        else
                        {
                            fab.SetImageResource(Resource.Drawable.empty_star);
                            fab.Selected = false;
                            user.Fravorites.Remove(product.ID);
                            dbCon.InsertUpdateProduct(user);
                            ((MainActivity)Utils.mainActivity).Favorites.ReloadData();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, Resources.GetString(Resource.String.please_login), ToastLength.Short).Show();
                    }
                };
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCreate: " + e.Message + " " + e.StackTrace);
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
                    break;           
            }
            return true;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            try { 
               
            var options = new MarkerOptions().SetPosition(new LatLng(double.Parse(product.Lat, CultureInfo.InvariantCulture), double.Parse(product.Long, CultureInfo.InvariantCulture))).SetTitle(product.FirmName);
            var cat = categories.FirstOrDefault(y => y.ID == product.Categories[0]).Marker;
            var drawImage = Utils.SetDrawableSize(this, Utils.GetImage(this, cat), 70, 80);
            var bitmap = Utils.convertDrawableToBitmap(drawImage);
            options.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
            googleMap.AddMarker(options);
                googleMap.UiSettings.ScrollGesturesEnabled=false;
                CameraPosition cameraPosition = new CameraPosition.Builder().Target(options.Position).Zoom(14.0f).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            googleMap.MoveCamera(cameraUpdate);
                
                View toolbar = ((View)mapFragment.View.FindViewById(int.Parse("1")).
            Parent).FindViewById(int.Parse("4"));

                // and next place it, for example, on bottom right (as Google Maps app)
                RelativeLayout.LayoutParams rlp = (RelativeLayout.LayoutParams)toolbar.LayoutParameters;
                // position on right bottom
                rlp.AddRule(LayoutRules.AlignParentTop, 0);
               rlp.AddRule(LayoutRules.AlignParentBottom,(int)LayoutRules.True);  
               rlp.AddRule(LayoutRules.AlignParentLeft,(int)LayoutRules.True);
                rlp.SetMargins(100, 0, 0,30);

            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnMapReady: " + e.Message + " " + e.StackTrace);

            }
        }

        private void GoogleMap_MapClick(object sender, object e)
        {
            Intent intent = new Intent();
            intent.PutExtra(Utils.PRODUCT_ID, product.ID);
            SetResult(Result.Ok, intent);
            Finish();
        }
    }

}