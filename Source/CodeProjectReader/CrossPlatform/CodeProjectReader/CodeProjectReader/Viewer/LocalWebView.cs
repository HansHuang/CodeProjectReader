using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeProjectReader.Viewer
{
    public class LocalWebView : ContentView
    {
        #region FileName (INotifyPropertyChanged Property)

        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set
            {
                OnPropertyChanging("FileName");
                if (_FileName != null && _FileName.Equals(value)) return;
                _FileName = value;
                OnPropertyChanged("FileName");
            }
        }

        #endregion

        public LocalWebView()
        {
            
        }

    }
}
