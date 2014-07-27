using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Model;

namespace CodeProjectReader
{
    public interface IArticleService:INotifyPropertyChanged
    {
        IWebHelper WebHelper { get; }
        ObservableCollection<ArticlePackage> ItemSource { get; }
        Task<IEnumerable<Article>> GetArticles(DateTime dateTime, ArticleType type);
    }
}
