using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeProjectReader.Viewer;
using Xamarin.Forms;

namespace CodeProjectReader
{
    public class App
    {
        public static Page GetMainPage()
        {
            var page = new MainPage();
            return page;
        }
    }
}
