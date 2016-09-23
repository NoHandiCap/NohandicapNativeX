using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Net;
using Java.Util;
using Android.Content.Res;
using Android.Util;
using Android.Preferences;
using Java.IO;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Threading.Tasks;

namespace NohandicapNative.Droid.Services
{
    public class Utils
    {       
        public const string HOME_TAG = "0";
        public const string MAP_TAG = "1";
        public const string LIST_TAG = "2";
        public const string FAVORITES_TAG = "3";
        public static string PATH = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public const string LOG_TAG = "NHC: ";
        public const string LANG_ID_TAG = "langID";
        public const string LANG_SHORT = "langShort";
        public const string TAB_ID = "tabID";
        public const string BACKGROUND = "#FFECB3";
        public const string TAB_COLOR = "#FF73012B";
        public const string IS_LOGIN = "isLogin";
        public const string IS_SUCCESS_LOGED = "true";
        public const string IS_NOT_LOGED = "false";
        public const string LOGIN_NAME = "loginName";
        public const string LOGIN_ID = "loginID";
        public const string PRODUCT_ID = "productId";
        public const string MAIN_CAT_SELECTED_ID = "maincat";
        public const string LAST_UPDATE_DATE = "lastUp";
       


        public static  Dictionary<string,string> GetLastUpadte(Context context)
        {
            Dictionary<string, string> lastUpdate = new Dictionary<string, string>();
            lastUpdate.Add(NohandicapLibrary.PRODUCT_TABLE, Utils.ReadFromSettings(context, NohandicapLibrary.PRODUCT_TABLE));
            lastUpdate.Add(NohandicapLibrary.CATEGORY_TABLE, Utils.ReadFromSettings(context, NohandicapLibrary.CATEGORY_TABLE));
            lastUpdate.Add(NohandicapLibrary.LANGUAGE_TABLE, Utils.ReadFromSettings(context, NohandicapLibrary.LANGUAGE_TABLE));
            return lastUpdate;
        }
        public static Android.Graphics.Drawables.Drawable GetImage(Context context, string image)

        {

            var id = context.Resources.GetIdentifier(image, "drawable", context.PackageName);
            return context.Resources.GetDrawable(id);
        }
        public static Drawable covertBitmapToDrawable(Context context, Bitmap bitmap)
        {
            Drawable d = new BitmapDrawable(context.Resources, bitmap);
            return d;
        }
        public static Bitmap changeImageColor(Bitmap sourceBitmap, int color)
        {
            Bitmap resultBitmap = Bitmap.CreateBitmap(sourceBitmap, 0, 0,
            sourceBitmap.Width - 1, sourceBitmap.Height - 1);
            Paint p = new Paint();
            ColorFilter filter = new LightingColorFilter(color, 1);
            p.SetColorFilter(filter);

            Canvas canvas = new Canvas(resultBitmap);
            canvas.DrawBitmap(resultBitmap, 0, 0, p);
            return resultBitmap;
        }
        public static Bitmap convertDrawableToBitmap(Drawable drawable)
        {


            Bitmap bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth,
            drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);

            return bitmap;
        }
        
