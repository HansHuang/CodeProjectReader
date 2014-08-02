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
    /// <summary>
    /// Interface: IArticleService
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: Requirment for article service
    /// Version: 0.1
    /// </summary> 
    public interface IArticleService:INotifyPropertyChanged
    {
        IWebHelper WebHelper { get; }
        IConnectivity Connectivity { get; }
        ObservableCollection<ArticleViewModel> ArticlePages { get; }
        Task<Dictionary<ArticleType,IList<Article>>> InitialArticles();
    }
}
