using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid.Activities
{
    [Activity(Label = "FirstStartActivity")]
    public class FirstStartActivity : AppCompatActivity, AdapterView.IOnItemClickListener
    {
        static readonly string TAG = "X:" + typeof(FirstStartActivity).Name;
        ProgressDialog _progressDialog;
        Button _nextButton;
        readonly int[] flags = { Resource.Drawable.german, Resource.Drawable.english, Resource.Drawable.france, Resource.Drawable.italy};
        private int _selecteLangID = 1;
        TextView _spinnerPrompt;
        ListView _langListView;
        Resources _res;
        List<LanguageModel> _languagesList;
        List<CategoryModel> _mainCategoriesList;
        RelativeLayout _languageLayout;
        LinearLayout _agreementLayout;
        LinearLayout _dataProtectionLayout;
        LanguageModel _currentLanguage;

        protected override void OnCreate(Bundle bundle)
        {
            Log.Debug(TAG, "Set Theme");
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(bundle);
            Log.Debug(TAG, "Set view");
            SetContentView(Resource.Layout.FirstStart);
            Log.Debug(TAG, "Set loadContent");
            _agreementLayout = FindViewById<LinearLayout>(Resource.Id.agreementLayout);
            _languageLayout = FindViewById<RelativeLayout>(Resource.Id.languageLayout);
            _dataProtectionLayout = FindViewById<LinearLayout>(Resource.Id.dataProtectionLayout);
            var agreementTextView = FindViewById<TextView>(Resource.Id.agreementTextView);
            var dataProtectionTextView = FindViewById<TextView>(Resource.Id.dataProtectionTextView);
            var agreeButton = _agreementLayout.FindViewById<Button>(Resource.Id.agreeButton);
            _languageLayout.Visibility = ViewStates.Visible;
            _agreementLayout.Visibility = ViewStates.Gone;
            _dataProtectionLayout.Visibility = ViewStates.Gone;
            agreeButton.Click += (s, e) =>
                {
                    _agreementLayout.Visibility = ViewStates.Gone;
                    _dataProtectionLayout.Visibility = ViewStates.Visible;
                };

            var agreeDataProtectionButton = _dataProtectionLayout.FindViewById<Button>(Resource.Id.agreeDataProtectionButton);
            agreeDataProtectionButton.Click += (s, e) =>
            {
                _progressDialog = new ProgressDialog(this,
                    Resource.Style.StyledDialog) {Indeterminate = true};
                _progressDialog.SetMessage(_res.GetString(Resource.String.load_data));
                _progressDialog.SetCanceledOnTouchOutside(false);
                _progressDialog.Show();
                new Thread(LoadData).Start();

            };
            _languagesList = new List<LanguageModel>();
            _nextButton = FindViewById<Button>(Resource.Id.next_button);
            _spinnerPrompt = FindViewById<TextView>(Resource.Id.lang_spinner_prompt);
            _langListView = FindViewById<ListView>(Resource.Id.languageList);
            FillLanguageLocalTable();
            FillMainCategoriesLocalTable();
            _nextButton.Text = Resources.GetString(Resource.String.next);

            _nextButton.Click += (s, e) =>
             {
                 agreementTextView.Text = Utils.ReadStream(this,"Agreement_",_currentLanguage.ShortName, ".txt"); ; // Set TextView.Text to our asset content
                 dataProtectionTextView.Text = Utils.ReadStream(this,"DataProtection_", _currentLanguage.ShortName, ".txt"); // Set TextView.Text to our asset content
                 _languageLayout.Visibility = ViewStates.Gone;
                 _agreementLayout.Visibility = ViewStates.Visible;
             };
            _langListView.OnItemClickListener = this;

        }
        
        private void SetLocale(int position)
        {
            Utils.WriteToSettings(this, Utils.LANG_ID_TAG, _languagesList[position].Id.ToString());
            Utils.WriteToSettings(this, Utils.LANG_SHORT, _languagesList[position].ShortName);
            _res = Utils.SetLocale(this, _languagesList[position].ShortName);
            _nextButton.Text = _res.GetString(Resource.String.next);
            _spinnerPrompt.Text = _res.GetString(Resource.String.lang_prompt);
        }
        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            if (position == 0)
            {
                SetLocale(position);
            }
            for (int i = 0; i < _langListView.ChildCount; i++)
            {
                var text = _langListView.GetChildAt(i).FindViewById<TextView>(Resource.Id.grid_text);
                var viewLayout = _langListView.GetChildAt(i).FindViewById<LinearLayout>(Resource.Id.grid_layout);
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
            _currentLanguage = _languagesList[position];
        }
        private void FillLanguageLocalTable()
        {
            try
            {
                string[] defaultLanguages = Resources.GetStringArray(Resource.Array.lang_array);
                var defaultShortLanguages = Resources.GetStringArray(Resource.Array.lang_short_array);

                for (int i = 0; i < defaultLanguages.Length-1; i++)
                {
                    var lang = new LanguageModel
                    {
                        Id = i + 1,
                        ShortName = defaultShortLanguages[i],
                        LanguageName = defaultLanguages[i]
                    };

                    _languagesList.Add(lang);
                }

                var langList = new List<CustomRadioButton>();
                _languagesList.ForEach(x => langList.Add(new CustomRadioButton()
                {
                    Text = x.LanguageName
                }));
                _langListView.Adapter = new RadioButtonListAdapter(this, flags, langList);

                _langListView.PerformItemClick(_langListView, 0, 0);
                OnItemClick(null, null, 0, 0);
            }
            catch (Exception)
            {
               
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
            }
        }
        private void FillMainCategoriesLocalTable()
        {

            var mainItems = Resources.GetStringArray(Resource.Array.main_category_array).ToList();
            _mainCategoriesList = new List<CategoryModel>();
            foreach (var item in mainItems)
            {
                _mainCategoriesList.Add(new CategoryModel()
                {
                    Id = mainItems.IndexOf(item) + 1,
                    Name = item,
                    Group = 2
                });
            }

        }
        private async void LoadData()
        {
            var conn = Utils.GetDatabaseConnection();
            conn.CreateTables();
            conn.InsertUpdateProductList(_languagesList);
            conn.InsertUpdateProductList(_mainCategoriesList);
            conn.SetSelectedCategory(_mainCategoriesList[0]);
            var result = await RestApiService.CheckUpdate(conn, _selecteLangID.ToString(), Utils.GetLastUpdate(this));

            if (result != null)
            {
                // On complete call either onLoginSuccess or OnLoginFailed

                // OnLoginFailed();

                Log.Debug(TAG, "Work is finished - start MainActivity.");

                if (result.Count != 0)
                {
                    Utils.WriteToSettings(this, NohandicapLibrary.CATEGORY_TABLE, result[NohandicapLibrary.CATEGORY_TABLE]);
                    Utils.WriteToSettings(this, NohandicapLibrary.LANGUAGE_TABLE, result[NohandicapLibrary.LANGUAGE_TABLE]);
                }
                Utils.WriteToSettings(this, Utils.LAST_UPDATE_DATE, DateTime.Now.ToShortDateString());
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();

            }
            else
            {
                RunOnUiThread(ShowProgressDialog);
            }
        }


        private void ShowProgressDialog()
        {
            _progressDialog.Dismiss();
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetPositiveButton(Resources.GetString(Resource.String.try_text), (sender, args) =>
            {
                LoadData();
            });
            builder.SetNegativeButton(Resources.GetString(Resource.String.continue_text), (sender, args) =>
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                Finish();
            });
            builder.SetMessage(Resources.GetString(Resource.String.server_not_responding));
            builder.SetTitle(Resources.GetString(Resource.String.error));
            builder.Show();
        }
    
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);          
        }      

        public void OnNothingSelected(AdapterView parent)
        {

        }
    }
}