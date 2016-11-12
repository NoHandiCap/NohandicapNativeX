using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using BottomNavigationBar.Listeners;
using NohandicapNative.Droid.Activities;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Model;
using NohandicapNative.Droid.Services;
using Square.Picasso;

namespace NohandicapNative.Droid.Fragments
{
  public  class HomeFragment: BaseFragment, IOnMenuTabSelectedListener
    {
      readonly int[] _mainCategoriesText = { Resource.Id.first_category, Resource.Id.second_category, Resource.Id.third_category };
      readonly int[] _mainCategoriesImgView = { Resource.Id.imageView, Resource.Id.imageView2, Resource.Id.imageView3 };
      readonly int[] _mainCategoriesLayout= { Resource.Id.category_linearLayout, Resource.Id.category_linearLayout3, Resource.Id.category_linearLayout2 };
      
        ButtonGridView _additionalCategory;     
        View _rootView;
        LayoutInflater _inflater;
        List<CategoryModel> _subCategoriesList;
        List<CategoryModel> _mainCategoriesList;

        GridViewAdapter _buttonsAdapter;

        public HomeFragment(Boolean loadFromCache = true) : base(loadFromCache) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _inflater = inflater;
          
            PopulateViewForOrientation();            
            return _rootView;
        }
        private void PopulateViewForOrientation()
        {
            Log.Debug(Tag, "Start HomeFragment ");

            _rootView = _inflater.Inflate(Resource.Layout.HomePage, null);
            _rootView.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.backgroundColor));
            this.HasOptionsMenu = true;
           
            _mainCategoriesList = DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.MainCatGroup);
            _subCategoriesList = DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup);

            TextView[] mainCat = new TextView[_mainCategoriesList.Count];
            ImageView[] mainImg = new ImageView[_mainCategoriesList.Count];
            LinearLayout[] mainLayout = new LinearLayout[_mainCategoriesList.Count];
            var askBtn1 = _rootView.FindViewById<ImageButton>(Resource.Id.imageViewAsk);
            var askBtn2 = _rootView.FindViewById<ImageButton>(Resource.Id.imageViewAsk3);
            var askBtn3 = _rootView.FindViewById<ImageButton>(Resource.Id.imageViewAsk2);
            askBtn1.Tag = _mainCategoriesList[0].Id;
            askBtn2.Tag = _mainCategoriesList[1].Id;
            askBtn3.Tag = _mainCategoriesList[2].Id;
            askBtn1.Click += AskBtn_Click;
            askBtn2.Click += AskBtn_Click;
            askBtn3.Click += AskBtn_Click;

            for (int i = 0; i < mainCat.Length; i++)
            {
                mainCat[i] = _rootView.FindViewById<TextView>(_mainCategoriesText[i]);
                mainCat[i].Text = _mainCategoriesList[i].Name;
                mainImg[i] = _rootView.FindViewById<ImageView>(_mainCategoriesImgView[i]);
                mainLayout[i] = _rootView.FindViewById<LinearLayout>(_mainCategoriesLayout[i]);
            }

            for (int i = 0; i < mainCat.Length; i++)
            {
                mainCat[i].SetTextColor(Color.Gray);
                mainCat[i].SetTypeface(null, TypefaceStyle.Normal);
                mainCat[i].SetBackgroundColor(Color.White);
                mainLayout[i].Selected = false;
                mainLayout[i].Click +=async(s, e) =>
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
                                conn2.SetSelectedCategory(_mainCategoriesList[y]);

                            }
                        }
                    }
                await   MainActivity.MapPage.LoadData();
                };

            }

            var categorySelected = SelectedMainCategory.Id;

            mainCat[categorySelected - 1].SetTextColor(Color.Black);
            mainCat[categorySelected - 1].SetTypeface(null, TypefaceStyle.Bold);
            mainLayout[categorySelected - 1].Selected = true;
            Picasso.With(Activity).Load(Resource.Drawable.wheelchair1).Resize(45,55).Into(mainImg[0]);
            Picasso.With(Activity).Load(Resource.Drawable.wheelchair2).Resize(90, 55).Into(mainImg[1]);
            Picasso.With(Activity).Load(Resource.Drawable.wheelchair3).Resize(135, 55).Into(mainImg[2]);
            //mainImg[0].SetImageDrawable(Utils.SetDrawableSize(Activity, Resource.Drawable.wheelchair1, 140, 65));
            //mainImg[1].SetImageDrawable(Utils.SetDrawableSize(Activity, Resource.Drawable.wheelchair2, 140, 65));
            //mainImg[2].SetImageDrawable(Utils.SetDrawableSize(Activity, Resource.Drawable.wheelchair3, 140, 65));

            _additionalCategory = _rootView.FindViewById<ButtonGridView>(Resource.Id.additionalCategory);
            GridRotation();


            if (_subCategoriesList.Count == 0)
            {
                var localCategories = NohandicapLibrary.GetAdditionalCategory();
                var localCategoriesLocalization = Resources.GetStringArray(Resource.Array.additional_category_array);
                for (int i = 0; i < localCategories.Count; i++)
                {
                    var cat = localCategories[i];
                    var loc = localCategoriesLocalization[i];
                    _subCategoriesList.Add(new CategoryModel()
                    {
                        Id = cat.Id,
                        Name = loc,
                        Color = cat.Color,
                        Icon = cat.Icon,
                        Group = cat.Group,
                        Sort = i + 1
                    });
                }
                DbConnection.InsertUpdateProductList(_subCategoriesList);
            }
            _subCategoriesList = _subCategoriesList.OrderBy(x => x.Sort).ToList();
           
            _buttonsAdapter= new GridViewAdapter(this);
            _additionalCategory.Adapter = _buttonsAdapter;
            _additionalCategory.ItemClick += SubCategory_ItemClick;
            //ThreadPool.QueueUserWorkItem(o => LoadCache());
       
        }
        private void AskBtn_Click(object sender, EventArgs e)
        {
            var btn = (ImageButton) sender;
            var category = _mainCategoriesList.FirstOrDefault(x => x.Id == (int)btn.Tag);
            if (category != null)
            {
                var description = Utils.ReadStream(MainActivity,"main_category_" + category.Id+"_", CurrentLang.ShortName,".txt");
                HelpPopup helpPopup=new HelpPopup(MainActivity);
                helpPopup.SetText(description);
                helpPopup.Show(btn);
            }
        }

      private void ShowDropDown(View view)
      {
          TextView text = new TextView(MainActivity) {Text = "Lol"};
          PopupWindow popupWindow = new PopupWindow(MainActivity.ApplicationContext)
          {
              Focusable = true,
              Width = 100,
              Height = 100,
              ContentView = text
          };

          popupWindow.SetBackgroundDrawable(new ColorDrawable(
                   Color.White));
            view.Measure(0, 0);
           
            popupWindow.ShowAsDropDown(view);
        
        }
     
        private void ShowPopup(string title,string text)
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(MainActivity);
            builder.SetPositiveButton("OK", (sender, args) =>
            {
            });
            builder.SetMessage(text);
            builder.SetTitle(title);
            builder.Show();


        }

        private async void SubCategory_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var idCategory = e.Id;
            var category = DbConnection.GetDataList<CategoryModel>(x => x.Id == idCategory).FirstOrDefault();
            if (IsTablet)
            {
                DbConnection.SetSelectedCategory(category, category != null && !category.IsSelected, true);

                var selectedCategories = DbConnection.GetSubSelectedCategory();
                MainActivity.MapPage.SetData(selectedCategories.Count != 0 ? selectedCategories : _subCategoriesList);
                await MainActivity.MapPage.LoadData();
                _buttonsAdapter.UpdateCategories();
                _buttonsAdapter.NotifyDataSetChanged();
            }
            else
            {
                MainActivity.MapPage.SetData(new List<CategoryModel> {category});
                MainActivity.SetCurrentTab(1);
                MainActivity.SupportActionBar.Title = category?.Name;
                DbConnection.SetSelectedCategory(category);
            }
        }

        private void GridRotation()
        {
            var orientation = Activity.Resources.Configuration.Orientation;
            _additionalCategory.NumColumns = orientation == Android.Content.Res.Orientation.Portrait ? 3 : 5;
            if (!IsTablet) return;
            _additionalCategory.NumColumns = orientation == Android.Content.Res.Orientation.Landscape ? 3 : 2;
            InitializeMapForTablet();
        }
        private void InitializeMapForTablet()
        {           
                var transaction = ChildFragmentManager.BeginTransaction();               
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
                
        private void LoadCache()
        {
            try
            {
                var selectedSubCategory = DbConnection.GetSubSelectedCategory();
                var position = NohandicapApplication.MainActivity.CurrentLocation;
                string lat = "";
                string lng = "";
                if (position != null)
                {
                    lat = position.Latitude.ToString(CultureInfo.InvariantCulture);
                    lng = position.Longitude.ToString(CultureInfo.InvariantCulture);
                }
               // var coll = await RestApiService.GetMarkers(NohandicapApplication.SelectedMainCategory, selectedSubCategory, NohandicapApplication.CurrentLang.Id, lat, lng, 1);
               // AddProductsToCache(coll);

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