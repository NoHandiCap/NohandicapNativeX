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
using static Android.Views.View;
using Android.Graphics.Drawables;
using Android.Content.PM;
using Android.Content.Res;
using static Android.Widget.AdapterView;
using BottomNavigationBar.Listeners;

namespace NohandicapNative.Droid
{
  public  class HomeFragment: Android.Support.V4.App.Fragment, IOnMenuTabSelectedListener
    {
        MainActivity myContext;
        int[] mainCategoriesText = { Resource.Id.first_category, Resource.Id.second_category, Resource.Id.thrity_category };
       int[] mainCategoriesImgView = { Resource.Id.imageView, Resource.Id.imageView2, Resource.Id.imageView3 };
       int[] mainCategoriesLayout= { Resource.Id.category_linearLayout, Resource.Id.category_linearLayout3, Resource.Id.category_linearLayout2 };
        private SqliteService dbCon;
        ButtonGridView additionalCategory;
        ListView mainCategory;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var view = inflater.Inflate(Resource.Layout.HomePage,container,false);
          view.SetBackgroundColor(myContext.Resources.GetColor(Resource.Color.backgroundColor));
            dbCon = Utils.GetDatabaseConnection();
            // mainCategory = view.FindViewById<ListView>(Resource.Id.mainCategory);
            //   string[] mainItems = Resources.GetStringArray(Resource.Array.main_category_array);
            //List<CustomRadioButton> main = new List<CustomRadioButton>();
            //for (int i = 0; i < mainItems.Length; i++)
            //{
            //    main.Add(new CustomRadioButton() {
            //        ResourceImage = "wheelchair" + (i + 1),
            //        Text = mainItems[i]

            //    });
            //}
            // mainCategory.Adapter = new RadioButtonListAdapter(myContext, main);
            
            this.HasOptionsMenu = true;
            TextView[] mainCat = new TextView[mainCategoriesText.Length];
            ImageView[] mainImg = new ImageView[mainCategoriesImgView.Length];
            LinearLayout[] mainLayout = new LinearLayout[mainCategoriesLayout.Length];

            string[] mainItems = Resources.GetStringArray(Resource.Array.main_category_array);

          
            for (int i = 0; i < mainCat.Length; i++)
            {
                mainCat[i] = view.FindViewById<TextView>(mainCategoriesText[i]);
                mainCat[i].Text = mainItems[i];
                mainImg[i] = view.FindViewById<ImageView>(mainCategoriesImgView[i]);
                mainLayout[i] = view.FindViewById<LinearLayout>(mainCategoriesLayout[i]);
          

            }
            for (int i = 0; i < mainCat.Length; i++)
            {
                mainCat[i].SetTextColor(Color.Gray);
                mainCat[i].SetTypeface(null, TypefaceStyle.Normal);
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
                                Utils.WriteToSettings(myContext,Utils.MAIN_CAT_SELECTED_ID, y.ToString());
                            }
                        }
                    }
                };

            }
            var categorySelected = int.Parse(Utils.ReadFromSettings(myContext, Utils.MAIN_CAT_SELECTED_ID, "0"));

            mainCat[categorySelected].SetTextColor(Color.Black);
            mainCat[categorySelected].SetTypeface(null, TypefaceStyle.Bold);
            mainLayout[categorySelected].Selected = true;
            mainImg[0].SetImageDrawable(Utils.SetDrawableSize(myContext, Resource.Drawable.wheelchair1, 140,65 ));
            mainImg[1].SetImageDrawable(Utils.SetDrawableSize(myContext, Resource.Drawable.wheelchair2, 140, 65));
            mainImg[2].SetImageDrawable(Utils.SetDrawableSize(myContext, Resource.Drawable.wheelchair3, 140, 65));

            additionalCategory = view.FindViewById<ButtonGridView>(Resource.Id.additionalCategory);
            GridRotation();

            List<CategoryModel> additItems = dbCon.GetDataList<CategoryModel>(false);
            if (additItems.Count == 0)
            {
                var localCategories = NohandiLibrary.GetAdditionalCategory();
                var localCategoriesLocalization = Resources.GetStringArray(Resource.Array.additional_category_array);
                for (int i = 0; i < localCategories.Count; i++)
                {
                    var cat = localCategories[i];
                    var loc = localCategoriesLocalization[i];
                    additItems.Add(new CategoryModel() {
                        ID = cat.ID,
                        Name = loc,
                        Color = cat.Color,
                        Icon=cat.Icon,
                         Sort=i+1
                    });
                }
            }
            //  List<TabItem> mainItems = NohandiLibrary.GetMainCategory();
            //  mainCategory.Adapter = new GridViewAdapter(myContext, mainItems);
        
              additionalCategory.Adapter= new GridViewAdapter(myContext, additItems.OrderBy(x => x.Sort).ToList());
            //mainCategory.OnItemClickListener = this;
            
            return view;
        }
        private void GridRotation()
        {
            var orientation = myContext.Resources.Configuration.Orientation;
            if(orientation==Android.Content.Res.Orientation.Portrait)
                additionalCategory.NumColumns = 3;
            else
                additionalCategory.NumColumns = 5;

         
        }
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
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
                 myContext.SetCurrentTab(0);
                    break;
                case Resource.Id.settings:
                   myContext.StartActivity(typeof(SettingsActivity));
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