using System;
using System.Globalization;
using System.Windows.Data;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Converters
{
    internal class RuleSetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RuleSet)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Preset)value;
        }
    }
}
