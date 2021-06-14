using ImageProcessing_Tasks.ViewModels;
using System.Windows;

namespace ImageProcessing_Tasks
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
