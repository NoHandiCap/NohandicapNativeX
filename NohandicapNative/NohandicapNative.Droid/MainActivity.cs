﻿using System;


using Android.Content;
using Android.Runtime;
using Android.Views;

using Android.OS;
using Android.Support.V7.App;
using BottomNavigationBar;
using Android.Support.V4.Content;
using Android.Graphics;
using BottomNavigationBar.Listeners;

using System.Collections.Generic;
using NohandicapNative;
using NohandicapNative.Droid;

using Android.Graphics.Drawables;

using Android.Support.V7.Widget;
using Android.Widget;
using Java.Lang;
using Android.Support.V4.App;
using Android.App;

using Android.Support.Design.Widget;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid
{
public    class Nohandicap : Application
    {
        public MainActivity MainActivity;
    }
	[Activity (Label = "NohandicapNative.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : AppCompatActivity, IOnMenuTabSelectedListener, IOnTabClickListener
    {
         int PICK_CONTACT_REQUEST = 1;  // The request code
        private BottomBar _bottomBar;
        Android.Support.V7.Widget.Toolbar toolbar;
        List<TabItem> items;

        HomeFragment homePage;
        GMapFragment mapPage;
        ListFragment listPage;
        FavoritesFragment favorites;

        int lastPos = 0;
        public void OnMenuItemSelected(int menuItemId)
        {
           
        }

        protected override void OnCreate (Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate (bundle);
         
            SetContentView(Resource.Layout.Main);
     //  ShowLoginWindow();
            // Create your application here
            _bottomBar = BottomBar.AttachShy(FindViewById<CoordinatorLayout>(Resource.Id.myCoordinator), FindViewById<LinearLayout>(Resource.Id.linContent), bundle);
           var s = new SqliteService(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(),Utils.PATH);
            s.CreateDB();
           

            CategoryModel ct = new CategoryModel();
            ct.Color = "dvfsd";
            ct.Name = "Total";
            TranslateModel trans = new TranslateModel();
            trans.Translate = "Hotel";
            LanguageModel lang = new LanguageModel();
            lang.LanguageName = "German";
            LanguagesDbModel langDb = new LanguagesDbModel();
            langDb.Translate = trans;
            langDb.Language = lang;
            var trans2 = new TranslateModel();
            LanguagesDbModel langDb2 = new LanguagesDbModel();
            trans2.Translate = "Part";
            langDb2.Translate = trans2;
            langDb2.Language = lang;
            CategoryModel ct2 = new CategoryModel();
            ct2.Color = "234";
            ct2.Name = "Teilweise";
            ct2.Languages = new List<LanguagesDbModel>() { langDb2 };
            ProductModel marker = new ProductModel();
            marker.Title = "lol";
            marker.Languages = new List<LanguagesDbModel>() { langDb };
            marker.Categories = new List<CategoryModel>() { ct2,ct };
            s.insertUpdateData(marker);
            var g = s.GetData(1);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar> (Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        //    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mapPage = new GMapFragment();
            listPage = new ListFragment();
            homePage = new HomeFragment();
           
            favorites = new FavoritesFragment();
            items = NohandiLibrary.GetTabs();
           var tabItems = new BottomBarTab[items.Count];
            for (int i = 0; i < tabItems.Length; i++)
            {
                var tab = items[i];
                tabItems[i] = new BottomBarTab(Utils.GetImage(this,tab.Image), tab.Title);
          
             
            }
            _bottomBar.SetItems(tabItems);
            for (int i = 0; i < tabItems.Length; i++)
            {
                var tab = items[i];
                _bottomBar.MapColorForTab(i, tab.Color);
                            }  
            _bottomBar.SetOnTabClickListener(this);
            Drawable dr = Resources.GetDrawable(Resource.Drawable.logo_small);
            Bitmap bitmap = ((BitmapDrawable)dr).Bitmap;
            // Scale it to 50 x 50
            Drawable d = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap, 70, 70, true));

           SupportActionBar.SetIcon(d);
           SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
        }
   

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
             // Necessary to restore the BottomBar's state, otherwise we would
            // lose the current tab on orientation change.
            _bottomBar.OnSaveInstanceState(outState);
        }
      private  void ShowLoginWindow()
        {
            var myIntent = new Intent(this, typeof(LoginActivity));
            StartActivityForResult(myIntent, 0);
        }
        
        #region IOnTabClickListener implementation
        public void SetCurrentTab(int position)
        {
            _bottomBar.SelectTabAtPosition(position, true);
        }
        public void OnTabSelected(int position)
        {
            switch (position)
            {
                case 0:                  
                        ShowFragment(homePage,position.ToString());
                    break;
                case 1:                   
                        ShowFragment(mapPage, position.ToString());                

                    break;
                case 2:
                    ShowFragment(listPage, position.ToString());
                    break;
                case 3:
                    ShowFragment(favorites, position.ToString());
                    break;
                default:
                    break;
            }

            //if (position == 0)
            //{
            //    SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            //}
            //else
            //    SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportActionBar.Show();
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor(items[position].Color)));
          SupportActionBar.Title=items[position].Title;
            if (position == 0) SupportActionBar.Title="Nohandicap";
         
                lastPos = position;
        }
       public void ShowFragment(Android.Support.V4.App.Fragment fragment,string tag)
        {
            Android.Support.V4.App.FragmentManager fragmentManager = SupportFragmentManager;

            if (fragmentManager.FindFragmentByTag(tag) != null)
            {
                //if the fragment exists, show it.
                fragmentManager.BeginTransaction().Show(fragmentManager.FindFragmentByTag(tag)).Commit();
            }
            else
            {
                //if the fragment does not exist, add it to fragment manager.
                fragmentManager.BeginTransaction().Add(Resource.Id.flContent, fragment, tag).Commit();
            }
            if (fragmentManager.FindFragmentByTag(lastPos.ToString()) != null)
            {
                //if the other fragment is visible, hide it.
                fragmentManager.BeginTransaction().Hide(fragmentManager.FindFragmentByTag(lastPos.ToString())).Commit();
            }

        }
        public void OnTabReSelected(int position)
        {
          
        }

        #endregion
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.main_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetCurrentTab(0);
            break;
                case Resource.Id.settings:
                    StartActivity(typeof(SettingsActivity));
                    break;
            }
            return true;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
              //  var helloLabel = FindViewById<TextView>(Resource.Id.helloLabel);
               // helloLabel.Text = data.GetStringExtra("greeting");
            }
        }
        
    }
}


