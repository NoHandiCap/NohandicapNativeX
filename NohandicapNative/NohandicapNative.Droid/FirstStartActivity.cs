
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

namespace NohandicapNative.Droid
{
    [Activity(Label = "FirstStartActivity", Theme = "@style/android:Theme.Holo.Light.NoActionBar")]
    public class FirstStartActivity : AppCompatActivity, AdapterView.IOnItemSelectedListener
    {
        private SqliteService dbCon;
        Button nextButton;
        int[] flags = {Resource.Drawable.german, Resource.Drawable.english , Resource.Drawable.france };
        private int _selecteLangID = 1;
        private int _selecteLang= 1;
        Spinner spin;
        TextView spinnerPrompt;
    
        Resources res;
        List<LanguageModel> Languages;
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);

            base.OnCreate(bundle);           
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.FirstStart);
            Languages = new List<LanguageModel>();
            dbCon = Utils.GetDatabaseConnection();
            dbCon.CreateDB();
            FillLanguageTable();
            nextButton = FindViewById<Button>(Resource.Id.next_button);
            spinnerPrompt = FindViewById<TextView>(Resource.Id.lang_spinner_prompt);
           
            nextButton.Text = Resources.GetString(Resource.String.next);
            nextButton.Click += (s, e) =>
            {
                FillCategoryTable();
                LoadData();
               
            };
            spin = (Spinner)FindViewById(Resource.Id.lang_spinner);

            spin.OnItemSelectedListener = this;



        }
        private async void FillLanguageTable()
        {
            var languages =await RestApiService.GetData<List<LanguageModel>>(NohandiLibrary.LINK_LANGUAGE);
            string[] defaultLanguages = Resources.GetStringArray(Resource.Array.lang_array);
          var defaultShortLanguages = Resources.GetStringArray(Resource.Array.lang_short_array);
            try
            {

                if (languages.Count == 0 || languages == null)
                {

                    for (int i = 0; i < defaultLanguages.Length; i++)
                    {
                        LanguageModel lang = new LanguageModel();
                        lang.ID = i + 1;
                        lang.ShortName = defaultShortLanguages[i];
                        lang.LanguageName = defaultLanguages[i];
                        dbCon.InsertUpdateProduct(lang);
                        Languages.Add(lang);
                    }
                }else
                {
                    foreach (LanguageModel lang in languages)
                    {
                        dbCon.InsertUpdateProduct(lang);
                        Languages.Add(lang);
                    }
                    defaultShortLanguages = languages.Select(x => x.ShortName).ToArray();              
                }
                SpinnerAdapter customAdapter = new SpinnerAdapter(ApplicationContext, flags, dbCon.GetDataList<LanguageModel>().Select(x => x.LanguageName).ToArray());
                spin.Adapter = customAdapter;
            }
            catch(Exception e)
            {
                Log.Error(Utils.LOG_TAG, e.Message);
               
            }
            //ProductModel prod = new ProductModel();
            //prod.Language = lang;
            //prod.FirmName = "adidas";
            //prod.Categories = new List<CategoryModel>() { cat };   
        }
        private async void FillCategoryTable()
        {
            var categories = await RestApiService.GetData<List<CategoryModel>>(NohandiLibrary.LINK_CATEGORY+_selecteLangID);
            string[] defaultCategories = Resources.GetStringArray(Resource.Array.additional_category_array);
            try
            {
                if (categories.Count == 0 || categories == null)
                {
                    for (int i = 0; i < defaultCategories.Length; i++)
                {
                    CategoryModel cat = new CategoryModel();
                    cat.ID = i;
                    cat.Name = defaultCategories[i];
                    cat.Sort = i;
                    cat.LangID = _selecteLangID;
                    dbCon.InsertUpdateProduct(cat);
                }
                }
                else
                {
                    foreach (CategoryModel cat in categories)
                    {
                        dbCon.InsertUpdateProduct(cat);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(Utils.LOG_TAG, e.Message);

            }

        }
        private async Task<bool> LoadData()
        {
           

        
        
            ProgressDialog progressDialog = new ProgressDialog(this,
                    Resource.Style.AppThemeDarkDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(res.GetString(Resource.String.load_data));
            progressDialog.Show();
            new Android.OS.Handler().PostDelayed(() => {
                // On complete call either onLoginSuccess or onLoginFailed
              
                // onLoginFailed();
                progressDialog.Dismiss();
                Utils.ReloadMainActivity(Application,this);
                Finish();
            }, 3000);
            var l = dbCon.GetDataList<LanguageModel>();
            var c = dbCon.GetDataList<CategoryModel>();
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
            Utils.WriteToSettings(this, Utils.LANG_ID_TAG, Languages[position].ID.ToString());      
            Utils.WriteToSettings(this, Utils.LANG_SHORT, Languages[position].ShortName);

            res =   Utils.SetLocale(this, Languages[position].ShortName);
            nextButton.Text = res.GetString(Resource.String.next);
            spinnerPrompt.Text = res.GetString(Resource.String.lang_prompt);
        }

        public void OnNothingSelected(AdapterView parent)
        {
            
        }
    }
}