using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;
using Android.Util;
using System.Linq;
using Android.Content.Res;
using static Android.Widget.AdapterView;
using Android.Graphics;
using System.IO;
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
        List<LanguageModel> LanguagesList;
        List<CategoryModel> CategoriesList;
        RelativeLayout languageLayout;
        LinearLayout agreementLayout;
        LinearLayout dataProtectionLayout;

        protected override void OnCreate(Bundle bundle)
        {
            Log.Debug(TAG, "Set Theme");
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);           
            Log.Debug(TAG, "Set view");        
            SetContentView(Resource.Layout.FirstStart);      
            Log.Debug(TAG, "Set loadContent");
                     
            agreementLayout = FindViewById<LinearLayout>(Resource.Id.agreementLayout);
            languageLayout = FindViewById<RelativeLayout>(Resource.Id.languageLayout);
            dataProtectionLayout = FindViewById<LinearLayout>(Resource.Id.dataProtectionLayout);
            var agreementTextView = FindViewById<TextView>(Resource.Id.agreementTextView);
            var dataProtectionTextView = FindViewById<TextView>(Resource.Id.dataProtectionTextView);

            var agreeButton = agreementLayout.FindViewById<Button>(Resource.Id.agreeButton);
                agreeButton.Click += (s, e) =>
                {
                    agreementLayout.Visibility = ViewStates.Gone;
                    dataProtectionLayout.Visibility = ViewStates.Visible;
                };
         
                string content;
                AssetManager assets = this.Assets;
                using (StreamReader sr = new StreamReader(assets.Open("Agreement.txt")))
                {
                    content = sr.ReadToEnd();
                }

            agreementTextView.Text = content; // Set TextView.Text to our asset content


            var agreeDataProtectionButton = dataProtectionLayout.FindViewById<Button>(Resource.Id.agreeDataProtectionButton);
            agreeDataProtectionButton.Click += (s, e) =>
            {
                dataProtectionLayout.Visibility = ViewStates.Gone;
                languageLayout.Visibility = ViewStates.Visible;
            };

            using (StreamReader sr = new StreamReader(assets.Open("DataProtection.txt")))
            {
                content = sr.ReadToEnd();
            }
            
            dataProtectionTextView.Text = content; // Set TextView.Text to our asset content

            LanguagesList = new List<LanguageModel>();         
                nextButton = FindViewById<Button>(Resource.Id.next_button);
                spinnerPrompt = FindViewById<TextView>(Resource.Id.lang_spinner_prompt);
                langListView = FindViewById<ListView>(Resource.Id.languageList);
                FillLanguageLocalTable();
                FillCategiesLocalTable();
                nextButton.Text = Resources.GetString(Resource.String.next);
                nextButton.Click += (s, e) =>
                {
                   progressDialog = new ProgressDialog(this,
           Resource.Style.StyledDialog);
                    progressDialog.Indeterminate = true;
                    progressDialog.SetMessage(res.GetString(Resource.String.load_data));
                    progressDialog.SetCanceledOnTouchOutside(false);
                    progressDialog.Show();
                    new System.Threading.Thread(new ThreadStart(delegate
                    {                
                        LoadData(); 
                    })).Start();               
                };
                langListView.OnItemClickListener = this;              
           
        }
        private void SetLocale(int position)
        {
            Utils.WriteToSettings(this, Utils.LANG_ID_TAG, LanguagesList[position].Id.ToString());
            Utils.WriteToSettings(this, Utils.LANG_SHORT, LanguagesList[position].ShortName);
            res = Utils.SetLocale(this, LanguagesList[position].ShortName);
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
        private async void FillLanguageLocalTable()
        {
            try
            {

          
            string[] defaultLanguages = Resources.GetStringArray(Resource.Array.lang_array);
            var defaultShortLanguages = Resources.GetStringArray(Resource.Array.lang_short_array);
           
                for (int i = 0; i < defaultLanguages.Length; i++)
                {
                    LanguageModel lang = new LanguageModel();
                    lang.Id = i + 1;
                    lang.ShortName = defaultShortLanguages[i];
                    lang.LanguageName = defaultLanguages[i];
               
                    LanguagesList.Add(lang);
                }
          
            List<CustomRadioButton> langList = new List<CustomRadioButton>();
            LanguagesList.ForEach(x => langList.Add(new CustomRadioButton()
            {
                Text = x.LanguageName
            }));
            langListView.Adapter = new RadioButtonListAdapter(this, flags, langList);
            langListView.PerformItemClick(langListView, 0, 0);
            OnItemClick(null, null, 0, 0);
            }
            catch (Exception)
            {

#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

            }
        }
        private void FillCategiesLocalTable()
        {

            List<string> mainItems = Resources.GetStringArray(Resource.Array.main_category_array).ToList();
           CategoriesList = new List<CategoryModel>();
            foreach (var item in mainItems)
            {
                CategoriesList.Add(new CategoryModel()
                {
                    Id = mainItems.IndexOf(item) + 1,
                    Name = item,
                    Group=2
                });
            }

        }
        private async void LoadData()
        {
            try { 
            var conn = Utils.GetDatabaseConnection();
            conn.CreateTables();
            conn.InsertUpdateProductList(LanguagesList);
            conn.InsertUpdateProductList(CategoriesList);
            conn.SetSelectedCategory(CategoriesList[0]);
            var result = await RestApiService.CheckUpdate(conn, _selecteLangID.ToString(), Utils.GetLastUpdate(this));
      
            if (result != null)
            {

                // On complete call either onLoginSuccess or onLoginFailed

                // onLoginFailed();
               
                Log.Debug(TAG, "Work is finished - start MainActivity.");
               
                    if (result.Count != 0)
                    {
                      //  Utils.WriteToSettings(this, NohandicapLibrary.PRODUCT_TABLE, result[NohandicapLibrary.PRODUCT_TABLE]);
                        Utils.WriteToSettings(this, NohandicapLibrary.CATEGORY_TABLE, result[NohandicapLibrary.CATEGORY_TABLE]);
                        Utils.WriteToSettings(this, NohandicapLibrary.LANGUAGE_TABLE, result[NohandicapLibrary.LANGUAGE_TABLE]);
                    }
                    Utils.WriteToSettings(this, Utils.LAST_UPDATE_DATE, DateTime.Now.ToShortDateString());
                
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();
             
            }
            else
            {
                progressDialog.Dismiss();
                
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
            catch (Exception e)
            {

#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif

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
            Utils.WriteToSettings(this, Utils.LANG_ID_TAG, LanguagesList[position].Id.ToString());
            Utils.WriteToSettings(this, Utils.LANG_SHORT, LanguagesList[position].ShortName);
            res = Utils.SetLocale(this, LanguagesList[position].ShortName);
            nextButton.Text = res.GetString(Resource.String.next);
            spinnerPrompt.Text = res.GetString(Resource.String.lang_prompt);
        }

        public void OnNothingSelected(AdapterView parent)
        {

        }
    }
}