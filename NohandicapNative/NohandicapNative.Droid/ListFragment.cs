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
using Android.App;

namespace NohandicapNative.Droid
{
  public  class ListFragment : Android.Support.V4.App.Fragment
    {
        MainActivity myContext;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
            var listView = view.FindViewById<ListView>(Resource.Id.listview);
            List<MarkerModel> items = new List<MarkerModel>();
            items.Add(new MarkerModel()
            {
                
               Title = "Restarant",
                Description = "Descript" ,
                Image ="eat"

            });
            items.Add(new MarkerModel()
            {

                Title = "Hotel",
                Description = "Descript",
                Image = "sleep"

            });
            items.Add(new MarkerModel()
            {

                Title = "Eavent",
                Description = "Descript",
                Image = "event"

            });
            var listAdapter = new ListAdapter(myContext,items);
            listView.Adapter = listAdapter;
            listView.ItemClick += (s, e) =>
            {
                int position = e.Position;
                    var detailIntent = new Intent(myContext, typeof(DetailActivity));
                    detailIntent.PutExtra("Title", items[position].Title);
                myContext.StartActivity(detailIntent);
               
            };
            return view;
        }
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
        }
    }
}