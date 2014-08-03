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
        public static readonly IArticleService ArticleService = DependencyService.Get<IArticleService>();
        public static readonly IHtmlService HtmlService = DependencyService.Get<IHtmlService>();
        public static readonly IConnectivity Connectivity = DependencyService.Get<IConnectivity>();
        public static readonly IFileHelper FileHelper = DependencyService.Get<IFileHelper>();
        public static readonly IWebHelper WebHelper = DependencyService.Get<IWebHelper>();

        public static Page GetMainPage()
        {
            var page = new MainPage();
            return page;
        }
    }
}
