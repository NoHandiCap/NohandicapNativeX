using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Services;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Globalization;
using NohandicapNative.Droid.Activities;

namespace NohandicapNative.Droid.Fragments
{
    public abstract class BaseFragment : Android.Support.V4.App.Fragment
    {
        private readonly SpinnerFragment _spinnerFragment;
        
        public  SqliteService DbConnection { get; private set; }

        protected BaseFragment(bool loadFromCache = true)
        {
            DbConnection = Utils.GetDatabaseConnection();
            _spinnerFragment = new SpinnerFragment();
            if (loadFromCache)
                ThreadPool.QueueUserWorkItem(o => LoadCache());
        }
        public MainActivity MainActivity => NohandicapApplication.MainActivity;

        public ObservableCollection<ProductMarkerModel> CurrentProductsList
        {
            get
            {
                return NohandicapApplication.MainActivity.CurrentProductsList;
            }
            set
            {
                NohandicapApplication.MainActivity.CurrentProductsList = value;
            }
        }
        public LanguageModel CurrentLang
        {
            get
            {
                return NohandicapApplication.CurrentLang;
            }
            set
            {
                NohandicapApplication.CurrentLang = value;
            }
        }

        protected static bool IsInternetConnection => NohandicapApplication.IsInternetConnection;

        public static CategoryModel SelectedMainCategory
        {
            get
            {
                return NohandicapApplication.SelectedMainCategory;
            }
            set
            {
                NohandicapApplication.SelectedMainCategory = value;
            }

        }

        protected static bool IsTablet {
            get
            {
                return NohandicapApplication.IsTablet;
            }
            set
            {
                NohandicapApplication.IsTablet = value;
            }
        }

        private void LoadCache()
        {
            try
            {
                var selectedSubCategory = DbConnection.GetSubSelectedCategory();
                var position = NohandicapApplication.MainActivity.CurrentLocation;
                string lat = "";
                string lng = "";
                if (position != null)
                {
                    lat = position.Latitude.ToString(CultureInfo.InvariantCulture);
                    lng = position.Longitude.ToString(CultureInfo.InvariantCulture);
                }

              //  var coll = await RestApiService.GetMarkers(NohandicapApplication.SelectedMainCategory, selectedSubCategory, NohandicapApplication.CurrentLang.Id, lat, lng, 1);
               // AddProductsToCache(coll);

            }
            catch (System.Exception e)
            {
                // ignored
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
            }
        }
        public async void AddProductsToCache(IEnumerable<ProductMarkerModel> products)
        {
           await Task.Run(() =>
            {
                foreach (var prod in products)
                {
                    if (!CurrentProductsList.Contains(prod))
                    {
                        CurrentProductsList.Add(prod);
                    }
                    else
                    {
                        if (CurrentProductsList.Any(x => x.Distance != prod.Distance))
                        {
                            var productMarkerModel = CurrentProductsList.FirstOrDefault(x => x.Id == prod.Id);
                            if (productMarkerModel != null)
                                productMarkerModel.Distance = prod.Distance;
                        }
                    }
                }
            });
        }
        public void ShowSpinner(bool visibility)
        {
           
            if (visibility)
            {
                MainActivity.SupportFragmentManager.BeginTransaction().Add(Resource.Id.flContent, _spinnerFragment).Commit();

            }
            else
            {
                MainActivity.SupportFragmentManager.BeginTransaction().Remove(_spinnerFragment).Commit();
            }

        }

    }
}