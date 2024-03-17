using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Zarp.GUI.Converters
{
    class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? true : false;
        }
    }
}
