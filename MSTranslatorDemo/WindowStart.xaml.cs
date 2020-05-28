using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for WindowStart.xaml
    /// </summary>
    public partial class WindowStart : Window
    {
        public WindowStart()
        {
            InitializeComponent();
            Main.Content = new PageMain();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new PageSettings();
            btnDashboard.IsEnabled = true;
            btnDashboard.Background = Brushes.White;//  "#FFBAB8B8";
            btnSettings.IsEnabled = false;
            btnSettings.Background = Brushes.Transparent;
            btnTest.IsEnabled = true;
            btnTest.Background = Brushes.White;
            
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new PageMain();
            btnDashboard.IsEnabled = false;
            btnDashboard.Background = Brushes.Transparent;
            btnSettings.IsEnabled = true;
            btnSettings.Background = Brushes.White;
            btnTest.IsEnabled = true;
            btnTest.Background = Brushes.White;
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new PageTest();
            btnDashboard.IsEnabled = true;
            btnDashboard.Background = Brushes.White;
            btnSettings.IsEnabled = true;
            btnSettings.Background = Brushes.White;
            btnTest.IsEnabled = false;
            btnTest.Background = Brushes.Transparent;
        }
    }
}
