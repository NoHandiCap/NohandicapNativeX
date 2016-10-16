﻿using System;


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
using Android.Locations;
using Xamarin.Auth;
using System.Json;
using Square.Picasso;
using System.Collections.ObjectModel;

using Java.Net;

namespace NohandicapNative.Droid
{
    #region Application region
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = true)]
#endif

    public class NohandicapApplication : Application
    {
        static readonly string TAG = "X:" + typeof(NohandicapApplication).Name;
        public static MainActivity MainActivity { get; set; }
        public static LanguageModel CurrentLang
        {
            get
            {
                var conn = Utils.GetDatabaseConnection();
                int id = int.Parse(Utils.ReadFromSettings(MainActivity, Utils.LANG_ID_TAG, "1"));
                var lang = conn.GetDataList<LanguageModel>(x => x.Id == id).FirstOrDefault();
                return lang;
            }
            set
            {
                Utils.WriteToSettings(MainActivity, Utils.LANG_ID_TAG, value.ToString());
                Utils.WriteToSettings(MainActivity, Utils.LANG_SHORT, value.ShortName);
            }
        }
        public static bool IsInternetConnection { get; set; }
       
        public static CategoryModel SelectedMainCategory
        {
            get
            {
                CategoryModel cat;
                var conn = Utils.GetDatabaseConnection();
                cat = conn.GetSelectedMainCategory();
                
                return cat;
            }
            set {
                var conn = Utils.GetDatabaseConnection();
                conn.SetSelectedCategory(value);
                
            }

        }
     
        public static bool isTablet {get;set;}
        
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
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Log.Debug(TAG, "Configure locale");
            ISharedPreferences settings = PreferenceManager.GetDefaultSharedPreferences(this);
            string lang = settings.GetString(Utils.LANG_SHORT, null);
            if (lang != null)
            {
                Utils.SetLocale(new Locale(lang));
                Log.Debug(TAG, "Language: " +lang);
                Utils.updateConfig(this, BaseContext.Resources.Configuration);
            }     
            Log.Debug(TAG, "Locale configuration finished");
        
            Picasso.Builder builder = new Picasso.Builder(this);
            builder.Downloader(new OkHttpDownloader(this, int.MaxValue));
            Picasso built = builder.Build();
            built.IndicatorsEnabled=false;
            built.LoggingEnabled=false;
            Picasso.SetSingletonInstance(built);

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            new Android.Support.V7.App.AlertDialog.Builder(this)
       .SetPositiveButton("Ok", (s, args) =>
       {
           MainActivity.Finish();

       })
       .SetMessage(e.ExceptionObject.ToString())
       .SetTitle("Error")
       .Show();
        }
        
        protected override void Dispose(bool disposing)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            
            base.Dispose(disposing);
        }
    }
    #endregion

    [Activity(Label = "Nohandicap", WindowSoftInputMode = SoftInput.AdjustPan,LaunchMode =Android.Content.PM.LaunchMode.SingleTop, Icon = "@drawable/logo_small", ConfigurationChanges =Android.Content.PM.ConfigChanges.Orientation | 
        Android.Content.PM.ConfigChanges.ScreenSize
       )]
	public class MainActivity :AppCompatActivity,  IOnTabClickListener, ILocationListener
    {
        #region Properties
        static readonly string TAG = "X:" + typeof(MainActivity).Name;
        private BottomBar _bottomBar;
        Android.Support.V7.Widget.Toolbar toolbar;
        List<TabItem> items;
        public HomeFragment HomePage { get; set; }
        public GMapFragment MapPage { get; set; }
        public ListFragment ListPage { get; set; }
        public FavoritesFragment Favorites { get; set; }        
        int lastPos = 0;
        public  Location CurrentLocation { get; set; }
        LocationManager _locationManager;
        string _locationProvider;
        ObservableCollection<ProductMarkerModel> _currentProductsList;
        public ObservableCollection<ProductMarkerModel> CurrentProductsList
        {
            get
            {
                if (_currentProductsList.Count == 0)
                {
                    var conn = Utils.GetDatabaseConnection();
                    var products = conn.GetDataList<ProductMarkerModel>(50);
                    _currentProductsList = new ObservableCollection<ProductMarkerModel>(products);
                }
                return _currentProductsList;
            }
            set
            {
                _currentProductsList = value;
            }
        }
        #endregion

        protected override void OnCreate(Bundle bundle)
        {

            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);

            //   CrashManager.Register(this);
            SetContentView(Resource.Layout.Main);
            NohandicapApplication.isTablet = Resources.GetBoolean(Resource.Boolean.is_tablet);
            NohandicapApplication.MainActivity = this;
            if (NohandicapApplication.SelectedMainCategory == null)
            {
                var conn = Utils.GetDatabaseConnection();
                var mainCategory = conn.GetDataList<CategoryModel>().FirstOrDefault(x => x.Group == NohandicapLibrary.MainCatGroup && x.Id == 1);
                conn.SetSelectedCategory(mainCategory);

            }
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            _bottomBar = BottomBar.AttachShy(FindViewById<CoordinatorLayout>(Resource.Id.myCoordinator), FindViewById<LinearLayout>(Resource.Id.linContent), bundle);
            HomePage = new HomeFragment();
            RunOnUiThread(() =>
            {
                MapPage = new GMapFragment();
                ListPage = new ListFragment();
                Favorites = new FavoritesFragment();
            });
            _currentProductsList = new ObservableCollection<ProductMarkerModel>();
            items = NohandicapLibrary.GetTabs(NohandicapApplication.isTablet);
            PrepareBar();
            if (bundle != null)
            {

                var postion = bundle.GetInt(Utils.TAB_ID);
                _bottomBar.SelectTabAtPosition(postion, false);
            }

            ThreadPool.QueueUserWorkItem(o => CheckUpdate());
            ThreadPool.QueueUserWorkItem(o => InitializeLocationManager());
            ThreadPool.QueueUserWorkItem(o => LoadCache());

        }

        public async void CheckUpdate()
        {
            try
            {
                string langId = Utils.ReadFromSettings(this, Utils.LANG_ID_TAG, "1");
                var conn = Utils.GetDatabaseConnection();
                var updateList = await RestApiService.CheckUpdate(conn, langId, Utils.GetLastUpdate(this));
                if (updateList != null)
                {
                    if (updateList.Count != 0)
                    {
                       // Utils.WriteToSettings(this, NohandicapLibrary.PRODUCT_TABLE, updateList[NohandicapLibrary.PRODUCT_TABLE]);
                        Utils.WriteToSettings(this, NohandicapLibrary.CATEGORY_TABLE, updateList[NohandicapLibrary.CATEGORY_TABLE]);
                        Utils.WriteToSettings(this, NohandicapLibrary.LANGUAGE_TABLE, updateList[NohandicapLibrary.LANGUAGE_TABLE]);
                    }
                    Utils.WriteToSettings(this, Utils.LAST_UPDATE_DATE, DateTime.Now.ToShortDateString());
                }
          
            }catch(Exception e)
            {       
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif         
            Log.Debug(TAG, "Check Update " + e.Message);
            }           
        }
        private async void LoadCache()
        {
            try
            {
                var conn = Utils.GetDatabaseConnection();
                var selectedSubCategory = conn.GetSubSelectedCategory();
                var position = NohandicapApplication.MainActivity.CurrentLocation;
                string lat = "";
                string lng = "";
                if (position != null)
                {
                    lat = position.Latitude.ToString();
                    lng = position.Longitude.ToString();
                }

                var coll = await RestApiService.GetMarkers(NohandicapApplication.SelectedMainCategory, selectedSubCategory, NohandicapApplication.CurrentLang.Id, lat, lng, 1);
                NohandicapApplication.MainActivity.AddProductsToCache(coll);

            }
            catch (System.Exception e)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

            }
        }
        private void PrepareBar()
        {
           
            Log.Debug(TAG, "Prepare Bar.....");
            SetSupportActionBar(toolbar);
          
            _bottomBar.NoTabletGoodness();
            _bottomBar.UseFixedMode();
          
            var tabItems = new BottomBarTab[items.Count];
            for (int i = 0; i < tabItems.Length; i++)
            {
             
                var tab = items[i];
                var icon = Utils.GetImage(this, tab.Image);

                tabItems[i] = new BottomBarTab(Utils.SetDrawableSize(this,icon, 40, 40), tab.Title);
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
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);           
            Log.Debug(TAG, "Bar prepared");
            _bottomBar.FindViewById(Resource.Id.bb_bottom_bar_background_view).SetBackgroundColor(Resources.GetColor(Resource.Color.themeColor));
            _bottomBar.SetActiveTabColor(Color.White);
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
                    ShowFragment(HomePage, position.ToString());
                    break;
                case 1:
                    if (NohandicapApplication.isTablet)
                    {
                        ShowFragment(ListPage, position.ToString());
                    }
                    else
                    {
                        ShowFragment(MapPage, position.ToString());
                    }

                    break;
                case 2:
                    if (NohandicapApplication.isTablet)
                    {
                        ShowFragment(Favorites, position.ToString());
                    }
                    else
                    {
                        ShowFragment(ListPage, position.ToString());
                    }
                    break;
                case 3:
                    ShowFragment(Favorites, position.ToString());
                    break;
                default:
                    break;
            }
            SupportActionBar.Show();
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Color.ParseColor(items[position].Color)));
            SupportActionBar.Title = items[position].Title;
            if (position == 0)
            {
                SupportActionBar.Title = "Nohandicap";
            }

            lastPos = position;
        }

      public void LoginToFacebook(FavoritesFragment fragment,bool allowCancel)
        {
            UserModel user = null;
            var auth = new OAuth2Authenticator(
                clientId: "105055836622734",
                scope: "",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            auth.AllowCancel = allowCancel;

            // If authorization succeeds or is canceled, .Completed will be fired.
            auth.Completed += (s, ee) => {
                if (!ee.IsAuthenticated)
                {
                    //var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    //builder.SetMessage("Not Authenticated");
                    //builder.SetPositiveButton("Ok", (o, e) => { });
                    //builder.Create().Show();
                    return;
                }

                // Now that we're logged in, make a OAuth2 request to get the user's info.
                var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me"), null, ee.Account);
                request.GetResponseAsync().ContinueWith(t => {
                    var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    if (t.IsFaulted)
                    {                       
                        builder.SetMessage(t.Exception.Flatten().InnerException.ToString());
                        fragment.UserLoginSuccess(null);
                    }
                    else if (t.IsCanceled)
                    {
                       
                        fragment.UserLoginSuccess(null);
                    }
                    else
                    {
                        var obj = JsonValue.Parse(t.Result.GetResponseText());

                        user = new UserModel();
                        user.Id = obj["id"].ToString();
                        user.Name = obj["name"];
                        user.Favorites = new List<int>();
                        fragment.UserLoginSuccess(user);
                    }
                   
                }, UIScheduler);
            };

            var intent = auth.GetUI(this);
            StartActivity(intent);
          
        }
        private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();
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
        public void RemoveFragment(Android.Support.V4.App.Fragment fragment)
        {
            Android.Support.V4.App.FragmentManager fragmentManager = SupportFragmentManager;
            fragmentManager.BeginTransaction().Remove(fragment).Commit();
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
      void InitializeLocationManager()
        {
            try
            {
                LocationManager lm = (LocationManager)GetSystemService(LocationService);

                IList<String> providers = lm.GetProviders(true);
               
                foreach (String provider in providers)
                {
                    Location l = lm.GetLastKnownLocation(provider);
                    if (l == null)
                    {
                        continue;
                    }
                    if (CurrentLocation == null || l.Accuracy < CurrentLocation.Accuracy)
                    {
                        CurrentLocation = l;
                    }
                }

                _locationManager = (LocationManager)GetSystemService(LocationService);
                Criteria criteriaForLocationService = new Criteria
                {
                    Accuracy = Accuracy.Fine
                };
                IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

                if (acceptableLocationProviders.Any())
                {
                    _locationProvider = acceptableLocationProviders.First();
                }
                else
                {
                    _locationProvider = string.Empty;
                }
                Log.Debug(TAG, "Using " + _locationProvider + ".");
            }
            catch (Exception e)
            {
                Log.Error(TAG, "InitializeLocationManager: " + e.Message);

            }
        }
        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

        }
        public void OnLocationChanged(Location location)
        {
            CurrentLocation = location;           
        }

        public void OnProviderDisabled(string provider)
        {
           
        }

        public void OnProviderEnabled(string provider)
        {
          
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
           
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
            Utils.UpdateConfig(this);
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
                        var conn = Utils.GetDatabaseConnection();
                        var products = conn.GetDataList<ProductDetailModel>();
                        var currentProduct = conn.GetDataList<ProductDetailModel>(x => x.ID == currentProductId);
                        MapPage.SetData(new List<CategoryModel>());
                        SetCurrentTab(1);
                        SupportActionBar.Title = currentProduct[0].FirmName;
                        
                    }
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            }catch(Exception e)
            {
                Log.Error(TAG, "OnResume(): " +e.Message);

            }
        }
        protected override void OnPause()
        {
            base.OnPause();
            SaveProductsBeforeExit();
            try
            {
                _locationManager.RemoveUpdates(this);
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnPause(): " +e.Message);
            }
       
        }
        public override void OnLowMemory()
        {
            base.OnLowMemory();
            SaveProductsBeforeExit();
            CurrentProductsList.Clear();
            GC.Collect();
        }
        protected override void OnDestroy()
        {
            SaveProductsBeforeExit();
            base.OnDestroy();

        }
        #endregion

        private void SaveProductsBeforeExit()
        {
            Task.Run(() => { 
            var conn = Utils.GetDatabaseConnection();
            conn.InsertUpdateProductList(CurrentProductsList.ToList());
            });
        }
        public void AddProductsToCache(List<ProductMarkerModel> products)
        {
            Task.Run(() =>
            {
                foreach (var prod in products)
                {
                    if (!_currentProductsList.Contains(prod))
                    {
                        _currentProductsList.Add(prod);
                    }
                    else
                    {
                        if (_currentProductsList.Any(x => x.Distance != prod.Distance))
                        {
                            _currentProductsList.FirstOrDefault(x => x.Id == prod.Id).Distance = prod.Distance;
                        }
                    }
                }
            });
        }
    }
}


