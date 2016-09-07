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

namespace NohandicapNative.Droid
{
    [Activity(Label = "DetailActivity", ParentActivity = typeof(MainActivity))]
    public class DetailActivity : AppCompatActivity, IOnMapReadyCallback
    {
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
            mapFragment =FragmentManager.FindFragmentById(Resource.Id.map) as MapFragment;

            mapFragment.GetMapAsync(this);
         
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            descriptionTextView = (TextView)FindViewById(Resource.Id.descriptionTextView);
            adressTextView = (TextView)FindViewById(Resource.Id.adressTextView);
            phoneTextView = (TextView)FindViewById(Resource.Id.phoneTextView);          
            categoriesTextView = (TextView)FindViewById(Resource.Id.categoriesTextView);
            //  mapImageView = (ImageView)FindViewById(Resource.Id.mapImageView);
           
            SetSupportActionBar(toolbar);
       
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.detailBarColor)));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);
            //SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            //SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.colorDefault)));
            //SupportActionBar.Title = Intent.GetStringExtra("Title");

            var productId = Intent.GetIntExtra(Utils.PRODUCT_ID,-1);
            product = dbCon.GetDataList<ProductModel>().FirstOrDefault(x => x.ID == productId);
            SupportActionBar.Title = product.FirmName;
            var viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            SliderAdapter adapter = new SliderAdapter(this,product.ImageCollection.Images);
            viewPager.Adapter = adapter;           
            categories = dbCon.GetDataList<CategoryModel>();
            var icon = Utils.GetImage(this, categories.FirstOrDefault(x => x.ID == product.Categories[0]).Icon);
            SupportActionBar.SetIcon(Utils.SetDrawableSize(this, icon, 70, 70));
            descriptionTextView.TextFormatted = Html.FromHtml(product.Description);
            adressTextView.Text = product.Adress;
            phoneTextView.Text = product.Telefon;
            string bulledList="";
            product.Categories.ForEach(x => {
                bulledList += "&#8226;" + categories.FirstOrDefault(y => y.ID == x).Name + "<br/>";
            });
            categoriesTextView.TextFormatted=Html.FromHtml(bulledList);
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
                        var url = String.Format(NohandiLibrary.LINK_SAVEFAV, user.ID, product.ID);
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
                    Toast.MakeText(this, "Please login", ToastLength.Short).Show();
                }

            };
   
        }    
        private void MapInitialize()
        {
          
            // Gets the MapView from the XML layout and creates it

            //mapView.OnResume();
            //// Gets to GoogleMap from the MapView and does initialization stuff
            //map = mapView.Map;
            //if (map != null)
            //{
            //    map.UiSettings.ScrollGesturesEnabled = false;
            //}
            //try
            //{
            //    MapsInitializer.Initialize(this);
            //}
            //catch (GooglePlayServicesNotAvailableException e)
            //{
            //    e.PrintStackTrace();
            //}
            var options = new MarkerOptions().SetPosition(new LatLng(double.Parse(product.Lat), double.Parse(product.Long))).SetTitle(product.FirmName);
            var cat = categories.FirstOrDefault(y => y.ID == product.Categories[0]).Marker;
            var drawImage = Utils.SetDrawableSize(this, Utils.GetImage(this, cat), 35, 42);
            var bitmap = Utils.convertDrawableToBitmap(drawImage);
            options.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
            map.AddMarker(options);
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

        protected override void OnResume()
        {
          //  mapView.OnResume();
            base.OnResume();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
         //   mapView.OnDestroy();

        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
         //   mapView.OnLowMemory();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var options = new MarkerOptions().SetPosition(new LatLng(double.Parse(product.Lat), double.Parse(product.Long))).SetTitle(product.FirmName);
            var cat = categories.FirstOrDefault(y => y.ID == product.Categories[0]).Marker;
            var drawImage = Utils.SetDrawableSize(this, Utils.GetImage(this, cat), 70, 80);
            var bitmap = Utils.convertDrawableToBitmap(drawImage);
            options.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
            googleMap.AddMarker(options);
            CameraPosition cameraPosition = new CameraPosition.Builder().Target(options.Position).Zoom(15.0f).Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            googleMap.MoveCamera(cameraUpdate);          
            
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