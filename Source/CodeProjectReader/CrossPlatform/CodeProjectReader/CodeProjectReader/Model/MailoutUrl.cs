using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader.Model
{
    /// <summary>
    /// Class: MailoutUrl
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 28th, 2014
    /// Description: The entity model of aritcle
    /// Version: 0.1
    /// </summary> 
    public class MailoutUrl
    {
        public ArticleType Type { get; set; }

        public Dictionary<DateTime, string> UrlDic { get; set; } 
    }
}
