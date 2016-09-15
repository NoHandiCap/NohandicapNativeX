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
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid
{
    [Activity(Label = "SigUpActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation |
        Android.Content.PM.ConfigChanges.ScreenSize
       )]
    public class SigUpActivity : AppCompatActivity
    {
        SqliteService dbCon;
        Button signUpButton;
        TextView loginLinkButton;
        EditText emailText;
        EditText passwordText;
        EditText passwordText2;

        EditText nameText;     
        EditText phoneText;
        EditText nachNameText;
        UserModel createdUser;
        RadioGroup radioSex;
        public SigUpActivity()
        {
            Utils.updateConfig(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);
           // Window.DecorView.SetBackgroundColor(Resources.GetColor(Resource.Color.backgroundColor));

            dbCon = Utils.GetDatabaseConnection();
            // Create your application here
            loginLinkButton = FindViewById<TextView>(Resource.Id.link_login);
            signUpButton = FindViewById<Button>(Resource.Id.btn_signup);
            radioSex= FindViewById<RadioGroup>(Resource.Id.radioSex);
            emailText = FindViewById<EditText>(Resource.Id.input_email);
            passwordText = FindViewById<EditText>(Resource.Id.input_password);
            passwordText2 = FindViewById<EditText>(Resource.Id.input_password2);
            phoneText = FindViewById<EditText>(Resource.Id.input_mobile);
            nachNameText = FindViewById<EditText>(Resource.Id.input_nachName);
            nameText = FindViewById<EditText>(Resource.Id.input_name);
            signUpButton.Click += (s, e) =>
            {
                signup();
            };
            loginLinkButton.Click += (s, e) =>
            {
                Finish();
            };
        }
        public async void signup()
        {


            if (!validate())
            {
                onSignupFailed();
                return;
            }

            signUpButton.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(this,
                   Resource.Style.AppThemeDarkDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage("Creating Account...");
            progressDialog.Show();

           var result = await RestApiService.SignUp(createdUser);
            if (result)
            {
                onSignupSuccess();
            }
            else
            {
                onSignupFailed();
            }
          
        }
        public void onSignupSuccess()
        {
            signUpButton.Enabled=true;
            SetResult(Result.Ok, null);
            Finish();
        }

        public void onSignupFailed()
        {
            Toast.MakeText(BaseContext, "Login failed", ToastLength.Short).Show();

            signUpButton.Enabled=true;
        }

        public bool validate()
        {
            bool valid = true;
            createdUser = new UserModel();
            string name = nameText.Text;
            string email = emailText.Text;
            string password = passwordText.Text;
            string password2 = passwordText2.Text;

            string phone = phoneText.Text;
            string nachName = nachNameText.Text;
            int radioButtonID = radioSex.CheckedRadioButtonId;
            RadioButton radioButton = (RadioButton)radioSex.FindViewById(radioButtonID);
            if (string.IsNullOrEmpty(name) || name.Length < 3)
            {
                nameText.Error="at least 3 characters";
                valid = false;
            }
            else
            {
                nameText.Error=null;
            }

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
            if (password2 != password2)
            {
                passwordText.Error = "Password not ";
                passwordText2.Error = "Password not ";
                valid = false;
            }


            if (string.IsNullOrEmpty(phone) || !Android.Util.Patterns.Phone.Matcher(phone).Matches())
            {
                phoneText.Error = "enter a valid phone";
                valid = false;
            }
            else
            {
                phoneText.Error = null;
            }
            if (string.IsNullOrEmpty(nachName) )
            {
               nachNameText.Error = "enter a Name";
                valid = false;
            }
            else
            {
                nachNameText.Error = null;
            }
            createdUser.Nname = nachName;
            createdUser.Vname = name;
            createdUser.Phone = phone.Replace("+","00");
            createdUser.Password = password;
            createdUser.Email = email;
            createdUser.Login = email.Substring(0,email.LastIndexOf('@') );
            if (radioButton == null)
            {
               Toast.MakeText(this,"Geschlecht",ToastLength.Short).Show();
                valid = false;
            }            
            if (radioButtonID == Resource.Id.radioM)
            {
                createdUser.Sex = "m";

            }
            else
            {
                createdUser.Sex = "w";

            }
            return valid;
        }
    }
}