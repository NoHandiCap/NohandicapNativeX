using System;


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
using System.IO;
using Android.Content.Res;
using Java.Util;
using Android.Preferences;
using Android.Content.PM;

namespace NohandicapNative.Droid
{
    [Application]
    public class NohandicapApplication : Application
    {
        public MainActivity MainActivity { get; set; }
        private Locale locale = null;
       

       
        public NohandicapApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
           
            base.OnConfigurationChanged(newConfig);
           Utils.updateConfig(this, newConfig);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            string lang = settings.GetString(Utils.LANG_SHORT, null);
            if (lang != null)
            {
                Utils.setLocale(new Locale(lang));
                Utils.updateConfig(this, BaseContext.Resources.Configuration);
            }     
         

           
        }
    }
    [Activity(Label = "Nohandicap", Icon = "@drawable/logo_small", ConfigurationChanges =Android.Content.PM.ConfigChanges.Orientation | 
        Android.Content.PM.ConfigChanges.ScreenSize
       )]
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
        SqliteService dbCon;
        int lastPos = 0;
        public void OnMenuItemSelected(int menuItemId)
        {
           
        }
        public MainActivity()
        {
            Utils.updateConfig(this);
        }
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Utils.mainActivity = this;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            // Create your application here
            _bottomBar = BottomBar.AttachShy(FindViewById<CoordinatorLayout>(Resource.Id.myCoordinator), FindViewById<LinearLayout>(Resource.Id.linContent), bundle);
            if (!File.Exists(System.IO.Path.Combine(Utils.PATH, SqliteService.DB_NAME)))
            {
                ShowFirstWindow();
                Finish();
            }   
            //    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            mapPage = new GMapFragment();
            listPage = new ListFragment();
            homePage = new HomeFragment();
            favorites = new FavoritesFragment();
            items = NohandiLibrary.GetTabs();
            dbCon = Utils.GetDatabaseConnection();
            PrepareBar();
            if (bundle != null)
            {
                var postion = bundle.GetInt(Utils.TAB_ID);
                _bottomBar.SelectTabAtPosition(postion, false);
            }
            //   LoadProducts();
        }
        private void PrepareBar()
        {
            SetSupportActionBar(toolbar);
            _bottomBar.NoNavBarGoodness();
            _bottomBar.NoTabletGoodness();
            var tabItems = new BottomBarTab[items.Count];
            for (int i = 0; i < tabItems.Length; i++)
            {
                var tab = items[i];
                var icon = Utils.GetImage(this, tab.Image);

                tabItems[i] = new BottomBarTab(icon, tab.Title);
                _bottomBar.SetActiveTabColor(Color.Red);

            }
            _bottomBar.SetItems(tabItems);
            for (int i = 0; i < tabItems.Length; i++)
            {
                var tab = items[i];
                _bottomBar.MapColorForTab(i, tab.Color);
                //  _bottomBar.MapColorForTab(i, Color.ParseColor(Utils.BACKGROUND));

            }
            _bottomBar.SetOnTabClickListener(this);
            SupportActionBar.SetIcon(Utils.SetDrawableSize(this, Resource.Drawable.logo_small, 70, 70));
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDefaultDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
           
            ((NohandicapApplication)Application).MainActivity = this;
        

            _bottomBar.HideShadow();
        }
        private async void LoadProducts()
        {
            var prod = await RestApiService.GetData<List<ProductModel>>(NohandiLibrary.LINK_PRODUCT);
            var product = await dbCon.LoadProductsFromInternet(Utils.ReadFromSettings(this, Utils.LANG_ID_TAG));
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
             // Necessary to restore the BottomBar's state, otherwise we would
            // lose the current tab on orientation change.
            _bottomBar.OnSaveInstanceState(outState);
            outState.PutInt(Utils.TAB_ID, _bottomBar.CurrentTabPosition);
            //if (outState != null)
            //{
            //    homePage= (HomeFragment)SupportFragmentManager.GetFragment(outState, stringValueA);
            //    fragmentB = (FragmentB)fragmentManager.getFragment(savedInstanceState, stringValueB);
            //}
        }
       
      private  void ShowFirstWindow()
        {
            var myIntent = new Intent(this, typeof(FirstStartActivity));
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
            if (position == 0) {        
                SupportActionBar.Title = "Nohandicap";
            }
     

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


