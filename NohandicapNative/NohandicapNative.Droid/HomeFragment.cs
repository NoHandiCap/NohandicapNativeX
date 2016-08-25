using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.App;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using NohandicapNative.Droid.Adapters;
using Android.App;

namespace NohandicapNative.Droid
{
  public  class HomeFragment: Android.Support.V4.App.Fragment
    {
        MainActivity myContext;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.HomePage,container,false);
            view.SetBackgroundColor(Color.White);
            GridView mainCategory = view.FindViewById<GridView>(Resource.Id.mainCategory);
            GridView additionalCategory = view.FindViewById<GridView>(Resource.Id.additionalCategory);
            List<TabItem> additItems = NohandiLibrary.GetAdditionalCategory();
            List<TabItem> mainItems = NohandiLibrary.GetMainCategory();
            mainCategory.Adapter = new GridViewAdapter(myContext, mainItems);
          additionalCategory.Adapter= new GridViewAdapter(myContext, additItems);
            
            return view;
        }
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
        }
    }
}