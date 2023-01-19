using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BLauncher.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool val)
            {
                return val ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new InvalidOperationException($"Trying to convert {targetType} to a bool");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // pass
            return true;
        }
    }

    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
            {
                return val ? Visibility.Collapsed : Visibility.Visible;
            }
            throw new InvalidOperationException($"Trying to convert {targetType} to a bool");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // pass
            return true;
        }
    }
}
