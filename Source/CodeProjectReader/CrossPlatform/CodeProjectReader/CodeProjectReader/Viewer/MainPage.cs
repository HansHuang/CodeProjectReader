using System;
using System.Collections.Generic;
using System.Linq;
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
        protected IArticleService ArticleService;

        public MainPage(IArticleService articleService)
        {
            ArticleService = articleService;
            BackgroundImage = "bg.png";
            //BackgroundColor = Color.Red;
            NavigationPage.SetHasNavigationBar(this, true);
            ItemTemplate = new DataTemplate(typeof (ArticleListPage));
            ItemsSource = ArticleService.ArticlePages;
            Appearing += PageAppearing;
        }

        private async void PageAppearing(object sender, EventArgs e)
        {
            if (!ArticleService.Connectivity.IsConnected)
            {
                //TODO: No internet
                return;
            }
            var mainPage = sender as TabbedPage;
            if (mainPage == null) return;
            //Set the status of ArticlePackage to buffering
            foreach (ArticlePackage pkg in mainPage.ItemsSource) pkg.IsBuffering = true;
            //Initial all the article list for each type
            var articleDic = await ArticleService.InitialArticles();

            foreach (ArticlePackage pkg in mainPage.ItemsSource)
            {
                pkg.IsBuffering = false;
                if (!articleDic.ContainsKey(pkg.Type)) continue;
                foreach (var article in articleDic[pkg.Type])
                    pkg.ArticleList.Add(article);
            }

        }


    }
}
