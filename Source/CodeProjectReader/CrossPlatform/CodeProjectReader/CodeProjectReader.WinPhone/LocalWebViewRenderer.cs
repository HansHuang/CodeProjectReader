using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CodeProjectReader.Viewer;
using CodeProjectReader.WinPhone;
using Microsoft.Phone.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(LocalWebView), typeof(LocalWebViewRenderer))]

namespace CodeProjectReader.WinPhone
{
    public class LocalWebViewRenderer : ViewRenderer
    {
        protected override void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.HandlePropertyChanged(sender, e);
            if (e.PropertyName.Equals("Renderer"))
            {
                var view = sender as LocalWebView;
                if (view == null || string.IsNullOrWhiteSpace(view.FileName)) return;
                var browser = new WebBrowser();
                browser.Navigate(new Uri(view.FileName, UriKind.Relative));
                SetNativeControl(browser);
            }
        }


    }
}
