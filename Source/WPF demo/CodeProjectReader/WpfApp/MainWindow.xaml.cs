using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;

namespace WpfApp {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
  [System.Runtime.InteropServices.ComVisibleAttribute(true)]
  public partial class MainWindow : Window,INotifyPropertyChanged {

    #region INotifyPropertyChanged RaisePropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) {
        handler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion

    #region ShowDetail (INotifyPropertyChanged Property)

    private bool _showDetail;

    public bool ShowDetail {
      get { return _showDetail; }
      set {
        if (_showDetail.Equals(value)) return;
        _showDetail = value;
        RaisePropertyChanged("ShowDetail");
      }
    }

    #endregion

    #region ArticleViewers (INotifyPropertyChanged Property)

    private ObservableCollection<ArticlePackage> _articleViewers;

    public ObservableCollection<ArticlePackage> ArticleViewers {
      get { return _articleViewers ?? (_articleViewers=new ObservableCollection<ArticlePackage>()); }
      set {
        if (_articleViewers != null && _articleViewers.Equals(value)) return;
        _articleViewers = value;
        RaisePropertyChanged("ArticleViewers");
      }
    }
    #endregion

    #region RelayCommand ReadDetailCmd

    private RelayCommand _readDetailCmd;

    public ICommand ReadDetailCmd {
      get { return _readDetailCmd ?? (_readDetailCmd = new RelayCommand(s => ReadDetailExecute(s as Article))); }
    }

    private void ReadDetailExecute(Article article) {
      if (article == null) return;
      Task.Factory.StartNew(() => {
        var request = (HttpWebRequest) WebRequest.Create(article.Url);
        request.AllowAutoRedirect = false;
        var response = request.GetResponse();
        var location = response.Headers[HttpResponseHeader.Location];
        var url = string.Format("http://www.codeproject.com{0}?display=Print", location);
        
        var html = HttpWebDealer.GetHtml(url, new WebHeaderCollection(), Encoding.UTF8);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var folder = Environment.CurrentDirectory + "\\Html\\" + article.Id;
        var imgFolder = folder + "\\Images";
        Directory.CreateDirectory(folder);

        var content = doc.GetElementbyId("contentdiv");
        //XPath Syntax: http://www.w3schools.com/xpath/xpath_syntax.asp
        //disable all the link
        var links = content.SelectNodes(".//a");
        if (links != null) {
          foreach (var link in links) {
            var u = link.GetAttributeValue("href", "");
            if (u.StartsWith("#")) continue;
            if (!u.StartsWith("http://"))
              u = "http://http://www.codeproject.com/" + u;
            link.SetAttributeValue("href", string.Format("javascript:window.external.OpenBorwser('{0}')", u));
          }
        }

        //Download all the image
        var imgs = content.SelectNodes(".//img");
        if (imgs != null) {
          int i = 0;
          foreach (var img in imgs) {
            var imgUrl = img.GetAttributeValue("src", "");
            if (imgUrl.StartsWith("/"))
              imgUrl = "http://www.codeproject.com" + imgUrl;
            Console.WriteLine(imgUrl);
            var name = string.Format("{0}.jpg", i++);
            HttpWebDealer.DownloadFile(name, imgUrl, imgFolder);
            img.SetAttributeValue("src", "Images/" + name);
            //TODO: Set the width/height of image
          }
        }


        if (string.IsNullOrEmpty(HtmlTemplate)) {
          using (var sr = new StreamReader("Html\\template.html")) {
            HtmlTemplate = sr.ReadToEnd();
          }
        }
        using (var outfile = new StreamWriter(folder + "\\index.html", false)) {
          outfile.Write(HtmlTemplate.Replace("@body@", content.OuterHtml));
        }
        Dispatcher.BeginInvoke((Action)(() =>
        {
          ShowDetail = true;
          var path = string.Format("file:///{0}/index.html", folder.Replace("\\", "/"));
          Browser.Navigate(path);
          Browser.ObjectForScripting = this;
        }));

      });
    }

    #endregion

