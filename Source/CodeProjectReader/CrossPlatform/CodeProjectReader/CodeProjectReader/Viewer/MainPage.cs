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
    /// <summary>
    /// Class: MainPage
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: The main page viewer for app
    /// Version: 0.1
    /// </summary> 
    internal class MainPage : TabbedPage
    {
        internal static Task PrepareHtmlTask;
        private bool _hasInitial;

        public MainPage()
        {
            BackgroundImage = "bg.png";
            //BackgroundImage = "CodeProjectReader.Images.bg.png";
            //BackgroundColor = Color.Red;
            
            //NavigationPage.SetHasNavigationBar(this, true);
            ItemTemplate = new DataTemplate(typeof (ArticleListPage));
            ItemsSource = App.ArticleService.ArticlePages;
            LoadCacheArticles();

            Appearing += PageAppearing;

            PrepareHtmlTask = App.HtmlService.InittalHtml();
        }

        private async void LoadCacheArticles()
        {
            var dic = await App.ArticleService.LoadCacheArticles();
            if (dic.Count == 0) return;

            foreach (ArticleViewModel pkg in ItemsSource)
            {
                if (!dic.ContainsKey(pkg.Type)) continue;
                foreach (var article in dic[pkg.Type])
                    pkg.ArticleList.Add(article);
            }
        }

        private async void PageAppearing(object sender, EventArgs e)
        {
            if (_hasInitial) return;
            if (!App.Connectivity.IsConnected)
            {
                //TODO: No internet
                return;
            }
            var mainPage = sender as TabbedPage;
            if (mainPage == null) return;
            //Set the status of ArticlePackage to buffering
            foreach (ArticleViewModel pkg in mainPage.ItemsSource) pkg.IsBuffering = true;
            //Initial all the article list for each type
            var articleDic = await App.ArticleService.InitialArticles();
            //The dic is empty
            if (articleDic == null || articleDic.All(s => s.Value == null || s.Value.Count == 0))
            {
                //TODO: has internet but failed to get article list, try again? 
                return;
            }

            foreach (ArticleViewModel pkg in mainPage.ItemsSource)
            {
                pkg.IsBuffering = false;
                if (!articleDic.ContainsKey(pkg.Type)) continue;
                foreach (var article in articleDic[pkg.Type])
                    pkg.ArticleList.Add(article);
            }
            _hasInitial = true;

        }
        
    }
}
