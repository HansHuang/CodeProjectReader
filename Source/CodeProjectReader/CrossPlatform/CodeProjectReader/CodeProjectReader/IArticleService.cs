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
        IConnectivity Connectivity { get; }
        ObservableCollection<ArticlePackage> ItemSource { get; }
        Task<IList<Article>> GetArticles(DateTime date, ArticleType type);
    }
}
