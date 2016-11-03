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
using NohandicapNative.Droid.Activities;
using String = System.String;

namespace NohandicapNative.Droid.Model
{
    public class HelpPopup
    {
        private readonly Context mContext;
        private readonly PopupWindow mWindow;

        private TextView _mHelpTextView;
        private ImageView _mUpImageView;
        private ImageView _mDownImageView;
        private View _mView;
        private IWindowManager _windowManager;
        private Drawable _mBackgroundDrawable = null;
        private IShowListener _showListener;

        private HelpPopup(Context context, string text, int viewResource)
        {
            mContext = context;
            mWindow = new PopupWindow(context);
            LayoutInflater layoutInflater = (LayoutInflater)context
                .GetSystemService(Context.LayoutInflaterService);

            SetContentView(layoutInflater.Inflate(viewResource, null));
            _windowManager = NohandicapApplication.MainActivity.WindowManager;
            _mHelpTextView = (TextView)_mView.FindViewById(Resource.Id.text);
            _mUpImageView = (ImageView)_mView.FindViewById(Resource.Id.arrow_up);
            _mDownImageView = (ImageView)_mView.FindViewById(Resource.Id.arrow_down);

            _mHelpTextView.MovementMethod = ScrollingMovementMethod.Instance;
            _mHelpTextView.Selected = true;
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

            _mView.Measure(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

            int rootHeight = _mView.MeasuredHeight;
            int rootWidth = _mView.MeasuredWidth;

            int screenWidth = NohandicapApplication.MainActivity.WindowManager.DefaultDisplay.Width;
            int screenHeight = NohandicapApplication.MainActivity.WindowManager.DefaultDisplay.Height;

            int yPos = anchorRect.Top - rootHeight;

            bool onTop = true;

            if (anchorRect.Top < screenHeight / 2)
            {
                yPos = anchorRect.Bottom;
                onTop = false;
            }

            var whichArrow = ((onTop) ? Resource.Id.arrow_down : Resource.Id.arrow_up);
            var requestedX = anchorRect.CenterX();

            View arrow = whichArrow == Resource.Id.arrow_up
                ? _mUpImageView
                : _mDownImageView;
            View hideArrow = whichArrow == Resource.Id.arrow_up
                ? _mDownImageView
                : _mUpImageView;

            int arrowWidth = arrow.MeasuredWidth;

            arrow.Visibility = ViewStates.Visible;

            var param = (ViewGroup.MarginLayoutParams)arrow.LayoutParameters;

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
                _mHelpTextView.SetMaxHeight(height);

            }
            else
            {
                _mHelpTextView.SetMaxHeight(screenHeight - yPos);
            }

            mWindow.ShowAtLocation(anchor, GravityFlags.NoGravity, xPos, yPos);

            _mView.Animation = AnimationUtils.LoadAnimation(mContext,
                Resource.Animation.float_anim);

        }

        private void PreShow()
        {
            if (_mView == null)
                throw new IllegalStateException("view undefined");



            if (_showListener != null)
            {
                _showListener.OnPreShow();
                _showListener.OnShow();
            }

            if (_mBackgroundDrawable == null)
                mWindow.SetBackgroundDrawable(new BitmapDrawable());
            else
                mWindow.SetBackgroundDrawable(_mBackgroundDrawable);

            mWindow.Width = ViewGroup.LayoutParams.WrapContent;
            mWindow.Height = ViewGroup.LayoutParams.WrapContent;
            mWindow.Touchable = true;
            mWindow.Focusable = true;
            mWindow.OutsideTouchable = true;
            mWindow.ContentView = _mView;
        }

        public void SetBackgroundDrawable(Drawable background)
        {
            _mBackgroundDrawable = background;
        }

        public void SetContentView(View root)
        {
            _mView = root;

            mWindow.ContentView = root;
        }

        public void SetContentView(int layoutResId)
        {
            LayoutInflater inflator = (LayoutInflater)mContext
                .GetSystemService(Context.LayoutInflaterService);

            SetContentView(inflator.Inflate(layoutResId, null));
        }

        public void SetOnDismissListener(PopupWindow.IOnDismissListener listener)
        {
            mWindow.SetOnDismissListener(listener);
        }

        public void Dismiss()
        {
            mWindow.Dismiss();
            _showListener?.OnDismiss();
        }

        public void SetText(string text)
        {
            _mHelpTextView.Text = text;
        }

        public interface IShowListener
        {
            void OnPreShow();
            void OnDismiss();
            void OnShow();
        }

        public void SetShowListener(IShowListener showListener)
        {
            this._showListener = showListener;
        }
    }
}
