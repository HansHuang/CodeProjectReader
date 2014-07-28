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

        private readonly Dictionary<Page, int> _failded=new Dictionary<Page, int>();

        private async void PageAppearing(object sender, EventArgs e)
        {
            var page = sender as Page;
            if (page == null) return;
            if (!ArticleService.Connectivity.IsConnected)
                //TODO: No internet
                return;
            if (!_failded.ContainsKey(page)) _failded.Add(page, 0);
            if (_failded[page] >= 5)
            {
                //TODO: Filded to get articles
                return;
            }

            var articlePkg = page.BindingContext as ArticlePackage;
            if (articlePkg == null) return;
            articlePkg.IsBuffering = true;
            var date = DateTime.Now.AddDays(-1*_failded[page]);
            var articleList = await ArticleService.GetArticles(date, articlePkg.Type);
            articlePkg.IsBuffering = false;
            if (articleList == null || articleList.Count == 0)
            {
                _failded[page]++;
                PageAppearing(sender, e);
                return;
            }
            foreach (var article in articleList)
            {
                articlePkg.ArticleList.Add(article);
            }
        }


    }
}
