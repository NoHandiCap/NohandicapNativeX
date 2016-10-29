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
using CameraPosition = Android.Gms.Maps.Model.CameraPosition;

namespace NohandicapNative.Droid
{
    public class GMapFragment : BaseFragment, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback, IOnCameraChangeListener
    {     

        static readonly string TAG = "X:" + typeof(GMapFragment).Name;
       
        LayoutInflater inflater;   
        GoogleMap map;            
        MapView mapView;
        List<CategoryModel> currentCategories;
        public  ObservableCollection<ProductMarkerModel> ProductsInBounds;
        public LatLngBounds LatLngBounds = null;
       
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);
        CameraPosition currentCameraPosition;
        bool isShownNoInternet = false;

        public GMapFragment(Boolean loadFromCache = true) : base(loadFromCache)
        {
            ProductsInBounds = new ObservableCollection<ProductMarkerModel>();
        }



        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.inflater = inflater;          
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            view.SetBackgroundColor(MainActivity.Resources.GetColor(Resource.Color.backgroundColor));        
            
            HasOptionsMenu = true;
            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();

            if (!NohandicapApplication.CheckIfGPSenabled())
                ShowNoGPSEnabled();
            //else
                //Toast.MakeText(this.Context, "GPS enabled", ToastLength.Short).Show(); //on some phones it shows every few seconds: Wiko i Samsumg Galaxy 

            mapView.GetMapAsync(this); //asynchronic loading map

