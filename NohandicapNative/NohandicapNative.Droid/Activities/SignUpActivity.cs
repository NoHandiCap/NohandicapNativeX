using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid.Activities
{
    [Activity(Label = "SigUpActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation |
        Android.Content.PM.ConfigChanges.ScreenSize
       )]
    public class SigUpActivity : AppCompatActivity
    {
    
        Button _signUpButton;
        TextView _loginLinkButton;
        EditText _emailText;
        EditText _passwordText;
        EditText _passwordText2;
        EditText _nameText;     
        EditText _phoneText;
        EditText _nachNameText;
        UserModel _createdUser;
        RadioGroup _radioSex;
        public SigUpActivity()
        {
            Utils.UpdateConfig(this);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignUp);           
            _loginLinkButton = FindViewById<TextView>(Resource.Id.link_login);
            _signUpButton = FindViewById<Button>(Resource.Id.btn_signup);
            _radioSex= FindViewById<RadioGroup>(Resource.Id.radioSex);
            _emailText = FindViewById<EditText>(Resource.Id.input_email);
            _passwordText = FindViewById<EditText>(Resource.Id.input_password);
            _passwordText2 = FindViewById<EditText>(Resource.Id.input_password2);
            _phoneText = FindViewById<EditText>(Resource.Id.input_mobile);
            _nachNameText = FindViewById<EditText>(Resource.Id.input_nachName);
            _nameText = FindViewById<EditText>(Resource.Id.input_name);
            _signUpButton.Click += (s, e) =>
            {
                Signup();
            };
            _loginLinkButton.Click += (s, e) =>
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

            _signUpButton.Enabled = false;

            ProgressDialog progressDialog = new ProgressDialog(this,
                        Resource.Style.StyledDialog);
            progressDialog.Indeterminate = true;
            progressDialog.SetMessage(Resources.GetString(Resource.String.creating_account));
            progressDialog.Show();

           var result = await RestApiService.SignUp(_createdUser);
            if (result.ContainsKey(1))
            {
                OnSignupSuccess(result[1].ToString());
            }
            else
            {
                progressDialog.Dismiss();
                OnSignupFailed(result[0].ToString());
            }
          
        }

        private void OnSignupSuccess(string message)
        {
            _signUpButton.Enabled=true;
            SetResult(Result.Ok, null);
            Toast.MakeText(BaseContext, message, ToastLength.Long).Show();
            Finish();
        }

        private void OnSignupFailed(string message)
        {
            Toast.MakeText(BaseContext, message, ToastLength.Long).Show();

            _signUpButton.Enabled=true;
        }

        private bool Validate()
        {
            bool valid = true;
            _createdUser = new UserModel();
            var name = _nameText.Text;
            var email = _emailText.Text;
            var password = _passwordText.Text;
            var password2 = _passwordText2.Text;
            var phone = _phoneText.Text;
            var nachName = _nachNameText.Text;
            int radioButtonId = _radioSex.CheckedRadioButtonId;
            RadioButton radioButton = (RadioButton)_radioSex.FindViewById(radioButtonId);
            if (string.IsNullOrEmpty(name) || name.Length < 3)
            {
                _nameText.Error=Resources.GetString(Resource.String.error_char);
                valid = false;
            }
            else
            {
                _nameText.Error=null;
            }

            if (string.IsNullOrEmpty(email) || !Android.Util.Patterns.EmailAddress.Matcher(email).Matches())
            {
                _emailText.Error= Resources.GetString(Resource.String.error_valid_email);
                valid = false;
            }
            else
            {
                _emailText.Error=null;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 4 || password.Length > 10)
            {
                _passwordText.Error = Resources.GetString(Resource.String.error_password_char);
                valid = false;
            }
            else
            {
                _passwordText.Error=null;
            }
            if (password != password2)
            {
                _passwordText.Error = Resources.GetString(Resource.String.error_password_equal);
                _passwordText2.Error = Resources.GetString(Resource.String.error_password_equal);
                valid = false;
            }


            if (string.IsNullOrEmpty(phone) || !Android.Util.Patterns.Phone.Matcher(phone).Matches())
            {
                _phoneText.Error = Resources.GetString(Resource.String.error_valid_phone);
                valid = false;
            }
            else
            {
                _phoneText.Error = null;
            }
            if (string.IsNullOrEmpty(nachName) )
            {
               _nachNameText.Error = Resources.GetString(Resource.String.error_valid_name);
                valid = false;
            }
            else
            {
                _nachNameText.Error = null;
            }
            _createdUser.Nname = nachName;
            _createdUser.Vname = name;
            _createdUser.Phone = phone.Replace("+","00");
            _createdUser.Password = password;
            _createdUser.Email = email;
            if (!string.IsNullOrEmpty(email))
            {
                _createdUser.Login = string.Concat(email.Substring(0, email.LastIndexOf('@')).Take(9));
            }
            if (radioButton == null)
            {
               Toast.MakeText(this, Resources.GetString(Resource.String.error_valid_sex),ToastLength.Short).Show();
                valid = false;
            }            
            _createdUser.Sex = radioButtonId == Resource.Id.radioM ? "m" : "w";
            return valid;
        }
    }
}