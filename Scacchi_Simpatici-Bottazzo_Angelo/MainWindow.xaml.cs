using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Scacchi_Simpatici_Bottazzo_Angelo
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Scacchiera scacchiera;

        private void SliderDimensione_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsInitialized)
            {
                Scacchiera.Children.Clear();
                scacchiera = new Scacchiera(Scacchiera, (int)(sender as Slider).Value, (int)(sender as Slider).Value);
            }
        }

        private async void BtnParti_Click(object sender, RoutedEventArgs e)
        {
            GridOnnipotente.Visibility = Visibility.Visible;
            await scacchiera.RisolviCavalli();
            GridOnnipotente.Visibility = Visibility.Collapsed;
        }

        private async void BtnRegine_Click(object sender, RoutedEventArgs e)
        {
            GridOnnipotente.Visibility = Visibility.Visible;
            await scacchiera.RisolviRegine();
            GridOnnipotente.Visibility = Visibility.Collapsed;
        }

        private void Finestra_Initialized(object sender, EventArgs e)
        {
            SliderDimensione_ValueChanged(SliderDimensione, null);
        }

        private void BoxDimensione_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Return)
            {
                e.Handled = true;
                Keyboard.ClearFocus();
                BtnParti.Focus();
            }
        }
    }

    
    class CosoCheToglieDecimali : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Floor((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse((string)value, out double o))
                return o;
            return null;
        }
    }
}
