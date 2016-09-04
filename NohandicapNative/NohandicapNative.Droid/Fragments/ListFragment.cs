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
        public const string PRODUCT_ALL = "allproducts";
        public const string PRODUCT_FAVORITES = "productFavorites";
        public string _currentProductType = PRODUCT_ALL;
        MainActivity _myContext;
        ListView _listView;
        List<ProductModel> _products;
        SqliteService dbCon;
        CardViewAdapter cardViewAdapter;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
         _listView = view.FindViewById<ListView>(Resource.Id.listview);    
          view.SetBackgroundColor(Color.ParseColor(Utils.BACKGROUND));
           
            _listView.ItemClick += (s, e) =>
            {
                int position = e.Position;
                var detailIntent = new Intent(_myContext, typeof(DetailActivity));
                 detailIntent.PutExtra("Title", _products[position].FirmName);
             _myContext.StartActivity(detailIntent);
               
            };
            LoadData();
            _listView.Adapter = cardViewAdapter;
            return view;
        }
        public ListFragment()
        {
            dbCon = Utils.GetDatabaseConnection();
        }
        public ListFragment(string productType)
        {
            _currentProductType = productType;
            dbCon = Utils.GetDatabaseConnection();
         
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {
                LoadData();
                _listView.Adapter = cardViewAdapter;
            }
        }
        private async void LoadData()
        {
                    _products = dbCon.GetDataList<ProductModel>();
                
                cardViewAdapter = new CardViewAdapter(_myContext, _products);

                //var listAdapter = new ListAdapter(myContext, product);
         
         
        }
        public override void OnAttach(Activity activity)
        {
            _myContext = (MainActivity)activity;
            base.OnAttach(activity);     
          

        }
    }
}