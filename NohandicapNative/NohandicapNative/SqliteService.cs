





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
        public bool CreateDB()
        {
            try
            {
                dbCon.CreateTable<ImageModel>();
                dbCon.CreateTable<CategoryModel>();              
                dbCon.CreateTable<LanguageModel>();
                dbCon.CreateTable<ProductModel>();           
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
                dbCon.InsertOrReplaceWithChildren(data, true);
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }
        public List<T> GetDataList<T>() where T : class
        {           
                return dbCon.GetAllWithChildren<T>().ToList();
            
        }           
        public T Find<T>(int pk) where T : class
        {
      var s= dbCon.GetAllWithChildren<T>().FirstOrDefault(m => m.GetHashCode() == pk);
            var k = s.GetHashCode();
            return s;
        }
    }
}
