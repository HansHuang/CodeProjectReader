using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Xamarin.Forms;


namespace CodeProjectReader.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            
            Forms.Init();

            Content = CodeProjectReader.App.GetMainPage().ConvertPageToUIElement(this);
            
            //Note: Hans comment codes from 643-650
            //I need costomize the color of systemtray
            ThemeManager.ToLightTheme();
            var color = new System.Windows.Media.Color() {R = 225, G = 197, B = 158, A = 255};
            SystemTray.SetBackgroundColor(this, Colors.Black);
            SystemTray.SetForegroundColor(this, color);

            //var canver = (Canvas) Content;
            //var c1 = (Pivot)canver.Children[0];
            //var page = (Xamarin.Forms.Page) c1.Items[0];
        }
    }
}
