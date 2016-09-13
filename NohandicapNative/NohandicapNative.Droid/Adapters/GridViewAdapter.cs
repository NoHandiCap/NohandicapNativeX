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
using Android.Util;

namespace NohandicapNative.Droid.Adapters
{
    public class GridViewAdapter : BaseAdapter, IOnTouchListener
    {
        private MainActivity context;
        private List<CategoryModel> categories;
        private SqliteService dbCon;
        public GridViewAdapter(MainActivity context, List<CategoryModel> items)
        {
            this.context = context;
            this.categories = items;
            dbCon = Utils.GetDatabaseConnection();
        }
        public override int Count
        {
            get
            {
              return  categories.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public  string GetItemText(int position)
        {
            return categories[position].Name.ToString();
        }
        public override long GetItemId(int position)
        {
            return categories[position].ID;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View grid;
            LayoutInflater inflater = (LayoutInflater)context
                .GetSystemService(Context.LayoutInflaterService);
          
                var item = categories[position];
                grid = new View(context);
                grid = inflater.Inflate(Resource.Layout.grid_item, null);
                TextView textView = (TextView)grid.FindViewById(Resource.Id.grid_text);
                ImageView imageView = (ImageView)grid.FindViewById(Resource.Id.grid_image);
                textView.Text = item.Name;
                imageView.SetImageDrawable(Utils.GetImage(context, item.Icon));
                grid.SetBackgroundColor(Color.ParseColor(item.Color));
            DisplayMetrics displaymetrics = new DisplayMetrics();
            context.WindowManager.DefaultDisplay.GetMetrics(displaymetrics);
            int width = displaymetrics.WidthPixels;
            var orientation = context.Resources.Configuration.Orientation;
            int imageWidth;
            if (orientation == Android.Content.Res.Orientation.Portrait)
                imageWidth = width / 8;
            else
                imageWidth = width / 11;
            imageView.LayoutParameters.Height = imageWidth;
            imageView.LayoutParameters.Width = imageWidth;


            grid.Click += (s, e) =>
               {
                   var mainActivity = (MainActivity)context;
                   var products = dbCon.GetDataList<ProductModel>();
                   var currentProducts = products.Where(prod => prod.Categories.Any(cat => cat == categories[position].ID)).ToList();
                   mainActivity.MapPage.SetData(currentProducts, categories[position]);         
                   mainActivity.SetCurrentTab(1);
                   context.SupportActionBar.Title =item.Name;   


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