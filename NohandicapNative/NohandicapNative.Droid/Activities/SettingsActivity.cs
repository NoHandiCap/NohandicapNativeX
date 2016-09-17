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
using static Android.Widget.AdapterView;
using Android.Util;
using System.Threading.Tasks;

namespace NohandicapNative.Droid
{
    [Activity(Label = "@string/settings")]
    public  class SettingsActivity : AppCompatActivity, IOnItemClickListener
    {
        Android.Support.V7.Widget.Toolbar toolbar;
         SqliteService dbCon;
        int[] flags = { Resource.Drawable.german, Resource.Drawable.english, Resource.Drawable.france };
        List<LanguageModel> languageList;
        Resources res;
        LanguageModel selectedLanguage;
        ListView langListView;
        Button syncButton;
        Button logoutButton;
        TextView lastUpdateTextView;
        TextView userTextView;
        LinearLayout loginLayout;
        public SettingsActivity()
        {
            Utils.updateConfig(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {

            SetTheme(Resource.Style.AppThemeNoBar);
            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsPage);
            // Window.DecorView.SetBackgroundColor(Resources.GetColor(Resource.Color.backgroundColor));
            loginLayout = (LinearLayout)FindViewById(Resource.Id.loginLayout);
            syncButton = (Button)FindViewById(Resource.Id.syncButton);
            logoutButton= (Button)FindViewById(Resource.Id.logoutButton);
            userTextView = (TextView)FindViewById(Resource.Id.userTextView);
            lastUpdateTextView= (TextView)FindViewById(Resource.Id.lastUpdateTextView);
            lastUpdateTextView.Text = Resources.GetString(Resource.String.last_update) + Utils.ReadFromSettings(this, Utils.LAST_UPDATE_DATE);
            dbCon = Utils.GetDatabaseConnection();
            languageList = dbCon.GetDataList<LanguageModel>();
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);           
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.themeColor)));
            SupportActionBar.Title=Resources.GetString(Resource.String.settings);
            langListView = FindViewById<ListView>(Resource.Id.languageList);
            langListView.OnItemClickListener = this;
            List<CustomRadioButton> langList = new List<CustomRadioButton>();
            dbCon.GetDataList<LanguageModel>().ForEach(x => langList.Add(new CustomRadioButton()
            {
                Text = x.LanguageName
            }));
            var currentLocale = Utils.ReadFromSettings(this, Utils.LANG_ID_TAG);
            langListView.Adapter = new RadioButtonListAdapter(this, flags, langList,int.Parse(currentLocale)-1);
            syncButton.Click += async(s,e)=>{
                ProgressDialog progressDialog = new ProgressDialog(this,
                Resource.Style.AppThemeDarkDialog);
                progressDialog.Indeterminate = true;
                var a = Resources.GetString(Resource.String.load_data);
                progressDialog.SetMessage(a);
                progressDialog.Show();
                var _selectedLangID = Utils.ReadFromSettings(this,Utils.LANG_ID_TAG);
                bool result = await dbCon.SynchronizeDataBase(_selectedLangID);
                if (result)
                {

                    // On complete call either onLoginSuccess or onLoginFailed

                    // onLoginFailed();
                    progressDialog.Dismiss();
                    Toast.MakeText(this, Resources.GetString(Resource.String.sync), ToastLength.Short).Show();



                }
                else
                {
                    progressDialog.Dismiss();
                    Toast.MakeText(this, Resources.GetString(Resource.String.error), ToastLength.Short).Show();                  
                }
            };
            SetLoginLayout();

        }
        private void SetLoginLayout()
        {
            var user = dbCon.GetDataList<UserModel>().FirstOrDefault();
            if (user != null)
            {
                userTextView.Text = user.Name??user.Vname;
                logoutButton.Click += (s, e) =>
                {
                    dbCon.Logout();
                    Utils.WriteToSettings(this, Utils.IS_LOGIN, Utils.IS_NOT_LOGED);
                    loginLayout.Visibility = ViewStates.Gone;
                    ((MainActivity)Utils.mainActivity).Favorites=new FavoritesFragment();
                };
            }
            else
            {
                loginLayout.Visibility = ViewStates.Gone;
            }
            
        }

        private async Task<bool> ReloadData()
        {
            
            ProgressDialog progressDialog = new ProgressDialog(this,
           Resource.Style.StyledDialog);

            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.load_data));
            progressDialog.Show();

            var result = await dbCon.SynchronizeDataBase(selectedLanguage.ID.ToString());
            if (result)
            {

                // On complete call either onLoginSuccess or onLoginFailed

                // onLoginFailed();
                progressDialog.Dismiss();

               
                Finish();
                return true;

            }
            else
            {
              
                progressDialog.Dismiss();
                return false;

            }
           
        }
        public void OnItemClick(AdapterView parent, View view, int position, long languageId)
        {
            if (position == 0)
            {
                selectedLanguage = languageList[position];
            }
            for (int i = 0; i < langListView.ChildCount; i++)
            {
                var text = langListView.GetChildAt(i).FindViewById<TextView>(Resource.Id.grid_text);
                var viewLayout = langListView.GetChildAt(i).FindViewById<LinearLayout>(Resource.Id.grid_layout);
                if (position == i)
                {
                    text.SetTextColor(Color.White);
                    viewLayout.SetBackgroundColor(Resources.GetColor(Resource.Color.themeColor));
                    selectedLanguage = languageList[position];
                }
                else
                {
                    text.SetTypeface(null, TypefaceStyle.Normal);
                    text.SetTextColor(Color.Black);
                    viewLayout.SetBackgroundColor(Color.White);
                }             
            }
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
                    SaveSettings();                
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }
      private async void SaveSettings()
        {
            if (selectedLanguage != null)
            {
                if (await ReloadData())
                {

                    Utils.WriteToSettings(this, Utils.LANG_ID_TAG, selectedLanguage.ID.ToString());
                    Utils.WriteToSettings(this, Utils.LANG_SHORT, selectedLanguage.ShortName);
                    res = Utils.SetLocale(this, selectedLanguage.ShortName);
                    Utils.ReloadMainActivity(Application, this);
                    Finish();
                }
                else
                {
                    new Android.Support.V7.App.AlertDialog.Builder(this)
   .SetPositiveButton(Resources.GetString(Resource.String.try_text), (sender, args) =>
   {
       SaveSettings();
   })
   .SetNegativeButton(Resources.GetString(Resource.String.continue_text), (sender, args) =>
   {
       this.Finish();
   })
   .SetMessage(Resources.GetString(Resource.String.server_not_responding))
   .SetTitle(Resources.GetString(Resource.String.error))
   .Show();

                }

            }
            else
            {
                Finish();
            }
          
        }

        public void OnNothingSelected(AdapterView parent)
        {
           
        }
    }
}