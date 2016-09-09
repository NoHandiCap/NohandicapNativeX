using System;


using Android.Content;
using Android.Runtime;
using Android.Views;

using Android.OS;
using Android.Support.V7.App;
using BottomNavigationBar;
using Android.Graphics;
using BottomNavigationBar.Listeners;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Widget;
using Android.App;
using Android.Support.Design.Widget;
using NohandicapNative.Droid.Services;
using Android.Content.Res;
using Java.Util;
using Android.Preferences;
using Android.Util;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using HockeyApp.Android;

namespace NohandicapNative.Droid
{
    #region Application region
#if DEBUG
[Application(Debuggable=true)]
#else
    [Application(Debuggable = true)]
#endif

    public class NohandicapApplication : Application
    {
        static readonly string TAG = "X:" + typeof(NohandicapApplication).Name;
        public MainActivity MainActivity { get; set; }
        private Locale locale = null;   
               
        public NohandicapApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
           
            base.OnConfigurationChanged(newConfig);
            Log.Debug(TAG, "Make config");
            Utils.updateConfig(this, newConfig);
            Log.Debug(TAG, "Config created");

        }
        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(TAG, "Configure locale");
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            string lang = settings.GetString(Utils.LANG_SHORT, null);
            if (lang != null)
            {
                Utils.setLocale(new Locale(lang));
                Log.Debug(TAG, "Language: " +lang);
                Utils.updateConfig(this, BaseContext.Resources.Configuration);
            }     
            Log.Debug(TAG, "Locale configuration finished");
           


        }
    }
    #endregion

    [Activity(Label = "Nohandicap", WindowSoftInputMode = SoftInput.AdjustPan,LaunchMode =Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/logo_small", ConfigurationChanges =Android.Content.PM.ConfigChanges.Orientation | 
        Android.Content.PM.ConfigChanges.ScreenSize
       )]
	public class MainActivity : AppCompatActivity,  IOnTabClickListener
    {
        #region Properties
        static readonly string TAG = "X:" + typeof(MainActivity).Name;
        int PICK_CONTACT_REQUEST = 1;  // The request code
        private BottomBar _bottomBar;
        Android.Support.V7.Widget.Toolbar toolbar;
        List<TabItem> items;
        public HomeFragment HomePage { get; set; }
        public GMapFragment MapPage { get; set; }
        public ListFragment ListPage { get; set; }
        public FavoritesFragment Favorites { get; set; }
        SqliteService dbCon;
        int lastPos = 0;
        #endregion      
       
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);
         //   CrashManager.Register(this);
            SetContentView(Resource.Layout.Main);
            Utils.mainActivity = this;
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);          
            _bottomBar = BottomBar.AttachShy(FindViewById<CoordinatorLayout>(Resource.Id.myCoordinator), FindViewById<LinearLayout>(Resource.Id.linContent), bundle);
            MapPage = new GMapFragment();
            ListPage = new ListFragment();
            HomePage = new HomeFragment();
            Favorites = new FavoritesFragment();
            items = NohandiLibrary.GetTabs();
            dbCon = Utils.GetDatabaseConnection();
            PrepareBar();
            if (bundle != null)
            {
                var postion = bundle.GetInt(Utils.TAB_ID);
                _bottomBar.SelectTabAtPosition(postion, false);
            }
            Utils.mainActivity = this;
            ThreadPool.QueueUserWorkItem(o => CheckDataBase());
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }
        public async Task<bool> CheckDataBase()
        {
            var language = dbCon.GetDataList<LanguageModel>();
            var categories = dbCon.GetDataList<CategoryModel>();
            var products = dbCon.GetDataList<ProductModel>();
            if (language.Count ==0 || categories.Count == 0|| products.Count == 0)
            {
                ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
                string lang = settings.GetString(Utils.LANG_ID_TAG, null);
            return await  dbCon.SynchronizeDataBase(lang);            
               
            }
            return false;
            
        }
        private void PrepareBar()
        {
            Log.Debug(TAG, "Prepare Bar.....");
            SetSupportActionBar(toolbar);
            _bottomBar.NoNavBarGoodness();
            _bottomBar.NoTabletGoodness();
          
            var tabItems = new BottomBarTab[items.Count];
            for (int i = 0; i < tabItems.Length; i++)
            {
                var tab = items[i];
                var icon = Utils.GetImage(this, tab.Image);

                tabItems[i] = new BottomBarTab(Utils.SetDrawableSize(this,icon, 50, 50), tab.Title);
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
         //   SupportActionBar.SetIcon(Utils.SetDrawableSize(this, Resource.Drawable.logo_small, 80, 80));
            SupportActionBar.SetHomeAsUpIndicator(Utils.SetDrawableSize(this, Resource.Drawable.logo_small, 80, 80));

            _bottomBar.HideShadow();
            Log.Debug(TAG, "Bar prepared");
            
        }

        #region IOnTabClickListener implementation
        public void SetCurrentTab(int position)
        {
            _bottomBar.SelectTabAtPosition(position, false);
        }
        public void OnTabSelected(int position)
        {
            switch (position)
            {
                case 0:                  
                        ShowFragment(HomePage,position.ToString());
                    break;
                case 1:                    
                        ShowFragment(MapPage, position.ToString());                          

                    break;
                case 2:
                    ShowFragment(ListPage, position.ToString());
                    break;
                case 3:
                    ShowFragment(Favorites, position.ToString());
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
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                // Respond to the action bar's Up/Home button
                case Android.Resource.Id.Home:
                    SetCurrentTab(0);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

      

        #region ActivityLifeCycle implementation

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            // Necessary to restore the BottomBar's state, otherwise we would
            // lose the current tab on orientation change.
            _bottomBar.OnSaveInstanceState(outState);
            outState.PutInt(Utils.TAB_ID, _bottomBar.CurrentTabPosition);
        }
        public MainActivity()
        {
            Utils.updateConfig(this);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1)
            {
                if (resultCode == Result.Ok)
                {
                  var currentProductId= data.GetIntExtra(Utils.PRODUCT_ID,-1);
                    if (currentProductId != -1)
                    {
                        var products = dbCon.GetDataList<ProductModel>();
                        var currentProduct = dbCon.GetDataList<ProductModel>().Where(x => x.ID == currentProductId).ToList();
                        MapPage.SetData(currentProduct);
                        SetCurrentTab(1);
                        SupportActionBar.Title = currentProduct[0].FirmName;
                    }
                }
            }
        }
       
        #endregion

    }
}


