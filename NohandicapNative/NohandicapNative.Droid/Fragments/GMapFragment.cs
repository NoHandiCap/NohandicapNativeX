using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;

using Android.App;

using Android.Gms.Common;
using Android.Gms.Maps.Model;
using System.Collections.ObjectModel;
using System.Collections;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using Android.Content.PM;
using NohandicapNative.Droid.Adapters;
using Android.Util;
using Android.Gms.Maps;
using System.Globalization;

namespace NohandicapNative.Droid
{
   public class GMapFragment : Android.Support.V4.App.Fragment, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback
    {
      

        static readonly string TAG = "X:" + typeof(GMapFragment).Name;
        MainActivity myContext;
        LayoutInflater inflater;
      //  MapView mapView;
        GoogleMap map;
        SqliteService dbCon;
        List<Marker> markersList;
        List<ProductModel> products;
        List<MarkerOptions> markerOptons;
      MapFragment mapFragment;
        CategoryModel currentCategory;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.inflater = inflater;
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            view.SetBackgroundColor(myContext.Resources.GetColor(Resource.Color.backgroundColor));
            HasOptionsMenu = true;           


                mapFragment = myContext.FragmentManager.FindFragmentById(Resource.Id.map).JavaCast<MapFragment>();

                mapFragment.GetMapAsync(this);        
            if (products.Count==0)
            SetData(dbCon.GetDataList<ProductModel>());

            // Updates the location and zoom of the MapView
          
            return view;
        }
        public GMapFragment()
        {
            dbCon = Utils.GetDatabaseConnection();
            markersList = new List<Marker>();
            markerOptons = new List<MarkerOptions>();
            products = new List<ProductModel>();
        }
        private void LoadData(bool filter = true)
        {
            try {          
                if (myContext != null&map!=null)
                {                    
                    markersList.Clear();
                    map.Clear();


                var category = dbCon.GetDataList<CategoryModel>();
                    int categorySelected;
                    if (filter)
                    {
                        categorySelected = int.Parse(Utils.ReadFromSettings(myContext, Utils.MAIN_CAT_SELECTED_ID, "1"));
                    }
                    else
                    {
                        categorySelected = 0;
                    }


                products = products.Where(x => x.MainCategoryID >= categorySelected).ToList();
                    products.ForEach(product =>
                    {
                       
                      
                        var pos = new LatLng(double.Parse(product.Lat, CultureInfo.InvariantCulture), double.Parse(product.Long, CultureInfo.InvariantCulture));
                  
                        var title = product.FirmName;
         
                        var options = new MarkerOptions().SetPosition(pos).SetTitle(product.FirmName);
                        string cat;
                        if (currentCategory != null)
                        {
                            cat = currentCategory.Marker;
                        }
                        else
                        {

                            cat = category.FirstOrDefault(y => y.ID == product.Categories[0]).Marker;

                        }
                        var drawImage = Utils.SetDrawableSize(myContext, Utils.GetImage(myContext, cat), 60, 70);
                        var bitmap = Utils.convertDrawableToBitmap(drawImage);
                        options.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
                        markerOptons.Add(options);


                    });     
                
                markerOptons.ForEach(x =>
                    {

                        var marker = map.AddMarker(x);
                        markersList.Add(marker);
                  

                    });

                }
                if (markersList.Count != 0)
                {
                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(double.Parse(products[0].Lat, CultureInfo.InvariantCulture), double.Parse(products[0].Long, CultureInfo.InvariantCulture))).Zoom(10);
                    CameraPosition cameraPosition = builder.Build();
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                    map.AnimateCamera(cameraUpdate);
                    

            }
            }
            catch (Exception e)
            {
                Log.Error(TAG, e.Message," "+e.InnerException+ " "+e.StackTrace);
            }
        }
        public void SetData(List<ProductModel> data,CategoryModel currentCategory=null,bool filter=true)
        {
           markerOptons.Clear();
            products = data;
            this.currentCategory = currentCategory;
        }
       
        public override void OnResume()
        {
            base.OnResume();
           // mapView.OnResume();
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
        

        }
        
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
           
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {
                if (products.Count == 0)
                {
                    products = dbCon.GetDataList<ProductModel>();
                }
                LoadData();

            }
        }
        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.MyLocationEnabled = true;
            map.UiSettings.MapToolbarEnabled = true;
            map.UiSettings.ZoomControlsEnabled = true;
            map.SetInfoWindowAdapter(this);
            map.InfoWindowClick += (s, e) =>
            {
                var product = FindProductFromMarker(e.Marker);
                var activity = new Intent(myContext, typeof(DetailActivity));
                activity.PutExtra(Utils.PRODUCT_ID, product.ID);
                myContext.StartActivity(activity);
            };
            LoadData();
        }
        public View GetInfoContents(Marker marker)
        {
            var info = inflater.Inflate(Resource.Layout.infoWindow, null);
            var product = FindProductFromMarker(marker);
            var imageView = info.FindViewById<ImageView>(Resource.Id.info_mainImageView);
            var title= info.FindViewById<TextView>(Resource.Id.info_titleTextView);
            var adress = info.FindViewById<TextView>(Resource.Id.info_adressTextView);          
            var mainimage = product.ImageCollection.Images;
            if (mainimage.Count != 0)
            {
                var img = mainimage[0];
                try
                {
                    if (string.IsNullOrWhiteSpace(img.LocalImage))
                    {
                        LoadImageAsync(imageView, img,product);
                    }
                    else
                    {
                        imageView.SetImageBitmap(Utils.GetBitmap(img.LocalImage));

                    }

                }
                catch (Exception e)
                {
                    Log.Error(TAG, e.Message);
                }
            }
           
            title.Text = product.FirmName;
            adress.Text = product.Adress;
         
            return info;
        }
        public async void LoadImageAsync(ImageView imageView, ImageModel img,ProductModel product)
        {
            imageView.SetBackgroundResource(Resource.Drawable.placeholder);
            string filename = "none";
            Uri uri = new Uri(img.LinkImage);
            filename = System.IO.Path.GetFileName(uri.LocalPath);
            var bitmap = await Utils.SaveImageBitmapFromUrl(img.LinkImage, filename);
            img.LocalImage = filename;
            dbCon.InsertUpdateProduct(product);
            imageView.SetImageBitmap(bitmap);

        }
        #region InfoWindowAdapter
        public View GetInfoWindow(Marker marker)
        {
            return null;
        }
        private ProductModel FindProductFromMarker (Marker marker)
        {
            return products.FirstOrDefault(x => x.FirmName == marker.Title);
        }
        #endregion
        #region Menu implementation
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.map_menu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    myContext.SetCurrentTab(0);
                    break;
                case Resource.Id.select_all:
                    SetData(dbCon.GetDataList<ProductModel>());
                    myContext.SupportActionBar.Title = "Map";
                    LoadData();
                    break;
            }
            return true;
        }
        public void OnMenuItemSelected(int menuItemId)
        {

        }
        #endregion
    }
}