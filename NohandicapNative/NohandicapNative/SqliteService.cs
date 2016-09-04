





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
    public  class SqliteService
    {
        public const string DB_NAME = "nohandicap.db3";
        private static SQLiteConnection dbCon;
        private string _path; 
        public SqliteService(ISQLitePlatform platform, string path)
        {
            path = Path.Combine(path, DB_NAME);
            dbCon = new SQLiteConnection(platform,path);
   
        }
        public bool CreateTables()
        {
            try
            {
                dbCon.RunInTransaction(() =>
                {
                    dbCon.CreateTable<ImageJsonModel>();
                dbCon.CreateTable<ImageModel>();
                dbCon.CreateTable<CategoryModel>();              
                dbCon.CreateTable<LanguageModel>();
                    dbCon.CreateTable<UserModel>();
                dbCon.CreateTable<ProductModel>();
                });
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public string InsertUpdateProduct<T>(T data)
        {
           
            try
            {
                dbCon.RunInTransaction(() =>
                {
                    dbCon.InsertOrReplaceWithChildren(data, true);
                });
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
         
        }
        public string InsertUpdateProductList<T>(List<T> data)
        {
        
            try
            {
                dbCon.RunInTransaction(() =>
                {
                    dbCon.InsertOrReplaceAllWithChildren(data, true);
                });
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
          
        }
        public List<T> GetDataList<T>() where T : class
        {
            List<T> result=default(List<T>);
            dbCon.RunInTransaction(() =>
            {
                result = dbCon.GetAllWithChildren<T>(null, true).ToList();
            });
                return result;
           
        
            
        }           
        public T Find<T>(int pk) where T : class
        {
            var s= dbCon.GetAllWithChildren<T>().FirstOrDefault(m => m.GetHashCode() == pk);
            var k = s.GetHashCode();
            return s;
        }
     
        public  async Task<bool> SynchronizeDataBase(string langID)
        {
      
        
            var languages = await RestApiService.GetDataFromUrl<List<LanguageModel>>(NohandiLibrary.LINK_LANGUAGE);
            if (languages != null)
            {
                dbCon.DeleteAll(typeof(LanguageModel));
                dbCon.CreateTable<LanguageModel>();
                InsertUpdateProductList(languages);
            }
            var products = await RestApiService.GetDataFromUrl<List<ProductModel>>(NohandiLibrary.LINK_PRODUCT + langID);
            if (products != null)
            {
                dbCon.DeleteAll(typeof(ProductModel));
                dbCon.CreateTable<ProductModel>();
                InsertUpdateProductList(products);
            }
            var categories = await RestApiService.GetDataFromUrl<List<CategoryModel>>(NohandiLibrary.LINK_CATEGORY + langID);

            if (categories != null)
            {
                dbCon.DeleteAll(typeof(CategoryModel));
                dbCon.CreateTable<CategoryModel>();
                var localCategories = NohandiLibrary.GetAdditionalCategory();
                categories.ForEach(x =>
                {
                    var cat = localCategories.FirstOrDefault(y => y.ID == x.ID);
                    x.Icon = cat.Icon;
                    x.Color = cat.Color;
                    x.Marker = "marker_" + cat.Icon;
                });

                InsertUpdateProductList(categories.OrderBy(x => x.Sort).ToList());
            }
            if (languages == null || products == null || categories == null)
                return false;
            return true;
         
           
        }
        public void Close()
        {
            dbCon.Close();
            
        }
        public void BeginTransaction()
        {
            dbCon.BeginTransaction();
        }
       
    }
}
