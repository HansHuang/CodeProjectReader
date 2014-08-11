using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeProjectReader.Helper
{
    /// <summary>
    /// Class: ElementTreeHelper
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: Auguest 11st, 2014
    /// Description: Element Tree Helper for xamarin form
    /// Version: 0.1
    /// </summary> 
    public static class ElementTreeHelper
    {
        /// <summary>
        /// Find nearest anchestor element by type
        /// </summary>
        /// <typeparam name="T"> type of anchestor element</typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static T FindAnchestor<T>(this Element current) where T : Element
        {
            try
            {
                do
                {
                    if (current is T) return (T)current;
                    current = current.Parent;
                } while (current != null);
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
