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

namespace NohandicapNative.Droid.Adapters
{
    public class GridViewAdapter : BaseAdapter
    {
        private Context context;
        private List<TabItem> items;
        public GridViewAdapter(Context context, List<TabItem> items)
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

        public override long GetItemId(int position)
        {
            return items[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
           var button = new Button(context);
            button.Text = item.Title;
            button.SetCompoundDrawablesWithIntrinsicBounds(null, Utils.GetImage(context, item.Image), null, null);
            button.SetBackgroundColor(Color.ParseColor(item.Color));
            button.SetWidth(170);
            button.SetHeight(180);
            button.TextSize = 9.5f;
            return button;
        }
    }
}