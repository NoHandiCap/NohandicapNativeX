using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using NohandicapNative.Droid.Adapters;
using Android.App;
using Android.Content.Res;
using BottomNavigationBar.Listeners;
using System;
using Android.Util;

namespace NohandicapNative.Droid
{
  public  class HomeFragment: Android.Support.V4.App.Fragment, IOnMenuTabSelectedListener
    {
        int[] mainCategoriesText = { Resource.Id.first_category, Resource.Id.second_category, Resource.Id.thrity_category };
       int[] mainCategoriesImgView = { Resource.Id.imageView, Resource.Id.imageView2, Resource.Id.imageView3 };
       int[] mainCategoriesLayout= { Resource.Id.category_linearLayout, Resource.Id.category_linearLayout3, Resource.Id.category_linearLayout2 };
      
        ButtonGridView additionalCategory;
        ListView mainCategory;
        View rootView;
        LayoutInflater inflater;
        ViewGroup container;
        bool isMapReady = false;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.inflater = inflater;
            this.container = container;
            PopulateViewForOrientation();
            
            return rootView;
        }
        private void PopulateViewForOrientation()
        {

            Log.Debug(Tag, "Start HomeFragment ");
           
                rootView = inflater.Inflate(Resource.Layout.HomePage, null);

           

            rootView.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.backgroundColor));
         
            this.HasOptionsMenu = true;
            
                var conn = Utils.GetDatabaseConnection();
               List<CategoryModel> mainCategoriesList = conn.GetDataList<CategoryModel>().Where(x => x.Group == 2).ToList();
                List<CategoryModel> subCategoriesList = conn.GetSubSelectedCategory();
                conn.Close();
                TextView[] mainCat = new TextView[mainCategoriesList.Count];
            ImageView[] mainImg = new ImageView[mainCategoriesList.Count];
            LinearLayout[] mainLayout = new LinearLayout[mainCategoriesList.Count];   
            for (int i = 0; i < mainCat.Length; i++)
            {
                mainCat[i] = rootView.FindViewById<TextView>(mainCategoriesText[i]);
                mainCat[i].Text = mainCategoriesList[i].Name;
                mainImg[i] = rootView.FindViewById<ImageView>(mainCategoriesImgView[i]);
                mainLayout[i] = rootView.FindViewById<LinearLayout>(mainCategoriesLayout[i]);
            }
            for (int i = 0; i < mainCat.Length; i++)
            {
                mainCat[i].SetTextColor(Color.Gray);
                mainCat[i].SetTypeface(null, TypefaceStyle.Normal);
                mainCat[i].SetBackgroundColor(Color.White);
                mainLayout[i].Selected = false;
                mainLayout[i].Click += (s, e) =>
                {
                    var layout = (LinearLayout)s;
                    if (!layout.Selected)
                    {

                        for (int y = 0; y < mainCat.Length; y++)
                        {
                            if (mainLayout[y] != layout)
                            {
                                mainCat[y].SetTextColor(Color.Gray);
                                mainCat[y].SetTypeface(null, TypefaceStyle.Normal);
                                mainLayout[y].Selected = false;
                            }
                            else
                            {
                                mainCat[y].SetTextColor(Color.Black);
                                mainCat[y].SetTypeface(null, TypefaceStyle.Bold);
                                mainLayout[y].Selected = true;
                                var conn2 = Utils.GetDatabaseConnection();
                                conn2.SetSelectedCategory(mainCategoriesList[y]);
                                conn2.Close();
                            }
                        }
                    }
                    NohandicapApplication.MainActivity.MapPage.LoadData();
                };

            }
          
            var categorySelected = NohandicapApplication.SelectedMainCategory.Id;

            mainCat[categorySelected-1].SetTextColor(Color.Black);
            mainCat[categorySelected-1].SetTypeface(null, TypefaceStyle.Bold);
            mainLayout[categorySelected - 1].Selected = true;
            mainImg[0].SetImageDrawable(Utils.SetDrawableSize(Activity, Resource.Drawable.wheelchair1, 140, 65));
            mainImg[1].SetImageDrawable(Utils.SetDrawableSize(Activity, Resource.Drawable.wheelchair2, 140, 65));
            mainImg[2].SetImageDrawable(Utils.SetDrawableSize(Activity, Resource.Drawable.wheelchair3, 140, 65));

            additionalCategory = rootView.FindViewById<ButtonGridView>(Resource.Id.additionalCategory);
            GridRotation();

           
                if (subCategoriesList.Count == 0)
                {
                    var localCategories = NohandicapLibrary.GetAdditionalCategory();
                    var localCategoriesLocalization = Resources.GetStringArray(Resource.Array.additional_category_array);
                    for (int i = 0; i < localCategories.Count; i++)
                    {
                        var cat = localCategories[i];
                        var loc = localCategoriesLocalization[i];
                        subCategoriesList.Add(new CategoryModel()
                        {
                            Id = cat.Id,
                            Name = loc,
                            Color = cat.Color,
                            Icon = cat.Icon,
                            Sort = i + 1
                        });
                    }
                }
              
         
            //  List<TabItem> mainItems = NohandiLibrary.GetMainCategory();
            //  mainCategory.Adapter = new GridViewAdapter(myContext, mainItems);

            additionalCategory.Adapter = new GridViewAdapter(NohandicapApplication.MainActivity, subCategoriesList.OrderBy(x => x.Sort).ToList());
                //mainCategory.OnItemClickListener = this;
           
        }

        private void GridRotation()
        {
            var orientation = Activity.Resources.Configuration.Orientation;


            if (orientation == Android.Content.Res.Orientation.Portrait)
            {
                additionalCategory.NumColumns = 3;
            }
            else
            {
                additionalCategory.NumColumns = 5;
            }
            if (NohandicapApplication.isTablet)
            {
                if (orientation == Android.Content.Res.Orientation.Landscape)
                {
                    additionalCategory.NumColumns = 3;
                }
                else
                {
                    additionalCategory.NumColumns = 2;
                }
                InitializeMapForTablet();
            }
         
        }
        private void InitializeMapForTablet()
        {
           
                Android.Support.V4.App.FragmentTransaction transaction = ChildFragmentManager.BeginTransaction();
               
                transaction.Replace(Resource.Id.mapFragment, NohandicapApplication.MainActivity.MapPage).Commit();
            
        }      
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            //NohandicapApplication.MainActivity.HomePage = null;
            //var fav = new HomeFragment();
            ////  _myContext.ShowFragment(fav, "fav");
            //Android.Support.V4.App.FragmentManager fragmentManager = myContext.SupportFragmentManager;
            //var trans = fragmentManager.BeginTransaction();
            //trans.Replace(Resource.Id.flContent, fav);
            //trans.Commit();
             GridRotation();
        }
        #region Menu implementation
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {          
            inflater.Inflate(Resource.Menu.main_menu, menu);
            base.OnCreateOptionsMenu(menu, inflater);
        }
       
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                 NohandicapApplication.MainActivity.SetCurrentTab(0);
                    break;
                case Resource.Id.settings:
                    NohandicapApplication.MainActivity.StartActivity(typeof(SettingsActivity));
                    break;
            }
            return true;
        }
        public void OnMenuItemSelected(int menuItemId)
        {

        }
        #endregion

    }
}