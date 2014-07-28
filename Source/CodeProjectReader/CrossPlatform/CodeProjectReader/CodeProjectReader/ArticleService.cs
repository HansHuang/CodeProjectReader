using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using HtmlAgilityPack;

namespace CodeProjectReader
{
    public class ArticleService:IArticleService
    {
        #region INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public IWebHelper WebHelper { get; private set; }
        public IConnectivity Connectivity { get; private set; }

        #region NotifyProperty ArticleList
        private ObservableCollection<ArticlePackage> _itemSource;
        public ObservableCollection<ArticlePackage> ItemSource
        {
            get { return _itemSource ?? (_itemSource = new ObservableCollection<ArticlePackage>()); }
            set
            {
                if (_itemSource != null && _itemSource.Equals(value)) return;
                _itemSource = value;
                RaisePropertyChanged("ItemSource");
            }
        }
        #endregion

        private readonly Random _random = new Random();
        private const string BaseMailUrl = "http://www.codeproject.com/script/Mailouts/View.aspx?mlid={0}&_z={1}";
        protected KeyValuePair<DateTime, int> Seed = new KeyValuePair<DateTime, int>(new DateTime(2014, 7, 25), 10974);
        protected Dictionary<DateTime, KeyValuePair<ArticleType, string>> HtmlDic =
            new Dictionary<DateTime, KeyValuePair<ArticleType, string>>();

        public ArticleService(IWebHelper webHelper,IConnectivity connectivity)
        {
            WebHelper = webHelper;
            Connectivity = connectivity;
            ItemSource = new ObservableCollection<ArticlePackage>();
            for (var i = 1; i < 5; i++)
                ItemSource.Add(new ArticlePackage((ArticleType) i));
        }

        public async Task<IList<Article>> GetArticles(DateTime date, ArticleType type)
        {
            switch (type)
            {
                case ArticleType.DailyBuilder:
                    return await GetArticlesForDailyBuilder(date);
                case ArticleType.Insider:
                    return await GetArticlesForInsider(date);
                case ArticleType.Mobile:
                    return await GetArticlesForMobile(date);
                case ArticleType.WebDev:
                    return await GetArticlesForWebDev(date);
            }
            return null;
        }

        private async Task<IList<Article>> GetArticlesForDailyBuilder(DateTime date)
        {
            var url = TryGetUrl(date);
            if (string.IsNullOrWhiteSpace(url)) return null;
            var html = await WebHelper.GetHtml(url);
            if (string.IsNullOrWhiteSpace(html)) return null;
            //Bulid html string to DOM
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var mainContent = doc.GetElementById("ctl00_MC_HtmlContent");
            if (mainContent == null) return null;
            doc.LoadHtml(mainContent.InnerHtml);
            //Get usefull content
            var h4List = GetNodesByName(doc.DocumentNode, "h4");
            if (h4List.Count == 0) return null;
            var list = new List<Article>();
            foreach (var category in h4List)
            {
                var article = new Article(date, category.InnerText);
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

        private async Task<IList<Article>> GetArticlesForInsider(DateTime date)
        {
            return await Task.Run(() => new List<Article>());
        }

        private async Task<IList<Article>> GetArticlesForMobile(DateTime date)
        {
            return await Task.Run(() => new List<Article>());
        }

        private async Task<IList<Article>> GetArticlesForWebDev(DateTime date)
        {
            return await Task.Run(() => new List<Article>());
        }

        private string TryGetUrl(DateTime dateTime)
        {
            if (IsWeekend(dateTime)) return string.Empty;
            var isNewer = dateTime.Date >= Seed.Key.Date;
            var isPlus = isNewer ? 1 : -1;
            var bigger = isNewer ? dateTime : Seed.Key;
            var smaller = isNewer ? Seed.Key : dateTime;
            var dis = 0;
            while (bigger.Date >= ((smaller = smaller.AddDays(1)).Date))
            {
                if (IsWeekend(smaller)) continue;
                dis++;
            }
            var id = Seed.Value + (dis*isPlus);
            return string.Format(BaseMailUrl, id, GetRandomStr());
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private string GetRandomStr()
        {
            return _random.Next(100000, 1000000).ToString();
        }

        private static List<HtmlNode> GetNodesByName(HtmlNode node, string name)
        {
            var nodeList = new List<HtmlNode>();
            if (node.Name == name) nodeList.Add(node);
            foreach (var nd in node.ChildNodes)
                nodeList.AddRange(GetNodesByName(nd, name));
            return nodeList;
        }




        
    }
}
