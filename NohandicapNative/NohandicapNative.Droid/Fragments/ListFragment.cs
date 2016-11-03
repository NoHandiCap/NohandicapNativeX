using System.Linq;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Activities;
using NohandicapNative.Droid.Adapters;
using NohandicapNative.Droid.Services;

namespace NohandicapNative.Droid.Fragments
{
  public  class ListFragment : BaseFragment
    {           
        ListView _listView;
        CardViewAdapter _cardViewAdapter;
        TextView _mainCategoryName;
        ImageView _mainCategoryImage;
        TextView _subCategoryName;
       
        public ListFragment(bool loadFromCache = true) : base(loadFromCache) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ListPage, container, false);
            _listView = view.FindViewById<ListView>(Resource.Id.listview);
            view.SetBackgroundColor(Resources.GetColor(Resource.Color.backgroundColor));
            _mainCategoryName = view.FindViewById<TextView>(Resource.Id.mainCategoryTextView);
            _mainCategoryImage = view.FindViewById<ImageView>(Resource.Id.mainCategoryImageView);
            _subCategoryName = view.FindViewById<TextView>(Resource.Id.subCategoryTextView);       
            _listView.ItemClick += ListView_ItemClick; 
            ReloadData();
            return view;
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {          
            var productId = (int)e.Id;
            var activity = new Intent(Activity, typeof(DetailActivity));
            activity.PutExtra(Utils.PRODUCT_ID, productId);
            MainActivity.StartActivityForResult(activity, 1);
        }
       
        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!hidden)
            {
                ReloadData();
            }
        }

        private void ReloadData()
        {
            var selectedSubCategory = DbConnection.GetSubSelectedCategory();
            if (selectedSubCategory.Count != 9)
            {
                var categories = selectedSubCategory.Aggregate("", (current, item) => current + (item.Name + ", "));
                categories = categories.Substring(0, categories.Length - 2);                
                _subCategoryName.Text = categories;
            }
            else
            {
                _subCategoryName.Text = Resources.GetString(Resource.String.all_cat);
            }
            _mainCategoryName.Text = SelectedMainCategory.Name;         
            var image = Utils.GetImage(Activity, "wheelchair" + SelectedMainCategory.Id);
            _mainCategoryImage.SetImageDrawable(Utils.SetDrawableSize(Activity, image, 140, 65));
            _cardViewAdapter = new CardViewAdapter(this,false);
            _listView.Adapter = _cardViewAdapter;   
        }
    }
}