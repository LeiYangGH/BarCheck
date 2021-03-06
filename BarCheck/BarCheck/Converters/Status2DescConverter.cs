﻿using BarCheck.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BarCheck.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class Status2DescConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            BarcodeStatus status = (BarcodeStatus)value;
            return Constants.dicStatusDesc[status];
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