            try
            {                                         
              
                var selectedCategories = DbConnection.GetSubSelectedCategory();
                if (selectedCategories.Count != 0)
                {
                    SetData(selectedCategories);
                }
                else
                {
                    SetData(DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup));
                }
                
            } catch(Exception e)
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
            if (map != null)
            {
                await Task.Run(() =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        LatLngBounds = map.Projection.VisibleRegion.LatLngBounds;

                    });
                }).ContinueWith(async t =>
                 {
                     try
                     {
                         if (LatLngBounds == null)
                         {
                             await semaphoreSlim.WaitAsync();
                             try
                             {
                                 await LoadData();

                             }
                             finally
                             {
                                 semaphoreSlim.Release();
                             }
                             return;
                         }
                         Log.Debug(TAG, "Start Load ");

                         IEnumerable<ProductMarkerModel> loadedProducts = null;

                         if (MainActivity.CurrentLocation != null)
                         {
                             loadedProducts = await RestApiService.GetMarkers(LatLngBounds.Southwest.Latitude, LatLngBounds.Southwest.Longitude,
                             LatLngBounds.Northeast.Latitude, LatLngBounds.Northeast.Longitude, CurrentLang.Id, MainActivity.CurrentLocation.Latitude, MainActivity.CurrentLocation.Longitude, SelectedMainCategory, currentCategories);
                         }
                         else
                         {
                             loadedProducts = await RestApiService.GetMarkers(LatLngBounds.Southwest.Latitude, LatLngBounds.Southwest.Longitude,
                             LatLngBounds.Northeast.Latitude, LatLngBounds.Northeast.Longitude,
                             SelectedMainCategory, currentCategories);
                         }

                         //       Log.Debug(TAG, "LoadedProducts " + loadedProducts.Count());

                         IEnumerable<ProductMarkerModel> newProductsInBound;
                         if (IsInternetConnection)
                         {
                             newProductsInBound = loadedProducts.Where(x => !ProductsInBounds.Contains(x));
                         }
                         else
                         {
                             newProductsInBound = DbConnection.GetDataList<ProductMarkerModel>(x => !ProductsInBounds.Contains(x));
                         }
                         ProductsInBounds.Clear();
                         LoadMarkerIntoMap(newProductsInBound);
                         AddProductsToCache(loadedProducts);
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
                Log.Debug(TAG, "CurrentCategories count "+ currentCategories.Count);
                var catMarker = currentCategories.FirstOrDefault(x => product.Categories.Any(y => y == x.Id)).Marker;
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
                    var marker = map.AddMarker(options);
                    var picassoMarker = new PicassoMarker(marker);

                    if (string.IsNullOrEmpty(customPinUrl))
                    {
                        customPinUrl = catPinUrl;
                    }

                    Picasso.With(Activity).Load(customPinUrl).Resize(70,70).Into(picassoMarker);

                    
                    ProductsInBounds.Add(product);
                    Log.Debug(TAG, "Added Marker ");

                });
            }
            });
        }
        public void SetData(List<CategoryModel> currentCategories)
        {
           //Reload markers if catecories changed
            if (map != null&&!this.currentCategories.Equals(currentCategories))
            {
                map.Clear();              
                ProductsInBounds.Clear();                      
            }   
            if(currentCategories.Count==0)
            {
                this.currentCategories = DbConnection.GetDataList<CategoryModel>(x => x.IsSelected && x.Group == NohandicapLibrary.SubCatGroup);
            }
            else
            {
                this.currentCategories = currentCategories;
            }
        }
       
        public override void OnResume()
        {
            base.OnResume();
           mapView.OnResume();
        }            
      
        public async override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {              
                await LoadData();           
            }
            isShownNoInternet = false;
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.MyLocationEnabled = true;
            map.UiSettings.MapToolbarEnabled = true;
            map.UiSettings.ZoomControlsEnabled = true;
            map.SetInfoWindowAdapter(this);

            map.InfoWindowClick += (s, e) => {
                var product = FindProductFromMarker((Marker)e.Marker);
                var activity = new Intent(Activity, typeof(DetailActivity));
               activity.PutExtra(Utils.PRODUCT_ID, product.Id);
               Activity.StartActivity(activity);
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
            map.SetOnCameraChangeListener(this);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            map.MoveCamera(cameraUpdate);     
        }

        #region InfoWindowAdapter
        public View GetInfoContents(Marker marker)
        {
            Log.Debug(TAG, "Get Infowindows");
            var info = inflater.Inflate(Resource.Layout.infoWindow, null);
            var product = FindProductFromMarker(marker);
            if (product == null) return null;
            Log.Debug(TAG, "Product id"+product.Id);

            var imageView = info.FindViewById<ImageView>(Resource.Id.info_mainImageView);
            var title = info.FindViewById<TextView>(Resource.Id.info_titleTextView);
            var adress = info.FindViewById<TextView>(Resource.Id.info_adressTextView);
            var  catImage = currentCategories.FirstOrDefault(x => product.Categories.Any(y => y == x.Id));
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
            product = CurrentProductsList.Where(x => x.Id.ToString() == marker.Title).FirstOrDefault();

            if (product == null)
                product = DbConnection.GetDataList<ProductMarkerModel>(x => x.Id.ToString() == marker.Title).FirstOrDefault();

            return product;
        }
        #endregion

        #region Menu implementation
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            if (!IsTablet)
            {
                inflater.Inflate(Resource.Menu.map_menu, menu);
            }
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public  override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    MainActivity.SetCurrentTab(0);
                    break;
                case Resource.Id.select_all:    
                    DbConnection.UnSelectAllCategories();                  
                    SetData(DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.MainCatGroup));                    
                    MainActivity.SupportActionBar.Title = "Map";
                    currentCategories = DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup);
                    OnCameraChange(currentCameraPosition);
                    break;
            }
            return true;
        }
        
        public void OnMenuItemSelected(int menuItemId)
        {

        }

        #endregion
        public async void OnCameraChange(CameraPosition position)
        {
            currentCameraPosition = position;
            if (!IsInternetConnection&&!isShownNoInternet) //if not internet then dont ask server and waste time for timeout, display toast information instead
            {
                Toast.MakeText(this.Activity, Resources.GetString(Resource.String.server_not_responding), ToastLength.Short).Show();
                isShownNoInternet = true;
            }

            //Make LoadData in queue 
            await semaphoreSlim.WaitAsync();
                try
                {                   
                    await LoadData();                  
                }
                catch (Exception e)
                {
                    Log.Error(TAG, "OnCameraChanged: "+e.Message) ;
                }
                finally
                {
                    //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                    //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                    semaphoreSlim.Release();
                }
        }
    }
    
}