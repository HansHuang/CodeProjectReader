using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeProjectReader.Helper;
using CodeProjectReader.Model;
using CodeProjectReader.Service;
using HtmlAgilityPack;
using Xamarin.Forms;

[assembly: Dependency(typeof(ArticleService))]
namespace CodeProjectReader.Service
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
        private ObservableCollection<ArticleViewModel> _articlePages;
        public ObservableCollection<ArticleViewModel> ArticlePages
        {
            get { return _articlePages ?? (_articlePages = new ObservableCollection<ArticleViewModel>()); }
            set
            {
                if (_articlePages != null && _articlePages.Equals(value)) return;
                _articlePages = value;
                RaisePropertyChanged("ArticlePages");
            }
        }
        #endregion

        public string BaseFolder { get { return "Article"; } }
        
        #endregion

        #region Non-public fields
        private const string DomainUrl = "http://www.codeproject.com";
        private const string PrefixMailUrl = DomainUrl + "/script/Mailouts/View.aspx?mlid=";
        private const string BaseAchiveUrl = DomainUrl + "/script/Mailouts/Archive.aspx?mtpid={0}";
        //string is the url of mailout in that day
        protected Dictionary<ArticleType, Dictionary<DateTime, string>> ArchiveMailDic =
            new Dictionary<ArticleType, Dictionary<DateTime, string>>();
        protected Dictionary<ArticleType, List<DateTime>> LoadedHistory = new Dictionary<ArticleType, List<DateTime>>();
        private readonly CultureInfo _websiteCulture = new CultureInfo("en-ca"); 
        #endregion

        #region Construaction
        public ArticleService()
        {
            ArticlePages = new ObservableCollection<ArticleViewModel>();
            for (var i = 1; i < 4; i++)
            {
                ArticlePages.Add(new ArticleViewModel((ArticleType)i));
                ArchiveMailDic.Add((ArticleType)i, new Dictionary<DateTime, string>());
                LoadedHistory.Add((ArticleType)i, new List<DateTime>());
            }
        }
        #endregion

        #region Methods Implementation for IArticleService

        public async Task<Dictionary<ArticleType, List<Article>>> LoadCacheArticles()
        {
            var result = new Dictionary<ArticleType, List<Article>>();
            var files = App.FileHelper.GetFiles(BaseFolder);
            if (files == null || files.Count < 1) return result;
            foreach (var file in files)
            {
                //if (!Path.GetExtension(file).Equals(".json")) continue;
                var json = await App.FileHelper.LoadString(BaseFolder + "\\" + Path.GetFileName(file));
                var dic = json.DeserializeFromWcfJson<Dictionary<ArticleType, List<Article>>>();
                if (dic == null) return result;

                foreach (var pair in dic.Where(s => s.Value != null && s.Value.Count > 0))
                {
                    var thisDate = pair.Value[0].Date;
                    var history = LoadedHistory[pair.Key];
                    if (history.Contains(thisDate)) continue;

                    if (!result.ContainsKey(pair.Key))
                        result.Add(pair.Key, pair.Value);
                    else
                    {
                        var articles = result[pair.Key].ToList();
                        var index = articles.FindLastIndex(s => s.Date > thisDate) + 1;
                        articles.InsertRange(index, pair.Value);
                        result[pair.Key] = articles;
                    }
                    history.Add(thisDate);
                }
            }
            return result;
        }


        public async Task<Dictionary<ArticleType, List<Article>>> InitialArticles()
        {
            var offset = 0;
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    offset = -1;
                    break;
                case DayOfWeek.Sunday:
                    offset = -2;
                    break;
            }
            //least avaliable day has already loaded from cache file
            var leastDate = DateTime.Now.AddDays(offset).Date;
            if (LoadedHistory.Any(s => s.Value.Contains(leastDate)))
                return null;

            //Make sure Daily Builder load first
            var dailyBuilderList = await GetArticles(ArticleType.DailyBuilder);
            var webDevList = await GetArticles(ArticleType.WebDev);
            var mobileLIst = await GetArticles(ArticleType.Mobile);

            var dic = new Dictionary<ArticleType, List<Article>>
            {
                {ArticleType.DailyBuilder, dailyBuilderList},
                {ArticleType.WebDev, webDevList},
                {ArticleType.Mobile, mobileLIst}
            };
            SaveArticlesToFile(dic);

            return dic;
        }

        #endregion

        #region Private processor

        private async Task<List<Article>> GetArticles(ArticleType type)
        {
            var list = new List<Article>();
            var urlDic = await GetMailUrl(type);
            var url = urlDic.Value;
            if (string.IsNullOrWhiteSpace(url)) return list;
            var html  = await App.WebHelper.GetHtml(url);
            
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
                var ul = category.NextSibling;
                //Read the content form li node
                var liList = ul.ChildNodes.Where(c => c.Name == "li");
                foreach (var li in liList)
                {
                    var article = new Article(urlDic.Key, category.InnerText);
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
                    list.Add(article);
                }
            }
            return list;
        }

        private async Task<KeyValuePair<DateTime, string>> GetMailUrl(ArticleType type)
        {
            var empty = new KeyValuePair<DateTime, string>();
            if (ArchiveMailDic[type].Count < 1) await LoadArchive(type);
            //if still empty, something wrong...
            if (ArchiveMailDic[type].Count < 1) return empty;
            var history = LoadedHistory[type];
            var dateUrl = ArchiveMailDic[type].FirstOrDefault(s => !history.Contains(s.Key));
            if (string.IsNullOrWhiteSpace(dateUrl.Value)) return empty;
            LoadedHistory[type].Add(dateUrl.Key);
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
                var html = await App.WebHelper.GetHtml(archiveUrl);
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
                        if (!DateTime.TryParse(txtList[0].Trim(), _websiteCulture, DateTimeStyles.AssumeLocal, out dt))
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
            dic = dic.OrderByDescending(s => s.Key).ToDictionary(s => s.Key.Date, s => s.Value);
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

        private void SaveArticlesToFile(Dictionary<ArticleType, List<Article>> articles)
        {
            Task.Run(() =>
            {
                var json = articles.SerializeToWcfJson();
                if (string.IsNullOrWhiteSpace(json)) return;
                var fileName = BaseFolder + "\\" + Guid.NewGuid() + ".json";
                App.FileHelper.SaveToFile(fileName, json);
                App.HtmlService.DownloadHtmlData(articles.Values.SelectMany(s => s).ToList());
            });
        }

        #endregion
    }
}
