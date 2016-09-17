

using System;
using System.Collections.Generic;
using System.Text;


namespace NohandicapNative
{
    public class NohandicapLibrary
    {
        public const string LINK_MAIN= "http://www.stage.nohandicap.net";
        public const string LINK_LANGUAGE = "http://www.stage.nohandicap.net/cms/component/placemap/ajax/api.php?action=getlang";
        public const string LINK_CATEGORY = "http://www.stage.nohandicap.net/cms/component/placemap/ajax/api.php?action=getcat&idlang=";
        public const string LINK_PRODUCT = "http://www.stage.nohandicap.net/cms/component/placemap/ajax/api.php?action=getprod&idlang=";
        public const string LINK_LOGIN = "http://www.stage.nohandicap.net/cms/index.php?item=artikel&id=1&com=login&action=login&type=app";
        public const string LINK_SAVEFAV = "http://www.stage.nohandicap.net/cms/component/placemap/ajax/api.php?action=savefave&iduser={0}&idprod={1}";
        public const string LINK_SIGN_UP = "http://www.stage.nohandicap.net/cms/index.php?item=artikel&id=73&com=login&action=register&type=app";
        public const string LINK_GET_UPDATE = "http://www.stage.nohandicap.net/cms/component/placemap/ajax/api.php?action=getlastupdated";
        public const string LINK_DELFAV = " http://www.stage.nohandicap.net/cms/component/placemap/ajax/api.php?action=delfave&iduser={0}&idprod={1}";
        public const string PRODUCT_TABLE = "Products";
        public const string CATEGORY_TABLE = "Categories";
        public const string LANGUAGE_TABLE = "Language";

        public static List<TabItem> GetTabs()
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
            return items;
        }
        public static List<CategoryModel> GetAdditionalCategory()
        {
            List<CategoryModel> items = new List<CategoryModel>();
            string[] buttonTitles = { "Kunsnt&Kultur", "Museen", "Shoppen", "Sport&Welsness", "Essen&Trinken", "Nächtigen", "Nützliches", "Sightseeing", "Location" };

            string[] buttonColors = { "#FF2C0C05", "#FFF0634B", "#FF60A42D", "#FFE78F02", "#FF7B0D0B", "#FFE78F02", "#FF60A42D", "#FFF0634B", "#FF2C0C05" };

            string[] buttonImage = { "kunst", "museum", "shop", "sport", "eat", "sleep", "search", "event", "location" };
            int[] buttonId = {6, 9,7, 8, 5, 4,12, 11, 10 };

            for (int i = 0; i < buttonTitles.Length; ++i)
            {
                items.Add(new CategoryModel()
                {
                    ID =buttonId[i],
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
            string[] buttonTitles = { "Barrierenfrei", "Teilweise Behinderten gerecht", "Total Behinderten gerecht"};

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
