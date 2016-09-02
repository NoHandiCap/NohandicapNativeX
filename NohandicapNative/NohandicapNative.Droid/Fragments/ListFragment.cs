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
using NohandicapNative.Droid.Adapters;
using Android.App;
using NohandicapNative.Droid.Services;
using Android.Graphics;

namespace NohandicapNative.Droid
{
  public  class ListFragment : Android.Support.V4.App.Fragment
    {
        MainActivity myContext;
        ListView listView;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
         listView = view.FindViewById<ListView>(Resource.Id.listview);    
          view.SetBackgroundColor(Color.ParseColor(Utils.BACKGROUND));

            listView.ItemClick += (s, e) =>
            {
                int position = e.Position;
                //    var detailIntent = new Intent(myContext, typeof(DetailActivity));
                  //  detailIntent.PutExtra("Title", items[position].FirmName);
             //   myContext.StartActivity(detailIntent);
               
            };
         LoadData();
            return view;
        }
       private async void LoadData()
        {
            var dbCon = Utils.GetDatabaseConnection();
            var product =dbCon.GetDataList<ProductModel>();
            var listAdapter = new CardViewAdapter(myContext, product);

            //var listAdapter = new ListAdapter(myContext, product);
            listView.Adapter = listAdapter;
        }
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
        }
    }
}