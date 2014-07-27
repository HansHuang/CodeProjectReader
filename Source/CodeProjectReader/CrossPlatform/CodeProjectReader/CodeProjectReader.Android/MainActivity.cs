using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;

namespace CodeProjectReader.Droid
{
    [Activity(Label = "CodeProjectReader", MainLauncher = true)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);
            var svc = new ArticleService(new WebHelper());
            SetPage(App.GetMainPage(svc));
        }
    }
}

