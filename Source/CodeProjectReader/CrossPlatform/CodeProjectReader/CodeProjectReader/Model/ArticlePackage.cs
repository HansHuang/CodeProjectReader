using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader.Model
{
    public class ArticlePackage : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region NotifyProperty IsBuffering
        private bool _isBuffering;
        public bool IsBuffering
        {
            get { return _isBuffering; }
            set
            {
                if (_isBuffering.Equals(value)) return;
                _isBuffering = value;
                RaisePropertyChanged("IsBuffering");
            }
        }

        #endregion

        #region NotifyProperty Name
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != null && _name.Equals(value)) return;
                _name = value;
                RaisePropertyChanged("Name");
            }
        }
        #endregion

        #region NotifyProperty Type
        private ArticleType _type;
        public ArticleType Type
        {
            get { return _type; }
            set
            {
                if (_type.Equals(value)) return;
                _type = value;
                RaisePropertyChanged("Type");
            }
        }
        #endregion

        #region NotifyProperty ArticleList
        private ObservableCollection<Article> _articleList;
        public ObservableCollection<Article> ArticleList
        {
            get { return _articleList ?? (_articleList=new ObservableCollection<Article>()); }
            set
            {
                if (_articleList != null && _articleList.Equals(value)) return;
                _articleList = value;
                RaisePropertyChanged("ArticleList");
            }
        }
        #endregion

        public ArticlePackage() { }

        public ArticlePackage(ArticleType type)
        {
            Type = type;
            Name = GetName(type);
        }

        private static string GetName(ArticleType type)
        {
            switch (type)
            {
                case ArticleType.DailyBuilder:
                    return "Daily Build";
                case ArticleType.Insider:
                    return "Insider";
                case ArticleType.Mobile:
                    return "Mobile";
                case ArticleType.WebDev:
                    return "Web Dev";
            }
            return string.Empty;
        }

    }
}
