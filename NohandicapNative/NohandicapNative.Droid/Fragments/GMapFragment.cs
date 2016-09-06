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
using Android.Gms.Maps;
using Android.Gms.Common;
using Android.Gms.Maps.Model;
using System.Collections.ObjectModel;
using System.Collections;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using Android.Content.PM;
using NohandicapNative.Droid.Adapters;
using Android.Util;

namespace NohandicapNative.Droid
{
   public class GMapFragment : Android.Support.V4.App.Fragment, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback
    {
        static readonly string TAG = "X:" + typeof(GMapFragment).Name;
        MainActivity myContext;
        LayoutInflater inflater;
        MapView mapView;
        GoogleMap map;
        SqliteService dbCon;
        List<Marker> markersList;
        List<ProductModel> products;
        List<MarkerOptions> markerOptons;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.inflater = inflater;
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            view.SetBackgroundColor(myContext.Resources.GetColor(Resource.Color.backgroundColor));
            HasOptionsMenu = true;
          
            // Gets the MapView from the XML layout and creates it
            mapView = (MapView)view.FindViewById(Resource.Id.mapView);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);
            // Gets to GoogleMap from the MapView and does initialization stuff
            //map = mapView.Map;
            //if (map != null)
            //{
          
            //    map.UiSettings.MyLocationButtonEnabled = true;
            //    map.MyLocationEnabled = true;
            //    map.UiSettings.MapToolbarEnabled = true;
            //    map.UiSettings.ZoomControlsEnabled = true;
            //    map.SetInfoWindowAdapter(this);
               
       
            //}
            //try
            //{
            //    MapsInitializer.Initialize(this.Activity);
            //}
            //catch (GooglePlayServicesNotAvailableException e)
            //{
            //    e.PrintStackTrace();
            //}

            // Needs to call MapsInitializer before doing any CameraUpdateFactory calls
            if(products.Count==0)
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
            try
            {
                if (myContext != null)
                {

                    map.Clear();
                    markersList.Clear();

                    var category = dbCon.GetDataList<CategoryModel>();
                    int categorySelected;
                    if (filter)
                    {
                        categorySelected = int.Parse(Utils.ReadFromSettings(myContext, Utils.MAIN_CAT_SELECTED_ID, "0"));
                    }
                    else
                    {
                        categorySelected = 0;
                    }
                    products = products.Where(x => x.MainCategoryID >= categorySelected).ToList();
                    products.ForEach(product =>
                    {
                        var options = new MarkerOptions().SetPosition(new LatLng(double.Parse(product.Lat), double.Parse(product.Long))).SetTitle(product.FirmName);
                        var cat = category.FirstOrDefault(y => y.ID == product.Categories[0]).Marker;
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
                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(markersList[0].Position, 11);
                    map.AnimateCamera(cameraUpdate);
                }
            }catch(Exception e)
            {
                Log.Debug(TAG, e.Message);
            }
        }
        public void SetData(List<ProductModel> data,bool filter=true)
        {
           markerOptons.Clear();
            products = data;
            LoadData();
        }
       
        public override void OnResume()
        {
            base.OnResume();
            mapView.OnResume();
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            //var _myMapFragment = FragmentManager.FindFragmentByTag("map") as SupportMapFragment;
            //if (_myMapFragment == null)
            //{
            //    GoogleMapOptions mapOptions = new GoogleMapOptions()
            //        .InvokeMapType(GoogleMap.MapTypeNormal)
            //        .InvokeZoomControlsEnabled(true)
            //        .InvokeAmbientEnabled(true)
            //        .InvokeCompassEnabled(true);

            //    Android.Support.V4.App.FragmentTransaction fragTx = FragmentManager.BeginTransaction();
            //    _myMapFragment = SupportMapFragment.NewInstance(mapOptions);
               
            //    fragTx.Add(Resource.Id.map, _myMapFragment, "map");
            //    fragTx.Commit();
            //    _myMapFragment.
           // }

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
            if (!hidden && markersList.Count == 0)
            {

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
            LoadData();
        }
        public View GetInfoContents(Marker marker)
        {
            var info = inflater.Inflate(Resource.Layout.infoWindow, null);
            var product = FindProductFromMarker(marker);
            var img = info.FindViewById<ImageView>(Resource.Id.info_mainImageView);
            var title= info.FindViewById<TextView>(Resource.Id.info_titleTextView);
            var adress = info.FindViewById<TextView>(Resource.Id.info_adressTextView);
            var button = info.FindViewById<TextView>(Resource.Id.info_detailButton);
            var mainimage = product.ImageCollection.Images;
            if (mainimage.Count != 0)
            {
                //  image.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));
                img.SetImageDrawable(new Android.Graphics.Drawables.BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));
            }
            title.Text = product.FirmName;
            adress.Text = product.Adress;
            button.Click += (s, e) => {

            };
            return info;
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