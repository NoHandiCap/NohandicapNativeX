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
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using static Android.Graphics.Paint;

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
            
          
            //border.SetStroke(2, context.Resources.GetColor(Resource.Color.selectedCategoryColor)); //border with full opacity
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
            GridView gridView = (GridView)parent;
                var category =dbCon.GetDataList<CategoryModel>().FirstOrDefault(x=>x.ID == categories[position].ID);
                grid = new View(context);
                grid = inflater.Inflate(Resource.Layout.grid_item, null);
         //   var backgroundButton = grid.FindViewById<RelativeLayout>(Resource.Id.backgroundLayout);
                TextView textView = (TextView)grid.FindViewById(Resource.Id.grid_text);
                ImageView imageView = (ImageView)grid.FindViewById(Resource.Id.grid_image);
                textView.Text = category.Name;
                imageView.SetImageDrawable(Utils.GetImage(context, category.Icon));

            LayerDrawable bgDrawable = (LayerDrawable)grid.Background;
            GradientDrawable bgShape = (GradientDrawable)bgDrawable.FindDrawableByLayerId(Resource.Id.shape_id);
            GradientDrawable bgBorder = (GradientDrawable)bgDrawable.FindDrawableByLayerId(Resource.Id.border_id);


            int width = gridView.ColumnWidth;
            var orientation = context.Resources.Configuration.Orientation;
            
        
            imageView.LayoutParameters.Height = width/3;
            imageView.LayoutParameters.Width = width/3;
            bgShape.SetColor(Color.ParseColor(category.Color));
            if (NohandicapApplication.isTablet)
            {


                if (category.IsSelected)
                {
                    bgBorder.SetColor(context.Resources.GetColor(Resource.Color.selectedCategoryColor));
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

            grid.Click += (s, e) =>
               {
                   var mainActivity = (MainActivity)context;
                   var products = dbCon.GetDataList<ProductModel>();


                   if (!NohandicapApplication.isTablet)
                   {                      
                       mainActivity.MapPage.SetData(new List<CategoryModel> { categories[position] });
                       mainActivity.SetCurrentTab(1);
                       context.SupportActionBar.Title = category.Name;
                       categories.ForEach(x => {
                           if (category.ID == x.ID)
                           {
                               category.IsSelected = true;
                               dbCon.InsertUpdateProduct(category);
                           }
                           else
                           {
                               x.IsSelected = false;
                               dbCon.InsertUpdateProduct(x);
                           }
                       });
                   }
                   else
                   {
                      
                       if (category.IsSelected)
                       {
                           category.IsSelected = false;
                           dbCon.InsertUpdateProduct(category);
                           NotifyDataSetChanged();
                       }else
                       {
                           category.IsSelected = true;
                           dbCon.InsertUpdateProduct(category);
                           NotifyDataSetChanged();
                       }
                       var selectedCategories = dbCon.GetDataList<CategoryModel>().Where(x => x.IsSelected).ToList();
                       if (selectedCategories.Count == 0)
                       {

                       mainActivity.MapPage.SetData(categories);
                       }
                       else
                       {
                           mainActivity.MapPage.SetData(selectedCategories);

                       }
                       mainActivity.MapPage.LoadData();
                   }                 

               };


            return grid;
        
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