using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using NohandicapNative.Droid.Adapters;
using Android.App;
using Android.Graphics;
using NohandicapNative.Droid.Services;
using NohandicapNative.Droid;
using System.Threading.Tasks;

namespace NohandicapNative.Droid
{
  public class FavoritesFragment : Android.Support.V4.App.Fragment
    {
        public const string TYPE_LOGIN = "login";
        public const string TYPE_LIST = "list";

        MainActivity myContext;       
        View view;
        string TypeFragment = TYPE_LIST;
        SqliteService dbCon;
        LinearLayout rootLayout;
        #region ctor

        public FavoritesFragment()
        {

        }
        public FavoritesFragment(string TypeFragment)
        {
            this.TypeFragment = TypeFragment;
        }
#endregion

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            dbCon = Utils.GetDatabaseConnection();
         
            if (Utils.ReadFromSettings(myContext, Utils.IS_LOGIN, Utils.IS_NOT_LOGED) == Utils.IS_NOT_LOGED)
            {
                InitiallizeLoginFragment(inflater, container);
                return view;
            }
            switch (TypeFragment)
            {
                case TYPE_LOGIN:
                 InitiallizeLoginFragment(inflater, container);
                    return view;
                    break;
                case TYPE_LIST:
                    InitiallizeFavListFragment();
                    return view;
                    break;
                default:
                    return view;
                    break;
            }

        }
        #region LoginFragment
        Button loginButton;
        Button signUpButton;
        TextView laterButton;
        EditText emailText;
        EditText passwordText;
       
        private void InitiallizeLoginFragment(LayoutInflater inflater, ViewGroup container)
        {
            view = inflater.Inflate(Resource.Layout.Login, container, false);
            view.SetBackgroundColor(Color.ParseColor(Utils.BACKGROUND));
           

            loginButton = view.FindViewById<Button>(Resource.Id.btn_login);
            laterButton = view.FindViewById<TextView>(Resource.Id.link_later);
            signUpButton = view.FindViewById<Button>(Resource.Id.btn_sign_up);

            emailText = view.FindViewById<EditText>(Resource.Id.input_email);
            passwordText = view.FindViewById<EditText>(Resource.Id.input_password);     
            loginButton.Click += (object sender, EventArgs e) =>
            {
                login();              
            };
            signUpButton.Click += (s, e) =>
            {
                StartActivityForResult(new Intent(Application.Context, typeof(SigUpActivity)), 1) ;
            };
          
        }
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
          
        }
        public async void login()
        { 

            if (!validate())
            {
                onLoginFailed();
                return;
            }

            loginButton.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(myContext,
                     Resource.Style.AppThemeDarkDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage("Authenticating...");
            progressDialog.Show();

            string email = emailText.Text;
            string password = passwordText.Text;
            if (await onLoginSuccess()) {

               
                progressDialog.Dismiss();
                InitiallizeFavListFragment();
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
                dbCon.InsertUpdateProduct(user);
                Utils.WriteToSettings(myContext, Utils.IS_LOGIN, Utils.IS_SUCCESS_LOGED);
                return true;
            }else
            {
                return false;
            }          
            
            // Finish();
        }
        public void onLoginFailed()
        {
            Toast.MakeText(myContext, "Login failed", ToastLength.Short).Show();

            loginButton.Enabled = true;
        }
        public bool validate()
        {
            bool valid = true;

            string email = emailText.Text;
            string password = passwordText.Text;

            if (string.IsNullOrEmpty(email) || !Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                emailText.Error = "enter a valid email address";
                valid = false;
            }
            else
            {
                emailText.Error = null;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 0 || password.Length > 10)
            {
                passwordText.Error = "between 4 and 10 alphanumeric characters";
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
        private void InitiallizeFavListFragment()
        {
            var favLis = new ListFragment(ListFragment.PRODUCT_FAVORITES);      
            Android.Support.V4.App.FragmentManager fragmentManager = myContext.SupportFragmentManager;
            myContext.ShowFragment(favLis, "fav");
        }
        #endregion
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
            if(TypeFragment==TYPE_LOGIN)
                InitiallizeFavListFragment();
        }
      
    }
}