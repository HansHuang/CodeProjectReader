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

        #region SwipeLeft

        public delegate void SwipeLeftHandler();

        public event SwipeLeftHandler SwipeLeft;

        public void OnSwipeLeft()
        {
            if (SwipeLeft != null)
            {
                SwipeLeft();
            }
        }

        #endregion

        #region SwipeRight

        public delegate void SwipeRightHandler();

        public event SwipeRightHandler SwipeRight;

        public void OnSwipeRight()
        {
            if (SwipeRight != null)
            {
                SwipeRight();
            }
        }

        #endregion

        public LocalWebView()
        {
            SwipeLeft += () => System.Diagnostics.Debug.WriteLine("left.....");
            SwipeRight += () => System.Diagnostics.Debug.WriteLine("left.....");
        }

    }
}
