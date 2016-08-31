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
        public const string BACKGROUND= "#FFECB3";

        public static Context mainActivity;
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
        public static void ReloadMainActivity(Application application,Context context)
        {
            ((NohandicapApplication)application).MainActivity.Finish();
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
        public static SqliteService GetDatabaseConnection()
        {
          return new SqliteService(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), Utils.PATH);
        }
        public static bool SaveImageBitmapFromUrl(string url, string name)
        {

            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            Save(imageBitmap, name);
            return true;
        }
        public static void Save(Bitmap bitmap, string name)
        {
            name = name.Replace(".jpg", "");
            var parentDir = new File(mainActivity.FilesDir.ToString());
            List<File> inFiles = new List<File>();
            File[] files = parentDir.ListFiles();
           
            
        
            using (var os = new System.IO.FileStream(System.IO.Path.Combine(mainActivity.FilesDir.ToString(), name), System.IO.FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 95, os);
            }
        }
        public static Bitmap GetBitmap(string name)
        {
            name = name.Replace(".jpg", "");
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            
            Bitmap bitmap = BitmapFactory.DecodeFile(System.IO.Path.Combine(mainActivity.FilesDir.ToString(), name), options);
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
        public static void WriteToSettings(Context context,string key, string value)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString(key, value);          
            editor.Apply();
        }
        public static string ReadFromSettings(Context context, string key)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);

            return prefs.GetString(key, null);
        }

    }

}
