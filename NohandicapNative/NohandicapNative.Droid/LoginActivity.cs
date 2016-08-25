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
using Java.Lang;

namespace NohandicapNative.Droid
{
    [Activity(Label = "LoginActivity",Theme = "@style/android:Theme.Holo.Light.NoActionBar")]
    public class LoginActivity : AppCompatActivity
    {
        Button loginButton;
        Button signUpButton;
        TextView laterButton;
        EditText emailText;
        EditText passwordText;

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppThemeNoBar);

            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Login);

            //Initializing button from layout
            loginButton = FindViewById<Button>(Resource.Id.btn_login);
             laterButton = FindViewById<TextView>(Resource.Id.link_later);
            signUpButton = FindViewById<Button>(Resource.Id.btn_sign_up);

            emailText = FindViewById<EditText>(Resource.Id.input_email);
            passwordText = FindViewById<EditText>(Resource.Id.input_password);

            ////Login button click action
            loginButton.Click += (object sender, EventArgs e) =>
            {
                login();
                //Intent myIntent = new Intent(this, typeof(MainActivity));
                //myIntent.PutExtra("greeting", "Hello from the Second Activity!");
                //SetResult(Result.Ok, myIntent);
                //Finish();
            };
            signUpButton.Click += (s, e) => {
                var myIntent = new Intent(this, typeof(SigUpActivity));
                StartActivityForResult(myIntent, 0);
                Finish();
            };
            laterButton.Click += (s, e) =>
            {
                Finish();
            };
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                //  var helloLabel = FindViewById<TextView>(Resource.Id.helloLabel);
                // helloLabel.Text = data.GetStringExtra("greeting");
            }
        }
        public void login()
        {


            if (!validate())
            {
                onLoginFailed();
                return;
            }

            loginButton.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(this,
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
        public override void OnBackPressed()
        {
            MoveTaskToBack(true);
        }
        public void onLoginSuccess()
        {
            loginButton.Enabled=true;
            Finish();
        }
        public void onLoginFailed()
        {
            Toast.MakeText(this, "Login failed", ToastLength.Short).Show();

            loginButton.Enabled = true;
        }
        public bool validate()
        {
            bool valid = true;

            string email = emailText.Text;
            string password = passwordText.Text;

            if (string.IsNullOrEmpty(email) || !Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                emailText.Error="enter a valid email address";
                valid = false;
            }
            else
            {
                emailText.Error=null;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 10)
            {
                passwordText.Error="between 4 and 10 alphanumeric characters";
                valid = false;
            }
            else
            {
                passwordText.Error=null;
            }

            return valid;
        }
    }
}