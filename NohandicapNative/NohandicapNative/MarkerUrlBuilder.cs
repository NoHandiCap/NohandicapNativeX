using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace NohandicapNative
{
    public class MarkerUrlBuilder
    {
        #region ctor

        public MarkerUrlBuilder()
        {
          
            _isBoundsEnaled = false;
            SubCategoriesList = new List<int>();
            CountMarkersToLoad = NohandicapLibrary.DefaultCountMarkersToLoad;
            PageNumber = 1;
        }

        #endregion

        #region Private properties

        private int _mainCategoryId;
        private string _url;
        private bool _isBoundsEnaled;

        #endregion

        #region Public properties

        public double SouthwestLatitude { get; set; }
        public double SouthwestLongitude { get; set; }
        public double NortheastLatitude { get; set; }
        public double NortheastLongitude { get; set; }
        public double MyLocationLatitude { get; set; }
        public double MyLocationLongitude { get; set; }
        public List<int> SubCategoriesList { get; set; }
        public int PageNumber { get; set; }
        public int CountMarkersToLoad { get; set; }

        public string Url => _url?? GenerateUrl();

        public int MainCategoryId
        {
            get
            {
                if (_mainCategoryId == 0)
                    return NohandicapLibrary.DEFAULT_MAIN_CATEGORY; //default main category in case of unselected maincategory

                return _mainCategoryId;
            }
            set { _mainCategoryId = value; }
        }

        public int LanguageId { get; set; }

        #endregion

        #region Public methods

        public void SetBounds(double southwestLatitude, double southwestLongitude, double northeastLatitude,
            double northeastLongitude)
        {
            SouthwestLatitude = southwestLatitude;
            SouthwestLongitude = southwestLongitude;
            NortheastLatitude = northeastLatitude;
            NortheastLongitude = northeastLongitude;
            _isBoundsEnaled = true;
        }

        public void SetMyLocation(double myLocationLatitude, double myLocationLongtitude)
        {
            MyLocationLatitude = myLocationLatitude;
            MyLocationLongitude = myLocationLongtitude;
           
        }

        public async Task<IEnumerable<ProductMarkerModel>> LoadDataAsync()
        {
           _url=  GenerateUrl();
            var products = await RestApiService.GetDataFromUrl<IEnumerable<ProductMarkerModel>>(_url);
            if (products == null)
            {
                return new List<ProductMarkerModel>();
            }
            return products;
        }

        #endregion

        #region Private methods

        private static string PrepareSubCategoryString(List<int> subCategories)
        {
            string subCatList = "";

            // checking against null and empty (and if then use default subcategory)
            if (subCategories.Count == 0)
            {
                subCategories.Add(NohandicapLibrary.DEFAULT_SUB_CATEGORY);
            }

            //join all subcategories with comma
            foreach (var item in subCategories)
                subCatList += item + ",";

            //take the last comma
            subCatList = subCatList.Substring(0, subCatList.Length - 1);

            return subCatList;
        }

        private string GenerateUrl()
        {
           
            string boundBox = "";

            boundBox = SouthwestLatitude.ToString(CultureInfo.InvariantCulture) + "," +
                       SouthwestLongitude.ToString(CultureInfo.InvariantCulture) + "," +
                       NortheastLatitude.ToString(CultureInfo.InvariantCulture) + "," +
                       NortheastLongitude.ToString(CultureInfo.InvariantCulture);
                //invariantculture to have double with "." and not with ","
            
           var url = string.Format(NohandicapLibrary.LINK_GET_MARKERS_GPS, MainCategoryId, CountMarkersToLoad, LanguageId,
                MyLocationLatitude.ToString(CultureInfo.InvariantCulture),
                MyLocationLongitude.ToString(CultureInfo.InvariantCulture), boundBox,
                PrepareSubCategoryString(SubCategoriesList), PageNumber);
            return url;
        }

        #endregion
    }
}
