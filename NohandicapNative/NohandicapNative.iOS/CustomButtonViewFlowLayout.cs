using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using UIKit;
using System.Drawing;
using Foundation;

namespace NohandicapNative.iOS
{
    public class CustomButtonViewFlowLayout : UICollectionViewFlowLayout
    {
        public CustomButtonViewFlowLayout()
        {
            SetupLayout();
        }
        public CustomButtonViewFlowLayout(NSCoder coder) : base(coder)
        {
            SetupLayout();
        }
        public void SetupLayout()
        {
            MinimumInteritemSpacing = 1f;
            MinimumLineSpacing = 1f;
            ScrollDirection = UICollectionViewScrollDirection.Vertical;
          
        }
       
        public override CGSize ItemSize
        {
            get
            {
                var numberOfColumns = 3;
                var itemWidth = (this.CollectionView.Frame.Width) - (numberOfColumns - 1) / numberOfColumns;
                return new CGSize(100,100);
            }

            set
            {
                base.ItemSize = value;
            }
        }

    }
   
}
