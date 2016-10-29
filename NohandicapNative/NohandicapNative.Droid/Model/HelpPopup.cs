using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using String = System.String;

namespace NohandicapNative.Droid.Model
{
    public class HelpPopup
    {
        protected Context mContext;
        protected PopupWindow mWindow;

        private TextView mHelpTextView;
        private ImageView mUpImageView;
        private ImageView mDownImageView;
        protected View mView;
        private IWindowManager WindowManager;
        protected Drawable mBackgroundDrawable = null;
        protected IShowListener showListener;

        public HelpPopup(Context context, String text, int viewResource)
        {
            mContext = context;
            mWindow = new PopupWindow(context);
            LayoutInflater layoutInflater = (LayoutInflater)context
                .GetSystemService(Context.LayoutInflaterService);

            SetContentView(layoutInflater.Inflate(viewResource, null));
            WindowManager = NohandicapApplication.MainActivity.WindowManager;
            mHelpTextView = (TextView)mView.FindViewById(Resource.Id.text);
            mUpImageView = (ImageView)mView.FindViewById(Resource.Id.arrow_up);
            mDownImageView = (ImageView)mView.FindViewById(Resource.Id.arrow_down);

            mHelpTextView.MovementMethod = ScrollingMovementMethod.Instance;
            mHelpTextView.Selected = true;
        }

        public HelpPopup(Context context) : this(context, "", Resource.Layout.popup_layout)
        {
        }

        public HelpPopup(Context context, String text) : this(context)
        {
            SetText(text);
        }

        public void Show(View anchor)
        {
            PreShow();

            int[] location = new int[2];

            anchor.GetLocationOnScreen(location);

            Rect anchorRect = new Rect(location[0], location[1], location[0]
                                                                 + anchor.Width, location[1] + anchor.Height);

            mView.Measure(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            int rootHeight = mView.MeasuredHeight;
            int rootWidth = mView.MeasuredWidth;

            int screenWidth = NohandicapApplication.MainActivity.WindowManager.DefaultDisplay.Width;
            int screenHeight = NohandicapApplication.MainActivity.WindowManager.DefaultDisplay.Height;

            int yPos = anchorRect.Top - rootHeight;

            bool onTop = true;

            if (anchorRect.Top < screenHeight / 2)
            {
                yPos = anchorRect.Bottom;
                onTop = false;
            }

            int whichArrow, requestedX;

            whichArrow = ((onTop) ? Resource.Id.arrow_down : Resource.Id.arrow_up);
            requestedX = anchorRect.CenterX();

            View arrow = whichArrow == Resource.Id.arrow_up
                ? mUpImageView
                : mDownImageView;
            View hideArrow = whichArrow == Resource.Id.arrow_up
                ? mDownImageView
                : mUpImageView;

            int arrowWidth = arrow.MeasuredWidth;

            arrow.Visibility = ViewStates.Visible;

            ViewGroup.MarginLayoutParams param = (ViewGroup.MarginLayoutParams)arrow.LayoutParameters;

            hideArrow.Visibility = ViewStates.Invisible;

            int xPos = 0;

            // ETXTREME RIGHT CLIKED
            if (anchorRect.Left + rootWidth > screenWidth)
            {
                xPos = (screenWidth - rootWidth);
            }
            // ETXTREME LEFT CLIKED
            else if (anchorRect.Left - (rootWidth / 2) < 0)
            {
                xPos = anchorRect.Left;
            }
            // INBETWEEN
            else
            {
                xPos = (anchorRect.CenterX() - (rootWidth / 2));
            }

            param.LeftMargin = (requestedX - xPos) - (arrowWidth / 2);

            if (onTop)
            {
                int height = anchorRect.Top - anchorRect.Height();
                mHelpTextView.SetMaxHeight(height);

            }
            else
            {
                mHelpTextView.SetMaxHeight(screenHeight - yPos);
            }

            mWindow.ShowAtLocation(anchor, GravityFlags.NoGravity, xPos, yPos);

            mView.Animation = AnimationUtils.LoadAnimation(mContext,
                Resource.Animation.float_anim);

        }

        protected void PreShow()
        {
            if (mView == null)
                throw new IllegalStateException("view undefined");



            if (showListener != null)
            {
                showListener.OnPreShow();
                showListener.OnShow();
            }

            if (mBackgroundDrawable == null)
                mWindow.SetBackgroundDrawable(new BitmapDrawable());
            else
                mWindow.SetBackgroundDrawable(mBackgroundDrawable);

            mWindow.Width = ViewGroup.LayoutParams.WrapContent;
            mWindow.Height = ViewGroup.LayoutParams.WrapContent;
            mWindow.Touchable = true;
            mWindow.Focusable = true;
            mWindow.OutsideTouchable = true;
            mWindow.ContentView = mView;
        }

        public void SetBackgroundDrawable(Drawable background)
        {
            mBackgroundDrawable = background;
        }

        public void SetContentView(View root)
        {
            mView = root;

            mWindow.ContentView = root;
        }

        public void SetContentView(int layoutResID)
        {
            LayoutInflater inflator = (LayoutInflater)mContext
                .GetSystemService(Context.LayoutInflaterService);

            SetContentView(inflator.Inflate(layoutResID, null));
        }

        public void SetOnDismissListener(PopupWindow.IOnDismissListener listener)
        {
            mWindow.SetOnDismissListener(listener);
        }

        public void Dismiss()
        {
            mWindow.Dismiss();
            if (showListener != null)
            {
                showListener.OnDismiss();
            }
        }

        public void SetText(String text)
        {
            mHelpTextView.Text = text;
        }

        public interface IShowListener
        {
            void OnPreShow();
            void OnDismiss();
            void OnShow();
        }

        public void SetShowListener(IShowListener showListener)
        {
            this.showListener = showListener;
        }
    }
}
