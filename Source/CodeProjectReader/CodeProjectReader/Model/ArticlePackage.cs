using System;
using System.Collections.Generic;
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
    }
}
