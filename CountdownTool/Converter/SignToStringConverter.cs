using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CountdownTool.Converter
{
    class SignToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return Binding.DoNothing;
            }

            return ConvertToString((TimeSpan)value);
        }

        private static string ConvertToString(TimeSpan value)
        {
            return value < TimeSpan.Zero ? "T- " : "T+ ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
