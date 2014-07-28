using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using HtmlAgilityPack;

namespace CodeProjectReader
{
    /// <summary>
    /// Class: ArticleService
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: The implementaton for IArticleService
    /// Version: 0.1
    /// </summary> 
    public class ArticleService:IArticleService
    {
        #region Properties implementation for IArticleService

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

        #region NotifyProperty ArticleList
        private ObservableCollection<ArticlePackage> _articlePages;
        public ObservableCollection<ArticlePackage> ArticlePages
        {
            get { return _articlePages ?? (_articlePages = new ObservableCollection<ArticlePackage>()); }
            set
            {
                if (_articlePages != null && _articlePages.Equals(value)) return;
                _articlePages = value;
                RaisePropertyChanged("ArticlePages");
            }
        }
        #endregion

        public IWebHelper WebHelper { get; private set; }
        public IConnectivity Connectivity { get; private set; } 
        #endregion

        #region Non-public fields
        private readonly Random _random = new Random();
        private const string DomainUrl = "http://www.codeproject.com";
        private const string PrefixMailUrl = DomainUrl + "/script/Mailouts/View.aspx?mlid=";
        private const string BaseAchiveUrl = DomainUrl + "/script/Mailouts/Archive.aspx?mtpid={0}";
        protected Dictionary<ArticleType, Dictionary<DateTime, string>> ArchiveMailDic =
            new Dictionary<ArticleType, Dictionary<DateTime, string>>();
        protected Dictionary<ArticleType, DateTime> LoadedPointer = new Dictionary<ArticleType, DateTime>();
        private readonly CultureInfo _websiteCulture = new CultureInfo("en-ca"); 
        #endregion

        #region Construaction
        public ArticleService(IWebHelper webHelper, IConnectivity connectivity)
        {
            WebHelper = webHelper;
            Connectivity = connectivity;
            ArticlePages = new ObservableCollection<ArticlePackage>();
            for (var i = 1; i < 4; i++)
            {
                ArticlePages.Add(new ArticlePackage((ArticleType)i));
                ArchiveMailDic.Add((ArticleType)i, new Dictionary<DateTime, string>());
                LoadedPointer.Add((ArticleType)i, DateTime.Now.Date.AddYears(1));
            }
        }
        #endregion

        #region Methods Implementation for IArticleService

        public async Task<Dictionary<ArticleType, IList<Article>>> InitialArticles()
        {
            //Make sure Daily Builder load first
            var dailyBuilderList = await GetArticles(ArticleType.DailyBuilder);
            var webDevList = await GetArticles(ArticleType.WebDev);
            var mobileLIst = await GetArticles(ArticleType.Mobile);

            return new Dictionary<ArticleType, IList<Article>>
            {
                {ArticleType.DailyBuilder, dailyBuilderList},
                {ArticleType.WebDev, webDevList},
                {ArticleType.Mobile, mobileLIst}
            };
        }

        #endregion

        #region Private processor

        private async Task<IList<Article>> GetArticles(ArticleType type)
        {
            var list = new List<Article>();
            var urlDic = await GetMailUrl(type);
            if (string.IsNullOrWhiteSpace(urlDic.Value)) return list;
            var html = await WebHelper.GetHtml(urlDic.Value);
            if (string.IsNullOrWhiteSpace(html)) return list;
            //Bulid html string to DOM
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var mainContent = doc.GetElementById("ctl00_MC_HtmlContent");
            if (mainContent == null) return list;
            doc.LoadHtml(mainContent.InnerHtml);
            //Get usefull content
            var h4List = GetNodesByName(doc.DocumentNode, "h4");
            if (h4List.Count == 0) return list;
            foreach (var category in h4List)
            {
                var article = new Article(urlDic.Key, category.InnerText);
                var ul = category.NextSibling;
                var liList = ul.ChildNodes.Where(c => c.Name == "li");
                //Read the content form li node
                foreach (var li in liList)
                {
                    var link = li.ChildNodes.FirstOrDefault(c => c.Name == "a");
                    if (link == null) continue;
                    article.Url = link.GetAttributeValue("href", "").Trim();
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

        private async Task<KeyValuePair<DateTime, string>> GetMailUrl(ArticleType type)
        {
            var empty = new KeyValuePair<DateTime, string>();
            if (ArchiveMailDic[type].Count < 1) await LoadArchive(type);
            //if still empty, something wrong...
            if (ArchiveMailDic[type].Count < 1) return empty;
            var dateUrl = ArchiveMailDic[type].FirstOrDefault(s => s.Key < LoadedPointer[type]);
            if (string.IsNullOrWhiteSpace(dateUrl.Value)) return empty;
            LoadedPointer[type] = dateUrl.Key;
            return dateUrl;
        }

        private async Task LoadArchive(ArticleType type)
        {
            var dic = new Dictionary<DateTime, string>();
            if (type == ArticleType.DailyBuilder)
            {
                //Maybe already got by other therad
                if (ArchiveMailDic[type].Count > 0) return;
                var archiveUrl = string.Format(BaseAchiveUrl, 3);
                var html = await WebHelper.GetHtml(archiveUrl);
                if (string.IsNullOrWhiteSpace(html)) return;
                try
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var table = GetNodesByName(doc.DocumentNode, "table")
                        .FirstOrDefault(s => s.GetAttributeValue("class", "") == "Archive");
                    if (table == null) return;
                    var items = GetNodesByName(table, "li");
                    foreach (var item in items)
                    {
                        var txtList = item.InnerText.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        DateTime dt;
                        if (!DateTime.TryParse(txtList[0], _websiteCulture, DateTimeStyles.AssumeUniversal, out dt))
                            continue;
                        var path = GetNodesByName(item, "a")[0].GetAttributeValue("href", "");
                        if (!string.IsNullOrWhiteSpace(path))
                            dic.Add(dt.Date, DomainUrl + path);
                    }
                }
                catch
                {
                }
            }
            else
            {
                if (ArchiveMailDic[ArticleType.DailyBuilder].Count < 1) await LoadArchive(ArticleType.DailyBuilder);
                //The Mobile: Only in Thursday, DailyBuilder.id - 1
                //The WebDev: Only in Tuesday, DailyBuilder.id - 1
                var day = type == ArticleType.Mobile ? DayOfWeek.Thursday : DayOfWeek.Tuesday;
                var dayList = ArchiveMailDic[ArticleType.DailyBuilder].Where(s => s.Key.DayOfWeek == day).ToList();
                foreach (var pair in dayList)
                {
                    try
                    {
                        var idStr = pair.Value.Replace(PrefixMailUrl, "").Split(new[] { '&' })[0];
                        var id = int.Parse(idStr) - 1;
                        dic.Add(pair.Key.Date, PrefixMailUrl + id);
                    }
                    catch
                    {
                    }
                }
            }
            ArchiveMailDic[type] = dic;
        }

        private static List<HtmlNode> GetNodesByName(HtmlNode node, string name)
        {
            var nodeList = new List<HtmlNode>();
            if (node.Name == name) nodeList.Add(node);
            foreach (var nd in node.ChildNodes)
                nodeList.AddRange(GetNodesByName(nd, name));
            return nodeList;
        }

        #endregion
    }
}
