using System.Windows;
using MarketioDesktop.ViewModels;

namespace MarketioDesktop
{
    /// <summary>
    /// Interactie logica voor MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set the DataContext naar MainViewModel
            this.DataContext = App.ServiceProvider.GetService(typeof(MainViewModel));
        }
    }
}