

using System;
using System.Collections.Generic;
using System.Text;


namespace NohandicapNative
{
    public class NohandicapLibrary
    {
        public const string COM = "placemap";
        public const string LOGIN_ARTIKEL_ID = "1";
        public const string REGISTER_ARTIKEL_ID = "73";
        public const string FACEBOOK_APP_KEY_API = "105055836622734";

        public const string LINK_MAIN           = "http://www.stage.nohandicap.net/cms/";
        public const string LINK_API            = LINK_MAIN + "component/" + COM + "/ajax/api.php?";

        public const string LINK_LANGUAGE       = LINK_API + "action=getlang";
        public const string LINK_CATEGORY       = LINK_API + "action=getcat&idlang=";
        public const string LINK_PRODUCT_DETAIL = LINK_API + "action=getproduct&idlang={0}&idprod={1}";
        public const string LINK_GET_UPDATE     = LINK_API + "action=getlastupdated";
        public const string LINK_DELFAV         = LINK_API + "action=delfave&iduser={0}&idprod={1}";
        public const string LINK_SAVEFAV        = LINK_API + "action=savefave&iduser={0}&idprod={1}";
        public const string LINK_SAVEPRODVIEW   = LINK_API + "action=saveproductview&idprod={0}";
        public const string LINK_SAVEAPPSTATI   = LINK_API + "action=saveappstatistic";   //POST:appid,viewname,clickid,clickaction,createdate,iduser
        public const string LINK_GET_MARKERS    = LINK_API + "action=getprodlist&typemap=true&listfave=false&maincat={0}&search=&num={1}&bbox={2}&cat={3}&app=true";
        public const string LINK_GET_PRODUCTS   = LINK_API + "action=getprodlist&typemap=true&listfave=false&maincat={0}&search=&cat={1}&idlang={2}&gpslat={3}&gpslng={4}&num={5}&page={6}&app=true";
        public const string LINK_GET_FAV        = LINK_API + "action=getprodlist&typemap=true&listfave=true&iduser={0}&search=&num={1}&page={2}&app=true";

        public const string LINK_LOGIN          = LINK_MAIN + "index.php?item=artikel&id="+ LOGIN_ARTIKEL_ID + "&com=login&action=login&type=app";
        public const string LINK_SIGN_UP        = LINK_MAIN + "index.php?item=artikel&id="+ REGISTER_ARTIKEL_ID + "&com=login&action=register&type=app";
        public const string LINK_SIGN_UP_WITH_FACEBOOK  = LINK_MAIN + "index.php?item=artikel&id=" + REGISTER_ARTIKEL_ID + "&com=login&action=registerorloginwithfacebook&type=app";


        public const string PRODUCT_TABLE   = "Products";
        public const string CATEGORY_TABLE  = "Categories";
        public const string LANGUAGE_TABLE  = "Language";
        public const int MainCatGroup = 2;
        public const int SubCatGroup = 1;

        public const int DEFAULT_MAIN_CATEGORY = 1; //as fallback, should be taken from DB by sync (reihe=1, gruppe=2)
        public const int DEFAULT_SUB_CATEGORY = 6; //as fallback, should be taken from DB by sync (reihe=1, gruppe=1)

        public const String DEFAULT_LANG_CODE = "de";

        public static List<TabItem> GetTabs(bool isTablet=false)
        {
            List<TabItem> items = new List<TabItem>();
            string[] tabTitles = { "Home", "Map", "List", "Favorites" };

            string[] tabColors = { "#FF74032C", "#FF74032C", "#FF74032C", "#FF74032C" };
            
            for (int i = 0; i < tabTitles.Length; ++i)
            {
                items.Add(new TabItem()
                {
                    Id = i,
                    Title = tabTitles[i],
                    Color = tabColors[i],
                    Image = string.Format("ic_{0}", tabTitles[i].ToLowerInvariant())
                });
            }
            if (isTablet) items.RemoveAt(1);
            return items;
        }
        public static List<CategoryModel> GetAdditionalCategory()
        {
            List<CategoryModel> items = new List<CategoryModel>();
            string[] buttonTitles = { "Kunst&Kultur", "Museen", "Shoppen", "Sport&Welness", "Essen&Trinken", "Nächtigen", "Nützliches", "Sightseeing", "Location" };
            string[] buttonColors = { "#FF2C0C05", "#FFF0634B", "#FF60A42D", "#FFE78F02", "#FF7B0D0B", "#FFE78F02", "#FF60A42D", "#FFF0634B", "#FF2C0C05" };
            string[] buttonImage = { "kunst", "museum", "shop", "sport", "eat", "sleep", "search", "event", "location" };
            int[] buttonId = {6, 9,7, 8, 5, 4,12, 11, 10 };

            for (int i = 0; i < buttonTitles.Length; ++i)
            {
                items.Add(new CategoryModel()
                {
                    Id =buttonId[i],
                    Name = buttonTitles[i],
                    Color = buttonColors[i],
                    Icon = buttonImage[i]
                });
            }
            return items;
        }
        public static List<TabItem> GetMainCategory()
        {
            List<TabItem> items = new List<TabItem>();
            string[] buttonTitles = { "Barrierenfrei", "Teilweise Behindertengerecht", "Total Behindertengerecht"};

            string[] buttonColors = { "#37474F", "#37474F", "#37474F" };

            string[] buttonImage = { "wheelchair", "wheelchair2", "wheelchair3" };
            for (int i = 0; i < buttonTitles.Length; ++i)
            {
                items.Add(new TabItem()
                {
                    Id = i,
                    Title = buttonTitles[i],
                    Color = buttonColors[i],
                    Image = buttonImage[i]
                });
            }
            return items;
        }
        public static string ConvertMetersToKilometers(float meters)
        {
            if (meters < 1000)
            {
                return meters + " m";
            }
            else
            {
                return String.Format("{0:0.00}km", meters / 1000);
            }
        }

    }
}
