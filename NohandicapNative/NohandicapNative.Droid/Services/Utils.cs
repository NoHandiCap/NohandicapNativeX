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
using System.IO;
using Java.Util;
using Android.Content.Res;
using Android.Util;

namespace NohandicapNative.Droid.Services
{
    public class Utils
    {
        public const string HOME_TAG = "0";
        public const string MAP_TAG = "1";
        public const string LIST_TAG = "2";
        public const string FAVORITES_TAG = "3";
        public static string PATH = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public static Android.Graphics.Drawables.Drawable GetImage(Context context, string image)

        {
            var id = context.Resources.GetIdentifier(image, "drawable", context.PackageName);
            return context.Resources.GetDrawable(id);
        }
        public static Drawable SetDrawableSize(Context context, int res, int width, int height)
        {
            Drawable dr = context.Resources.GetDrawable(res);
            Bitmap bitmap = ((BitmapDrawable)dr).Bitmap;
            // Scale it to 50 x 50
            Drawable d = new BitmapDrawable(context.Resources, Bitmap.CreateScaledBitmap(bitmap, width, height, true));
            return d;
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

            using (var os = new FileStream(name, FileMode.CreateNew))
            {
                bitmap.Compress(Bitmap.CompressFormat.Png, 95, os);
            }
        }
        public static Bitmap GetBitmap(string name)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(name, options);
            return bitmap;
        }
        public static void SetLocale(MainActivity context, string lang)
        {
            var myLocale = new Locale(lang);
            Resources res = context.Resources;
            DisplayMetrics dm = res.DisplayMetrics;
            Configuration conf = res.Configuration;
            conf.Locale = myLocale;
            res.UpdateConfiguration(conf, dm);
            Intent refresh = new Intent(context, typeof(MainActivity));

            context.StartActivity(refresh);
            context.Finish();
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

    }

}
