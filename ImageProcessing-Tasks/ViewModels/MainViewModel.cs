using ImageProcessing_Tasks.Infrastructure;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Interop;
using System.Threading.Tasks;

namespace ImageProcessing_Tasks.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string imageLocation;
        private long sizeBeforeCompressing;
        private long sizeAlfterCompressing;
        private Image BmpImage;
        private Image TiffImage;
        private Image JpegImage;
        private BitmapSource dispayImage;

        private RGB color;
        private int writingTime;
        private int readingTime;
        private double encodingTime;
        private double decodingTime;
        private int colorType;
        private int progressMaximum, progressValue;

        public string ImageLocation
        {
            get
            {
                return imageLocation;
            }
            set
            {
                imageLocation = value;
                OnPropertyChanged(nameof(ImageLocation));
            }
        }
        public long SizeBeforeCompressing
        {
            get
            {
                return sizeBeforeCompressing;
            }
            set
            {
                sizeBeforeCompressing = value;
                OnPropertyChanged(nameof(SizeBeforeCompressing));
            }
        }
        public long SizeAfterCompressing
        {
            get
            {
                return sizeAlfterCompressing;
            }
            set
            {
                sizeAlfterCompressing = value;
                OnPropertyChanged(nameof(SizeAfterCompressing));
            }
        }
        public BitmapSource DisplayImage
        {
            get
            {
                return dispayImage;
            }
            set
            {
                dispayImage = value;
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        public RGB RGBColor
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                OnPropertyChanged(nameof(RGBColor));
            }
        }
        public int ReadingTime
        {
            get
            {
                return readingTime;
            }
            set
            {
                readingTime = value;
                OnPropertyChanged(nameof(ReadingTime));
            }
        }
        public double DecodingTime
        {
            get
            {
                return decodingTime;
            }
            set
            {
                decodingTime = value;
                OnPropertyChanged(nameof(DecodingTime));
            }
        }
        public double EncodingTime
        {
            get
            {
                return encodingTime;
            }
            set
            {
                encodingTime = value;
                OnPropertyChanged(nameof(EncodingTime));
            }
        }
        public int WritingTime
        {
            get
            {
                return writingTime;
            }
            set
            {
                writingTime = value;
                OnPropertyChanged(nameof(WritingTime));
            }
        }
        public int ColorType
        {
            get
            {
                return colorType;
            }
            set
            {
                colorType = value;
                OnPropertyChanged(nameof(ColorType));
            }
        }
        public int ProgressMaximum
        {
            get
            {
                return progressMaximum;
            }
            set
            {
                progressMaximum = value;
                OnPropertyChanged(nameof(ProgressMaximum));
            }
        }
        public int ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public ICommand ChooseImageCommand { get; private set; }
        public ICommand SaveAsBMPRLECommand { get; private set; }
        public ICommand SaveAsTIFFCommand { get; private set; }
        public ICommand SaveAsJPEGCommand { get; private set; }

        public ICommand BmpDiffTiffCommand { get; private set; }
        public ICommand BmpDiffJpegCommand { get; private set; }
        public ICommand TiffDiffJpegCommand { get; private set; }

        public MainViewModel()
        {
            ChooseImageCommand = new Command(ChooseImage);

            SaveAsBMPRLECommand = new Command(SaveAsBMPRLE);
            SaveAsTIFFCommand = new Command(SaveAsTIFF);
            SaveAsJPEGCommand = new Command(SaveAsJPEG);

            BmpDiffTiffCommand = new Command(BmpDiffTiff);
            BmpDiffJpegCommand = new Command(BmpDiffJpeg);
            TiffDiffJpegCommand = new Command(TiffDiffJpeg);

            ColorType = 0;
        }

        private void ChooseImage(object parametr)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "bmp(*.bmp)|*.bmp"
            };
            if (ofd.ShowDialog() ?? true)
            {
                DisplayImage = new BitmapImage(new Uri(ofd.FileName));
                BmpImage = Image.FromFile(ofd.FileName);
                ImageLocation = ofd.FileName;
                SizeBeforeCompressing = new FileInfo(ofd.FileName).Length;
            }
        }

        private void SaveAsBMPRLE(object parametr)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "bmp(*.bmp)|*.bmp";
            if (sfd.ShowDialog() ?? true)
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/bmp"); ;
                Encoder myEncoder = Encoder.Compression;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionRle);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = myEncoderParameter;

                Stopwatch timer = new Stopwatch();
                timer.Start();
                BmpImage.Save(sfd.FileName, myImageCodecInfo, myEncoderParameters);
                timer.Stop();
                WritingTime = timer.Elapsed.Milliseconds;

                DisplayImage = new BitmapImage(new Uri(sfd.FileName));

                timer.Restart();
                BmpImage = Image.FromFile(sfd.FileName);
                timer.Stop();
                ReadingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                DisplayImage = new BitmapImage(new Uri(sfd.FileName));
                timer.Stop();
                DecodingTime = timer.Elapsed.TotalMilliseconds;

                timer.Restart();
                BitmapImage encoded = (BitmapImage)DisplayImage;
                timer.Stop();
                EncodingTime = timer.Elapsed.TotalMilliseconds;

                SizeAfterCompressing = new FileInfo(sfd.FileName).Length;
            }
        }

        private void SaveAsTIFF(object parametr)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "tif(*.tif)|*.tif"
            };
            if (sfd.ShowDialog() ?? true)
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff"); ;
                Encoder myEncoder = Encoder.Compression;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionLZW);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = myEncoderParameter;

                Stopwatch timer = new Stopwatch();
                timer.Start();
                BmpImage.Save(sfd.FileName, ImageFormat.Tiff);
                timer.Stop();
                WritingTime = timer.Elapsed.Milliseconds;

                DisplayImage = new BitmapImage(new Uri(sfd.FileName));

                timer.Restart();
                TiffImage = Image.FromFile(sfd.FileName);
                timer.Stop();
                ReadingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                DisplayImage = new BitmapImage(new Uri(sfd.FileName));
                timer.Stop();
                DecodingTime = timer.Elapsed.TotalMilliseconds;

                timer.Restart();
                BitmapImage encoded = (BitmapImage)DisplayImage;
                timer.Stop();
                EncodingTime = timer.Elapsed.TotalMilliseconds;

                SizeAfterCompressing = new FileInfo(sfd.FileName).Length;
            }
        }

        private void SaveAsJPEG(object parametr)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "jpeg(*.jpeg)|*.jpeg"
            };
            if (sfd.ShowDialog() ?? true)
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg"); ;
                Encoder myEncoder = Encoder.Compression;
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionNone);
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = myEncoderParameter;

                Stopwatch timer = new Stopwatch();
                timer.Start();
                BmpImage.Save(sfd.FileName, ImageFormat.Jpeg);
                timer.Stop();
                WritingTime = timer.Elapsed.Milliseconds;

                DisplayImage = new BitmapImage(new Uri(sfd.FileName));

                timer.Restart();
                JpegImage = Image.FromFile(sfd.FileName);
                timer.Stop();
                ReadingTime = timer.Elapsed.Milliseconds;

                timer.Restart();
                DisplayImage = new BitmapImage(new Uri(sfd.FileName));
                timer.Stop();
                DecodingTime = timer.Elapsed.TotalMilliseconds;

                timer.Restart();
                BitmapImage encoded = (BitmapImage)DisplayImage;
                timer.Stop();
                EncodingTime = timer.Elapsed.TotalMilliseconds;

                SizeAfterCompressing = new FileInfo(sfd.FileName).Length;
            }
        }

        private void BmpDiffTiff(object parametr)
        {
            ImagesDifferenceAsync(BmpImage, TiffImage);
        }

        private void BmpDiffJpeg(object parametr)
        {
            ImagesDifferenceAsync(BmpImage, JpegImage);
        } 

        private void TiffDiffJpeg(object parametr)
        {
            ImagesDifferenceAsync(TiffImage, JpegImage);
        }

        private async void ImagesDifferenceAsync(Image firstImage, Image secondImage)
        {
            int width = BmpImage.Width;
            int height = BmpImage.Height;
            Bitmap first = (Bitmap)firstImage;
            Bitmap second = (Bitmap)secondImage;
            Bitmap diff = new Bitmap(width, height);
            RGBColor = new RGB();

            ProgressValue = 0;
            ProgressMaximum = width * height;
            await Task.Run(() =>
            {
                int r = 0, g = 0, b = 0;
                int multiplier = 6;
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        r = (ColorType == 0 || ColorType == 1) ? Math.Abs(first.GetPixel(i, j).R - second.GetPixel(i, j).R) : 0;
                        g = (ColorType == 0 || ColorType == 2) ? Math.Abs(first.GetPixel(i, j).G - second.GetPixel(i, j).G) : 0;
                        b = (ColorType == 0 || ColorType == 3) ? Math.Abs(first.GetPixel(i, j).B - second.GetPixel(i, j).B) : 0;
                        RGBColor.AppendRGB(r, g, b);
                        r = r * multiplier;
                        g = g * multiplier;
                        b = b * multiplier;
                        r = (r > 255) ? 255 : r;
                        g = (g > 255) ? 255 : g;
                        b = (b > 255) ? 255 : b;
                        diff.SetPixel(i, j, Color.FromArgb(byte.MaxValue, r, g, b));
                        ProgressValue++;
                    }
                }
            });
            DisplayImage = Imaging.CreateBitmapSourceFromHBitmap(diff.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(width, height));
        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
