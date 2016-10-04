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
using Com.Google.Maps.Android.Clustering.View;
using Com.Google.Maps.Android.Clustering.Algo;

using static Android.Gms.Maps.GoogleMap;
using NohandicapNative.Droid.Model;
using static Com.Google.Maps.Android.Clustering.ClusterManager;
using Java.Lang;
using System.Threading;
using Square.Picasso;

namespace NohandicapNative.Droid
{
    public class GMapFragment : Android.Support.V4.App.Fragment, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback,IOnCameraChangeListener
    {
      

        static readonly string TAG = "X:" + typeof(GMapFragment).Name;
        MainActivity mainActivity;
        LayoutInflater inflater;
      //  MapView mapView;
        GoogleMap map;
       
        List<Marker> markersList;
        List<ProductModel> products;
        List<MarkerOptions> markerOptons;
        //MapFragment mapFragment;
        MapView mapView;
        List<CategoryModel> currentCategories;
        List<ProductModel> productsInBounds;
        LatLngBounds latLngBounds = null;
   
        ClusterManager _clusterManager;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);
        CameraPosition currentCameraPosition;
        private readonly object syncLock = new object();
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.inflater = inflater;
            var view = inflater.Inflate(Resource.Layout.MapPage, container, false);
            view.SetBackgroundColor(mainActivity.Resources.GetColor(Resource.Color.backgroundColor));
            HasOptionsMenu = true;
            mapView = view.FindViewById<MapView>(Resource.Id.map);
            mapView.OnCreate(savedInstanceState);
            mapView.OnResume();
            mapView.GetMapAsync(this);
            //   mapFragment = mainActivity.FragmentManager.FindFragmentById(Resource.Id.map).JavaCast<MapFragment>();
          var  dbCon = Utils.GetDatabaseConnection();
            //  mapFragment.GetMapAsync(this);
            var startList = dbCon.GetDataList<CategoryModel>().Where(x=>x.IsSelected).ToList();
            if (startList.Count != 0)
            {
                SetData(startList);
            }
            else
            {
                SetData(dbCon.GetDataList<CategoryModel>());
            }
            dbCon.Close();
            // Updates the location and zoom of the MapView
            return view;
        }
        public GMapFragment()
        {
            mainActivity = NohandicapApplication.MainActivity;
          var  dbCon = Utils.GetDatabaseConnection();
            markersList = new List<Marker>();
            markerOptons = new List<MarkerOptions>();
            int mainCategorySelected= int.Parse(Utils.ReadFromSettings(NohandicapApplication.MainActivity, Utils.MAIN_CAT_SELECTED_ID, "1"));                    
            products = dbCon.GetDataList<ProductModel>().Where(x => x.MainCategoryID >= mainCategorySelected).ToList();
           productsInBounds = new List<ProductModel>();
            dbCon.Close();
       

        }
        public async Task<bool> LoadData()
        {

            if (mainActivity != null & map != null)
            {

                    await Task.Run(() =>
                    {
                        mainActivity.RunOnUiThread(() =>
                        {
                            latLngBounds = map.Projection.VisibleRegion.LatLngBounds;

                        });


                    }).ContinueWith(async t =>
                     {

                         var productsForCategories = products.Where(x => x.Categories.Any(y => currentCategories.Any(z => z.ID == y))).ToList();
                         //Reload task untill latLngBounds not null
                         if (latLngBounds == null)
                         {
                             await semaphoreSlim.WaitAsync();
                             try
                             {
                                 await LoadData();

                             }
                             finally
                             {
                                 //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                                 //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                                 semaphoreSlim.Release();
                             }
                         }
                         //---------------
                         var newProductsInBound = productsForCategories
                             .Where(x => latLngBounds.Contains(new LatLng(
                             double.Parse(x.Lat, CultureInfo.InvariantCulture),
                             double.Parse(x.Long, CultureInfo.InvariantCulture))) && !productsInBounds.Contains(x))
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
                             var lng = double.Parse(product.Long, CultureInfo.InvariantCulture);
                             var clusterItem = new ClusterItem(lat, lng);

                             var catMarker = currentCategories.FirstOrDefault(x => product.Categories.Any(y => y == x.ID)).Marker;

                             Bitmap markerImg;
                             if (string.IsNullOrEmpty(product.ProductMarkerImg))
                             {

                                 var drawImage = Utils.SetDrawableSize(mainActivity, Utils.GetImage(mainActivity, catMarker), 32, 34);
                                 markerImg = Utils.convertDrawableToBitmap(drawImage);

                             }
                             else
                             {
                                 markerImg = await Utils.LoadBitmapAsync(product.ProductMarkerImg);
                                 clusterItem.Icon = BitmapDescriptorFactory.FromBitmap(markerImg);
                             }
                             clusterItem.Icon = BitmapDescriptorFactory.FromBitmap(markerImg);
                             clusterItem.ProductId = product.ID;

                             productsInBounds.Add(product);

                             _clusterManager.AddItem(clusterItem);
                         }

                     }).ContinueWith(t =>
                     {

                         mainActivity.RunOnUiThread(() =>
                         {
                             _clusterManager.Cluster();//Workaround show markers after add new
                         });

                     });

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
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
        

        }       
      
        public override void OnDestroy()
        {
            base.OnDestroy();
        
        }
        public async  override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {
                if (products.Count == 0)
                {
                    var dbCon = Utils.GetDatabaseConnection();
                    products = dbCon.GetDataList<ProductModel>();
                    dbCon.Close();
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
                var activity = new Intent(mainActivity, typeof(DetailActivity));
                activity.PutExtra(Utils.PRODUCT_ID, product.ID);
                mainActivity.StartActivity(activity);
            };
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();                     
            if (NohandicapApplication.MainActivity.CurrentLocation != null)
            {
                var myLocation = NohandicapApplication.MainActivity.CurrentLocation;
                var lat = myLocation.Latitude;
                var lng = myLocation.Longitude;
                builder.Target(new LatLng(lat, lng)).Zoom(15);
            }
            else
            {
                builder.Target(new LatLng(48.2205998, 16.2399776)).Zoom(15);
            }
          
            _clusterManager = new ClusterManager(mainActivity, map);      
                 
            _clusterManager.SetRenderer(new ClusterIconRendered(mainActivity, map, _clusterManager));
            map.SetOnCameraChangeListener(this);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
            map.MoveCamera(cameraUpdate);
        

        }

        string lastmarker = null;
        public View GetInfoContents(Marker marker)
        {
            if (marker.Title == null) return null;
          
            var info = inflater.Inflate(Resource.Layout.infoWindow, null);
            var product = FindProductFromMarker(marker);
            var imageView = info.FindViewById<ImageView>(Resource.Id.info_mainImageView);
            var title= info.FindViewById<TextView>(Resource.Id.info_titleTextView);
            var adress = info.FindViewById<TextView>(Resource.Id.info_adressTextView);          
            var mainimage = product.ImageCollection.Images;
            if (mainimage.Count != 0)
            {
                var img = mainimage[0];
                try
                {
                    if (img!=null)
                    {
                        Picasso.With(mainActivity).Load(img).Placeholder(Resource.Drawable.placeholder).Into(imageView,new CustomCallback(()=>
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
    
        public async void LoadImageAsync(ImageView imageView, string url,Marker marker)
        {
            await Utils.LoadBitmapAsync(url);
            imageView.SetBackgroundResource(Resource.Drawable.placeholder);
            Uri uri = new Uri(url);
           var filename = System.IO.Path.GetFileName(uri.LocalPath);
            imageView.SetImageBitmap(Utils.GetBitmap(filename));

            

        }
        #region InfoWindowAdapter
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
                    mainActivity.SetCurrentTab(0);
                    break;
                case Resource.Id.select_all:
                    var dbCon = Utils.GetDatabaseConnection();
                    dbCon.UnSelectAllCategories();                  
                    SetData(dbCon.GetDataList<CategoryModel>());
                    dbCon.Close();
                    mainActivity.SupportActionBar.Title = "Map";
                             
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