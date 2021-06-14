using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Equalization_and_Filters.Infrastructure
{
    public static class BitmapExtensionMethods
    {
        public static BitmapSource ConvertToBitmapSource(this Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
