
using System.Collections.Generic;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

using NohandicapNative.Droid.Adapters;

using System.Threading.Tasks;
using NohandicapNative.Droid.Services;
using Java.Lang;
using Android.Util;
using System.Linq;
using Android.Content.Res;
using static Android.Widget.AdapterView;
using Android.Graphics;
using Java.IO;
using System.IO;
using System.Globalization;
using System.Text;
using System;
using System.Threading;

namespace NohandicapNative.Droid
{
    [Activity(Label = "FirstStartActivity")]
    public class FirstStartActivity : AppCompatActivity, AdapterView.IOnItemSelectedListener, IOnItemClickListener
    {
        static readonly string TAG = "X:" + typeof(FirstStartActivity).Name;

        ProgressDialog progressDialog;
        Button nextButton;
        int[] flags = { Resource.Drawable.german, Resource.Drawable.english, Resource.Drawable.france };
        private int _selecteLangID = 1;   
        TextView spinnerPrompt;
        ListView langListView;
        Resources res;
        List<LanguageModel> Languages;
        RelativeLayout languageLayout;
        LinearLayout agreementLayout;
        protected override void OnCreate(Bundle bundle)
        {
            Log.Debug(TAG, "Set Theme");

            SetTheme(Resource.Style.AppThemeNoBar);

            base.OnCreate(bundle);
           
                Log.Debug(TAG, "Set view");
                // Set our view from the "main" layout resource

                SetContentView(Resource.Layout.FirstStart);
        //    Window.DecorView.SetBackgroundColor(Resources.GetColor(Resource.Color.backgroundColor));
            Log.Debug(TAG, "Set loadContent");
            try
            {
             
                agreementLayout = FindViewById<LinearLayout>(Resource.Id.agreementLayout);
                languageLayout = FindViewById<RelativeLayout>(Resource.Id.languageLayout);
                var agreementTextView = FindViewById<TextView>(Resource.Id.agreementTextView);
                var agreeButton = agreementLayout.FindViewById<Button>(Resource.Id.agreeButton);
                agreeButton.Click += (s, e) =>
                {
                    agreementLayout.Visibility = ViewStates.Gone;
                    languageLayout.Visibility = ViewStates.Visible;
                };
         
                string content;
                AssetManager assets = this.Assets;
                using (StreamReader sr = new StreamReader(assets.Open("Agreement.txt")))
                {
                    content = sr.ReadToEnd();
                }

                // Set TextView.Text to our asset content
                agreementTextView.Text = content;


                Languages = new List<LanguageModel>();


                Log.Debug(TAG, "Get database");


                Log.Debug(TAG, "Creat Table");
                nextButton = FindViewById<Button>(Resource.Id.next_button);
                spinnerPrompt = FindViewById<TextView>(Resource.Id.lang_spinner_prompt);
                langListView = FindViewById<ListView>(Resource.Id.languageList);
                FillLanguageTable();
                nextButton.Text = Resources.GetString(Resource.String.next);
                nextButton.Click += (s, e) =>
                {

                   progressDialog = new ProgressDialog(this,
           Resource.Style.StyledDialog);
                    progressDialog.Indeterminate = true;
                    progressDialog.SetMessage(res.GetString(Resource.String.load_data));
                    progressDialog.Show();
                    new System.Threading.Thread(new ThreadStart(delegate
                    {                
                        LoadData(); 
                    })).Start();
                  
                   

                };
                langListView.OnItemClickListener = this;
              
            }catch(System.Exception e)
            {
                Log.Error(TAG, e.Message + " " + e.StackTrace);
            }
        }
        private void SetLocale(int position)
        {
            Utils.WriteToSettings(this, Utils.LANG_ID_TAG, Languages[position].ID.ToString());
            Utils.WriteToSettings(this, Utils.LANG_SHORT, Languages[position].ShortName);
            res = Utils.SetLocale(this, Languages[position].ShortName);
            nextButton.Text = res.GetString(Resource.String.next);
            spinnerPrompt.Text = res.GetString(Resource.String.lang_prompt);
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (position == 0)
            {
                SetLocale(position);
            }
            for (int i = 0; i < langListView.ChildCount; i++)
            {

                var text = langListView.GetChildAt(i).FindViewById<TextView>(Resource.Id.grid_text);
                var viewLayout = langListView.GetChildAt(i).FindViewById<LinearLayout>(Resource.Id.grid_layout);
                if (position == i)
                {                 
                    text.SetTextColor(Color.White);
                    viewLayout.SetBackgroundColor(Resources.GetColor(Resource.Color.themeColor));
                    SetLocale(position);
                }
                else
                {
                    text.SetTypeface(null, TypefaceStyle.Normal);
                    text.SetTextColor(Color.Black);
                    viewLayout.SetBackgroundColor(Color.White);

                }               
            }
        }
        private async void FillLanguageTable()
        {
 
            string[] defaultLanguages = Resources.GetStringArray(Resource.Array.lang_array);
            var defaultShortLanguages = Resources.GetStringArray(Resource.Array.lang_short_array);
           
                for (int i = 0; i < defaultLanguages.Length; i++)
                {
                    LanguageModel lang = new LanguageModel();
                    lang.ID = i + 1;
                    lang.ShortName = defaultShortLanguages[i];
                    lang.LanguageName = defaultLanguages[i];
               
                    Languages.Add(lang);
                }
          
            List<CustomRadioButton> langList = new List<CustomRadioButton>();
            Languages.ForEach(x => langList.Add(new CustomRadioButton()
            {
                Text = x.LanguageName
            }));
            langListView.Adapter = new RadioButtonListAdapter(this, flags, langList);
            langListView.PerformItemClick(langListView, 0, 0);
            OnItemClick(null, null, 0, 0);

        }
        private async void LoadData()
        {
               
            var dbCon = Utils.GetDatabaseConnection();
            dbCon.CreateTables();
            Languages.ForEach(x =>
            {
                dbCon.InsertUpdateProduct(x);
            });
            var result = await RestApiService.CheckUpdate(dbCon, _selecteLangID.ToString(), Utils.GetLastUpadte(this));
      
            if (result != null)
            {

                // On complete call either onLoginSuccess or onLoginFailed

                // onLoginFailed();
               
                Log.Debug(TAG, "Work is finished - start MainActivity.");
               
                    if (result.Count != 0)
                    {
                        Utils.WriteToSettings(this, NohandicapLibrary.PRODUCT_TABLE, result[NohandicapLibrary.PRODUCT_TABLE]);
                        Utils.WriteToSettings(this, NohandicapLibrary.CATEGORY_TABLE, result[NohandicapLibrary.CATEGORY_TABLE]);
                        Utils.WriteToSettings(this, NohandicapLibrary.LANGUAGE_TABLE, result[NohandicapLibrary.LANGUAGE_TABLE]);
                    }
                    Utils.WriteToSettings(this, Utils.LAST_UPDATE_DATE, DateTime.Now.ToShortDateString());
                dbCon.Close();
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();
             
            }
            else
            {
                progressDialog.Dismiss();
                dbCon.Close();
                new Android.Support.V7.App.AlertDialog.Builder(this)
     .SetPositiveButton(Resources.GetString(Resource.String.try_text), (sender, args) =>
     {
         LoadData();

     })
     .SetNegativeButton(Resources.GetString(Resource.String.continue_text), (sender, args) =>
     {

         StartActivity(new Intent(Application.Context, typeof(MainActivity)));
         Finish();

     })
     .SetMessage(Resources.GetString(Resource.String.server_not_responding))
     .SetTitle(Resources.GetString(Resource.String.error))
     .Show();


            }

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
            Utils.WriteToSettings(this, Utils.LANG_ID_TAG, Languages[position].ID.ToString());
            Utils.WriteToSettings(this, Utils.LANG_SHORT, Languages[position].ShortName);
            res = Utils.SetLocale(this, Languages[position].ShortName);
            nextButton.Text = res.GetString(Resource.String.next);
            spinnerPrompt.Text = res.GetString(Resource.String.lang_prompt);
        }

        public void OnNothingSelected(AdapterView parent)
        {

        }
    }
}