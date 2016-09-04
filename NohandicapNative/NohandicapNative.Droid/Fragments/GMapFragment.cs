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

namespace NohandicapNative.Droid
{
    class GMapFragment : Android.Support.V4.App.Fragment, GoogleMap.IInfoWindowAdapter
    {
        
        MainActivity myContext;
        LayoutInflater inflater;
        MapView mapView;
        GoogleMap map;
        SqliteService dbCon;
        List<Marker> markersList;
        List<ProductModel> products;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.inflater = inflater;
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
          view.SetBackgroundColor(Color.ParseColor(Utils.BACKGROUND));
            dbCon = Utils.GetDatabaseConnection();
            markersList = new List<Marker>();
            // Gets the MapView from the XML layout and creates it
            mapView = (MapView)view.FindViewById(Resource.Id.mapView);
            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();
            // Gets to GoogleMap from the MapView and does initialization stuff
            map = mapView.Map;
            if (map != null)
            {
          
                map.UiSettings.MyLocationButtonEnabled = true;
                map.MyLocationEnabled = true;
                map.UiSettings.MapToolbarEnabled = true;
                map.UiSettings.ZoomControlsEnabled = true;
                map.SetInfoWindowAdapter(this);
               
       
            }
           

            // Needs to call MapsInitializer before doing any CameraUpdateFactory calls
            try
            {
                MapsInitializer.Initialize(this.Activity);
            }
            catch (GooglePlayServicesNotAvailableException e)
            {
                e.PrintStackTrace();
            }
           LoadData();
            // Updates the location and zoom of the MapView
            
            return view;
        }
        private async void LoadData()
        {
         
             products = dbCon.GetDataList<ProductModel>();
            var category = dbCon.GetDataList<CategoryModel>();
            products.ForEach(product => {
              var options = new MarkerOptions().SetPosition(new LatLng(double.Parse(product.Lat), double.Parse(product.Long))).SetTitle(product.FirmName);
                var cat = category.FirstOrDefault(y => y.ID == product.Categories[0]).Marker;
                var drawImage =Utils.SetDrawableSize(myContext,Utils.GetImage(myContext, cat),35,42);                
                var bitmap = Utils.convertDrawableToBitmap(drawImage);
                options.SetIcon(BitmapDescriptorFactory.FromBitmap(bitmap));
                var marke = map.AddMarker(options);
                
                markersList.Add(map.AddMarker(options));
               
            });
            if (products.Count != 0)
            {
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(markersList[2].Position, 11);
                map.AnimateCamera(cameraUpdate);
            }
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

        public View GetInfoWindow(Marker marker)
        {
            return null;
        }
        private ProductModel FindProductFromMarker (Marker marker)
        {
            return products.FirstOrDefault(x => x.FirmName == marker.Title);
        }
    }
}