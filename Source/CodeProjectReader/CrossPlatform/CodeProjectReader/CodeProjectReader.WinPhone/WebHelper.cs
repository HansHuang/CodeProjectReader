using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.WinPhone;
using CodeProjectReader.WinPhone.Helper;
using Xamarin.Forms;

[assembly: Dependency(typeof(WebHelper))]
namespace CodeProjectReader.WinPhone
{
    
    public class WebHelper:IWebHelper
    {
        public async Task<string> GetHtml(string url)
        {
            try
            {
                var hc = new HttpClient { Encoding = Encoding.UTF8 };
                hc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
                return await hc.GetStringAsync(url);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Stream> GetStream(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            
            var tcs = new TaskCompletionSource<Stream>();
            var callback = new AsyncCallback(s =>
            {
                var response = request.EndGetResponse(s);
                tcs.TrySetResult(response.GetResponseStream());
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
                var rqs = (HttpWebRequest) s.AsyncState;
                var response = rqs.EndGetResponse(s);
                var location = response.Headers["Location"];
                tcs.TrySetResult(location);
            });
            request.BeginGetResponse(callback, request);

            var realUrl =await tcs.Task;
            return realUrl;
        }
    }
}
