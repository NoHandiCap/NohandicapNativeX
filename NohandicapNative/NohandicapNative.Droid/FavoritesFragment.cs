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

namespace NohandicapNative.Droid
{
    class FavoritesFragment : Android.Support.V4.App.Fragment
    {
        MainActivity myContext;
        Button loginButton;
        Button signUpButton;
        TextView laterButton;
        EditText emailText;
        EditText passwordText;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Login, container, false);
            loginButton = view.FindViewById<Button>(Resource.Id.btn_login);
            laterButton = view.FindViewById<TextView>(Resource.Id.link_later);
            signUpButton = view.FindViewById<Button>(Resource.Id.btn_sign_up);

            emailText = view.FindViewById<EditText>(Resource.Id.input_email);
            passwordText = view.FindViewById<EditText>(Resource.Id.input_password);
            ////Login button click action
            loginButton.Click += (object sender, EventArgs e) =>
            {
                login();
                //Intent myIntent = new Intent(this, typeof(MainActivity));
                //myIntent.PutExtra("greeting", "Hello from the Second Activity!");
                //SetResult(Result.Ok, myIntent);
                //Finish();
            };
            signUpButton.Click += (s, e) =>
            {

            };
            //var listView = view.FindViewById<ListView>(Resource.Id.listview);
            //List<MarkerModel> items = new List<MarkerModel>();
            //items.Add(new MarkerModel()
            //{
            //    //Id = 0.ToString(),
            //    //Properties=new PropertiesModel() { Title= "Hello", Description = "Descript" },

            //    //Image = "eat"

            //});
            //var listAdapter = new ListAdapter(Activity, items);
            //listView.Adapter = listAdapter;
            return view;
        }
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
        }
        public void login()
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

            // TODO: Implement your own authentication logic here.
            new Android.OS.Handler().PostDelayed(() => {
                // On complete call either onLoginSuccess or onLoginFailed
                onLoginSuccess();
                // onLoginFailed();
                progressDialog.Dismiss();
            }, 3000);
        }
       
        public void onLoginSuccess()
        {
            loginButton.Enabled = true;
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

            if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 10)
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
    }
}