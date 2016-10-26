
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using static Android.Views.View;
using Android.Graphics.Drawables;
using NohandicapNative.Droid.Fragments;

namespace NohandicapNative.Droid.Adapters
{
    public class GridViewAdapter : BaseAdapter
    {
        private BaseFragment baseFragment;
        private List<CategoryModel> categories;   
   
        public GridViewAdapter(BaseFragment baseFragment)
        {
            this.baseFragment = baseFragment;
            UpdateCategories();
        }
        public void UpdateCategories()
        {
            categories = baseFragment.DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup);
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
            return categories[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {

            View grid;
            LayoutInflater inflater = (LayoutInflater)baseFragment.MainActivity
                .GetSystemService(Context.LayoutInflaterService);
            GridView gridView = (GridView)parent;
            var category = categories[position];
            grid = new View(baseFragment.MainActivity);
            grid = inflater.Inflate(Resource.Layout.grid_item, null);
            //   var backgroundButton = grid.FindViewById<RelativeLayout>(Resource.Id.backgroundLayout);
            TextView textView = (TextView)grid.FindViewById(Resource.Id.grid_text);
            ImageView imageView = (ImageView)grid.FindViewById(Resource.Id.grid_image);
            textView.Text = category.Name;
            imageView.SetImageDrawable(Utils.GetImage(baseFragment.MainActivity, category.Icon));
            LayerDrawable bgDrawable = (LayerDrawable)grid.Background;
            GradientDrawable bgShape = (GradientDrawable)bgDrawable.FindDrawableByLayerId(Resource.Id.shape_id);
            GradientDrawable bgBorder = (GradientDrawable)bgDrawable.FindDrawableByLayerId(Resource.Id.border_id);

            int width = gridView.ColumnWidth;
            var orientation = baseFragment.Resources.Configuration.Orientation;
            imageView.LayoutParameters.Height = width / 3;
            imageView.LayoutParameters.Width = width / 3;
            bgShape.SetColor(Color.ParseColor(category.Color));
            if (NohandicapApplication.IsTablet)
            {
                if (category.IsSelected)
                {
                    bgBorder.SetColor(baseFragment.Resources.GetColor(Resource.Color.selectedCategoryColor));
                }
                else
                {
                    bgBorder.SetColor(Color.ParseColor(category.Color));
                }
            }
            else
            {
                bgBorder.SetColor(Color.ParseColor(category.Color));
            }
            return grid;

        }  
    }
}