using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PngifyMe.Views.Converter
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<string> stringList)
            {
                return string.Join("; ", stringList);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            }
            return new List<string>();
        }
    }

}
