using CarStore.Models;
using System.Windows;

namespace CarClient.Views
{
    /// <summary>
    /// Interaction logic for CarWindow.xaml
    /// </summary>
    public partial class CarWindow : Window
    {
        public Car Car { get; set; }
        public CarWindow(Car car)
        {
            InitializeComponent();
            Car = car;
            DataContext = Car;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
