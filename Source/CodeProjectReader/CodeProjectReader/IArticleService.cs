using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;

namespace CodeProjectReader
{
    public interface IArticleService
    {

        IWebHelper WebHelper { get; }
        Task<IEnumerable<Article>> GetArticles(DateTime dateTime, ArticleType type);
    }
}
