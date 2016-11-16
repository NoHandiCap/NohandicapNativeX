using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;
using NohandicapNative.Droid.Services;
using Android.Util;
using Android.Gms.Maps;
using System.Globalization;
using System.Threading.Tasks;
using static Android.Gms.Maps.GoogleMap;
using NohandicapNative.Droid.Model;
using System.Threading;
using Square.Picasso;
using System.Collections.ObjectModel;
using NohandicapNative.Droid.Fragments;
using Android.Provider;
using System.Diagnostics;
using Android.Graphics;
using NohandicapNative.Droid.Activities;
using CameraPosition = Android.Gms.Maps.Model.CameraPosition;

namespace NohandicapNative.Droid
{
    public class GMapFragment : BaseFragment, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback
    {     

        static readonly string TAG = "X:" + typeof(GMapFragment).Name;
        protected int pixelDensityIndex = 0, defaultMarkerSize = 32, markerResizeIndex = 32;
        LayoutInflater _inflater;   
        GoogleMap _map;            
        MapView _mapView;
        List<CategoryModel> _currentCategories;
        public  ObservableCollection<ProductMarkerModel> ProductsInBounds;
        public LatLngBounds LatLngBounds = null;
        MarkerUrlBuilder _markerUrlBuilder;
        static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1,1);
        bool isShownNoInternet = false;

        public GMapFragment(Boolean loadFromCache = true) : base(loadFromCache)
        {
            ProductsInBounds = new ObservableCollection<ProductMarkerModel>();
            _markerUrlBuilder = new MarkerUrlBuilder();
        }

        /*
         * For screens with higher pixel density there is a need to multiply marker size.
        */

