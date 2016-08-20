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

namespace NohandicapNative.Droid
{
    class GMapFragment : Android.Support.V4.App.Fragment
    {
        private MainActivity myContext;
        public void SetData(string title)
        {
            myContext.SupportActionBar.Title = title;
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            
        
            return view;
        }
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            var _myMapFragment = FragmentManager.FindFragmentByTag("map") as SupportMapFragment;
            if (_myMapFragment == null)
            {
                GoogleMapOptions mapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeAmbientEnabled(true)
                    .InvokeCompassEnabled(true);

                Android.Support.V4.App.FragmentTransaction fragTx = FragmentManager.BeginTransaction();
                _myMapFragment = SupportMapFragment.NewInstance(mapOptions);
                fragTx.Add(Resource.Id.map, _myMapFragment, "map");
                fragTx.Commit();
            }

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