using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CodeProjectReader.Helper
{
    /// <summary>
    /// Class: NumberToBoolConverter
    /// Author: Hans Huang @ Jungo Studio
    /// Create On: August 6th, 2014
    /// Description: lesst or equal than 0:false
    /// Version: 0.1
    /// </summary> 
    public class NumberToBoolConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value is int) && (int) value >= 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
