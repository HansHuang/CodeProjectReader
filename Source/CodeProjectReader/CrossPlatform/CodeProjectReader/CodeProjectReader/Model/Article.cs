using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CodeProjectReader.Model
{
    /// <summary>
    /// Class: Article
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: July 26th, 2014
    /// Description: The entity model of aritcle
    /// Version: 0.1
    /// </summary> 
    [DataContract]
    public class Article:INotifyPropertyChanged
    {
        #region INotifyPropertyChanged RaisePropertyChanged

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

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Category { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        #region IsCached (INotifyPropertyChanged Property)

        private bool _isCached;

        public bool IsCached
        {
            get { return _isCached; }
            set
            {
                if (_isCached.Equals(value)) return;
                _isCached = value;
                RaisePropertyChanged("IsCached");
            }
        }

        #endregion

        #region IsReaded (INotifyPropertyChanged Property)

        private bool _isReaded;

        public bool IsReaded
        {
            get { return _isReaded; }
            set
            {
                if (_isReaded.Equals(value)) return;
                _isReaded = value;
                RaisePropertyChanged("IsReaded");
            }
        }

        #endregion

        public string DateString
        {
            get { return Date.ToString("M"); }
        }

        public string FullTitle
        {
            get { return string.Format("{0} - {1}", Title, Author); }
        }

        public Article() { }

        public Article(DateTime date,string category)
        {
            Date = date;
            Category = category;
        }
    }
}
