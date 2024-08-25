using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace PngTuberSharp.Views.Converter
{
    public class FloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (float.TryParse(value as string, NumberStyles.Float, culture, out float result))
            {
                return result;
            }
            return Avalonia.Data.BindingOperations.DoNothing;
        }
    }
}
