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
using System.Collections.ObjectModel;
using NohandicapNative.Droid.Fragments;
using System;

namespace NohandicapNative.Droid
{
  public  class ListFragment : BaseFragment
    {           
        ListView listView;
        CardViewAdapter cardViewAdapter;
        TextView mainCategoryName;
        ImageView mainCategoryImage;
        TextView subCategoryName;
       
        public ListFragment(bool loadFromCache = true) : base(loadFromCache) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
            listView = view.FindViewById<ListView>(Resource.Id.listview);
            view.SetBackgroundColor(Resources.GetColor(Resource.Color.backgroundColor));
            mainCategoryName = view.FindViewById<TextView>(Resource.Id.mainCategoryTextView);
            mainCategoryImage = view.FindViewById<ImageView>(Resource.Id.mainCategoryImageView);
            subCategoryName = view.FindViewById<TextView>(Resource.Id.subCategoryTextView);       
            listView.ItemClick += ListView_ItemClick; 
            ReloadData();
            return view;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {          
            var productId = (int)e.Id;
            var activity = new Intent(Activity, typeof(DetailActivity));
            activity.PutExtra(Utils.PRODUCT_ID, productId);
            MainActivity.StartActivityForResult(activity, 1);
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
            var selectedSubCategory = DbConnection.GetSubSelectedCategory();
            if (selectedSubCategory.Count != 9)
            {
                var categories = "";

                foreach (var item in selectedSubCategory)
                {
                    categories += item.Name + ",";
                }

                categories = categories.Substring(0, categories.Length - 1);

                //selectedSubCategory.ForEach(x => categories += x.Name+",");
                //categories.Remove(categories.Length - 1);
                subCategoryName.Text = categories;
            }
            else
            {
                subCategoryName.Text = Resources.GetString(Resource.String.all_cat);
            }
            mainCategoryName.Text = SelectedMainCategory.Name;         
            var image = Utils.GetImage(Activity, "wheelchair" + SelectedMainCategory.Id);
            mainCategoryImage.SetImageDrawable(Utils.SetDrawableSize(Activity, image, 140, 65));
            cardViewAdapter = new CardViewAdapter(this,false);
            listView.Adapter = cardViewAdapter;   
        }
    }
}