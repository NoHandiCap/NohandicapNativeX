
using System.Collections.Generic;

using Android.App;

using Android.Views;
using Android.Widget;

using Android.Graphics;

namespace NohandicapNative.Droid.Adapters
{
   public class RadioButtonListAdapter : BaseAdapter
    {
        private List<CustomRadioButton> _items;
        readonly Activity _context;
        private Android.Support.V4.App.FragmentActivity _activity;
        int[] _flags;
        int _defaultSelected;
        public RadioButtonListAdapter(Activity context,int[] flags, List<CustomRadioButton> items, int defaultSelected=0)
        {
            this._context = context;
            this._items = items;
            this._flags = flags;
            this._defaultSelected = defaultSelected;
        }



        public override int Count => _items.Count;

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
            var view = convertView ?? _context.LayoutInflater.Inflate(
        Resource.Layout.RadioButton, parent, false);       
            var image = view.FindViewById<ImageView>(Resource.Id.grid_image);
          var text= view.FindViewById<TextView>(Resource.Id.grid_text);
            text.Text = _items[position].Text;             
            text.TextSize = 12;    
            if (_defaultSelected == position)
            {
              
                text.SetTextColor(Color.White);
                view.SetBackgroundColor(_context.Resources.GetColor(Resource.Color.themeColor));
            }
            else
            {

                text.SetTextColor(Color.Black);
                view.SetBackgroundColor(Color.White);
            }
            var img = _context.Resources.GetDrawable(_flags[position]);    
            image.SetImageDrawable(img);
                
           
            //var description = view.FindViewById<TextView>(Resource.Id.description_item);
            //title.Text = _items[position].FirmName;
            //description.Text = _items[position].Description;
            //var mainimage = _items[position].ImageCollection.Images;
            //if (mainimage.Count != 0)
            //   image.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));   
            //image.LayoutParameters.Height = 100;
            //image.LayoutParameters.Width = 100;
            //image.SetPadding(5, 0, 5, 0);
            return view;

        }
    }
}