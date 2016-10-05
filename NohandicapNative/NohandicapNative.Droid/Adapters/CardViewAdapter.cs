using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using NohandicapNative.Droid.Services;
using Android.Graphics;
using System.Threading.Tasks;
using Square.Picasso;
using NohandicapNative.Droid.Model;

namespace NohandicapNative.Droid.Adapters
{
    public class CardViewAdapter : BaseAdapter<ProductModel>
    {
        string TAG = "X: " + typeof(CardView).Name;
        private readonly Activity context;
        private readonly List<ProductModel> products;
        List<CategoryModel> selectedCategory;
        List<CategoryModel> categories;
        public CardViewAdapter(Activity context, List<ProductModel> products)
        {
            this.context = context;
            this.products = products;
            var dbCon = Utils.GetDatabaseConnection();
            categories = dbCon.GetDataList<CategoryModel>();         
           selectedCategory = dbCon.GetDataList<CategoryModel>().Where(x => x.IsSelected).ToList();
   
            dbCon.Close();
        }
      

        public override ProductModel this[int position]
        {
            get
            {
                return products[position];
            }
        }

        public override int Count
        {
            get
            {
                return products.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {// view = context.LayoutInflater.Inflate(Resource.Layout.list_item, parent, false);
             view = context.LayoutInflater.Inflate(Resource.Layout.list_item_first, parent, false);
                view.SetBackgroundColor(Color.White);
            }
            var imageView = view.FindViewById<ImageView>(Resource.Id.mainImageView);
            var title = view.FindViewById<TextView>(Resource.Id.titleTextView);
            var adress = view.FindViewById<TextView>(Resource.Id.adressTextView);
            var positionTextView = view.FindViewById<TextView>(Resource.Id.positionTextView);
            var distanceLayout = view.FindViewById<LinearLayout>(Resource.Id.distanceLayout);
            title.Text = products[position].FirmName;
            adress.Text = products[position].Adress;
            if (products[position].DistanceToMyLocation != 0)
            {
                positionTextView.Text = NohandicapLibrary.ConvertMetersToKilometers(products[position].DistanceToMyLocation);
            }
            else
            {
                distanceLayout.Visibility = ViewStates.Gone;
            }
            if (!string.IsNullOrEmpty(products[position].MainImageUrl))
            {               
                    Picasso.With(context).Load(products[position].MainImageUrl).Resize(60,60).Into(imageView);
            }
            else
            {
                
                CategoryModel catImage;
                if (selectedCategory.Count != 0)
                {
                    catImage = selectedCategory.FirstOrDefault(x => products[position].Categories.Any(y => y == x.ID));
                }
                else
                {
                    catImage = categories.FirstOrDefault(x => products[position].Categories.Any(y => y == x.ID));

                }
                if (catImage != null)
                {
                    imageView.SetImageDrawable(Utils.GetImage(context, catImage.Icon));
                    imageView.SetBackgroundColor(Color.ParseColor(catImage.Color));
                }
            }
            //var frame = view.FindViewById<LinearLayout>(Resource.Id.itemFrame);
            //var mainimage = products[position].ImageCollection.Images;
            //if (mainimage.Count != 0)
            //{
            //    var bitmap = Utils.GetBitmap(mainimage[0].LocalImage);
            //    frame.SetBackgroundDrawable(new BitmapDrawable(bitmap));
            //        }

            //var title= view.FindViewById<TextView>(Resource.Id.titleTextViewItem);
            //title.Text = products[position].FirmName;
            #region DefaultStyle      
            //var categories = dbCon.GetDataList<CategoryModel>();
            //var titleTextView = view.FindViewById<TextView>(Resource.Id.title_text);
            //var image = view.FindViewById<ImageView>(Resource.Id.logo_image);
            //var adress = view.FindViewById<TextView>(Resource.Id.adress_text);
            //var body = view.FindViewById<TextView>(Resource.Id.body_text);
            //var photo = view.FindViewById<ImageView>(Resource.Id.image_photo);
            //var hours = view.FindViewById<TextView>(Resource.Id.hours_text);
            //var booking = view.FindViewById<TextView>(Resource.Id.booking_link);
            //var homeLink = view.FindViewById<TextView>(Resource.Id.main_link);
            //var mainimage = products[position].ImageCollection.Images;
            //if (mainimage.Count != 0)
            //{
            //    var img = mainimage[0];                       
            //        try
            //        {
            //            if (string.IsNullOrWhiteSpace(img.LocalImage))
            //            {
            //                string filename = "none";
            //                Uri uri = new Uri(img.LinkImage);
            //                filename = System.IO.Path.GetFileName(uri.LocalPath);                           
            //                Utils.SaveImageBitmapFromUrl(img.LinkImage, filename);
            //                img.LocalImage = filename;
            //                dbCon.InsertUpdateProduct(products[position].ImageCollection);
            //            }
            //        }catch(Exception e)
            //        {
            //            Log.Error(TAG, e.Message);
            //        }                            
            //    photo.SetImageDrawable(new BitmapDrawable(Utils.GetBitmap(mainimage[0].LocalImage)));
            //}
            //else
            //{
            //    photo.Visibility = ViewStates.Gone;
            //}

            //var catImage = categories.FirstOrDefault(x => x.ID == products[position].Categories[0]);

            //if (catImage != null)
            //{
            //    image.SetImageDrawable(Utils.GetImage(context, catImage.Icon));
            //    image.SetBackgroundColor(Color.ParseColor(catImage.Color));
            //}
            //image.SetPadding(5, 0, 5, 0);
            //titleTextView.Text = products[position].FirmName;
            //adress.Text = products[position].Adress;
            //body.TextFormatted = Html.FromHtml(products[position].Description);
            //if (products[position].OpenTime != "") {
            //    var time = products[position].OpenTime.Replace("</p><p>", "\n").Replace("<p>","").Replace("</p>", "");
            //    hours.TextFormatted = Html.FromHtml(time);                    
            //        };
            //if (products[position].BookingPage != "") booking.Text = products[position].BookingPage;
            //if (products[position].HomePage != "") homeLink.Text = products[position].HomePage;
            #endregion
            return view;
        }
        public async Task LoadImageAsync(ImageView imageView, string url)
        {
          
            imageView.SetBackgroundResource(Resource.Drawable.placeholder);
            Uri uri = new Uri(url);
            var filename = System.IO.Path.GetFileName(uri.LocalPath);
            imageView.SetImageBitmap(await Utils.LoadBitmapAsync(url));



        }
    }
}