        public static void ReloadMainActivity(Application application, Context context)
        {
           NohandicapApplication.MainActivity.Finish();
            Intent refresh = new Intent(context, typeof(MainActivity));
            context.StartActivity(refresh);
        }
        public static Drawable SetDrawableSize(Context context, int res, int width, int height)
        {
            Drawable dr = context.Resources.GetDrawable(res);
            Bitmap bitmap = ((BitmapDrawable)dr).Bitmap;
            // Scale it to 50 x 50
            Drawable d = new BitmapDrawable(context.Resources, Bitmap.CreateScaledBitmap(bitmap, width, height, true));
            return d;
        }
        public static Drawable SetDrawableSize(Context context, Drawable drawable, int width, int height)
        {
           
            Bitmap bitmap = ((BitmapDrawable)drawable).Bitmap;
            // Scale it to 50 x 50
            Drawable d = new BitmapDrawable(context.Resources, Bitmap.CreateScaledBitmap(bitmap, width, height, true));
            return d;
        }
        public static SqliteService GetDatabaseConnection()
        {
            try
            {
                return new SqliteService(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), Utils.PATH);

            }catch(Exception e)
            {
                Log.Debug("UTILS: ", e.Message);
                return null;
            }
        }
        public async static Task<Bitmap> SaveImageBitmapFromUrl(string url, string name)
        {

            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {           

                    imageBitmap = await GetBitmapOptionsOfImageAsync(imageBytes);
                }
            }
            Save(imageBitmap, name);
            return imageBitmap;
        }
        async static Task<Bitmap> GetBitmapOptionsOfImageAsync(byte[] imageBytes)
        {
            BitmapFactory.Options options = new BitmapFactory.Options
            {
                InJustDecodeBounds = false
            };

            // The result will be null because InJustDecodeBounds == true.
            Bitmap result = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length,options);        

            return result;
        }
        public static void Save(Bitmap bitmap, string name)
        {
            name = name.Replace(".jpg", "");
            var parentDir = new File(NohandicapApplication.MainActivity.FilesDir.ToString());
            List<File> inFiles = new List<File>();
            File[] files = parentDir.ListFiles();



            using (var os = new System.IO.FileStream(System.IO.Path.Combine(NohandicapApplication.MainActivity.FilesDir.ToString(), name), System.IO.FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 95, os);
            }
        }
        public static Bitmap GetBitmap(string name)
        {
            name = name.Replace(".jpg", "");
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;

            Bitmap bitmap = BitmapFactory.DecodeFile(System.IO.Path.Combine(NohandicapApplication.MainActivity.FilesDir.ToString(), name), options);
            return bitmap;
        }
        public static Resources SetLocale(Activity context, string lang)
        {
            var myLocale = new Locale(lang);
            Resources res = context.Resources;
            DisplayMetrics dm = res.DisplayMetrics;
            Configuration conf = res.Configuration;
            conf.Locale = myLocale;
            res.UpdateConfiguration(conf, dm);
            return res;

        }
        protected void saveset()
        {

            //store
            var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("PrefName", "Some value");
            prefEditor.Commit();

        }

        // Function called from OnCreate
        protected void retrieveset()
        {
            //retreive 
            var prefs = Application.Context.GetSharedPreferences("MyApp", FileCreationMode.Private);
            var somePref = prefs.GetString("PrefName", null);

        }
        public static void WriteToSettings(Context context, string key, string value)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(key, value);
            editor.Apply();
        }
        public static string ReadFromSettings(Context context, string key,string defValue=null)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);

            return prefs.GetString(key, defValue);
        }
        private static Locale sLocale;

        public static void setLocale(Locale locale)
        {
            sLocale = locale;
            if (sLocale != null)
            {
                Locale.Default = sLocale;
            }
        }
        public static float GetDistance(Location myLocation, Location point)
        {
            var d = myLocation.DistanceTo(point);
            return d;
        }
        public static void updateConfig(ContextThemeWrapper wrapper)
        {
            if (sLocale != null && Build.VERSION.SdkInt >= Build.VERSION_CODES.JellyBeanMr1)
            {
                Configuration configuration = new Configuration();
                configuration.SetLocale(sLocale);
                wrapper.ApplyOverrideConfiguration(configuration);
            }
        }

        public static void updateConfig(Application app, Configuration configuration)
        {
            if (sLocale != null && Build.VERSION.SdkInt < Build.VERSION_CODES.JellyBeanMr1)
            {
                //Wrapping the configuration to avoid Activity endless loop
                Configuration config = new Configuration(configuration);
                config.Locale = sLocale;
                Resources res = app.BaseContext.Resources;
         
                res.UpdateConfiguration(config, res.DisplayMetrics);
            }
        }

    }

}