        private void SetScreenDensity()
        {
            DisplayMetrics metrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
            pixelDensityIndex = (int)metrics.Density <= 2 ? 1 : 2;
            markerResizeIndex = defaultMarkerSize * pixelDensityIndex;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this._inflater = inflater;          
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            view.SetBackgroundColor(MainActivity.Resources.GetColor(Resource.Color.backgroundColor));
            SetScreenDensity();
            HasOptionsMenu = true;
            _mapView = view.FindViewById<MapView>(Resource.Id.map);
            _mapView.OnCreate(savedInstanceState);
            _mapView.OnResume();
         
            if (!NohandicapApplication.CheckIfGPSenabled())
                ShowNoGPSEnabled();
            //else
                //Toast.MakeText(this.Context, "GPS enabled", ToastLength.Short).Show(); //on some phones it shows every few seconds: Wiko i Samsumg Galaxy 

            _mapView.GetMapAsync(this); //asynchronic loading _map

            try
            {                                         
                var selectedCategories = DbConnection.GetSubSelectedCategory();

                if (selectedCategories.Count != 0)
                    SetData(selectedCategories);
                else
                    SetData(DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup));
            }
            catch (Exception e)
            {
                Log.Debug(TAG, "Check Update " + e.Message);
            }
            return view;
        }

        /**
         * Function to show settings alert dialog
         * On pressing Settings button will lauch Settings Options
         * */
        public void ShowNoGPSEnabled()
        {
            var alertDialog = new Android.Support.V7.App.AlertDialog.Builder(MainActivity);

            alertDialog.SetTitle(Resources.GetString(Resource.String.dialog_gps_title));
            alertDialog.SetMessage(Resources.GetString(Resource.String.dialog_gps_message));

            // On pressing Settings button
            alertDialog.SetPositiveButton(Resources.GetString(Resource.String.dialog_gps_title_positive), (sender, args) =>
            {
                StartActivity(new Intent(Settings.ActionLocationSourceSettings));
            });

            // on pressing cancel button
            alertDialog.SetNegativeButton(Resources.GetString(Resource.String.dialog_gps_title_negative), (sender, args) =>
            {
                alertDialog.Dispose();
            });

            alertDialog.Show();
        }

        public async Task<bool> LoadData()
        {
            if (_map != null)
            {
                await Task.Run(() =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        LatLngBounds = _map.Projection.VisibleRegion.LatLngBounds;

                    });
                }).ContinueWith(async t =>
                 {
                     try
                     {
                         if (LatLngBounds == null)
                         {
                             await SemaphoreSlim.WaitAsync();
                             try
                             {
                                 await LoadData();
                             }
                             finally
                             {
                                 SemaphoreSlim.Release();
                             }
                             return;
                         }
                         Log.Debug(TAG, "Start Load ");

                         IEnumerable<ProductMarkerModel> loadedProducts = null;
                         if (MainActivity.CurrentLocation != null)
                         {
                             _markerUrlBuilder.SetBounds(LatLngBounds.Southwest.Latitude, LatLngBounds.Southwest.Longitude,
                             LatLngBounds.Northeast.Latitude, LatLngBounds.Northeast.Longitude);

                             _markerUrlBuilder.SetMyLocation(MainActivity.CurrentLocation.Latitude, MainActivity.CurrentLocation.Longitude);
                             loadedProducts = await _markerUrlBuilder.LoadDataAsync();
                         }
                         else
                         {
                             _markerUrlBuilder.SetBounds(LatLngBounds.Southwest.Latitude, LatLngBounds.Southwest.Longitude,
                           LatLngBounds.Northeast.Latitude, LatLngBounds.Northeast.Longitude);

                             loadedProducts = await _markerUrlBuilder.LoadDataAsync();
                         }
                        
                         IEnumerable<ProductMarkerModel> newProductsInBound;
                         if (IsInternetConnection)
                         {
                             newProductsInBound = loadedProducts.Where(x => ProductsInBounds.All(y => x.Id != y.Id));
                         }
                         else
                         {
                             newProductsInBound = DbConnection.GetDataList<ProductMarkerModel>(x => !ProductsInBounds.Contains(x));
                         }
                         LoadMarkerIntoMap(newProductsInBound);
                        // AddProductsToCache(loadedProducts);
                     }
                     catch (Exception e)
                     {
                         Debugger.Break();
                     }
                 });
                return true;             
            }
            return false;
        }
       
       private async void LoadMarkerIntoMap(IEnumerable<ProductMarkerModel> products)
        {
           await Task.Run(() => {
            foreach (var product in products)
            {
                var lat = double.Parse(product.Lat, CultureInfo.InvariantCulture);
                var lng = double.Parse(product.Lng, CultureInfo.InvariantCulture);
                Log.Debug(TAG, "CurrentCategories count "+ _currentCategories.Count);
                var catMarker = _currentCategories.FirstOrDefault(x => product.Categories.Any(y => y == x.Id))?.Marker;
                Log.Info(TAG, product.Id + " : " + product.Name + " : " + catMarker);
                string catPinUrl = Utils.RESOURCE_PATH+ catMarker;   // ContentResolver.SchemeAndroidResource + "://" + Activity.PackageName + "/drawable/" + catMarker;
                string customPinUrl = product.ProdimgPin;
                Log.Debug(TAG, "Set customPin ");
                var options = new MarkerOptions();
                options.SetPosition(new LatLng(lat, lng));
                options.Visible(false);
                options.SetTitle(product.Id.ToString());
                options.SetSnippet(catMarker);
                 
                Log.Debug(TAG, "Set Marker Options.");
           
                Activity.RunOnUiThread(() =>
                {
                    var marker = _map.AddMarker(options);
                    var picassoMarker = new PicassoMarker(marker);

                    if (string.IsNullOrEmpty(customPinUrl))
                        customPinUrl = catPinUrl;

                    Picasso.With(Activity).Load(customPinUrl).Resize(0, markerResizeIndex).Into(picassoMarker);
                  
                    Log.Debug(TAG, "Added Marker ");

                });

                ProductsInBounds.Add(product);
               }
            });
        }
        public void SetData(List<CategoryModel> currentCategories)
        {
           //Reload markers if catecories changed
            if (_map != null&&!this._currentCategories.Equals(currentCategories))
            {
                _map.Clear();              
                ProductsInBounds.Clear();                      
            }   
            if(currentCategories.Count==0)
            {
                this._currentCategories = DbConnection.GetDataList<CategoryModel>(x => x.IsSelected && x.Group == NohandicapLibrary.SubCatGroup);
            }
            else
            {
                this._currentCategories = currentCategories;
            }
            _markerUrlBuilder.SubCategoriesList = currentCategories.Select(x => x.Id).ToList();
        }
       
        public override void OnResume()
        {
            base.OnResume();
           _mapView.OnResume();
        }            
      
        public async override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {
                _markerUrlBuilder.LanguageId = CurrentLang.Id;
                _markerUrlBuilder.MainCategoryId = SelectedMainCategory.Id;
                _markerUrlBuilder.SubCategoriesList = _currentCategories.Select(x => x.Id).ToList();
                await LoadData();           
            }
            isShownNoInternet = false;
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            _map.UiSettings.MyLocationButtonEnabled = true;
            _map.MyLocationEnabled = true;
            _map.UiSettings.MapToolbarEnabled = true;
            _map.UiSettings.ZoomControlsEnabled = true;
            _map.SetInfoWindowAdapter(this);
            _map.CameraIdle += Map_CameraIdle;

            _map.InfoWindowClick += (s, e) => {
                var product = FindProductFromMarker((Marker)e.Marker);
                var detailActivity = new Intent(Activity, typeof(DetailActivity));
               detailActivity.PutExtra(Utils.PRODUCT_ID, product.Id);
               Activity.StartActivity(detailActivity);
            };
          
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();         
                        
            if (MainActivity.CurrentLocation != null)
            {
                var myLocation = MainActivity.CurrentLocation;
                var lat = myLocation.Latitude;
                var lng = myLocation.Longitude;
                builder.Target(new LatLng(lat, lng)).Zoom(13);
            }
            else
            {
                builder.Target(new LatLng(48.2274656, 16.4067023)).Zoom(10); //Vienna
            }

            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            _map.MoveCamera(cameraUpdate);     
        }

        private async void Map_CameraIdle(object sender, EventArgs arg)
        {
            if (!IsInternetConnection && !isShownNoInternet) //if not internet then dont ask server and waste time for timeout, display toast information instead
            {
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.server_not_responding), ToastLength.Short).Show();
                isShownNoInternet = true;
            }

            //Make LoadData in queue 
            await SemaphoreSlim.WaitAsync();
            try
            {
                await LoadData();
            }
            catch (Exception e)
            {
                Log.Error(TAG, "OnCameraChanged: " + e.Message);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                SemaphoreSlim.Release();
            }
        }

        #region InfoWindowAdapter
        public View GetInfoContents(Marker marker)
        {
            Log.Debug(TAG, "Get Infowindows");
            var info = _inflater.Inflate(Resource.Layout.infoWindow, null);
            var product = FindProductFromMarker(marker);
            if (product == null) return null;
            Log.Debug(TAG, "Product id"+product.Id);

            var imageView = info.FindViewById<ImageView>(Resource.Id.info_mainImageView);
            var title = info.FindViewById<TextView>(Resource.Id.info_titleTextView);
            var adress = info.FindViewById<TextView>(Resource.Id.info_adressTextView);
            var  catImage = _currentCategories.FirstOrDefault(x => product.Categories.Any(y => y == x.Id));
            try
            {
                string tooltipProdImg = product.ProdImg;

                if (string.IsNullOrEmpty(tooltipProdImg) && !string.IsNullOrEmpty(marker.Snippet))
                {
                    var name = marker.Snippet.Replace("marker_", ""); //remove that marker prefix from name
                    tooltipProdImg = Utils.RESOURCE_PATH + name;
                    imageView.SetBackgroundColor(Color.ParseColor(catImage.Color));
                }
                else
                {
                    imageView.SetBackgroundColor(Color.White);
                }

                Picasso.With(Activity).Load(tooltipProdImg).Placeholder(Resource.Drawable.placeholder).Resize(50, 50).Into(imageView, 
                    new CustomCallback(() =>
                {
                    if (marker.IsInfoWindowShown)
                    {
                        marker.HideInfoWindow();
                        marker.ShowInfoWindow();
                    }
                    
                }));

            }
            catch (System.Exception e)
            {
                Log.Error(TAG, e.Message);
            }

            title.Text = product.Name;
            adress.Text = product.Address;
            Log.Debug(TAG, "return infowindow");

            return info;
        }

        public View GetInfoWindow(Marker marker)
        {
            return null;
        }

        private ProductMarkerModel FindProductFromMarker (Marker marker)
        {
            ProductMarkerModel product;
            product = ProductsInBounds.FirstOrDefault(x => x.Id.ToString() == marker.Title);

            if (product == null)
                product = DbConnection.GetDataList<ProductMarkerModel>(x => x.Id.ToString() == marker.Title).FirstOrDefault();

            return product;
        }
        #endregion

        #region Menu implementation
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            if (!IsTablet)
                inflater.Inflate(Resource.Menu.map_menu, menu);

            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    MainActivity.SetCurrentTab(0);
                    break;
                case Resource.Id.select_all:    
                    DbConnection.UnSelectAllCategories();                  
                    SetData(DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup));                    
                    MainActivity.SupportActionBar.Title = "Map";
                    break;
            }

            Task.Run(async () => { await LoadData(); }).Wait();

            return true;
        }
        
        public void OnMenuItemSelected(int menuItemId)
        {

        }

        #endregion
       
    }
    
}