using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Helper;
using CodeProjectReader.Model;
using Xamarin.Forms;

namespace CodeProjectReader.Viewer
{
    /// <summary>
    /// Class: ArticleDetail
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: Augest 4th, 2014
    /// Description: page for display the detail content of article
    /// Version: 0.1
    /// Note: Issur for Xamarin.Forms.WebView:  "An unknown error has occurred. Error: 80004005." in WP
    /// </summary> 
    public class ArticleDetail : ContentPage
    {
        protected ArticleViewModel ViewModel;
        protected Article ContextArticle;

        protected LocalWebView WebView;

        public ArticleDetail(Article article, ArticleViewModel viewModel)
        {
            if (article == null) return;
            BackgroundImage = "bg.png";
            BindingContext = ContextArticle = article;
            ViewModel = viewModel;
            
            var path = App.HtmlService.IndexPage(article.Id);
            WebView = new LocalWebView { FileName = path };
            WebView.SwipeLeft += WebViewSwipeLeft;
            WebView.SwipeRight += WebViewSwipeRight;
            Disappearing += (s, e) => WebView.OnDisappearing();

            Content = WebView;
        }

        private void WebViewSwipeRight()
        {
            SwiptWebView(1);
        }

        private void WebViewSwipeLeft()
        {
            SwiptWebView(-1);
        }

        private void SwiptWebView(int offset)
        {
            if (ViewModel == null || ViewModel.ArticleList == null) return;
            var index = ViewModel.ArticleList.IndexOf(ContextArticle) + offset;
            if (index < 0 || index >= ViewModel.ArticleList.Count) return;

            ContextArticle = ViewModel.ArticleList[index];
            var path = App.HtmlService.IndexPage(ContextArticle.Id);
            WebView.FileName = path;
        }

    }
}
