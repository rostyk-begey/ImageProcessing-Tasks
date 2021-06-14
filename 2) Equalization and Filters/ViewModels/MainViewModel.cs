using Equalization_and_Filters.Infrastructure;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Threading.Tasks;

namespace Equalization_and_Filters.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Bitmap input;
        private Bitmap redHistogram;
        private Bitmap greenHistogram;
        private Bitmap blueHistogram;
        private Bitmap equalized;
        private Bitmap roberts;
        private Bitmap previt;
        private Bitmap sobel;

        private BitmapImage inputImage;
        private BitmapSource redHistogramImage;
        private BitmapSource greenHistogramImage;
        private BitmapSource blueHistogramImage;
        private BitmapSource eqRedHistogramImage;
        private BitmapSource eqGreenHistogramImage;
        private BitmapSource eqBlueHistogramImage;
        private BitmapSource equalizedImage;
        private string imageLocation;

        public BitmapImage InputImage
        {
            get
            {
                return inputImage;
            }
            set
            {
                inputImage = value;
                OnPropertyChanged(nameof(InputImage));
            }
        }

        public BitmapSource RedHistogram
        {
            get { return redHistogramImage; }
            set
            {
                redHistogramImage = value;
                OnPropertyChanged(nameof(RedHistogram));
            }
        }
        public BitmapSource GreenHistogram
        {
            get { return greenHistogramImage; }
            set
            {
                greenHistogramImage = value;
                OnPropertyChanged(nameof(GreenHistogram));
            }
        }
        public BitmapSource BlueHistogram
        {
            get { return blueHistogramImage; }
            set
            {
                blueHistogramImage = value;
                OnPropertyChanged(nameof(BlueHistogram));
            }
        }

        public BitmapSource EqRedHistogram
        {
            get { return eqRedHistogramImage; }
            set
            {
                eqRedHistogramImage = value;
                OnPropertyChanged(nameof(EqRedHistogram));
            }
        }
        public BitmapSource EqGreenHistogram
        {
            get { return eqGreenHistogramImage; }
            set
            {
                eqGreenHistogramImage = value;
                OnPropertyChanged(nameof(EqGreenHistogram));
            }
        }
        public BitmapSource EqBlueHistogram
        {
            get { return eqBlueHistogramImage; }
            set
            {
                eqBlueHistogramImage = value;
                OnPropertyChanged(nameof(EqBlueHistogram));
            }
        }
        public BitmapSource EqualizedImage
        {
            get
            {
                return equalizedImage;
            }
            set
            {
                equalizedImage = value;
                OnPropertyChanged(nameof(EqualizedImage));
            }
        }
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

        public ICommand ChooseImageCommand { get; private set; }
        public ICommand EqualizationCommand { get; private set; }
        public ICommand SaveEqualizedCommand { get; private set; }
        public ICommand RobertsCommand { get; private set; }
        public ICommand PrevitCommand { get; private set; }
        public ICommand SobelCommand { get; private set; }
        
        public MainViewModel()
        {
            ChooseImageCommand = new Command(ChooseImage);
            EqualizationCommand = new Command(Equalization);
            SaveEqualizedCommand = new Command(SaveEqualized);
            RobertsCommand = new Command(Roberts);
            PrevitCommand = new Command(Previt);
            SobelCommand = new Command(Sobel);
        }

        private void ChooseImage(object parametr)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Картинки|*.jpg;*.jpeg;*.png;*.bmp;*.tiff";
            if (ofd.ShowDialog() ?? true)
            {
                input = (Bitmap)Image.FromFile(ofd.FileName);
                InputImage = new BitmapImage(new Uri(ofd.FileName));
                ImageLocation = ofd.FileName;
                redHistogram = CalculateBarChart(input, 0);
                greenHistogram = CalculateBarChart(input, 1);
                blueHistogram = CalculateBarChart(input, 2);
                RedHistogram = redHistogram.ConvertToBitmapSource();
                GreenHistogram = greenHistogram.ConvertToBitmapSource();
                BlueHistogram = blueHistogram.ConvertToBitmapSource();
            }
        }

        private async void Equalization(object parametr)
        {
            await Task.Run(() =>
            {
                Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
                equalized = Filters.HistEq(copy);
            });
            EqualizedImage = equalized.ConvertToBitmapSource();
            EqRedHistogram = CalculateBarChart(equalized, 0).ConvertToBitmapSource();
            EqGreenHistogram = CalculateBarChart(equalized, 1).ConvertToBitmapSource();
            EqBlueHistogram = CalculateBarChart(equalized, 2).ConvertToBitmapSource();
        }

        private async void Roberts(object parametr)
        {
            await Task.Run(() =>
            {
                Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
                roberts = Filters.Roberts(copy);
            });
            SaveImage(roberts);
        }


        private async void Previt(object parametr)
        {
            await Task.Run(() =>
            {
                Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
                previt = Filters.Previt(copy);
            });
            SaveImage(previt);
        }

        private async void Sobel(object parametr)
        {
            await Task.Run(() =>
            {
                Bitmap copy = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
                sobel = Filters.Sobel(copy);
            });
            SaveImage(sobel);
        }

        private void SaveEqualized(object parametr)
        {
            SaveImage(equalized);
        }


        private void SaveImage(Bitmap image)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Картинки|*.jpg;*.jpeg;*.png;*.bmp;*.tiff";
            if (sfd.ShowDialog() ?? true)
            {
                image.Save(sfd.FileName);
            }
        }

        public Bitmap CalculateBarChart(Bitmap bmp, int c)
        {
            Bitmap barChart = null;
            if (bmp != null)
            {
                int width = 256, height = 600;
                barChart = new Bitmap(width, height);
                int[] R = new int[256];
                int[] G = new int[256];
                int[] B = new int[256];
                int i, j;
                Color color;
                for (i = 0; i < bmp.Width; ++i)
                for (j = 0; j < bmp.Height; ++j)
                {
                    color = bmp.GetPixel(i, j);
                    if(c == 0)
                        ++R[color.R];
                    else if(c == 1)
                        ++G[color.G];
                    else
                        ++B[color.B];
                }
                int max = 0;
                for (i = 0; i < 256; ++i)
                {
                    if (c == 0 && R[i] > max)
                        max = R[i];
                    if (c == 1 && G[i] > max)
                        max = G[i];
                    if (c == 2 && B[i] > max)
                        max = B[i];
                }
                double point = (double)max / height;
                for (i = 0; i < width - 3; ++i)
                {
                    double y;
                    if (c == 0)
                    {
                        y = R[i / 3];
                        color = Color.Red;
                    }
                    else if (c == 1)
                    {
                        y = G[i / 3];
                        color = Color.Green;
                    }
                    else
                    {
                        y = B[i / 3];
                        color = Color.Blue;
                    }
                    y /= point;
                    for (j = height - 1; j > height - y; --j)
                    {
                        barChart.SetPixel(i, j, color);
                    }
                }
            }
            else
                barChart = new Bitmap(1, 1);
            return barChart;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
