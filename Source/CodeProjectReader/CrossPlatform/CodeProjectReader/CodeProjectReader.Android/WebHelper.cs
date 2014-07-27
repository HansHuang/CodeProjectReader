using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader.Droid
{
    internal class WebHelper:IWebHelper
    {
        public async Task<string> GetHtml(string url)
        {
            //var hc = new HttpClient {Encoding = Encoding.UTF8};
            //hc.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0";
            //return await hc.GetStringAsync(url);
            return await Task.Run(() => string.Empty);
        }
    }
}
