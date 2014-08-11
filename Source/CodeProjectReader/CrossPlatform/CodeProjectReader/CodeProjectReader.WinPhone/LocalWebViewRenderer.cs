using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeProjectReader.Viewer;
using CodeProjectReader.WinPhone;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Input.Touch;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(LocalWebView), typeof(LocalWebViewRenderer))]
namespace CodeProjectReader.WinPhone
{
    public class LocalWebViewRenderer : ViewRenderer
    {
        protected LocalWebView Host;
        protected WebBrowser Browser;

        protected override void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.HandlePropertyChanged(sender, e);
            if (e.PropertyName.Equals("Renderer"))
            {
                Host = sender as LocalWebView;
                if (Host == null || string.IsNullOrWhiteSpace(Host.FileName)) return;
                Host.Disappearing += HostDisappearing;
                //Only opaque, solid color backgrounds are supported for the WebBrowser background
                //var bg = new ImageBrush { ImageSource = new BitmapImage(new Uri("/bg.png", UriKind.Relative)) };
                var bg = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 226, 198, 159));
                Browser = new WebBrowser {Background = bg};
                //Swipe event
                TouchPanel.EnabledGestures = GestureType.Flick | GestureType.HorizontalDrag;
                Touch.FrameReported += TouchFrameReported;

                //Broswer local web page
                Browser.Navigate(new Uri(Host.FileName, UriKind.Relative));
                SetNativeControl(Browser);
            }
            else if (e.PropertyName.Equals("FileName"))
            {
                if (Browser == null || Host==null || string.IsNullOrWhiteSpace(Host.FileName)) return;
                Browser.Navigate(new Uri(Host.FileName, UriKind.Relative));
            }
        }

        private TouchPoint _firstPoint;
        private void TouchFrameReported(object sender, TouchFrameEventArgs e)
        {
            if (Host == null || Browser == null) return;
            var mainTouch = e.GetPrimaryTouchPoint(Browser);

            if (mainTouch.Action == TouchAction.Down) _firstPoint = mainTouch;
            else if (mainTouch.Action == TouchAction.Up && TouchPanel.IsGestureAvailable)
            {
                var deltaX = mainTouch.Position.X - _firstPoint.Position.X;
                var deltaY = mainTouch.Position.Y - _firstPoint.Position.Y;
                if (Math.Abs(deltaX) <= 2 * Math.Abs(deltaY)) return;
                if (deltaX < 0) Host.OnSwipeRight();
                if (deltaX > 0) Host.OnSwipeLeft();
            }
        }

        private void HostDisappearing()
        {
            Touch.FrameReported -= TouchFrameReported;
        }

    }
}
