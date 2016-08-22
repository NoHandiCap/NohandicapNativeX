


using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NohandicapNative
{
    public class SqliteService
    {
        private const string DB_NAME = "nohandicap.db3";
        private static SQLiteAsyncConnection dbCon;
        public SqliteService(string path)
        {
            dbCon = new SQLiteAsyncConnection(Path.Combine(path, DB_NAME));
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

                await dbCon.CreateTableAsync<PropertiesModel>();
                    await dbCon.CreateTableAsync<MarkerModel>();
                  
               
                  return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
        public  async Task<string> insertUpdateData(MarkerModel data)
        {
            try
            {
              
                if (await dbCon.InsertAsync(data) != 0)
                    await dbCon.UpdateAsync(data);
                return "Single data file inserted or updated";
            }
            catch (SQLiteException ex)
            {
                return ex.Message;
            }
        }
        public async Task<MarkerModel> GetData(int id)
        {
            try
            {

                return await dbCon.Table<MarkerModel>().FirstOrDefaultAsync();
                
            }
            catch (SQLiteException ex)
            {
                return null;
            }
        }
        private async Task<int> findNumberRecords(string path)
        {
            try
            {
                var db = new SQLiteAsyncConnection(path);
                // this counts all records in the database, it can be slow depending on the size of the database
                var count = await db.Table<MarkerModel>().CountAsync();

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
