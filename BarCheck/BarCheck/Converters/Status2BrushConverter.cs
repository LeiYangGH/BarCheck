using BarCheck.ViewModel;
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
    public class Status2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BarcodeStatus status = (BarcodeStatus)value;
            return Constants.dicStatusBrush[status];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
