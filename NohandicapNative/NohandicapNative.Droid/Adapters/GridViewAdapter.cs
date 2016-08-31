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
using Java.Lang;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using Android.Support.V4.App;
using static Android.Views.View;

namespace NohandicapNative.Droid.Adapters
{
    public class GridViewAdapter : BaseAdapter, IOnTouchListener
    {
        private MainActivity context;
        private List<TabItem> items;
        public GridViewAdapter(MainActivity context, List<TabItem> items)
        {
            this.context = context;
            this.items = items;
        }
        public override int Count
        {
            get
            {
              return  items.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public  string GetItemText(int position)
        {
            return items[position].Title;
        }
        public override long GetItemId(int position)
        {
            return items[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View grid;
            LayoutInflater inflater = (LayoutInflater)context
                .GetSystemService(Context.LayoutInflaterService);
          
                var item = items[position];
                grid = new View(context);
                grid = inflater.Inflate(Resource.Layout.grid_item, null);
                TextView textView = (TextView)grid.FindViewById(Resource.Id.grid_text);
                ImageView imageView = (ImageView)grid.FindViewById(Resource.Id.grid_image);
                textView.Text = item.Title;
                imageView.SetImageDrawable(Utils.GetImage(context, item.Image));
                grid.SetBackgroundColor(Color.ParseColor(item.Color));

            grid.Click += (s, e) =>
               {
                   var activity = (MainActivity)context;
                   Android.Support.V4.App.FragmentManager fragmentManager = activity.SupportFragmentManager;
                   activity.SetCurrentTab(1);
                   context.SupportActionBar.Title = GetItemText(position);



               };


            return grid;
            // var grid = (GridView)parent;
            // int h = grid.ColumnWidth;
            // int minSize = h * 3;
            // grid.SetMinimumHeight(minSize);
            // grid.SetOnTouchListener(this);
            // var item = items[position];
            //var button = new Button(context);
            // button.Text = item.Title;
            // button.SetTextColor(Color.White);

            // button.SetCompoundDrawablesWithIntrinsicBounds(null, Utils.GetImage(context, item.Image), null, null);
            // button.SetBackgroundColor(Color.ParseColor(item.Color));
            // button.SetWidth(h);
            // button.SetHeight(h-50);

            // button.TextSize = 10f;
            // button.Click += (s,e)=>
            //   {
            //       var activity = (MainActivity)context;
            //       Android.Support.V4.App.FragmentManager fragmentManager = activity.SupportFragmentManager;
            //       activity.SetCurrentTab(1);
            //       context.SupportActionBar.Title = (s as Button).Text;



            //   };
            // return button;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Scroll)
            {
            return false;
        }
        return false;
        }
    }
}