using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageProcessing_Tasks.Converters
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double time = System.Convert.ToDouble(value);
            if(time<1000)
            {
                return $"{time} мс";
            }
            return $"{(int)time / 1000} с {(int)time % 1000} мс";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
