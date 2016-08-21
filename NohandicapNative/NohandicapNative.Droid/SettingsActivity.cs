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
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.V4.Content;

namespace NohandicapNative.Droid
{
    [Activity(Label = "@string/settings")]
    public  class SettingsActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppThemeNoBar);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsPage);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.colorDefault)));
            SupportActionBar.Title=Resources.GetString(Resource.String.settings);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.settings_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    break;
                case Resource.Id.done:
                    this.Finish();

                    break;

            }
            return base.OnOptionsItemSelected(item);
        }
    }
}