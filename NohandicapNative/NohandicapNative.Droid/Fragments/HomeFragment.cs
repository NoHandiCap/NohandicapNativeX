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
using NohandicapNative.Droid.Fragments;

using System.Threading;

namespace NohandicapNative.Droid
{
  public  class HomeFragment: BaseFragment, IOnMenuTabSelectedListener
    {
        int[] mainCategoriesText = { Resource.Id.first_category, Resource.Id.second_category, Resource.Id.thrity_category };
       int[] mainCategoriesImgView = { Resource.Id.imageView, Resource.Id.imageView2, Resource.Id.imageView3 };
       int[] mainCategoriesLayout= { Resource.Id.category_linearLayout, Resource.Id.category_linearLayout3, Resource.Id.category_linearLayout2 };
      
        ButtonGridView additionalCategory;     
        View rootView;
        LayoutInflater inflater;
        ViewGroup container;
        List<CategoryModel> subCategoriesList;
        List<CategoryModel> mainCategoriesList;

        GridViewAdapter buttonsAdapter;

        public HomeFragment(Boolean loadFromCache = true) : base(loadFromCache) { }

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
           
            mainCategoriesList = DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.MainCatGroup);
            subCategoriesList = DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup);

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

                            }
                        }
                    }
                   MainActivity.MapPage.LoadData();
                };

            }

            var categorySelected = SelectedMainCategory.Id;

            mainCat[categorySelected - 1].SetTextColor(Color.Black);
            mainCat[categorySelected - 1].SetTypeface(null, TypefaceStyle.Bold);
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
                        Group = cat.Group,
                        Sort = i + 1
                    });
                }
            }
            subCategoriesList = subCategoriesList.OrderBy(x => x.Sort).ToList();
           
            buttonsAdapter= new GridViewAdapter(MainActivity, subCategoriesList);
            additionalCategory.Adapter = buttonsAdapter;
            additionalCategory.ItemClick += SubCategory_ItemClick;
            //ThreadPool.QueueUserWorkItem(o => LoadCache());

        }

        private async void SubCategory_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var position = e.Position;
            var category = subCategoriesList[position];
           // OnSubCategoryChaged(new List<CategoryModel> { category });
            if (!IsTablet)
            {
                MainActivity.MapPage.SetData(new List<CategoryModel> { category });
                MainActivity.SetCurrentTab(1);
                MainActivity.SupportActionBar.Title = category.Name;
                DbConnection.SetSelectedCategory(category);               
            }
            else
            {
                DbConnection.SetSelectedCategory(category);              
                buttonsAdapter.NotifyDataSetChanged();
                
                var selectedCategories = DbConnection.GetDataList<CategoryModel>(x => x.IsSelected).ToList();
                if (selectedCategories.Count == 0)
                {

                    MainActivity.MapPage.SetData(subCategoriesList);
                }
                else
                {
                    MainActivity.MapPage.SetData(selectedCategories);

                }

                await MainActivity.MapPage.LoadData();

            }
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
            if (IsTablet)
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
               
                transaction.Replace(Resource.Id.mapFragment, MainActivity.MapPage).Commit();
            
        }      
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);      
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
                 MainActivity.SetCurrentTab(0);
                    break;
                case Resource.Id.settings:
                   MainActivity.StartActivity(typeof(SettingsActivity));
                    break;
            }
            return true;
        }
        public  void OnMenuItemSelected(int menuItemId)
        {

        }


        #endregion

        
        private async void LoadCache()
        {
            try
            {

                var selectedSubCategory = DbConnection.GetSubSelectedCategory();
                var position = NohandicapApplication.MainActivity.CurrentLocation;
                string lat = "";
                string lng = "";
                if (position != null)
                {
                    lat = position.Latitude.ToString();
                    lng = position.Longitude.ToString();
                }

                var coll = await RestApiService.GetMarkers(NohandicapApplication.SelectedMainCategory, selectedSubCategory, NohandicapApplication.CurrentLang.Id, lat, lng, 1);
                AddProductsToCache(coll);

            }
            catch (System.Exception e)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

            }
        }
    }
}