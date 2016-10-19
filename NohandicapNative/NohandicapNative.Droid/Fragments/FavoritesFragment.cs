using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Adapters;
using Android.App;
using NohandicapNative.Droid.Services;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Content.Res;
using NohandicapNative.Droid.Fragments;

namespace NohandicapNative.Droid
{
  public class FavoritesFragment :BaseFragment
    {            
        View view;          
        ListView listView;
        List<ProductDetailModel> products;
        TextView noFav;
        CardViewAdapter cardViewAdapter;
        public FavoritesFragment(Boolean loadFromCache = true) : base(loadFromCache)
        {
            products = new List<ProductDetailModel>();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
          
         
            if (Utils.ReadFromSettings(Activity, Utils.IS_LOGIN, Utils.IS_NOT_LOGED) == Utils.IS_NOT_LOGED)
            {
                InitiallizeLoginFragment(inflater, container);
                view.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.backgroundColor));
                return view;
            }
            else
            {
                InitiallizeFavListFragment(inflater, container);
                view.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.backgroundColor));
                return view;
            }
            

        }
        #region LoginFragment
        Button loginButton;
        AppCompatButton fbButton;

        Button signUpButton;
        TextView laterButton;
        EditText emailText;
        EditText passwordText;
       
        private void InitiallizeLoginFragment(LayoutInflater inflater, ViewGroup container)
        {
            view = inflater.Inflate(Resource.Layout.Login, container, false);
           
           
            fbButton = view.FindViewById<AppCompatButton>(Resource.Id.btn_login_facebook);
            loginButton = view.FindViewById<Button>(Resource.Id.btn_login);         
            signUpButton = view.FindViewById<Button>(Resource.Id.btn_sign_up);

            emailText = view.FindViewById<EditText>(Resource.Id.input_email);
            passwordText = view.FindViewById<EditText>(Resource.Id.input_password);
            var fbImg = Resources.GetDrawable(Resource.Drawable.facebook);
            Drawable[] drawables = fbButton.GetCompoundDrawables();
            fbButton.SetCompoundDrawablesWithIntrinsicBounds(Utils.SetDrawableSize(Activity, fbImg, 35, 35),drawables[1], drawables[2],drawables[3]);
            ColorStateList csl = new ColorStateList(new int[][] { new int[0] }, new int[] { Resources.GetColor(Resource.Color.fb_button_color)});
            fbButton.SupportBackgroundTintList=csl;
            loginButton.Click += (object sender, EventArgs e) =>
            {
                login();              
            };
            signUpButton.Click += (s, e) =>
            {
                StartActivityForResult(new Intent(Application.Context, typeof(SigUpActivity)), 1) ;
            };
            fbButton.Click +=  (s, e) => {
           MainActivity.LoginToFacebook(this,true);               
            };
          
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {
                ReloadData();
              
            }
          
        }
        public void UserLoginSuccess(UserModel user)
        {
            if (user != null)
            {

                DbConnection.InsertUpdateProduct(user);
                
                Utils.WriteToSettings(Activity, Utils.IS_LOGIN, Utils.IS_SUCCESS_LOGED);
                ReloadFragment();
            }
        }
        public void ReloadFragment()
        {
            var fav = new FavoritesFragment();
            //  _myContext.ShowFragment(fav, "fav");
            Android.Support.V4.App.FragmentManager fragmentManager = MainActivity.SupportFragmentManager;
            var trans = fragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.flContent, fav);
            trans.Commit();

        }
        public async void login()
        { 

            if (!validate())
            {
                onLoginFailed();
                return;
            }

            loginButton.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(Activity,
                     Resource.Style.AppThemeDarkDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.authentication));
            progressDialog.Show();

            string email = emailText.Text;
            string password = passwordText.Text;
            if (await onLoginSuccess()) {

               
                progressDialog.Dismiss();
                ReloadFragment();
               
            }
            else
            {
                progressDialog.Dismiss();
                onLoginFailed();
            }
            // TODO: Implement your own authentication logic here.
            
        }

        public async Task<bool> onLoginSuccess()
        {
            loginButton.Enabled = true;
            var user = await RestApiService.Login(emailText.Text, passwordText.Text);
            if (user != null)
            {

                DbConnection.InsertUpdateProduct(user);
                
                Utils.WriteToSettings(Activity, Utils.IS_LOGIN, Utils.IS_SUCCESS_LOGED);
                return true;
            }else
            {
                return false;
            }          
            
            // Finish();
        }
        public void onLoginFailed()
        {
            Toast.MakeText(Activity, Resources.GetString(Resource.String.error_login), ToastLength.Short).Show();

            loginButton.Enabled = true;
        }
        public bool validate()
        {
            bool valid = true;

            string email = emailText.Text;
            string password = passwordText.Text;

            if (string.IsNullOrEmpty(email) || !Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                emailText.Error = Resources.GetString(Resource.String.error_valid_email);
                valid = false;
            }
            else
            {
                emailText.Error = null;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 0 || password.Length > 10)
            {
                passwordText.Error = Resources.GetString(Resource.String.error_password_char);
                valid = false;
            }
            else
            {
                passwordText.Error = null;
            }

            return valid;
        }
        #endregion

        #region FavListFragment
        private void InitiallizeFavListFragment(LayoutInflater inflater, ViewGroup container)
        {          
         
                view = inflater.Inflate(Resource.Layout.ListPage, container, false);
                listView = view.FindViewById<ListView>(Resource.Id.listview);
            var categoryLabel = view.FindViewById<LinearLayout>(Resource.Id.category_linearLayout);
            categoryLabel.Visibility = ViewStates.Gone;

                listView.ItemClick += (s, e) =>
                {
                    int position = e.Position;

                    var activity = new Intent(Activity, typeof(DetailActivity));
                    activity.PutExtra(Utils.PRODUCT_ID, products[position].ID);
                   MainActivity.StartActivity(activity);
                };
            noFav = view.FindViewById<TextView>(Resource.Id.noFavoritesTextView);
       

            ReloadData();


        }
        public async void ReloadData()
        {
            
            var user = DbConnection.GetDataList<UserModel>().FirstOrDefault();
            if (user != null)
            {
            
                if (products.Count == 0)
                {
                    noFav.Visibility = ViewStates.Visible;
                }
                else
                {
                    noFav.Visibility = ViewStates.Gone;

                }
               cardViewAdapter = new CardViewAdapter(Activity, false);
            listView.Adapter = cardViewAdapter;
            }
        }
        #endregion


      
    }
}