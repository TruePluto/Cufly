using System;
using System.Globalization;
using System.Windows.Data;

namespace CefSharp.MinimalExample.Wpf.Converter
{
    public class TitleConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "DeArray - " + (value ?? "No Title");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
