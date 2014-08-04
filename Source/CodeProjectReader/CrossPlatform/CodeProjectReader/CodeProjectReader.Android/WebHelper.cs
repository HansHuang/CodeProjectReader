using System;
using System.Net;
using System.Threading.Tasks;
using CodeProjectReader.Droid;
using Xamarin.Forms;
using Encoding = System.Text.Encoding;
using System.IO;

[assembly: Dependency(typeof(WebHelper))]
namespace CodeProjectReader.Droid
{
    internal class WebHelper:IWebHelper
    {
        public async Task<string> GetHtml(string url)
        {
            try
            {
                var wc = new WebClient { Encoding = Encoding.UTF8 };
                wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
                return await wc.DownloadStringTaskAsync(url);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<Stream> GetStream(string url)
        {

            System.Diagnostics.Debug.WriteLine(url);
            var request = (HttpWebRequest)WebRequest.Create(url);

            var tcs = new TaskCompletionSource<Stream>();
            var callback = new AsyncCallback(s =>
            {
                try
                {
                    var response = request.EndGetResponse(s);
                    tcs.TrySetResult(response.GetResponseStream());
                }
                catch (Exception e)
                {
                    tcs.TrySetResult(null);
                }
            });
            request.BeginGetResponse(callback, request);

            var stream = await tcs.Task;
            return stream;
           
        }


        public async Task<string> GetRedirectUrl(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = false;

            var tcs = new TaskCompletionSource<string>();
            var callback = new AsyncCallback(s =>
            {
                var rqs = (HttpWebRequest)s.AsyncState;
                var response = rqs.EndGetResponse(s);
                var location = response.Headers["Location"];
                tcs.TrySetResult(location);
            });
            request.BeginGetResponse(callback, request);

            var realUrl = await tcs.Task;
            return realUrl;
        }
    }
}
