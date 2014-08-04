using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CodeProjectReader.Droid;
using CodeProjectReader.Viewer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LocalWebView), typeof(LocalWebViewRenderer))]
namespace CodeProjectReader.Droid
{
    public class LocalWebViewRenderer : ViewRenderer
    {
        //protected override void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    base.HandlePropertyChanged(sender, e);
        //    if (e.PropertyName.Equals("Renderer"))
        //    {
        //        var view = sender as LocalWebView;
        //        if (view == null || string.IsNullOrWhiteSpace(view.FileName)) return;
        //        var browser = new WebBrowser();
        //        browser.Navigate(new Uri(view.FileName, UriKind.Relative));
        //        SetNativeControl(browser);
        //    }
        //}


        protected override void OnModelChanged(VisualElement oldModel, VisualElement newModel)
        {
            base.OnModelChanged(oldModel, newModel);
            Background = new ColorDrawable(Android.Graphics.Color.Red);

        }

    }
}