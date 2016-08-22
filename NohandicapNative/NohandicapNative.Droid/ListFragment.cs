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

namespace NohandicapNative.Droid
{
  public  class ListFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
            var listView = view.FindViewById<ListView>(Resource.Id.listview);
            List<MarkerModel> items = new List<MarkerModel>();
            items.Add(new MarkerModel()
            {
                Id = 0.ToString(),
                Properties = new PropertiesModel() { Title = "Hello", Description = "Descript" },
                Image ="eat"

            });
            var listAdapter = new ListAdapter(Activity,items);
            listView.Adapter = listAdapter;
            return view;
        }
    }
}