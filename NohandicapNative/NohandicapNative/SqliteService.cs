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
        private static SQLiteConnection conn;
        private string _path; 
        public SqliteService(ISQLitePlatform platform, string path)
        {
            path = Path.Combine(path, DB_NAME);
            conn = new SQLiteConnection(platform,path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache|SQLiteOpenFlags.FullMutex,true);
      
   
        }
        public bool CreateTables()
        {
            try
            {
                conn.RunInTransaction(() =>
                {
                    conn.CreateTable<ImageJsonModel>();             
                conn.CreateTable<CategoryModel>();              
                conn.CreateTable<LanguageModel>();
                    conn.CreateTable<UserModel>();
                conn.CreateTable<ProductModel>();
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
           
           
             
                    conn.InsertOrReplaceWithChildren(data, true);
          
                return "Single data file inserted or updated";
          
         
        }
        public string InsertUpdateProductList<T>(List<T> data)
        {
            
                conn.InsertOrReplaceAllWithChildren(data, true);
      
           
                return "Single data file inserted or updated";
         
          
        }
        public List<T> GetDataList<T>(bool r=true) where T : class
        {
            List<T> result = default(List<T>);


                result = conn.GetAllWithChildren<T>(null, r).ToList();
            
            return result;
           
        
            
        }           
        public T Find<T>(int pk) where T : class
        {
            var s= conn.GetAllWithChildren<T>().FirstOrDefault(m => m.GetHashCode() == pk);
            var k = s.GetHashCode();
            return s;
        }
        public void SetSelectedCategory(CategoryModel category,bool isSelected=true)
        {
            if (category.Group == 2)
            {
                category.IsSelected = true;
                conn.InsertOrReplace(category);
                //Uncheck another category
                var mainCategories = conn.Table<CategoryModel>().Where(x => x.Group == 2).ToList();
                foreach (var cat in mainCategories)
                {
                    if (cat.Id != category.Id)
                    {
                        cat.IsSelected = false;
                        conn.InsertOrReplace(cat);
                    }

                }
            }
            if (category.Group == 1)
            {
                category.IsSelected = isSelected;
                conn.InsertOrReplace(category);
            }
        }
        public List<CategoryModel> GetSubSelectedCategory()
        {
            var categories = conn.Table<CategoryModel>().Where(x => x.IsSelected && x.Group == 1).ToList();
            return categories;
        }
        public CategoryModel GetSelectedMainCategory()
        {
            var mainCat= conn.Table<CategoryModel>().Where(x=>x.Group == 2).ToList();
            return conn.Table<CategoryModel>().FirstOrDefault(x => x.IsSelected && x.Group == 2);
        }
     public void Logout()
        {
            conn.DeleteAll(typeof(UserModel));
            conn.CreateTable<UserModel>();
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
                           
                        conn.DeleteAll(typeof(LanguageModel));
                        conn.CreateTable<LanguageModel>();
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
                         conn.DeleteAll(typeof(CategoryModel));
                        conn.CreateTable<CategoryModel>();
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
                        }

                        InsertUpdateProductList(categories);
                            return true;
                        
                      

                    }
                    return false;
                    break;
                case NohandicapLibrary.PRODUCT_TABLE:
                    products = await RestApiService.GetDataFromUrl<List<ProductModel>>(NohandicapLibrary.LINK_PRODUCT + langID);
                    if (products != null)
                    {
                        try
                        {
                            conn.DeleteAll(typeof(ProductModel));
                            conn.CreateTable<ProductModel>();
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
                    if (l|| c  || p )
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
           var categories= conn.Table<CategoryModel>().ToList();
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
            conn.Close();
            
        }
        public void BeginTransaction()
        {
            conn.BeginTransaction();
        }
       
    }
}
