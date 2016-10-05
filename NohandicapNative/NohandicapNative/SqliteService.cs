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
            dbCon = new SQLiteConnection(platform,path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache|SQLiteOpenFlags.FullMutex,true);
      
   
        }
        public bool CreateTables()
        {
            try
            {
                dbCon.RunInTransaction(() =>
                {
                    dbCon.CreateTable<ImageJsonModel>();             
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
           
           
             
                    dbCon.InsertOrReplaceWithChildren(data, true);
          
                return "Single data file inserted or updated";
          
         
        }
        public string InsertUpdateProductList<T>(List<T> data)
        {
            
                dbCon.InsertOrReplaceAllWithChildren(data, true);
      
           
                return "Single data file inserted or updated";
         
          
        }
        public List<T> GetDataList<T>(bool r=true) where T : class
        {
            List<T> result = default(List<T>);


                result = dbCon.GetAllWithChildren<T>(null, r).ToList();
            
            return result;
           
        
            
        }           
        public T Find<T>(int pk) where T : class
        {
            var s= dbCon.GetAllWithChildren<T>().FirstOrDefault(m => m.GetHashCode() == pk);
            var k = s.GetHashCode();
            return s;
        }
     public void Logout()
        {
            dbCon.DeleteAll(typeof(UserModel));
            dbCon.CreateTable<UserModel>();
        }
        public  async Task<bool> SynchronizeDataBase(string langID,string TableName=null)
        {
            bool result;
            List<LanguageModel> languages;
            List<CategoryModel> categories;
            List<ProductModel> products;

            switch (TableName)
            {
                case NohandicapLibrary.LANGUAGE_TABLE:
                    languages = await RestApiService.GetDataFromUrl<List<LanguageModel>>(NohandicapLibrary.LINK_LANGUAGE);

                    if (languages != null)
                    {
                        try { 
                        dbCon.DeleteAll(typeof(LanguageModel));
                        dbCon.CreateTable<LanguageModel>();
                        InsertUpdateProductList(languages);
                    }
                        catch (Exception e)
                    {
                        var s = e;
                    }
                    return true;
                    }
                    return false;
                    break;
                case NohandicapLibrary.CATEGORY_TABLE:
                    categories = await RestApiService.GetDataFromUrl<List<CategoryModel>>(NohandicapLibrary.LINK_CATEGORY + langID);
                    if (categories != null)
                    {
                        try
                        {
                            dbCon.DeleteAll(typeof(CategoryModel));
                        dbCon.CreateTable<CategoryModel>();
                        var localCategories = NohandicapLibrary.GetAdditionalCategory();
                        categories.ForEach(x =>
                        {
                            var cat = localCategories.FirstOrDefault(y => y.ID == x.ID);
                            x.Icon = cat.Icon;
                            x.Color = cat.Color;
                            x.Marker = "marker_" + cat.Icon;
                        });

                        InsertUpdateProductList(categories);
                            return true;
                        }
                        catch (Exception e)
                        {
                            var s = e;
                            return false;
                        }
                      

                    }
                    return false;
                    break;
                case NohandicapLibrary.PRODUCT_TABLE:
                    products = await RestApiService.GetDataFromUrl<List<ProductModel>>(NohandicapLibrary.LINK_PRODUCT + langID);
                    if (products != null)
                    {
                        try
                        {
                            dbCon.DeleteAll(typeof(ProductModel));
                            dbCon.CreateTable<ProductModel>();
                            InsertUpdateProductList(products);
                            return true;
                        }catch(Exception e)
                        {
                            var s = e;
                        }
                    }
                    return false;
                    break;
                default:
                   var l= await SynchronizeDataBase(langID, NohandicapLibrary.LANGUAGE_TABLE);
                  var c=await  SynchronizeDataBase(langID, NohandicapLibrary.CATEGORY_TABLE);
                  var p= await SynchronizeDataBase(langID, NohandicapLibrary.PRODUCT_TABLE);
                    if (l|| c  | p )
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
           var categories= dbCon.Table<CategoryModel>().ToList();
            categories.ForEach(x =>
            {
                if (x.IsSelected)
                {
                    x.IsSelected = false;
                    InsertUpdateProduct(x);
                }
            });

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
