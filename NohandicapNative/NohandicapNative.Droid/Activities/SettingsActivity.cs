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
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.V4.Content;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;
using Java.Util;
using Android.Content.Res;

namespace NohandicapNative.Droid
{
    [Activity(Label = "@string/settings")]
    public  class SettingsActivity : AppCompatActivity, AdapterView.IOnItemSelectedListener
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        Spinner spin;
        SqliteService dbCon;
        int[] flags = { Resource.Drawable.german, Resource.Drawable.english, Resource.Drawable.france };
        List<LanguageModel> languageList;
        LanguageModel selectedLanguage;
        public SettingsActivity()
        {
            Utils.updateConfig(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {

            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsPage);
            dbCon = Utils.GetDatabaseConnection();
            languageList = dbCon.GetDataList<LanguageModel>();
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.colorDefault)));
            SupportActionBar.Title=Resources.GetString(Resource.String.settings);
            spin = (Spinner)FindViewById(Resource.Id.lang_spinner);
            SpinnerAdapter customAdapter = new SpinnerAdapter(ApplicationContext, flags, languageList.Select(x => x.LanguageName).ToArray());
            spin.Adapter = customAdapter;
            var currentLocale = Utils.ReadFromSettings(this, Utils.LANG_SHORT);
            spin.SetSelection(languageList.FirstOrDefault(x => x.ShortName == currentLocale).ID - 1);
            spin.OnItemSelectedListener = this;

                     
           
        }
       
       
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.settings_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    break;
                case Resource.Id.done:
                    if (selectedLanguage != null)
                    {
                        Utils.WriteToSettings(this, Utils.LANG_ID_TAG, selectedLanguage.ID.ToString());
                        Utils.WriteToSettings(this, Utils.LANG_SHORT, selectedLanguage.ShortName);
                        Utils.SetLocale(this, selectedLanguage.ShortName);
                   
                    Utils.ReloadMainActivity(Application,this);
                    }
                    this.Finish();

                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnItemSelected(AdapterView parent, View view, int position, long id)
        {
            selectedLanguage = languageList[position];
        }

        public void OnNothingSelected(AdapterView parent)
        {
            throw new NotImplementedException();
        }
    }
}