using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CountdownTool.Converter
{
    class UtcToLocalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                return dateTime.ToLocalTime();
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                return dateTime.ToUniversalTime();
            }

            return Binding.DoNothing;
        }
    }
}
