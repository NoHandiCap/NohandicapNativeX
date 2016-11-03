
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using static Android.Views.View;
using Android.Graphics.Drawables;
using Java.Lang;
using NohandicapNative.Droid.Activities;
using NohandicapNative.Droid.Fragments;
using Context = Android.Content.Context;


namespace NohandicapNative.Droid.Adapters
{
    public class GridViewAdapter : BaseAdapter
    {
        private  BaseFragment _baseFragment;
        private List<CategoryModel> _categories;   
   
        public GridViewAdapter(BaseFragment baseFragment)
        {
            this._baseFragment = baseFragment;
            UpdateCategories();
        }
        public void UpdateCategories()
        {
            _categories = _baseFragment.DbConnection.GetDataList<CategoryModel>(x => x.Group == NohandicapLibrary.SubCatGroup).OrderBy(x=>x.Sort).ToList();
        }
        public override int Count => _categories.Count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }
        public  string GetItemText(int position)
        {
            return _categories[position].Name.ToString();
        }
        public override long GetItemId(int position)
        {
            return _categories[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflater = (LayoutInflater)_baseFragment.MainActivity
                .GetSystemService(Context.LayoutInflaterService);
            GridView gridView = (GridView)parent;
            var category = _categories[position];
            var grid = inflater.Inflate(Resource.Layout.grid_item, null);
            //   var backgroundButton = grid.FindViewById<RelativeLayout>(Resource.Id.backgroundLayout);
            TextView textView = (TextView)grid.FindViewById(Resource.Id.grid_text);
            ImageView imageView = (ImageView)grid.FindViewById(Resource.Id.grid_image);
            textView.Text = category.Name;
            imageView.SetImageDrawable(Utils.GetImage(_baseFragment.MainActivity, category.Icon));
            LayerDrawable bgDrawable = (LayerDrawable)grid.Background;
            GradientDrawable bgShape = (GradientDrawable)bgDrawable.FindDrawableByLayerId(Resource.Id.shape_id);
            GradientDrawable bgBorder = (GradientDrawable)bgDrawable.FindDrawableByLayerId(Resource.Id.border_id);

            int width = gridView.ColumnWidth;
            var orientation = _baseFragment.Resources.Configuration.Orientation;
            imageView.LayoutParameters.Height = width / 3;
            imageView.LayoutParameters.Width = width / 3;
            bgShape.SetColor(Color.ParseColor(category.Color));
            if (NohandicapApplication.IsTablet)
            {
                if (category.IsSelected)
                {
                    bgBorder.SetColor(_baseFragment.Resources.GetColor(Resource.Color.selectedCategoryColor));
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