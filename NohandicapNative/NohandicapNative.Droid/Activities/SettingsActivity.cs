using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Graphics.Drawables;
using Android.Graphics;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;
using Android.Content.Res;
using static Android.Widget.AdapterView;
using System.Threading.Tasks;
using System;
using NohandicapNative.Droid.Activities;
using NohandicapNative.Droid.Fragments;

namespace NohandicapNative.Droid
{
    [Activity(Label = "@string/settings")]
    public  class SettingsActivity : AppCompatActivity, IOnItemClickListener
    {
        Android.Support.V7.Widget.Toolbar _toolbar;
        readonly int[] flags = { Resource.Drawable.german, Resource.Drawable.english, Resource.Drawable.france };
        List<LanguageModel> _languageList;
        LanguageModel _selectedLanguage;
        ListView _langListView;
        Button _syncButton;
        Button _logoutButton;
        TextView _lastUpdateTextView;
        TextView _userTextView;
        LinearLayout _loginLayout;
        private Resources _res;
      
 
        public SettingsActivity()
        {
            Utils.UpdateConfig(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppThemeNoBar);            
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsPage);
            PrepareBar();              
            _loginLayout = (LinearLayout)FindViewById(Resource.Id.loginLayout);
            _syncButton = (Button)FindViewById(Resource.Id.syncButton);
            _logoutButton= (Button)FindViewById(Resource.Id.logoutButton);
            _userTextView = (TextView)FindViewById(Resource.Id.userTextView);
            _lastUpdateTextView= (TextView)FindViewById(Resource.Id.lastUpdateTextView);
            _lastUpdateTextView.Text = Resources.GetString(Resource.String.last_update) + Utils.ReadFromSettings(this, Utils.LAST_UPDATE_DATE);
            var conn = Utils.GetDatabaseConnection();
            _languageList = conn.GetDataList<LanguageModel>();           
            _langListView = FindViewById<ListView>(Resource.Id.languageList);
           
            _langListView.OnItemClickListener = this;
            List<CustomRadioButton> langList = new List<CustomRadioButton>();
            conn.GetDataList<LanguageModel>().ForEach(x => langList.Add(new CustomRadioButton()
            {
                Text = x.LanguageName
            }));
            var currentLocale = Utils.ReadFromSettings(this, Utils.LANG_ID_TAG);
            _langListView.Adapter = new RadioButtonListAdapter(this, flags, langList,int.Parse(currentLocale)-1);
            Utils.SetListViewHeightBasedOnChildren(_langListView);
            _syncButton.Click += async(s,e)=>{
                ProgressDialog progressDialog = new ProgressDialog(this,
                 Resource.Style.StyledDialog);
                progressDialog.Indeterminate = true;
                var a = Resources.GetString(Resource.String.load_data);
                progressDialog.SetMessage(a);
                progressDialog.Show();
                var selectedLangId = Utils.ReadFromSettings(this,Utils.LANG_ID_TAG);
                bool result = await conn.SynchronizeDataBase(selectedLangId);
                
                if (result)
                {                 
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
        private void PrepareBar()
        {
            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.themeColor)));
            SupportActionBar.Title = Resources.GetString(Resource.String.settings);
        }
        private void SetLoginLayout()
        {
            var conn = Utils.GetDatabaseConnection();
            var user = conn.GetDataList<UserModel>().FirstOrDefault();
            if (user != null)
            {
                _userTextView.Text = user.Vname;
                _logoutButton.Click += (s, e) =>
                {
                    conn.Logout();
                    
                    Utils.WriteToSettings(this, Utils.IS_LOGIN, Utils.IS_NOT_LOGED);
                    _loginLayout.Visibility = ViewStates.Gone;
                   NohandicapApplication.MainActivity.Favorites=new FavoritesFragment();
                };
            }
            else
            {
                _loginLayout.Visibility = ViewStates.Gone;
            }            
        }       
        private async Task<bool> ReloadData()
        {
            try {
                ProgressDialog progressDialog = new ProgressDialog(this,
                    Resource.Style.StyledDialog) {Indeterminate = true};

                progressDialog.SetMessage(Resources.GetString(Resource.String.load_data));
            progressDialog.Show();
            var conn = Utils.GetDatabaseConnection();
            var result = await conn.SynchronizeDataBase(_selectedLanguage.Id.ToString());
            
            if (result)
            {                
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
            catch (Exception e)
            {

#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                return false;
            }
        }
        public void OnItemClick(AdapterView parent, View view, int position, long languageId)
        {
            if (position == 0)
            {
                _selectedLanguage = _languageList[position];
            }
            for (int i = 0; i < _langListView.ChildCount; i++)
            {
                var text = _langListView.GetChildAt(i).FindViewById<TextView>(Resource.Id.grid_text);
                var viewLayout = _langListView.GetChildAt(i).FindViewById<LinearLayout>(Resource.Id.grid_layout);
                if (position == i)
                {
                    text.SetTextColor(Color.White);
                    viewLayout.SetBackgroundColor(Resources.GetColor(Resource.Color.themeColor));
                    _selectedLanguage = _languageList[position];
                }
                else
                {
                    text.SetTypeface(null, TypefaceStyle.Normal);
                    text.SetTextColor(Color.Black);
                    viewLayout.SetBackgroundColor(Color.White);
                }             
            }
        }
        private async void SaveSettings()
        {
            if (_selectedLanguage != null)
            {
                if (await ReloadData())
                {
                    NohandicapApplication.CurrentLang = _selectedLanguage;                 
                    _res = Utils.SetLocale(this, _selectedLanguage.ShortName);
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
        #region MenuImplement
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
        public void OnNothingSelected(AdapterView parent)
        {
           
        }
        #endregion

    }
}