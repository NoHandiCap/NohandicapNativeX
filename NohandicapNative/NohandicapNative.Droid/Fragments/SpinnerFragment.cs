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

namespace NohandicapNative.Droid.Fragments
{
  public  class SpinnerFragment:Fragment
    {
        private static readonly int SPINNER_WIDTH = 100;
        private static readonly int SPINNER_HEIGHT = 100;    
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState)
        {
            var progressBar = new ProgressBar(container.Context);
        if (container.GetType() != typeof (FrameLayout)) return progressBar;
        var layoutParams =
            new FrameLayout.LayoutParams(SPINNER_WIDTH, SPINNER_HEIGHT,GravityFlags.Center);
        progressBar.LayoutParameters=layoutParams;
        return progressBar;
        }
    }
}