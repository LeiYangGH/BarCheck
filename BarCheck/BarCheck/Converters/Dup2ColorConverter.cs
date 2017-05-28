using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BarCheck.Converters
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class Dup2ColorConverter : IMultiValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    bool b = (bool)value;
        //    return b ? Brushes.Red : Brushes.Black;
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    return null;
        //}
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasDup = (bool)values[0];
            bool deleted = (bool)values[1];
            if (hasDup)
            {
                if (deleted)
                    return Brushes.Gray;
                else
                    return Brushes.Red;

            }
            else
                return Brushes.Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
