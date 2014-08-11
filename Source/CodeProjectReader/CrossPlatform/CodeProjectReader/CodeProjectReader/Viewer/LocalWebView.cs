using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeProjectReader.Helper;
using Xamarin.Forms;

namespace CodeProjectReader.Viewer
{
    /// <summary>
    /// Class: LocalWebView
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: Augest 4th, 2014
    /// Description: provide a web brower to view local html file
    /// Version: 0.1
    /// Note: Issur for Xamarin.Forms.WebView:  "An unknown error has occurred. Error: 80004005." in WP
    /// </summary> 
    public class LocalWebView : ContentView
    {

        #region FileName (INotifyPropertyChanged Property)

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                OnPropertyChanging("FileName");
                if (_fileName != null && _fileName.Equals(value)) return;
                _fileName = value;
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

        #region SwipeRight

        public delegate void DisappearingHandler();

        public event DisappearingHandler Disappearing;

        public void OnDisappearing()
        {
            if (Disappearing != null)
            {
                Disappearing();
            }
        }

        #endregion

        public LocalWebView()
        {
            //SwipeLeft += () => System.Diagnostics.Debug.WriteLine("Left.....");
            //SwipeRight += () => System.Diagnostics.Debug.WriteLine("Right.....");
        }

    }
}
