using System;
using System.Globalization;
using System.Windows.Data;
using Zarp.Core.Datatypes;

namespace Zarp.GUI.Converters
{
    internal class RulePresetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RulePreset)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (IPreset)value;
        }
    }

    internal class FocusSessionPresetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (FocusSessionPreset)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (IPreset)value;
        }
    }

    internal class RewardPresetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (RewardPreset)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (IPreset)value;
        }
    }
}
