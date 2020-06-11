using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for PageSettings.xaml
    /// </summary>
    public partial class PageSettings : Page
    {
        public PageSettings()
        {
            InitializeComponent();
            // Get languages for drop-downs
            List<string> list = new LanguageClass().GetTranslatedXsltLanguages();

            cmbLanguage.ItemsSource = list;

            
            List<string> originalList= new LanguageClass().GetWords();
            foreach (var word in originalList)
            {
                if (word != "")
                    cmbWord.Items.Add(word);
            };
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("are you sure you want to delete this language file", "Delete language", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                cmbLanguage.ItemsSource = new LanguageClass().DeleteXsltFile(cmbLanguage.SelectedItem.ToString());
            }
        }
        private void cmbWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtTranslation.Text = new LanguageClass().GetTranslation((string)cmbWord.SelectedItem, (string)cmbLanguage.SelectedItem);
            if (txtTranslation.Text == "")
            {
                btnSave.IsEnabled = false;
            }
            else
                btnSave.IsEnabled = true;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            string selectedWord = (string)cmbWord.SelectedItem;
           string result = new LanguageClass().UpdateTranslation(selectedWord, (string)cmbLanguage.SelectedItem, txtTranslation.Text);
            
            if (result == "")
            {
                MessageBox.Show("Missing word to translate", "Not saved", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = cmbLanguage.Text + "-stylesheet-ubl.xslt";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xslt Stylesheet (.xslt)|*.xslt";
            if (dlg.ShowDialog() == true)
            {
                string xsltfile = new LanguageClass().GetXslt4Language(cmbLanguage.Text);
                File.WriteAllText(dlg.FileName, xsltfile);
            }
        }
        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSave.IsEnabled = false;
            txtTranslation.Text = "";
            btnDownload.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }
    }
}
