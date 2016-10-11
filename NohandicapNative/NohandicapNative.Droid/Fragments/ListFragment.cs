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
        ListView listView;
        List<ProductModel> Products;     
        CardViewAdapter cardViewAdapter;
        TextView mainCategoryName;
        ImageView mainCategoryImage;
        TextView subCategoryName;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
            listView = view.FindViewById<ListView>(Resource.Id.listview);
            view.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.backgroundColor));
            mainCategoryName = view.FindViewById<TextView>(Resource.Id.mainCategoryTextView);
            mainCategoryImage = view.FindViewById<ImageView>(Resource.Id.mainCategoryImageView);
            subCategoryName = view.FindViewById<TextView>(Resource.Id.subCategoryTextView);        

            listView.ItemClick += (s, e) =>
            {
                int position = e.Position;

                var activity = new Intent(Activity, typeof(DetailActivity));
                activity.PutExtra(Utils.PRODUCT_ID, Products[position].ID);
                NohandicapApplication.MainActivity.StartActivityForResult(activity,1);         
            };
            ReloadData();
            return view;
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
            var conn = Utils.GetDatabaseConnection();
            var category = conn.GetDataList<CategoryModel>();

            var selectedSubCategory = conn.GetSubSelectedCategory();
            Products = conn.GetDataList<ProductModel>().Where(x => x.MainCategoryID >= NohandicapApplication.SelectedMainCategory.Id).ToList();
            conn.Close();

            if (selectedSubCategory.Count != 0)
            {
                Products = Products.Where(x => x.Categories.Any(y => selectedSubCategory.Any(z => z.Id == y))).ToList();
                var categories = "";
                selectedSubCategory.ForEach(x => categories += x.Name+",");
                categories.Remove(categories.Length - 1);
                subCategoryName.Text = categories;
            }
            else
            {
                subCategoryName.Text = Resources.GetString(Resource.String.all_cat);
            }

            mainCategoryName.Text = NohandicapApplication.SelectedMainCategory.Name;         
            var image = Utils.GetImage(Activity, "wheelchair" + NohandicapApplication.SelectedMainCategory.Id);
            mainCategoryImage.SetImageDrawable(Utils.SetDrawableSize(Activity, image, 140, 65));

            Products = SortProductsByDistance(Products);
            cardViewAdapter = new CardViewAdapter(Activity, Products);
            listView.Adapter = cardViewAdapter;   
        }

        public List<ProductModel> SortProductsByDistance(List<ProductModel> products)
        {
            var myLocation = NohandicapApplication.MainActivity.CurrentLocation;
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

       
    }
}