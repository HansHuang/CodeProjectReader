using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader
{
    /// <summary>
    /// Interface: IWebHelper
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: Requirment for web helper
    /// Version: 0.1
    /// </summary> 
    public interface IWebHelper
    {
        Task<string> GetHtml(string url);
    }
}
