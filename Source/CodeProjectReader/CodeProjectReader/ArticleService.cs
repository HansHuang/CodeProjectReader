using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using HtmlAgilityPack;

namespace CodeProjectReader
{
    public class ArticleService:IArticleService
    {
        public IWebHelper WebHelper { get; private set; }

        private readonly Random _random = new Random();

        public async Task<IEnumerable<Article>> GetArticles(DateTime dateTime, ArticleType type)
        {
            var url = await GetUrl(dateTime, type);
            var html = await WebHelper.GetHtml(url);
            if (string.IsNullOrWhiteSpace(html)) return null;
            //Bulid html string to DOM
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            //Get usefull content
            var h4List = doc.DocumentNode.SelectNodes("//h4");
            if (h4List.Count == 0) return null;
            var list = new List<Article>();
            foreach (var category in h4List)
            {
                var article = new Article(category.InnerText);
                var ul = category.NextSibling;
                var liList = ul.ChildNodes.Where(c => c.Name == "li");
                //Read the content form li node
                foreach (var li in liList)
                {
                    var link = li.ChildNodes.FirstOrDefault(c => c.Name == "a");
                    if (link == null) continue;
                    article.Url = link.GetAttributeValue("href", "").Trim() + "_z=" + GetRandomStr();
                    article.Id = article.Url.Split(new[] { '=', '&' })[1];
                    article.Title = link.InnerText.Trim();
                    var author = link.NextSibling;
                    if (author == null) continue;
                    article.Author = author.InnerText.Replace('-', ' ').Trim();
                    var desc = li.ChildNodes.FirstOrDefault(c => c.Name == "span");
                    if (desc == null) continue;
                    article.Description = desc.InnerText.Trim();
                }
                list.Add(article);
            }

            return list;
        }

        public ArticleService(IWebHelper webHelper)
        {
            WebHelper = webHelper;
        }

        private async Task<string> GetUrl(DateTime dateTime, ArticleType type)
        {
            await Task.Delay(1);
            //TODO
            return "http://www.codeproject.com/script/Mailouts/View.aspx?mlid=10964&_z=" + GetRandomStr();
        }

        private string GetRandomStr()
        {
            return _random.Next(100000, 1000000).ToString();
        }
    }
}
