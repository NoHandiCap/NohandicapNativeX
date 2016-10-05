using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using System.Threading.Tasks;
using System.IO;
using NohandicapNative.Droid.Services;


namespace NohandicapNative.Droid.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true,Icon = "@drawable/logo_small", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation |
        Android.Content.PM.ConfigChanges.ScreenSize
       )]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);           
            Log.Debug(TAG, "SplashActivity.OnCreate");
         
        }

        protected override void OnResume()
        {
            base.OnResume();

            Task startupWork = new Task(() =>
            {
               
                Log.Debug(TAG, "Working in the background - important stuff.");
            });

            startupWork.ContinueWith(t =>
            {
                try
                {
                    Log.Debug(TAG, "Check DataBase.");             
                    if (!File.Exists(System.IO.Path.Combine(Utils.PATH, SqliteService.DB_NAME)))
                    {
                        Log.Debug(TAG, "Work is finished - start FirstActivity.");
                        StartActivity(new Intent(Application.Context, typeof(FirstStartActivity)));
                    }
                    else
                    {
                        Log.Debug(TAG, "Work is finished - start MainActivity.");
                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }

                }
                catch(Exception e)
                {
                    Log.Debug(TAG, e.Message);
                }
              
                
            }, TaskScheduler.FromCurrentSynchronizationContext());

            startupWork.Start();
        }
    }
}