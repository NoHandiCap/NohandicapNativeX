using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using Android.Util;
using Android.Gms.Maps;
using System.Globalization;
using System.Threading.Tasks;
using static Android.Gms.Maps.GoogleMap;
using NohandicapNative.Droid.Model;
using System.Threading;
using Square.Picasso;
using Android.Graphics.Drawables;
using System.Collections.ObjectModel;
using NohandicapNative.Droid.Fragments;

namespace NohandicapNative.Droid
{
    public class GMapFragment : BaseFragment, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback,IOnCameraChangeListener
    {     

        static readonly string TAG = "X:" + typeof(GMapFragment).Name;
        LayoutInflater inflater;
        GoogleMap map;       
        List<Marker> markersList;
        private static int DELAY_TIME_IN_MILLI = 500;
        List<MarkerOptions> markerOptons;    
        MapView mapView;
        List<CategoryModel> currentCategories;
        List<ProductMarkerModel> productsInBounds;
        LatLngBounds latLngBounds = null;
        SqliteService conn;
      //  ClusterManager _clusterManager;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);
        CameraPosition currentCameraPosition;     
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            this.inflater = inflater;          
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            view.SetBackgroundColor(MainActivity.Resources.GetColor(Resource.Color.backgroundColor));
            conn = Utils.GetDatabaseConnection();
            HasOptionsMenu = true;
            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();
            mapView.GetMapAsync(this);
            try
            {               
            
                markersList = new List<Marker>();
                markerOptons = new List<MarkerOptions>();             
              //  products = conn.GetDataList<ProductModel>().Where(x => x.MainCategoryID >= NohandicapApplication.SelectedMainCategory.Id).ToList();
                productsInBounds = new List<ProductMarkerModel>();
                var selectedCategories = conn.GetSubSelectedCategory();
                if (selectedCategories.Count != 0)
                {
                    SetData(selectedCategories);
                }
                else
                {
                    SetData(conn.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup));
                }
                
            } catch(Exception e)
            {
                Log.Debug(TAG, "Check Update " + e.Message);
            }
            return view;
        }
        public async Task<bool> LoadData()
        {
            if (map != null)
            {
                await Task.Run(() =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        latLngBounds = map.Projection.VisibleRegion.LatLngBounds;

                    });
                }).ContinueWith(async t =>
                 {
                     if (latLngBounds == null)
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
                     var loadedProducts =
                       await Utils.GetMarkers(latLngBounds.Southwest.Latitude, latLngBounds.Southwest.Longitude,
                       latLngBounds.Northeast.Latitude, latLngBounds.Northeast.Longitude,
                       SelectedMainCategory, currentCategories);
                     Log.Debug(TAG, "LoadedProducts " + loadedProducts.Count);
                     List<ProductMarkerModel> newProductsInBound;
                     if (IsInternetConnection)
                     {
                         newProductsInBound = loadedProducts.Where(x => !productsInBounds.Contains(x)).ToList();                                        
                     }
                     else
                     {
                         newProductsInBound = conn.GetDataList<ProductMarkerModel>(x => !productsInBounds.Contains(x))
                     .ToList();
                     }
                     LoadMarkerIntoMap(newProductsInBound);                                                               
                     AddProductsToCache(loadedProducts);
                 });
                return true;
            }
            return false;
        }
       private async void LoadMarkerIntoMap(List<ProductMarkerModel> prdoucts)
        {
           await Task.Run(() => {
            foreach (var product in prdoucts)
            {
                var lat = double.Parse(product.Lat, CultureInfo.InvariantCulture);
                var lng = double.Parse(product.Lng, CultureInfo.InvariantCulture);
                   Log.Debug(TAG, "CurrentCategories count "+ currentCategories.Count);
                   var catMarker = currentCategories.FirstOrDefault(x => product.Categories.Any(y => y == x.Id)).Marker;
                string catPinUrl = ContentResolver.SchemeAndroidResource + "://" + Activity.PackageName + "/drawable/" + catMarker;
                string customPinUrl = product.ProdimgPin;
                Log.Debug(TAG, "Set customPin ");

                var options = new MarkerOptions();
                options.SetPosition(new LatLng(lat, lng));
                options.Visible(false);
                options.SetTitle(product.Id.ToString());
                 
                Log.Debug(TAG, "SetOptions ");
                Activity.RunOnUiThread(() =>
                {
                    var marker = map.AddMarker(options);                  
                    var picassoMarker = new PicassoMarker(marker);
                    if (!string.IsNullOrEmpty(customPinUrl))
                    {
                        Picasso.With(Activity).Load(customPinUrl).Into(picassoMarker);
                    }
                    else
                    {
                        Picasso.With(Activity).Load(catPinUrl).Into(picassoMarker);
                    }
                    markersList.Add(marker);
                    productsInBounds.Add(product);
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
                markerOptons.Clear();
                markersList.Clear();
                productsInBounds.Clear();                      
            }   
            if(currentCategories.Count==0)
            {

                this.currentCategories = conn.GetDataList<CategoryModel>(x => x.IsSelected && x.Group == NohandicapLibrary.SubCatGroup);

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
      
        public async  override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {              
                await LoadData();           
            }
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
                builder.Target(new LatLng(48.2274656, 16.4067023)).Zoom(10);
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
            try
            {
                Picasso.With(Activity).Load(product.ProdImg).Placeholder(Resource.Drawable.placeholder).Resize(50, 50).Into(imageView, 
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
            {
                product = conn.GetDataList<ProductMarkerModel>(x => x.Id.ToString() == marker.Title).FirstOrDefault();

            }

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
                    conn.UnSelectAllCategories();                  
                    SetData(conn.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.MainCatGroup));                    
                    MainActivity.SupportActionBar.Title = "Map";                            
                      LoadData();                 
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
            if (position.Zoom > 8)
            {
                //Make LoadData in queue 
                await semaphoreSlim.WaitAsync();
                try
                {                   
                        await LoadData();                  
                   
                }catch(Exception e)
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
    
}