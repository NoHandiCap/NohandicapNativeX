





using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using SQLite.Net.Interop;

namespace NohandicapNative
{
    public class SqliteService
    {
        private const string DB_NAME = "nohandicap.db3";
        private static SQLiteConnection dbCon;
        public SqliteService(ISQLitePlatform platform, string path)
        {
            dbCon = new SQLiteConnection(platform,Path.Combine(path, DB_NAME));
   
        }
        public async Task<bool> CreateDB()
        {
            try
            {

                //#if SILVERLIGHT
                //    path = filename;
                //#else

                //#if __ANDROID__
                //   path = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
                //#else
                //                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                //                // (they don't want non-user-generated data in Documents)
                //                 path= Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder

                //#endif

                dbCon.CreateTable<CategoryModel>();
                dbCon.CreateTable<LanguagesDbModel>();
                dbCon.CreateTable<TranslateModel>();
                dbCon.CreateTable<LanguageModel>();
                dbCon.CreateTable<ProductModel>();
                  
               
                  return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public string insertUpdateData(ProductModel data)
        {
            try
            {

                dbCon.InsertWithChildren(data,true);
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }
        public List<ProductModel> GetData(int id)
        {
            try
            {

                return  dbCon.GetAllWithChildren<ProductModel>(null,true);
                
            }
            catch (SQLiteException ex)
            {
                return null;
            }
        }
        private int findNumberRecords(string path)
        {
            try
            {
              
                // this counts all records in the database, it can be slow depending on the size of the database
                var count =  dbCon.GetAllWithChildren<ProductModel>().Count;

                // for a non-parameterless query
                // var count = db.ExecuteScalarAsync<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");

                return count;
            }
            catch (SQLiteException ex)
            {
                return -1;
            }
        }
    }
}
