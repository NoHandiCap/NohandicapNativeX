using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.App;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using NohandicapNative.Droid.Adapters;
using Android.App;
using static Android.Views.View;
using Android.Graphics.Drawables;

namespace NohandicapNative.Droid
{
  public  class HomeFragment: Android.Support.V4.App.Fragment
    {
        MainActivity myContext;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            var view = inflater.Inflate(Resource.Layout.HomePage,container,false);
            //view.SetBackgroundColor(Color.ParseColor("#FFECB3"));

            //  GridView mainCategory = view.FindViewById<GridView>(Resource.Id.mainCategory);
            TextView first = view.FindViewById<TextView>(Resource.Id.first_category);
            TextView second = view.FindViewById<TextView>(Resource.Id.second_category);
            TextView three = view.FindViewById<TextView>(Resource.Id.thrity_category);
            ImageView firstImg = view.FindViewById<ImageView>(Resource.Id.imageView);
            ImageView secondImg = view.FindViewById<ImageView>(Resource.Id.imageView2);
            ImageView threeImg = view.FindViewById<ImageView>(Resource.Id.imageView3);
            first.SetTextColor(Color.Gray);
            second.SetTextColor(Color.Black);
            three.SetTextColor(Color.Gray);
            first.Click += (s, e) => {
                TextView textView = (TextView)s;
                if (!textView.Selected)
                {
                    textView.SetTextColor(Color.Black);
                    textView.Selected = true;
                    second.SetTextColor(Color.Gray);
                    second.Selected = false;
                    three.SetTextColor(Color.Gray);
                    three.Selected = false;
                }                
            };
            second.Click += (s, e) => {
                TextView textView = (TextView)s;
                if (!textView.Selected)
                {
                    textView.SetTextColor(Color.Black);
                    textView.Selected = true;
                    first.SetTextColor(Color.Gray);
                    first.Selected = false;
                    three.SetTextColor(Color.Gray);
                    three.Selected = false;
                }               
            };
            three.Click += (s, e) => {
                TextView textView = (TextView)s;
                if (!textView.Selected)
                {
                    textView.SetTextColor(Color.Black);
                    textView.Selected = true;
                    first.SetTextColor(Color.Gray);
                    first.Selected = false;
                    second.SetTextColor(Color.Gray);
                    second.Selected = false;
                }                
            };
            firstImg.SetImageDrawable(Utils.SetDrawableSize(myContext,Resource.Drawable.wheelchair,40,40));
            secondImg.SetImageDrawable(Utils.SetDrawableSize(myContext, Resource.Drawable.wheelchair2, 75, 40));
            threeImg.SetImageDrawable(Utils.SetDrawableSize(myContext, Resource.Drawable.wheelchair3, 105, 40));

            List<TabItem> mainItems = NohandiLibrary.GetMainCategory();
            first.Text = mainItems[0].Title;
            
            second.Text = mainItems[1].Title;
            three.Text = mainItems[2].Title;
          // GridView additionalCategory = view.FindViewById<GridView>(Resource.Id.additionalCategory);
            List<TabItem> additItems = NohandiLibrary.GetAdditionalCategory();

            GridLayout gridLayout = view.FindViewById<GridLayout>(Resource.Id.additionalCategoryGrid);

            for (int i = 0; i < gridLayout.ChildCount; i++)
            {
                var item = additItems[i];
                var button =(Button)gridLayout.GetChildAt(i);
                button.Text = item.Title;
                button.SetTextColor(Color.White);
                button.SetCompoundDrawablesWithIntrinsicBounds(null, Utils.GetImage(myContext, item.Image), null, null);
                button.SetBackgroundColor(Color.ParseColor(item.Color));
                button.TextSize = 10f;
                button.Click += (s, e) =>
                {

                    Android.Support.V4.App.FragmentManager fragmentManager = myContext.SupportFragmentManager;
                    myContext.SetCurrentTab(1);
                    myContext.SupportActionBar.Title = (s as Button).Text;
                };
            }
            //int total = additItems.Count;
            //int column = 3;
            //int row = total / column;

            //for (int i = 0, c = 0, r = 0; i < total; i++, c++)
            //{
            //    if (c == column)
            //    {
            //        c = 0;
            //        r++;
            //    }
            //    var item = additItems[i];
            //    var button = new Button(myContext);
            //    button.Text = item.Title;
            //    button.SetTextColor(Color.White);            
            //    button.SetCompoundDrawablesWithIntrinsicBounds(null, Utils.GetImage(myContext, item.Image), null, null);
            //    button.SetBackgroundColor(Color.ParseColor(item.Color));

            //    // button.SetWidth(h);
            //    //  button.SetHeight(h - 50);

            //    button.TextSize = 10f;
            //    button.Click += (s, e) =>
            //    {

            //        Android.Support.V4.App.FragmentManager fragmentManager = myContext.SupportFragmentManager;
            //        myContext.SetCurrentTab(1);
            //        myContext.SupportActionBar.Title = (s as Button).Text;
            //    };
            //    GridLayout.LayoutParams param = new GridLayout.LayoutParams();
            //    param.Height = GridLayout.LayoutParams.WrapContent;
            //    param.Width = GridLayout.LayoutParams.WrapContent;
            //    param.RightMargin = 5;
            //    param.TopMargin = 5;
            //    param.SetGravity(GravityFlags.FillVertical);
            //    param.ColumnSpec = GridLayout.InvokeSpec(c);
            //    param.RowSpec = GridLayout.InvokeSpec(r);
            //    button.LayoutParameters = param;
            //    gridLayout.AddView(button);
            //}
            //for (int i = 0; i < additItems.Count-1; i++)
            //{
            //    var item = additItems[i];
            //    var button = new Button(myContext);
            //    button.Text = item.Title;
            //    button.SetTextColor(Color.White);

            //    button.SetCompoundDrawablesWithIntrinsicBounds(null, Utils.GetImage(myContext, item.Image), null, null);
            //    button.SetBackgroundColor(Color.ParseColor(item.Color));

            //    // button.SetWidth(h);
            //    //  button.SetHeight(h - 50);

            //    button.TextSize = 10f;
            //    button.Click += (s, e) =>
            //    {

            //        Android.Support.V4.App.FragmentManager fragmentManager = myContext.SupportFragmentManager;
            //        myContext.SetCurrentTab(1);
            //        myContext.SupportActionBar.Title = (s as Button).Text;
            //    };
            //}
            //  List<TabItem> mainItems = NohandiLibrary.GetMainCategory();
            //  mainCategory.Adapter = new GridViewAdapter(myContext, mainItems);
            //     additionalCategory.Adapter= new GridViewAdapter(myContext, additItems);


            return view;
        }
        public override void OnAttach(Activity activity)
        {
            myContext = (MainActivity)activity;
            base.OnAttach(activity);
        }
        //public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        //{

        //    base.OnCreateOptionsMenu(menu, inflater);
        //    menu.Clear();

        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.Base11)
        //    {
        //        // selectMenu(menu);
        //        inflater.Inflate(Resource.Menu.home_menu, menu);
        //    }

        //}

    }
}