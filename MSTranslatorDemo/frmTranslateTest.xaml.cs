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
    /// Interaction logic for frmTranslateTest.xaml
    /// </summary>
    public partial class frmTranslateTest : Window
    {
        public frmTranslateTest()
        {
            InitializeComponent();
        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            string textToTranslate = TextToTranslate.Text.Trim();

            //new translate.

            //await translate(textToTranslate);

            //// Update the translation field
            //TranslatedTextLabel.Content = translation;

        }
    }
}
