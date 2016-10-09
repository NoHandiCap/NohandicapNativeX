using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Adapters;
using Android.App;
using NohandicapNative.Droid.Services;
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
     
        CardViewAdapter cardViewAdapter;
        TextView mainCategoryName;
        ImageView mainCategoryImage;
        TextView subCategoryName;
        ImageView subCategoryImage;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
            listView = view.FindViewById<ListView>(Resource.Id.listview);
            view.SetBackgroundColor(myContext.Resources.GetColor(Resource.Color.backgroundColor));

            mainCategoryName = view.FindViewById<TextView>(Resource.Id.mainCategoryTextView);
            mainCategoryImage = view.FindViewById<ImageView>(Resource.Id.mainCategoryImageView);

            subCategoryName = view.FindViewById<TextView>(Resource.Id.subCategoryTextView);
            subCategoryImage = view.FindViewById<ImageView>(Resource.Id.subCategoryImageView);

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
         
        }
        public ListFragment(string productType)
        {
            _currentProductType = productType;
           
         
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
            var dbCon = Utils.GetDatabaseConnection();
            var category = dbCon.GetDataList<CategoryModel>();
            int mainCategorySelectedId = int.Parse(Utils.ReadFromSettings(myContext, Utils.MAIN_CAT_SELECTED_ID, "1"));
            var selectedCategory = dbCon.GetDataList<CategoryModel>().Where(x => x.IsSelected).ToList();
            Products = dbCon.GetDataList<ProductModel>().Where(x => x.MainCategoryID >= mainCategorySelectedId).ToList();
            dbCon.Close();

            if (selectedCategory.Count!=0)
            {
                Products = Products.Where(x => x.Categories.Any(y => selectedCategory.Any(z => z.ID == y))).ToList();
            }

            mainCategoryName.Text = Resources.GetString(mainCategoriesText[mainCategorySelectedId-1]);
            var image = Utils.GetImage(myContext, "wheelchair" + mainCategorySelectedId);
            mainCategoryImage.SetImageDrawable(Utils.SetDrawableSize(myContext, image, 140, 65));

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
             var sorted = products.Select(product =>
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