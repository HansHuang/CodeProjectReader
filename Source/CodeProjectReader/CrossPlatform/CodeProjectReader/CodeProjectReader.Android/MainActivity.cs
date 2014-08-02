using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

[assembly:Dependency(typeof(AndroidActivity))]
namespace CodeProjectReader.Droid
{
    [Activity(Label = "CodeProjectReader", MainLauncher = true)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            SetPage(App.GetMainPage());
        }
    }
}

