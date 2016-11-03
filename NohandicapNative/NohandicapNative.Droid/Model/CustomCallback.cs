using System;
using Square.Picasso;


namespace NohandicapNative.Droid.Model
{
    public class CustomCallback : Java.Lang.Object, ICallback
    {
        Action _action = null;

        public CustomCallback(Action action)
        {
            this._action = action;
        }    

        public void OnError()
        {
            
        }

        public void OnSuccess()
        {
            _action?.Invoke();
        }
    }
}