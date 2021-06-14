using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageProcessing_Tasks.Infrastructure
{
    public class RGB: INotifyPropertyChanged
    {
        private int r, g, b;
        public int R
        {
            get
            {
                return r;
            }
            set
            {
                r = value;
                OnPropertyChanged(nameof(R));
            }
        }
        public int G
        {
            get
            {
                return g;
            }
            set
            {
                g = value;
                OnPropertyChanged(nameof(G));
            }
        }
        public int B
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
                OnPropertyChanged(nameof(B));
            }
        }

        public RGB()
        {
            R = 0;
            G = 0;
            B = 0;
        }

        public RGB(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
        }

        public void AppendRGB(int r, int g, int b)
        {
            R += r;
            G += g;
            B += b;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