    protected string HtmlTemplate = "";
    private const string DomainUrl = "http://www.codeproject.com";
    private const string PrefixMailUrl = DomainUrl + "/script/Mailouts/View.aspx?mlid=";
    private const string BaseAchiveUrl = DomainUrl + "/script/Mailouts/Archive.aspx?mtpid={0}";
    private readonly object _dailyBuildLocker = new object();
    private readonly object _insiderLocker = new object();

    protected Dictionary<ArticleType, Dictionary<DateTime, string>> ArchiveMailDic =
      new Dictionary<ArticleType, Dictionary<DateTime, string>>();

    protected Dictionary<ArticleType, DateTime> LoadedPointer = new Dictionary<ArticleType, DateTime>();

    private WebClient _webClient = new WebClient {Encoding = Encoding.UTF8};
    private CultureInfo _websiteCulture = new System.Globalization.CultureInfo("en-ca", true);

    public MainWindow() {
      InitializeComponent();

      //Ignore the Insider
      //TODO: Put the Insider to the last postion
      for (var i = 2; i < 5; i++) {
        ArchiveMailDic.Add((ArticleType) i, new Dictionary<DateTime, string>());
        ArticleViewers.Add(new ArticlePackage((ArticleType) i));
        LoadedPointer.Add((ArticleType) i, DateTime.Now.Date.AddYears(1));
      }

      LoadArticle(ArticleType.DailyBuilder);
      LoadArticle(ArticleType.Mobile);
      LoadArticle(ArticleType.WebDev);
    }

    public void OpenBorwser(string url) {
      MessageBox.Show(url);
    }

    private void LoadArticle(ArticleType type)
    {
      var path = GetMailUrl(type);
      if (string.IsNullOrWhiteSpace(path.Value)) return;
      var wc = new WebClient { Encoding = Encoding.UTF8 };
      var itemSource = ArticleViewers.First(s => s.Type == type).ArticleList;
      wc.DownloadStringCompleted += (s, e) =>
      {
        var doc = new HtmlDocument();
        doc.LoadHtml(e.Result);
        var h4List = doc.DocumentNode.SelectNodes("//h4");
        if (h4List.Count == 0) return;
        foreach (var category in h4List) {
          var article = new Article(category.InnerText);
          var ul = category.NextSibling;
          var liList = ul.ChildNodes.Where(c => c.Name == "li");
          //Read the content form li node
          foreach (var li in liList) {
            var link = li.ChildNodes.FirstOrDefault(c => c.Name == "a");
            if (link == null) continue;
            article.Url = link.GetAttributeValue("href", "").Trim();
            article.Id = article.Url.Split(new[] {'=', '&'})[1];
            article.Title = link.InnerText.Trim();
            var author = link.NextSibling;
            if (author == null) continue;
            article.Author = author.InnerText.Replace('-', ' ').Trim();
            var desc = li.ChildNodes.FirstOrDefault(c => c.Name == "span");
            if (desc == null) continue;
            article.Description = desc.InnerText.Trim();
          }
          itemSource.Add(article);
        }

        Console.WriteLine(itemSource.Count);
        
        //doc.LoadHtml(doc.GetElementbyId("ctl00_MC_HtmlContent").InnerHtml);
        //var main = doc.DocumentNode.ChildNodes.FirstOrDefault(c => c.Name == "html");
        //if (main == null) return;
        //var body = main.ChildNodes.FirstOrDefault(c => c.Name == "body");
        //Console.WriteLine(body.ChildNodes.Count);



      };
      wc.DownloadStringAsync(new Uri(path.Value));

    }

    private KeyValuePair<DateTime,string> GetMailUrl(ArticleType type) {
      if (ArchiveMailDic[type].Count < 1) LoadArchive(type);
      var empty = new KeyValuePair<DateTime, string>();
      //if still empty, something wrong...
      if (ArchiveMailDic[type].Count < 1) return empty;
      var dateUrl = ArchiveMailDic[type].FirstOrDefault(s => s.Key < LoadedPointer[type]);
      if (string.IsNullOrWhiteSpace(dateUrl.Value)) return empty;
      LoadedPointer[type] = dateUrl.Key;
      return dateUrl;
    }

