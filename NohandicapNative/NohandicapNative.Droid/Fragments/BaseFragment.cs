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

namespace NohandicapNative.Droid.Fragments
{
    public abstract class BaseFragment : Android.Support.V4.App.Fragment
    {
        public  SqliteService DbConnection { get; set; }

        public BaseFragment(Boolean loadFromCache = true)
        {
            DbConnection = Utils.GetDatabaseConnection();

            if(loadFromCache)
                ThreadPool.QueueUserWorkItem(o => LoadCache());
        }
        public MainActivity MainActivity
        {
            get
            {
                return NohandicapApplication.MainActivity;
            }
        }
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
        public bool IsInternetConnection
        {
            get
            {
                return NohandicapApplication.IsInternetConnection;
            }
        }

        public CategoryModel SelectedMainCategory
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

        public bool IsTablet {
            get
            {
                return NohandicapApplication.IsTablet;
            }
            set
            {
                NohandicapApplication.IsTablet = value;
            }
        }

        private async void LoadCache()
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

                var coll = await RestApiService.GetMarkers(NohandicapApplication.SelectedMainCategory, selectedSubCategory, NohandicapApplication.CurrentLang.Id, lat, lng, 1);
                AddProductsToCache(coll);

            }
            catch (System.Exception e)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

            }
        }
        public async void AddProductsToCache(List<ProductMarkerModel> products)
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
                            CurrentProductsList.FirstOrDefault(x => x.Id == prod.Id).Distance = prod.Distance;
                        }
                    }
                }
            });
        }

    }
}