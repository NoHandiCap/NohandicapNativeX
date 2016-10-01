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
using Xamarin.Auth;
using System.Json;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Content.Res;

namespace NohandicapNative.Droid
{
  public class FavoritesFragment : Android.Support.V4.App.Fragment
    {
        MainActivity myContext;       
        View view;
          
        ListView listView;
        List<ProductModel> products;
        TextView noFav;
        CardViewAdapter cardViewAdapter;
        public FavoritesFragment()
        {
           
            products = new List<ProductModel>();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
          
         
            if (Utils.ReadFromSettings(myContext, Utils.IS_LOGIN, Utils.IS_NOT_LOGED) == Utils.IS_NOT_LOGED)
            {
                InitiallizeLoginFragment(inflater, container);
                view.SetBackgroundColor(myContext.Resources.GetColor(Resource.Color.backgroundColor));
                return view;
            }
            else
            {
                InitiallizeFavListFragment(inflater, container);
                view.SetBackgroundColor(myContext.Resources.GetColor(Resource.Color.backgroundColor));
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
            fbButton.SetCompoundDrawablesWithIntrinsicBounds(Utils.SetDrawableSize(myContext,fbImg, 35, 35),drawables[1], drawables[2],drawables[3]);
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
             myContext.LoginToFacebook(this,true);               
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
                var dbCon = Utils.GetDatabaseConnection();
                dbCon.InsertUpdateProduct(user);
                dbCon.Close();
                Utils.WriteToSettings(myContext, Utils.IS_LOGIN, Utils.IS_SUCCESS_LOGED);
                ReloadFragment();
            }
        }
        public void ReloadFragment()
        {
            var fav = new FavoritesFragment();
            //  _myContext.ShowFragment(fav, "fav");
            Android.Support.V4.App.FragmentManager fragmentManager = myContext.SupportFragmentManager;
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

            ProgressDialog progressDialog = new ProgressDialog(myContext,
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
                var dbCon = Utils.GetDatabaseConnection();
                dbCon.InsertUpdateProduct(user);
                dbCon.Close();
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
            Toast.MakeText(myContext, Resources.GetString(Resource.String.error_login), ToastLength.Short).Show();

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


                listView.ItemClick += (s, e) =>
                {
                    int position = e.Position;

                    var activity = new Intent(myContext, typeof(DetailActivity));
                    activity.PutExtra(Utils.PRODUCT_ID, products[position].ID);
                    myContext.StartActivity(activity);
                };
            noFav = view.FindViewById<TextView>(Resource.Id.noFavoritesTextView);
       

            ReloadData();


        }
        public async void ReloadData()
        {
            var dbCon = Utils.GetDatabaseConnection();
            var user = dbCon.GetDataList<UserModel>().FirstOrDefault();
            if (user != null)
            {
                products = dbCon.GetDataList<ProductModel>().Where(x => user.Favorites.Any(y => y == x.ID)).ToList();
                dbCon.Close();
                if (products.Count == 0)
                {
                    noFav.Visibility = ViewStates.Visible;
                }
                else
                {
                    noFav.Visibility = ViewStates.Gone;

                }
                cardViewAdapter = new CardViewAdapter(myContext, products);
                listView.Adapter = cardViewAdapter;
            }
        }
        #endregion


        public override void OnAttach(Activity activity)
        {
           myContext = (MainActivity)activity;
            base.OnAttach(activity);           
        }
      
    }
}