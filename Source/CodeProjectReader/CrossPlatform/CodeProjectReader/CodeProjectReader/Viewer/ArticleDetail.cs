using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using Xamarin.Forms;

namespace CodeProjectReader.Viewer
{
    public class ArticleDetail : ContentPage
    {
        public ArticleDetail(object bindingContext)
        {
            BackgroundImage = "bg.png";
            BindingContext = bindingContext;

            //Issur for Xamarin.Forms.WebView:  An unknown error has occurred. Error: 80004005.
            //var browser = new WebView();
            //browser.SetBinding<Article>(WebView.SourceProperty, s => s.Url);
            //var path = App.FileHelper.AppFolder + @"\Html\template.html";
            //var url = "file:///" + path.Replace("\\", "/");
            //browser.Source = new UrlWebViewSource { Url = url };
            //Content = browser;
            var article = (Article) bindingContext;
            var path = App.HtmlService.IndexPage(article.Id);
            var webView = new LocalWebView {FileName = path};

            Content = webView;
        }
    }
}
