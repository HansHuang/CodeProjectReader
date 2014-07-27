using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using Xamarin.Forms;

namespace CodeProjectReader.Viewer
{
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
            ItemsSource = ArticleService.ItemSource;
            foreach (var page in Children)
                page.Appearing += PageAppearing;
        }

        private async void PageAppearing(object sender, EventArgs e)
        {
            var page = sender as Page;
            if (page == null) return;
            var articlePkg = page.BindingContext as ArticlePackage;
            if (articlePkg == null) return;
            articlePkg.IsBuffering = true;
            var articleList = await ArticleService.GetArticles(DateTime.Now, articlePkg.Type);
            if (articleList == null) return;
            articlePkg.IsBuffering = false;
            foreach (var article in articleList)
            {
                articlePkg.ArticleList.Add(article);
            }
        }
    }
}
