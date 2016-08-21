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

namespace NohandicapNative.Droid
{
    class GMapFragment : Android.Support.V4.App.Fragment
    {
        IEnumerable<MarkerModel> lists;
        private MainActivity myContext;
        MapView mapView;
        GoogleMap map;
        public void SetData(string title)
        {
            myContext.SupportActionBar.Title = title;
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
        
            // Gets the MapView from the XML layout and creates it
            mapView = (MapView)view.FindViewById(Resource.Id.mapView);
            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();
            // Gets to GoogleMap from the MapView and does initialization stuff
            map = mapView.Map;
            map.UiSettings.MyLocationButtonEnabled=true;
            map.MyLocationEnabled=true;
            map.UiSettings.MapToolbarEnabled = true;
            map.UiSettings.ZoomControlsEnabled = true;
           
           
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
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(48.219406, 16.387580), 10);
            map.AnimateCamera(cameraUpdate);


            return view;
        }
        private async void LoadData()
        {
            ObservableCollection<MarkerModel> markers = await RestApiService.GetData<ObservableCollection<MarkerModel>>(null, null, "features");
          
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    string[] coordinat = ((IEnumerable)markers[i].Coordinates.coordinates).Cast<object>()
                                     .Select(x => x.ToString())
                                     .ToArray();
                    LatLng latlng = new LatLng(double.Parse(coordinat[1]), double.Parse(coordinat[0]));
                    MarkerOptions options = new MarkerOptions().SetPosition(latlng).SetTitle(markers[i].Title);
                    map.AddMarker(options);
                }catch(Exception e)
                {

                }
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
    }
}