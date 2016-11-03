using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Activities;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid.Fragments
{
  public class FavoritesFragment :BaseFragment
    {            
        View _view;          
        ListView _listView;
        List<ProductDetailModel> _products;
        TextView _noFav;
        CardViewAdapter _cardViewAdapter;
        public FavoritesFragment(bool loadFromCache = true) : base(loadFromCache)
        {
            _products = new List<ProductDetailModel>();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
          
         
            if (Utils.ReadFromSettings(Activity, Utils.IS_LOGIN, Utils.IS_NOT_LOGED) == Utils.IS_NOT_LOGED)
            {
                InitiallizeLoginFragment(inflater, container);
                _view.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.backgroundColor));
                return _view;
            }
            else
            {
                InitiallizeFavListFragment(inflater, container);
                _view.SetBackgroundColor(Activity.Resources.GetColor(Resource.Color.backgroundColor));
                return _view;
            }
            

        }
        #region LoginFragment
        Button _loginButton;
        AppCompatButton _fbButton;

        Button _signUpButton;
        TextView _laterButton;
        EditText _emailText;
        EditText _passwordText;
       
        private void InitiallizeLoginFragment(LayoutInflater inflater, ViewGroup container)
        {
            _view = inflater.Inflate(Resource.Layout.Login, container, false);
           
           
            _fbButton = _view.FindViewById<AppCompatButton>(Resource.Id.btn_login_facebook);
            _loginButton = _view.FindViewById<Button>(Resource.Id.btn_login);         
            _signUpButton = _view.FindViewById<Button>(Resource.Id.btn_sign_up);

            _emailText = _view.FindViewById<EditText>(Resource.Id.input_email);
            _passwordText = _view.FindViewById<EditText>(Resource.Id.input_password);
            var fbImg = Resources.GetDrawable(Resource.Drawable.facebook);
            Drawable[] drawables = _fbButton.GetCompoundDrawables();
            _fbButton.SetCompoundDrawablesWithIntrinsicBounds(Utils.SetDrawableSize(Activity, fbImg, 35, 35),drawables[1], drawables[2],drawables[3]);
            ColorStateList csl = new ColorStateList(new int[][] { new int[0] }, new int[] { Resources.GetColor(Resource.Color.fb_button_color)});
            _fbButton.SupportBackgroundTintList=csl;
            _loginButton.Click += (object sender, EventArgs e) =>
            {
                Login();              
            };
            _signUpButton.Click += (s, e) =>
            {
                StartActivityForResult(new Intent(Application.Context, typeof(SigUpActivity)), 1) ;
            };
            _fbButton.Click +=  (s, e) => {
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

      private void ReloadFragment()
        {
            var fav = new FavoritesFragment();
            //  _myContext.ShowFragment(fav, "fav");
            Android.Support.V4.App.FragmentManager fragmentManager = MainActivity.SupportFragmentManager;
            var trans = fragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.flContent, fav);
            trans.Commit();

        }

      private async void Login()
        { 

            if (!Validate())
            {
                OnLoginFailed();
                return;
            }

            _loginButton.Enabled = false;

          ProgressDialog progressDialog = new ProgressDialog(Activity,
              Resource.Style.AppThemeDarkDialog) {Indeterminate = true};
          progressDialog.SetMessage(Resources.GetString(Resource.String.authentication));
            progressDialog.Show();

            string email = _emailText.Text;
            string password = _passwordText.Text;
            if (await OnLoginSuccess()) {

               
                progressDialog.Dismiss();
                ReloadFragment();
               
            }
            else
            {
                progressDialog.Dismiss();
                OnLoginFailed();
            }
            // TODO: Implement your own authentication logic here.
            
        }

      private async Task<bool> OnLoginSuccess()
        {
            _loginButton.Enabled = true;
            var user = await RestApiService.Login(_emailText.Text, _passwordText.Text);
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

      private void OnLoginFailed()
        {
            Toast.MakeText(Activity, Resources.GetString(Resource.String.error_login), ToastLength.Short).Show();

            _loginButton.Enabled = true;
        }

      private bool Validate()
        {
            bool valid = true;

            string email = _emailText.Text;
            string password = _passwordText.Text;

            if (string.IsNullOrEmpty(email) || !Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                _emailText.Error = Resources.GetString(Resource.String.error_valid_email);
                valid = false;
            }
            else
            {
                _emailText.Error = null;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 0 || password.Length > 10)
            {
                _passwordText.Error = Resources.GetString(Resource.String.error_password_char);
                valid = false;
            }
            else
            {
                _passwordText.Error = null;
            }

            return valid;
        }
        #endregion

        #region FavListFragment
        private void InitiallizeFavListFragment(LayoutInflater inflater, ViewGroup container)
        {          
                _view = inflater.Inflate(Resource.Layout.ListPage, container, false);
                _listView = _view.FindViewById<ListView>(Resource.Id.listview);
            _noFav = _view.FindViewById<TextView>(Resource.Id.noFavoritesTextView);
            _noFav.Visibility = ViewStates.Gone;

            var categoryLabel = _view.FindViewById<LinearLayout>(Resource.Id.categoryContainer);
            categoryLabel.Visibility = ViewStates.Gone;
            _listView.ItemClick += ListView_ItemClick;
           
            ReloadData();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
                {
            int productId = (int)e.Id;
                    var activity = new Intent(Activity, typeof(DetailActivity));
            activity.PutExtra(Utils.PRODUCT_ID, productId);
                   MainActivity.StartActivity(activity);
        }

        public void ReloadData()
        {
            var user = DbConnection.GetDataList<UserModel>().FirstOrDefault();
            if (user != null)
            {               
            _cardViewAdapter = new CardViewAdapter(this, true);
            _listView.Adapter = _cardViewAdapter;
            }
        }
        public void NoFavLayoutVisibility(ViewStates state)
        {
            try
            {             
                _noFav = _view.FindViewById<TextView>(Resource.Id.noFavoritesTextView);
                _noFav.Visibility = state;
            }
            catch (Exception)
            {
                // ignored
            }
        }
        #endregion


      
    }
}