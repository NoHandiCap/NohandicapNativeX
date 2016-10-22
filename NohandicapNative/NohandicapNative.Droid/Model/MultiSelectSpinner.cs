using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Util;
using Java.Util;

namespace NohandicapNative.Droid.Model
{
    class MultiSelectSpinner : Spinner, IDialogInterfaceOnMultiChoiceClickListener
    {   string[] _items = null;
        bool[] _selection = null;
        ArrayAdapter<string> _proxyAdapter;


        public MultiSelectSpinner(Context context) : base(context)
        {



            _proxyAdapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem);

            base.Adapter = _proxyAdapter;

        }



        /**

         * Constructor used by the layout inflater.

         * @param context

         * @param attrs

         */

        public MultiSelectSpinner(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _proxyAdapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleSpinnerItem);

            base.Adapter = _proxyAdapter;
        }

        public void OnClick(IDialogInterface dialog, int which, bool isChecked)
        {

            if (_selection != null && which < _selection.Length)
            {

                _selection[which] = isChecked;



                _proxyAdapter.Clear();

                _proxyAdapter.Add(BuildSelectedItemString());

                SetSelection(0);

            }

            else
            {

                //   throw new IllegalArgumentException("Argument 'which' is out of bounds.");

            }

        }


        public override bool PerformClick()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetMultiChoiceItems(_items, _selection, this);
            builder.Show();
            return true;
        }
        public void SetItems(string[] items)
        {

            _items = items;

            _selection = new bool[_items.Length];



            Arrays.Fill(_selection, false);
        }
        

        public void SetItems(List<string> items)
        {

            _items = items.ToArray();

            _selection = new bool[_items.Length];



            Arrays.Fill(_selection, false);

        }

        public void setSelection(string[] selection)
        {

            foreach (string sel in selection)
            {

                for (int j = 0; j < _items.Length; ++j)
                {

                    if (_items[j].Equals(sel))
                    {

                        _selection[j] = true;

                    }

                }

            }

        }

        public void SetSelection(List<string> selection)
        {

            foreach (string sel in selection)
            {

                for (int j = 0; j < _items.Length; ++j)
                {

                    if (_items[j].Equals(sel))
                    {

                        _selection[j] = true;

                    }

                }

            }

        }
        
        public void SetSelection(int[] selectedIndicies)
        {

            foreach (int index in selectedIndicies)
            {

                if (index >= 0 && index < _selection.Length)
                {

                    _selection[index] = true;

                }

                else
                {

                     throw new Exception("Index " + index + " is out of bounds.");

                }

            }

        }



        /**

         * Returns a list of strings, one for each selected item.

         * @return

         */

        public List<string> GetSelectedStrings()
        {

            List<string> selection = new List<string>();

            for (int i = 0; i < _items.Length; ++i)
            {

                if (_selection[i])
                {

                    selection.Add(_items[i]);

                }

            }

            return selection;

        }



        /**

         * Returns a list of positions, one for each selected item.

         * @return

         */

        public List<int> GetSelectedIndicies()
        {

            List<int> selection = new List<int>();

            for (int i = 0; i < _items.Length; ++i)
            {

                if (_selection[i])
                {

                    selection.Add(i);

                }

            }

            return selection;

        }



        /**

         * Builds the string for display in the spinner.

         * @return comma-separated list of selected items

         */

        private string BuildSelectedItemString()
        {

            StringBuilder sb = new StringBuilder();

            bool foundOne = false;



            for (int i = 0; i < _items.Length; ++i)
            {

                if (_selection[i])
                {

                    if (foundOne)
                    {

                        sb.Append(", ");

                    }

                    foundOne = true;



                    sb.Append(_items[i]);

                }

            }



            return sb.ToString();

        }


    }
}