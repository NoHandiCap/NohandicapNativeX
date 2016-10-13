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
using Com.Google.Maps.Android.Clustering;
using static Android.Gms.Maps.GoogleMap;
using NohandicapNative.Droid.Model;
using System.Threading;
using Square.Picasso;

namespace NohandicapNative.Droid
{
    public class GMapFragment : Android.Support.V4.App.Fragment, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback,IOnCameraChangeListener
    {     

        static readonly string TAG = "X:" + typeof(GMapFragment).Name;
        LayoutInflater inflater;
        GoogleMap map;       
        List<Marker> markersList;
        List<ProductModel> products;
        List<MarkerOptions> markerOptons;    
        MapView mapView;
        List<CategoryModel> currentCategories;
        List<ProductMarkerModel> productsInBounds;
        LatLngBounds latLngBounds = null;   
        ClusterManager _clusterManager;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);
        CameraPosition currentCameraPosition;     
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            this.inflater = inflater;          
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            view.SetBackgroundColor(NohandicapApplication.MainActivity.Resources.GetColor(Resource.Color.backgroundColor));
            HasOptionsMenu = true;
            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();
            mapView.GetMapAsync(this);
            try
            {
                var conn = Utils.GetDatabaseConnection();
                markersList = new List<Marker>();
                markerOptons = new List<MarkerOptions>();             
                products = conn.GetDataList<ProductModel>().Where(x => x.MainCategoryID >= NohandicapApplication.SelectedMainCategory.Id).ToList();
                productsInBounds = new List<ProductMarkerModel>();
                var allCategoriesList = conn.GetSubSelectedCategory();
                if (allCategoriesList.Count != 0)
                {
                    SetData(allCategoriesList);
                }
                else
                {
                    SetData(conn.GetDataList<CategoryModel>().Where(x=>x.Group==1).ToList());
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
                       
                        var productsForCategories = await RestApiService.GetProductMarkerListForMap(latLngBounds.Southwest.Latitude, latLngBounds.Southwest.Longitude, latLngBounds.Northeast.Latitude, latLngBounds.Northeast.Longitude,NohandicapApplication.SelectedMainCategory, currentCategories);
                     //var productsForCategories = products.Where(x => x.Categories.Any(y => currentCategories.Any(z => z.Id == y))).ToList();
                     //    //Reload task untill latLngBounds not null
                     //   
                     //}
                     //    //---------------

                     var newProductsInBound = productsForCategories
                     .Where(x => latLngBounds.Contains(new LatLng(
                     double.Parse(x.Lat, CultureInfo.InvariantCulture),
                     double.Parse(x.Lng, CultureInfo.InvariantCulture))) && !productsInBounds.Contains(x))
                     .ToList();
                     if (currentCameraPosition != null)
                     {
                         if (currentCameraPosition.Zoom < 11)
                         {
                             newProductsInBound = newProductsInBound.Take(50).ToList();
                         }
                     }
                     foreach (var product in newProductsInBound)
                     {
                         var lat = double.Parse(product.Lat, CultureInfo.InvariantCulture);
                         var lng = double.Parse(product.Lng, CultureInfo.InvariantCulture);
                         var clusterItem = new ClusterItem(lat, lng);
                         //var catMarker = currentCategories.FirstOrDefault(x => product.Categories.Any(y => y == x.Id)).Marker;
                         //string imageUrl = "";
                         //if (string.IsNullOrEmpty(product.ProductMarkerImg))
                         //{
                         //    imageUrl = ContentResolver.SchemeAndroidResource + "://" + Activity.PackageName + "/drawable/" + catMarker;
                         //}
                         //else
                         //{
                         //    imageUrl = product.ProductMarkerImg;
                         //}
                         var markerImg = Picasso.With(Activity).Load(product.ProdimgPin).Resize(32, 34).Get();
                         clusterItem.Icon = BitmapDescriptorFactory.FromBitmap(markerImg);
                         clusterItem.ProductId = product.Id;
                         //   productsInBounds.Add(product);
                      
                         _clusterManager.AddItem(clusterItem);
                     }
                 }).ContinueWith(t =>
                 {
                     //Activity.RunOnUiThread(() =>
                     //{
                     //    _clusterManager.Cluster();//Workaround show markers after add new
                     //    });

                 }, TaskScheduler.FromCurrentSynchronizationContext());

                return true;

            }
            return false;
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
                _clusterManager.ClearItems();
              
            }
            //----
           this.currentCategories = currentCategories;
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
                if (products.Count == 0)
                {
                    var conn = Utils.GetDatabaseConnection();
                    products = conn.GetDataList<ProductModel>();
                    
                }
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
                activity.PutExtra(Utils.PRODUCT_ID, product.ID);
               Activity.StartActivity(activity);
            };
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();                     
            if (NohandicapApplication.MainActivity.CurrentLocation != null)
            {
                var myLocation = NohandicapApplication.MainActivity.CurrentLocation;
                var lat = myLocation.Latitude;
                var lng = myLocation.Longitude;
                builder.Target(new LatLng(lat, lng)).Zoom(13);
            }
            else
            {
                builder.Target(new LatLng(48.2274656, 16.4067023)).Zoom(10);
            }          
            _clusterManager = new ClusterManager(Activity, map);                    
            _clusterManager.SetRenderer(new ClusterIconRendered(Activity, map, _clusterManager));
            map.SetOnCameraChangeListener(this);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            map.MoveCamera(cameraUpdate);     
        }
        #region InfoWindowAdapter
        public View GetInfoContents(Marker marker)
        {
            var info = inflater.Inflate(Resource.Layout.infoWindow, null);
            var product = FindProductFromMarker(marker);
            var imageView = info.FindViewById<ImageView>(Resource.Id.info_mainImageView);
            var title = info.FindViewById<TextView>(Resource.Id.info_titleTextView);
            var adress = info.FindViewById<TextView>(Resource.Id.info_adressTextView);
            var mainimage = product.ImageCollection.Images;
            if (mainimage.Count != 0)
            {
                var img = mainimage[0];
                try
                {
                    if (img != null)
                    {
                        Picasso.With(Activity).Load(img).Placeholder(Resource.Drawable.placeholder).Resize(50, 50).Into(imageView, new CustomCallback(() =>
                        {
                            if (marker.IsInfoWindowShown)
                            {
                                marker.HideInfoWindow();
                                marker.ShowInfoWindow();
                            }

                        }));

                    }


                }
                catch (System.Exception e)
                {
                    Log.Error(TAG, e.Message);
                }
            }
            title.Text = product.FirmName;
            adress.Text = product.Adress;
            return info;
        }
        public View GetInfoWindow(Marker marker)
        {
            return null;
        }
        private ProductModel FindProductFromMarker (Marker marker)
        {
            return products.FirstOrDefault(x => x.ID.ToString() == marker.Title);
        }
        #endregion

        #region Menu implementation
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            if (!NohandicapApplication.isTablet)
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
                    NohandicapApplication.MainActivity.SetCurrentTab(0);
                    break;
                case Resource.Id.select_all:
                    var conn = Utils.GetDatabaseConnection();
                    conn.UnSelectAllCategories();                  
                    SetData(conn.GetDataList<CategoryModel>().Where(x=>x.Group==1).ToList());
                    
                    NohandicapApplication.MainActivity.SupportActionBar.Title = "Map";                            
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
                    if (await LoadData())
                    {
                        _clusterManager.OnCameraChange(position);                    
                    }
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
            else
            {
                _clusterManager.OnCameraChange(position);
            }
           
        }

     
    }
    
}