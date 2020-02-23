using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Lang;

namespace CustomRenderer.Droid
{
    class BackButtonAttemptCallback : Java.Lang.Object, IValueCallback
    {

        /// <summary>
        /// Here we are trying to get a callback from a WiseJ WebMethod, but its not working. result is always null.
        /// https://wisej.com/support/question/forcing-form-toolbutton-click-with-javascript#sabai-entity-content-9216
        /// </summary>
        /// <param name="result"></param>
        public void OnReceiveValue(Java.Lang.Object result)
    {
        if (result.ToString() == "False") {

                JavaSystem.Exit(0);
        }
    }
}

}