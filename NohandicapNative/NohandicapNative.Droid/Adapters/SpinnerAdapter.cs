
using Android.Content;

using Android.Views;
using Android.Widget;


namespace NohandicapNative.Droid.Adapters
{
    

    public class SpinnerAdapter : BaseAdapter
    {
        int[] flags;
        string[] countryNames;
        LayoutInflater inflter;

        public override int Count
        {
            get
            {
                return flags.Length;
            }
        }

        public SpinnerAdapter(Context applicationContext, int[] flags, string[] countryNames)
        {

          
            this.flags = flags;
            this.countryNames = countryNames;
            inflter = (LayoutInflater.From(applicationContext));
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
           var view = inflter.Inflate(Resource.Layout.spinner_row, null);
            ImageView icon = (ImageView)view.FindViewById(Resource.Id.imageView);
            TextView names = (TextView)view.FindViewById(Resource.Id.textView);
            icon.SetImageResource(flags[position]);
            names.Text=countryNames[position];
            return view;
        }
    }
}