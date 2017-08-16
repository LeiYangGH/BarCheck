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
    public class Bool2ColorConverter : IValueConverter
    {
        private static readonly Brush g = (SolidColorBrush)(new BrushConverter().ConvertFrom("#308080"));
        private static readonly Brush r = (SolidColorBrush)(new BrushConverter().ConvertFrom("#803080"));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = (bool)value;
#if MFE
            return b ? g : r;
#else
            return b ? Brushes.Green : Brushes.Red;
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

    }
}
