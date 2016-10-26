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
            Utils.UpdateConfig(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);           
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
                Signup();
            };
            loginLinkButton.Click += (s, e) =>
            {
                Finish();
            };
        }
        public async void Signup()
        {


            if (!Validate())
            { 
                return;
            }

            signUpButton.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(this,
                        Resource.Style.StyledDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.creating_account));
            progressDialog.Show();

           var result = await RestApiService.SignUp(createdUser);
            if (result.ContainsKey(1))
            {
                onSignupSuccess(result[1].ToString());
            }
            else
            {
                progressDialog.Dismiss();
                OnSignupFailed(result[0].ToString());
            }
          
        }
        public void onSignupSuccess(string message)
        {
            signUpButton.Enabled=true;
            SetResult(Result.Ok, null);
            Toast.MakeText(BaseContext, message, ToastLength.Long).Show();
            Finish();
        }

        public void OnSignupFailed(string message)
        {
            Toast.MakeText(BaseContext, message, ToastLength.Long).Show();

            signUpButton.Enabled=true;
        }

        public bool Validate()
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
                nameText.Error=Resources.GetString(Resource.String.error_char);
                valid = false;
            }
            else
            {
                nameText.Error=null;
            }

            if (string.IsNullOrEmpty(email) || !Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                emailText.Error= Resources.GetString(Resource.String.error_valid_email);
                valid = false;
            }
            else
            {
                emailText.Error=null;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 10)
            {
                passwordText.Error = Resources.GetString(Resource.String.error_password_char);
                valid = false;
            }
            else
            {
                passwordText.Error=null;
            }
            if (password != password2)
            {
                passwordText.Error = Resources.GetString(Resource.String.error_password_equal);
                passwordText2.Error = Resources.GetString(Resource.String.error_password_equal);
                valid = false;
            }


            if (string.IsNullOrEmpty(phone) || !Android.Util.Patterns.Phone.Matcher(phone).Matches())
            {
                phoneText.Error = Resources.GetString(Resource.String.error_valid_phone);
                valid = false;
            }
            else
            {
                phoneText.Error = null;
            }
            if (string.IsNullOrEmpty(nachName) )
            {
               nachNameText.Error = Resources.GetString(Resource.String.error_valid_name);
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
            if (!string.IsNullOrEmpty(email))
            {
                createdUser.Login = string.Concat(email.Substring(0, email.LastIndexOf('@')).Take(9));
            }
            if (radioButton == null)
            {
               Toast.MakeText(this, Resources.GetString(Resource.String.error_valid_sex),ToastLength.Short).Show();
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