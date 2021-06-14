using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageProcessing_Tasks.Converters
{
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double length = System.Convert.ToDouble(value);
            string[] sizes = new string[7] { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            int type = 0;
            while (length >= 1024)
            {
                length /= 1024;
                ++type;
            }
            return $"{Math.Round(length, 2)} {sizes[type]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
