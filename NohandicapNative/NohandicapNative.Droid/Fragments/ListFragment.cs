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
using Android.Util;
using Android.Locations;
using System.Globalization;

namespace NohandicapNative.Droid
{
  public  class ListFragment : Android.Support.V4.App.Fragment
    {
        public const string PRODUCT_ALL = "allproducts";
        public const string PRODUCT_FAVORITES = "productFavorites";
        int[] mainCategoriesText = { Resource.String.main_cat1, Resource.String.main_cat2, Resource.String.main_cat3};
        public string _currentProductType = PRODUCT_ALL;
        MainActivity myContext;
        ListView listView;
        List<ProductModel> Products;
        SqliteService dbCon;
        CardViewAdapter cardViewAdapter;
        TextView categoryName;
        ImageView categoryImage;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
         listView = view.FindViewById<ListView>(Resource.Id.listview);
            view.SetBackgroundColor(myContext.Resources.GetColor(Resource.Color.backgroundColor));
             categoryName = view.FindViewById<TextView>(Resource.Id.first_categoryTextView);
             categoryImage = view.FindViewById<ImageView>(Resource.Id.categoryImageView);           
            listView.ItemClick += (s, e) =>
            {
                int position = e.Position;

                var activity = new Intent(myContext, typeof(DetailActivity));
                activity.PutExtra(Utils.PRODUCT_ID, Products[position].ID);
                myContext.StartActivityForResult(activity,1);
              
           
               
            };
            ReloadData();
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
                ReloadData();
               
            }
        }
        private async void ReloadData()
        {
            var category = dbCon.GetDataList<CategoryModel>();
            int categorySelectedId = int.Parse(Utils.ReadFromSettings(myContext, Utils.MAIN_CAT_SELECTED_ID, "1"));                    
            Products = dbCon.GetDataList<ProductModel>().Where(x => x.MainCategoryID >= categorySelectedId).ToList();
            categoryName.Text = Resources.GetString(mainCategoriesText[categorySelectedId-1]);
            var image = Utils.GetImage(myContext, "wheelchair" + categorySelectedId);
            categoryImage.SetImageDrawable(Utils.SetDrawableSize(myContext, image, 140, 65));
            Products = SortProductsByDistance(Products);
            cardViewAdapter = new CardViewAdapter(myContext, Products);
            listView.Adapter = cardViewAdapter;

            //var listAdapter = new ListAdapter(myContext, product);


        }
        public List<ProductModel> SortProductsByDistance(List<ProductModel> products)
        {
            var myLocation = myContext.CurrentLocation;
            if (myLocation != null)
            {
             var sorted=   products.Select(product =>
                {
                    var point = new Location("");
                    point.Latitude = double.Parse(product.Lat, CultureInfo.InvariantCulture);
                    point.Longitude = double.Parse(product.Long, CultureInfo.InvariantCulture);
                    var distance = Utils.GetDistance(myLocation, point);
                    product.DistanceToMyLocation = distance;
                    return product;
                }).OrderBy(x => x.DistanceToMyLocation).ToList();
                return sorted;
            }
            return products;
        }
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);     
          

        }
    }
}