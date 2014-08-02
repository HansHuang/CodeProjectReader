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
        protected IArticleService ArticleService;

        private bool _hasInitial;

        public MainPage()
        {
            BackgroundImage = "bg.png";
            //BackgroundImage = "CodeProjectReader.Images.bg.png";
            //BackgroundColor = Color.Red;
            //var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            //foreach (var res in assembly.GetManifestResourceNames())
            //    System.Diagnostics.Debug.WriteLine("found resource: " + res);
            NavigationPage.SetHasNavigationBar(this, true);
            ItemTemplate = new DataTemplate(typeof (ArticleListPage));
            ArticleService = DependencyService.Get<IArticleService>();
            ItemsSource = ArticleService.ArticlePages;
            Appearing += PageAppearing;
        }

        private async void PageAppearing(object sender, EventArgs e)
        {
            if (_hasInitial) return;
            var mainPage = sender as TabbedPage;
            if (mainPage == null) return;
            //Set the status of ArticlePackage to buffering
            foreach (ArticleViewModel pkg in mainPage.ItemsSource) pkg.IsBuffering = true;
            //Initial all the article list for each type
            var articleDic = await ArticleService.InitialArticles();
            //The dic is empty
            if (articleDic == null || articleDic.All(s => s.Value == null || s.Value.Count == 0))
            {
                if (!ArticleService.Connectivity.IsConnected)
                {
                    //TODO: No internet
                    return;
                }
                else
                {
                    //TODO: has internet, try again? 
                    return;
                }
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
