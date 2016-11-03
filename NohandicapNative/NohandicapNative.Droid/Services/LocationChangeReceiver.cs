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
using Android.Net;
using NohandicapNative.Droid.Activities;

namespace NohandicapNative.Droid.Services
{
    [BroadcastReceiver]
   //[IntentFilter(new[] { Android. Android.ConnectivityManager.ConnectivityAction, Android.Content.Intent.ActionBootCompleted})]
    public class LocationChangeReceiver : BroadcastReceiver
    {
        public static readonly string LOCATION_UPDATED = "LOCATION_UPDATED";

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action.Equals(LOCATION_UPDATED))
            {
                MainActivity.Instance.UpdateUI(intent);
            }

            /*if (intent.Extras != null)
            {
                NetworkInfo ni = (NetworkInfo)intent.Extras.Get(ConnectivityManager.ExtraNetworkInfo);               
                    NohandicapApplication.IsInternetConnection = ni.IsConnected;
            }*/
        }
    }
}
