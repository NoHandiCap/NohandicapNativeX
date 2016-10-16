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

namespace NohandicapNative.Droid.Services
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Android.Net.ConnectivityManager.ConnectivityAction, Android.Content.Intent.ActionBootCompleted})]
    public class NetworkStateReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Extras != null)
            {
                NetworkInfo ni = (NetworkInfo)intent.Extras.Get(ConnectivityManager.ExtraNetworkInfo);               
                    NohandicapApplication.IsInternetConnection = ni.IsConnected;
               
            }
        }
    }
}
