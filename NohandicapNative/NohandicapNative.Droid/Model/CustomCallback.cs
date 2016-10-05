using System;
using Square.Picasso;


namespace NohandicapNative.Droid.Model
{
    public class CustomCallback : Java.Lang.Object, ICallback
    {
        Action action = null;

        public CustomCallback(Action action)
        {
            this.action = action;
        }    

        public void OnError()
        {
            
        }

        public void OnSuccess()
        {
            action?.Invoke();
        }
    }
}