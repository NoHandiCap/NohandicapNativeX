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
        MainActivity _myContext;       
        View view;
        SqliteService dbCon;       
        ListView _listView;
        List<ProductModel> _products;
      
        CardViewAdapter cardViewAdapter;    

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            dbCon = Utils.GetDatabaseConnection();
         
            if (Utils.ReadFromSettings(_myContext, Utils.IS_LOGIN, Utils.IS_NOT_LOGED) == Utils.IS_NOT_LOGED)
            {
                InitiallizeLoginFragment(inflater, container);
                return view;
            }
            else
            {
                InitiallizeFavListFragment(inflater, container);
                return view;
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

            ProgressDialog progressDialog = new ProgressDialog(_myContext,
                     Resource.Style.AppThemeDarkDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage("Authenticating...");
            progressDialog.Show();

            string email = emailText.Text;
            string password = passwordText.Text;
            if (await onLoginSuccess()) {

               
                progressDialog.Dismiss();
                
                var fav = new FavoritesFragment();
              //  _myContext.ShowFragment(fav, "fav");
                Android.Support.V4.App.FragmentManager fragmentManager = _myContext.SupportFragmentManager;
               var trans = fragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.flContent, fav);
                trans.Commit();
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
                Utils.WriteToSettings(_myContext, Utils.IS_LOGIN, Utils.IS_SUCCESS_LOGED);
                return true;
            }else
            {
                return false;
            }          
            
            // Finish();
        }
        public void onLoginFailed()
        {
            Toast.MakeText(_myContext, "Login failed", ToastLength.Short).Show();

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
        private void InitiallizeFavListFragment(LayoutInflater inflater, ViewGroup container)
        {
            view = inflater.Inflate(Resource.Layout.ListPage, container, false);
            _listView = view.FindViewById<ListView>(Resource.Id.listview);
            view.SetBackgroundColor(Color.ParseColor(Utils.BACKGROUND));

            _listView.ItemClick += (s, e) =>
            {
                int position = e.Position;
                var detailIntent = new Intent(_myContext, typeof(DetailActivity));
                detailIntent.PutExtra("Title", _products[position].FirmName);
                _myContext.StartActivity(detailIntent);

            };
            LoadData();
            _listView.Adapter = cardViewAdapter;
          
        }
        private async void LoadData()
        {          
                var user = dbCon.GetDataList<UserModel>().FirstOrDefault();
                _products = dbCon.GetDataList<ProductModel>().Where(x => user.Fravorites.Any(y => y == x.ID)).ToList();
         
            cardViewAdapter = new CardViewAdapter(_myContext, _products);

            //var listAdapter = new ListAdapter(myContext, product);


        }
        #endregion
        public override void OnAttach(Activity activity)
        {
           _myContext = (MainActivity)activity;
            base.OnAttach(activity);           
        }
      
    }
}