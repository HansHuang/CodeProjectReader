using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader
{
    public interface IWebHelper
    {
        Task<string> GetHtml(string url);
    }
}
