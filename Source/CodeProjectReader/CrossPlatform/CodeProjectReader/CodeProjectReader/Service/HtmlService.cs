using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;
using CodeProjectReader.Service;
using CodeProjectReader.Viewer;
using HtmlAgilityPack;
using Xamarin.Forms;

[assembly: Dependency(typeof(HtmlService))]
namespace CodeProjectReader.Service
{
    public class HtmlService:IHtmlService
    {
        protected const string BaseUrl = "http://www.codeproject.com{0}?display=Print";

        protected string HtmlTemplate = "";

        public string BaseFolder
        {
            get { return "Html"; }
        }

        public Task InittalHtml()
        {
            return Task.Run(async () =>
            {
                var tempHtmlPath = BaseFolder + "\\template.html";
                //if (App.FileHelper.HasFile(tempHtmlPath)) return;

                const string basePrefix = "CodeProjectReader.Resource.Html.";
                const string imagePrefix = basePrefix + "images.";
                const string symbolPrefix = imagePrefix + "symbols.";
                var assembly = typeof(HtmlService).GetTypeInfo().Assembly;
                foreach (var res in assembly.GetManifestResourceNames().Where(s => s.StartsWith(basePrefix)))
                {
                    string fileName;
                    if (res.StartsWith(symbolPrefix))
                        fileName = res.Replace(symbolPrefix, BaseFolder + "\\Images\\symbols\\");
                    else if (res.StartsWith(imagePrefix))
                        fileName = res.Replace(imagePrefix, BaseFolder + "\\Images\\");
                    else
                        fileName = res.Replace(basePrefix, BaseFolder + "\\");
                    using (var stream = assembly.GetManifestResourceStream(res))
                    {
                        await App.FileHelper.SaveToFile(fileName, stream);
                    }
                }
            });

        }

        public void DownloadHtmlData(List<Article> articles)
        {
            if (articles == null) return;
            var workTasks = new List<Task>();
            var que = new Queue<Article>(articles);
            //every 10 articles in one task
            while (que.Count > 0)
            {
                workTasks.Add(Task.Run(async () =>
                {
                    for (var i = 0; i < 10; i++)
                        if (que.Count > 0)
                            await DownloadArticle(que.Dequeue());
                }));
            }
            Task.WaitAll(workTasks.ToArray());
        }


        private async Task DownloadArticle(Article article)
        {
            var url = await App.WebHelper.GetRedirectUrl(article.Url);
            url = string.Format("http://www.codeproject.com{0}?display=Print", url);

            var folder = BaseFolder + "\\" + article.Id;
            var imgFolder = folder + "\\Images";
            //0. retrive the html data
            var html = await App.WebHelper.GetHtml(url);
            if (string.IsNullOrWhiteSpace(html)) return;
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var content = doc.GetElementById("contentdiv");
            //1. disable all the link
            var links = GetNodesByName(content, "a");
            if (links != null)
            {
                foreach (var link in links)
                {
                    var u = link.GetAttributeValue("href", "");
                    if (u.StartsWith("#")) continue;
                    if (!u.StartsWith("http://"))
                        u = "http://http://www.codeproject.com/" + u;
                    //link.SetAttributeValue("href", string.Format("javascript:window.external.OpenBorwser('{0}')", u));
                }
            }
            //2. Download all the image
            var imgs = GetNodesByName(content, "img");
            if (imgs != null)
            {
                var i = 0;
                foreach (var img in imgs)
                {
                    var imgUrl = img.GetAttributeValue("src", "");
                    Stream stream;
                    if (imgUrl.StartsWith("data:image;"))
                    {
                        var bytes = Convert.FromBase64String(imgUrl.Substring(18));
                        stream = new MemoryStream(bytes);
                    }
                    else
                    {
                        if (imgUrl.StartsWith("/")) imgUrl = "http://www.codeproject.com" + imgUrl;
                        if (!imgUrl.StartsWith("http")) continue;
                        stream = await App.WebHelper.GetStream(imgUrl);
                    }
                    if (stream == null) continue;
                    var name = string.Format("{0}.jpg", i++);
                    await App.FileHelper.SaveToFile(imgFolder + "\\" + name, stream);
                    img.SetAttributeValue("src", "Images/" + name);
                    //TODO: Set the width/height of image ?
                }
            }
            //3. genreate the html file
            if (string.IsNullOrEmpty(HtmlTemplate))
                HtmlTemplate = await App.FileHelper.LoadString(BaseFolder + "\\template.html");
            var newHtml = HtmlTemplate.Replace("$body$", content.OuterHtml);
            await App.FileHelper.SaveToFile(folder + "\\index.html", newHtml);
            article.IsHtmlDownloaded = true;
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
