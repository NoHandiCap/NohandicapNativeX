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
using NohandicapNative.Droid.Services;
using Android.Graphics;

namespace NohandicapNative.Droid.Adapters
{
   public class RadioButtonListAdapter : BaseAdapter
    {
        private List<CustomRadioButton> items;
        Activity context;
        private Android.Support.V4.App.FragmentActivity activity;
        int[] flags;
        int defaultSelected;
        public RadioButtonListAdapter(Activity context,int[] flags, List<CustomRadioButton> items, int defaultSelected=0)
        {
            this.context = context;
            this.items = items;
            this.flags = flags;
            this.defaultSelected = defaultSelected;
        }



        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }
       
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? context.LayoutInflater.Inflate(
        Resource.Layout.RadioButton, parent, false);       
            var image = view.FindViewById<ImageView>(Resource.Id.grid_image);
          var text= view.FindViewById<TextView>(Resource.Id.grid_text);
            text.Text = items[position].Text;             
            text.TextSize = 12;    
            if (defaultSelected == position)
            {
              
                text.SetTextColor(Color.White);
                view.SetBackgroundColor(context.Resources.GetColor(Resource.Color.themeColor));
            }
            else
            {

                text.SetTextColor(Color.Black);
                view.SetBackgroundColor(Color.White);
            }
            var img = context.Resources.GetDrawable(flags[position]);    
            image.SetImageDrawable(img);
                
           
            //var description = view.FindViewById<TextView>(Resource.Id.description_item);
            //title.Text = items[position].FirmName;
            //description.Text = items[position].Description;
            //var mainimage = items[position].ImageCollection.Images;
            //if (mainimage.Count != 0)
            //   image.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));   
            //image.LayoutParameters.Height = 100;
            //image.LayoutParameters.Width = 100;
            //image.SetPadding(5, 0, 5, 0);
            return view;

        }
    }
}