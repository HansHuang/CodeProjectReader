using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

    #region ItemScource (INotifyPropertyChanged Property)

    private ObservableCollection<Article> _itemScource;

    public ObservableCollection<Article> ItemScource {
      get { return _itemScource; }
      set {
        if (_itemScource != null && _itemScource.Equals(value)) return;
        _itemScource = value;
        RaisePropertyChanged("ItemScource");
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

    private Random _random = new Random();
    public MainWindow() {
      InitializeComponent();
      ItemScource = new ObservableCollection<Article>();
      LoadDailyBuild();
    }

    public void OpenBorwser(string url) {
      MessageBox.Show(url);
    }

    private void LoadDailyBuild() {
      var path = "http://www.codeproject.com/script/Mailouts/View.aspx?mlid=10964&_z=" + GetRandomStr();
      var wc = new WebClient { Encoding = Encoding.UTF8 };
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
            article.Url = link.GetAttributeValue("href", "").Trim() + "_z=" + GetRandomStr();
            article.Id = article.Url.Split(new[] {'=', '&'})[1];
            article.Title = link.InnerText.Trim();
            var author = link.NextSibling;
            if (author == null) continue;
            article.Author = author.InnerText.Replace('-', ' ').Trim();
            var desc = li.ChildNodes.FirstOrDefault(c => c.Name == "span");
            if (desc == null) continue;
            article.Description = desc.InnerText.Trim();
          }
          ItemScource.Add(article);
        }

        Console.WriteLine(ItemScource.Count);
        
        //doc.LoadHtml(doc.GetElementbyId("ctl00_MC_HtmlContent").InnerHtml);
        //var main = doc.DocumentNode.ChildNodes.FirstOrDefault(c => c.Name == "html");
        //if (main == null) return;
        //var body = main.ChildNodes.FirstOrDefault(c => c.Name == "body");
        //Console.WriteLine(body.ChildNodes.Count);



      };
      wc.DownloadStringAsync(new Uri(path));

    }

    private string GetRandomStr() {
      return _random.Next(100000, 1000000).ToString();
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

  public class MyWebClient : WebClient
  {
    Uri _responseUri;

    public Uri ResponseUri
    {
      get { return _responseUri; }
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
      var response = base.GetWebResponse(request);
      _responseUri = response.ResponseUri;
      return response;
    }
  }


}
