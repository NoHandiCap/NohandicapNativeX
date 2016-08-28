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
using Android.Support.V7.App;
using Java.Lang;
using NohandicapNative.Droid.Adapters;
using static Android.Widget.AdapterView;
using System.Threading.Tasks;

namespace NohandicapNative.Droid
{
    [Activity(Label = "FirstStartActivity", Theme = "@style/android:Theme.Holo.Light.NoActionBar")]
    public class FirstStartActivity : AppCompatActivity, IOnItemSelectedListener
    {
        string[] countryNames = { "German", "English","French"};
        int[] flags = {Resource.Drawable.german, Resource.Drawable.english , Resource.Drawable.france };


        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);

            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.FirstStart);
            Spinner spin = (Spinner)FindViewById(Resource.Id.lang_spinner);
            spin.OnItemSelectedListener = this;
            SpinnerAdapter customAdapter = new SpinnerAdapter(ApplicationContext, flags, countryNames);
            spin.Adapter=customAdapter;
            var nextButton = FindViewById<Button>(Resource.Id.next_button);
            nextButton.Click += (s, e) =>
            {
                LoadData();
            };
        }
        private async Task<bool> LoadData()
        {
            ProgressDialog progressDialog = new ProgressDialog(this,
                    Resource.Style.AppThemeDarkDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.load_data));
            progressDialog.Show();
            new Android.OS.Handler().PostDelayed(() => {
                // On complete call either onLoginSuccess or onLoginFailed
              
                // onLoginFailed();
                progressDialog.Dismiss();
                Finish();
            }, 3000);
            return true;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
               
            }
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
        
        }

        public void OnNothingSelected(AdapterView parent)
        {
            
        }
    }
}