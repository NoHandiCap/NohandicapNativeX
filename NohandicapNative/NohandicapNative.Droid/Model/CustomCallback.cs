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
using Square.Picasso;
using Android.Gms.Maps.Model;

namespace NohandicapNative.Droid.Model
{
    public class CustomCallback : Java.Lang.Object, ICallback
    {
        Action action = null;

        public CustomCallback(Action action)
        {
            this.action = action;
        }    

        public void OnError()
        {
            
        }

        public void OnSuccess()
        {
            action?.Invoke();
        }
    }
}