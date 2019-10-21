using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Asv.TextConverter
{
    public class VisibilityByResourceStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null || (value as string).IsNullOrWhiteSpace())
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}