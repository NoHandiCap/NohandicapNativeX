using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using SQLite.Net.Interop;
using System.Linq;

namespace NohandicapNative
{
    public class SqliteService
    {
        public const string DB_NAME = "nohandicap.db3";

        private string path;
        ISQLitePlatform platform;
        public SqliteService(ISQLitePlatform platform, string path)
        {
            this.platform = platform;
            this.path = Path.Combine(path, DB_NAME);


        }
        private SQLiteConnection GetSQLiteConnetion()
        {
            var conn = new SQLiteConnection(platform, path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.FullMutex, true);
            return conn;
        }

        public bool CreateTables()
        {
            try
            {
                using (var conn = GetSQLiteConnetion())
                {
                    conn.CreateTable<ImageJsonModel>();
                    conn.CreateTable<CategoryModel>();
                    conn.CreateTable<LanguageModel>();
                    conn.CreateTable<UserModel>();
                    conn.CreateTable<ProductDetailModel>();
                    conn.CreateTable<ProductMarkerModel>();
                }

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

                return false;
            }

        }
        public string InsertUpdateProduct<T>(T data)
        {
            using (var conn = GetSQLiteConnetion())
            {
                try
                {
                    conn.InsertOrReplaceWithChildren(data,true);

                }
                catch (Exception e)
                {

                }
            }
            return "Single data file inserted or updated";
        }
        public string InsertUpdateProductList<T>(List<T> data)
        {
            using (var conn = GetSQLiteConnetion())
            {
                conn.InsertOrReplaceAllWithChildren(data,true);
                return "Single data file inserted or updated";
            }
        }
        public List<T> GetDataList<T>() where T : class
        {
            List<T> result = default(List<T>);

            using (var conn = GetSQLiteConnetion())
            {
                result = conn.GetAllWithChildren<T>(null, true).ToList();
                return result;
            }
        }
        public List<T> GetDataList<T>(int count) where T : class
        {
            List<T> result = default(List<T>);

            using (var conn = GetSQLiteConnetion())
            {
                result = conn.GetAllWithChildren<T>(null, true).Take(count).ToList();
                return result;
            }
        }
        public List<T> GetDataList<T>(Func<T,bool> where) where T : class
        {        
            using (var conn = GetSQLiteConnetion())
            {
              var result = conn.GetAllWithChildren<T>(null, true).Where(where).ToList();
                return result;
            }
        }
        public void SetSelectedCategory(CategoryModel category, bool isSelected = true, bool isTablet = false)
        {
            using (var conn = GetSQLiteConnetion())
            {
                if (category.Group == NohandicapLibrary.MainCatGroup)
                {
                    category.IsSelected = true;
                    conn.InsertOrReplace(category);
                    //Uncheck another category
                    var mainCategories = conn.Table<CategoryModel>().Where(x => x.Group == NohandicapLibrary.MainCatGroup).ToList();
                    foreach (var cat in mainCategories)
                    {
                        if (cat.Id != category.Id)
                        {
                            cat.IsSelected = false;
                            conn.InsertOrReplace(cat);
                        }
                    }
                }
                if (category.Group == NohandicapLibrary.SubCatGroup)
                {
                    category.IsSelected = isSelected;
                    conn.InsertOrReplace(category);
                    if (!isTablet)
                    {
                        var subCategories = conn.Table<CategoryModel>().Where(x => x.Group == NohandicapLibrary.SubCatGroup).ToList();
                        foreach (var cat in subCategories)
                        {
                            if (cat.Id != category.Id)
                            {
                                cat.IsSelected = false;
                                conn.InsertOrReplace(cat);
                            }
                        }
                    }
                }
                var mainCat = conn.Table<CategoryModel>().Where(x => x.Group == NohandicapLibrary.MainCatGroup).ToList();

            }
        }
        public List<CategoryModel> GetSubSelectedCategory()
        {
            using (var conn = GetSQLiteConnetion())
            {
                var categories = conn.Table<CategoryModel>().Where(x => x.IsSelected && x.Group == NohandicapLibrary.SubCatGroup).ToList();
                if(categories.Count==0)
                {
                    categories = conn.Table<CategoryModel>().Where(x => x.Group == NohandicapLibrary.SubCatGroup).ToList();
                }
                return categories;
            }
        }
        public CategoryModel GetSelectedMainCategory()
        {
            using (var conn = GetSQLiteConnetion())
            {
                var mainCat = conn.Table<CategoryModel>().Where(x => x.Group == NohandicapLibrary.MainCatGroup&&x.IsSelected).FirstOrDefault() ;
                return mainCat ;
            }
        }
        public void Logout()
        {
            using (var conn = GetSQLiteConnetion())
            {
                conn.DeleteAll(typeof(UserModel));
                conn.CreateTable<UserModel>();
            }
        }
        public async Task<bool> SynchronizeDataBase(string langID, string TableName = null)
        {
            bool result;
            List<LanguageModel> languages;
            List<CategoryModel> categories;
            List<ProductDetailModel> products;

            switch (TableName)
            {
                case NohandicapLibrary.LANGUAGE_TABLE:
                    languages = await RestApiService.GetDataFromUrl<List<LanguageModel>>(NohandicapLibrary.LINK_LANGUAGE);

                    if (languages != null)
                    {
                        try {
                            using (var conn = GetSQLiteConnetion())
                            {
                                conn.DeleteAll(typeof(LanguageModel));
                                conn.CreateTable<LanguageModel>();
                                InsertUpdateProductList(languages);
                            }
                        }
                        catch (Exception e)
                        {
#if DEBUG
                            System.Diagnostics.Debugger.Break();
#endif
                        }
                        return true;
                    }
                    return false;
                    break;
                case NohandicapLibrary.CATEGORY_TABLE:
                    categories = await RestApiService.GetDataFromUrl<List<CategoryModel>>(NohandicapLibrary.LINK_CATEGORY + langID);
                    if (categories != null)
                    {
                        CategoryModel saveSelectedCat;
                        using (var conn = GetSQLiteConnetion())
                        {
                            saveSelectedCat = conn.Table<CategoryModel>().Where(x => x.IsSelected && x.Group == NohandicapLibrary.MainCatGroup).FirstOrDefault();
                            conn.DeleteAll(typeof(CategoryModel));
                            conn.CreateTable<CategoryModel>();
                           
                        }
                        var localCategories = NohandicapLibrary.GetAdditionalCategory();
                        foreach (var x in categories)
                        {
                            if (x.Group != 2)
                            {
                                var cat = localCategories.FirstOrDefault(y => y.Id == x.Id);
                                x.Icon = cat.Icon;
                                x.Color = cat.Color;
                                x.Marker = "marker_" + cat.Icon;
                            }
                            else
                            {
                                if (x.Id == saveSelectedCat.Id)
                                {
                                    x.IsSelected = true;
                                }
                            }
                            
                        }
                        
                        InsertUpdateProductList(categories);
                        return true;



                    }
                    return false;
                    break;                
                default:
                    var l = await SynchronizeDataBase(langID, NohandicapLibrary.LANGUAGE_TABLE);
                    var c = await SynchronizeDataBase(langID, NohandicapLibrary.CATEGORY_TABLE);
            
                    if (l || c)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                    break;
            }



            return true;


        }
        public void UnSelectAllCategories()
        {
            using (var conn = GetSQLiteConnetion())
            {
                var categories = conn.Table<CategoryModel>().ToList();
                categories.ForEach(x =>
                {
                    if (x.IsSelected)
                    {
                        x.IsSelected = false;
                        InsertUpdateProduct(x);
                    }
                });
            }
        }
    }
}
