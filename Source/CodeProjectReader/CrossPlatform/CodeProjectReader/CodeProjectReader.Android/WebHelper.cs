using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using CodeProjectReader.Droid;
using Xamarin.Forms;

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
    }
}
