using System;
using System.Collections.Generic;
using System.Linq;
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
            BindingContext = bindingContext;

            //var label = new Label();
            //label.SetBinding<Article>(Label.TextProperty, s => s.FullTitle);
            //var source = new UrlWebViewSource();
            //source.SetBinding<Article>(UrlWebViewSource.UrlProperty, s => s.Url);
            //System.Diagnostics.Debug.WriteLine(((Article)bindingContext).Url);
            
            //var webView = new WebView { Source = source };
            //Content = new StackLayout
            //{
            //    Children = { label, webView }
            //};

            var browser = new WebView();
            browser.SetBinding<Article>(WebView.SourceProperty, s => s.Url);

            Content = browser;
            
        }
    }
}