    private void LoadArchive(ArticleType type) {
      var dic = new Dictionary<DateTime, string>();
      if (type == ArticleType.DailyBuilder || type == ArticleType.Insider) {
        var isInsider = type == ArticleType.Insider;
        lock (isInsider ? _insiderLocker : _dailyBuildLocker) {
          //Maybe already got by other therad
          if (ArchiveMailDic[type].Count > 0) return;
          var archiveUrl = string.Format(BaseAchiveUrl, isInsider ? 4 : 3);

          try {
            var html = _webClient.DownloadString(archiveUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var table = GetNodesByName(doc.DocumentNode, "table")
              .FirstOrDefault(s => s.GetAttributeValue("class", "") == "Archive");
            if (table == null) return;
            var items = GetNodesByName(table, "li");
            foreach (var item in items) {
              var txtList = item.InnerText.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
              DateTime dt;
              if (! DateTime.TryParse(txtList[0], _websiteCulture, DateTimeStyles.AssumeUniversal, out dt)) continue;
              var path = GetNodesByName(item, "a")[0].GetAttributeValue("href", "");
              if (!string.IsNullOrWhiteSpace(path))
                dic.Add(dt.Date, DomainUrl + path);
            }
          }
          catch {
          }
        }
      }
      else {
        if (ArchiveMailDic[ArticleType.DailyBuilder].Count < 1) LoadArchive(ArticleType.DailyBuilder);
        //The Mobile: Only in Thursday, DailyBuilder.id - 1
        //The WebDev: Only in Tuesday, DailyBuilder.id - 1
        var day = type == ArticleType.Mobile ? DayOfWeek.Thursday : DayOfWeek.Tuesday;
        var dayList = ArchiveMailDic[ArticleType.DailyBuilder].Where(s => s.Key.DayOfWeek == day).ToList();
        foreach (var pair in dayList) {
          try {
            var idStr = pair.Value.Replace(PrefixMailUrl, "").Split(new[] {'&'})[0];
            var id = int.Parse(idStr) - 1;
            dic.Add(pair.Key.Date, PrefixMailUrl + id);
          }
          catch {
            continue;
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

  }


  public class Article
  {
    public string Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string Category { get; set; }

    public Article() { }

    public Article(string category)
    {
      Category = category;
    }
  }

  public enum ArticleType
  {
    //Not support desc ?
    //[Description("Insider")]
    Insider = 1,
    DailyBuilder,
    WebDev,
    Mobile
  }

  public class ArticlePackage : INotifyPropertyChanged
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

    #region NotifyProperty IsBuffering
    private bool _isBuffering;
    public bool IsBuffering
    {
      get { return _isBuffering; }
      set
      {
        if (_isBuffering.Equals(value)) return;
        _isBuffering = value;
        RaisePropertyChanged("IsBuffering");
      }
    }

    #endregion

    #region NotifyProperty Name
    private string _name;
    public string Name
    {
      get { return _name; }
      set
      {
        if (_name != null && _name.Equals(value)) return;
        _name = value;
        RaisePropertyChanged("Name");
      }
    }
    #endregion

    #region NotifyProperty Type
    private ArticleType _type;
    public ArticleType Type
    {
      get { return _type; }
      set
      {
        if (_type.Equals(value)) return;
        _type = value;
        RaisePropertyChanged("Type");
      }
    }
    #endregion

    #region NotifyProperty ArticleList
    private ObservableCollection<Article> _articleList;
    public ObservableCollection<Article> ArticleList
    {
      get { return _articleList ?? (_articleList = new ObservableCollection<Article>()); }
      set
      {
        if (_articleList != null && _articleList.Equals(value)) return;
        _articleList = value;
        RaisePropertyChanged("ArticleList");
      }
    }
    #endregion

    public ArticlePackage() { }

    public ArticlePackage(ArticleType type)
    {
      Type = type;
      Name = GetName(type);
    }

    private static string GetName(ArticleType type)
    {
      switch (type)
      {
        case ArticleType.DailyBuilder:
          return "Daily Build";
        case ArticleType.Insider:
          return "Insider";
        case ArticleType.Mobile:
          return "Mobile";
        case ArticleType.WebDev:
          return "Web Dev";
      }
      return string.Empty;
    }

  }

  public class MailoutUrl
  {
    public ArticleType Type { get; set; }

    public Dictionary<DateTime, string> UrlDic { get; set; }
  }


}
