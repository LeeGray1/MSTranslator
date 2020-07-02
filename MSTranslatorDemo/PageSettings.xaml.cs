using LanguageService;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        const string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=mstranslation;AccountKey=DhlfSrT66vg/I5CwpD0WrpeviWp5jrv/eyPaSTt7Pe8I0rv1PJnD3j8I7gGyc8oP0Jxs1+OpaL0U8Ku7kjFlFQ==;EndpointSuffix=core.windows.net";
        const string containerName = "xsltstorage";
        const string baseUri = "https://localhost:44330/";
        HttpClient httpClient = new HttpClient();
        public PageSettings()
        {
            InitializeComponent();
            // Get languages for drop-downs
            List<string> list = new LanguageClass(blobConnectionString, containerName).GetTranslatedXsltLanguages();

            cmbLanguage.ItemsSource = list;

            
            List<string> originalList= new LanguageClass(blobConnectionString, containerName).GetWords();
            foreach (var word in originalList)
            {
                if (word != "")
                    cmbWord.Items.Add(word);
            };
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("are you sure you want to delete this language file", "Delete language", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                string uri = baseUri;
                string route = "/api/fileapi/deletexslt";
                string parameter = "/?language=" + cmbLanguage.SelectedItem.ToString();
                cmbLanguage.ItemsSource = await new WebAPIHandler(httpClient).DeleteFromWebAPI<List<string>>(uri, route, parameter);
                //cmbLanguage.ItemsSource = new LanguageClass(blobConnectionString, containerName).DeleteXsltFile(cmbLanguage.SelectedItem.ToString());
            }
        }
        private async void cmbWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string uri = baseUri;
            string route = "/api/gettranslation";
            ToTranslate toTranslate = new ToTranslate();
            toTranslate.ToLanguage = (string)cmbLanguage.SelectedItem;
            toTranslate.TextToTranslate = (string)cmbWord.SelectedItem;
            TxtTranslation.Text = await new WebAPIHandler(httpClient).PostWebAPIToTranslate<ToTranslate>(uri, route, toTranslate);

            //txtTranslation.Text = new LanguageClass(blobConnectionString, containerName).GetTranslation((string)cmbWord.SelectedItem, (string)cmbLanguage.SelectedItem);
            if (String.IsNullOrEmpty(TxtTranslation.Text))
            {
                btnSave.IsEnabled = false;
            }
            else
                btnSave.IsEnabled = true;
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            string selectedWord = (string)cmbWord.SelectedItem;
            string uri = baseUri;
            string route = "api/updateTranslation";
            UpdateTranslation updateTranslation = new UpdateTranslation();
            updateTranslation.SelectedWord = selectedWord;
            updateTranslation.Language = (string)cmbLanguage.SelectedItem;
            updateTranslation.TranslatedWord = TxtTranslation.Text;
            //Task<string> result = new LanguageClass(blobConnectionString, containerName).UpdateTranslation(selectedWord, (string)cmbLanguage.SelectedItem, txtTranslation.Text);
            string result = await new WebAPIHandler(httpClient).PostWebAPI2TranslateText<UpdateTranslation>(uri, route, updateTranslation);
            if (result == null)
            {
                MessageBox.Show("Missing word to translate", "Not saved", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = cmbLanguage.Text + "-stylesheet-ubl.xslt";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xslt Stylesheet (.xslt)|*.xslt";
            if (dlg.ShowDialog() == true)
            {
                string uri = baseUri;
                string route = "api/fileapi/xsltfile/";
                string parameter = "?language=French";
                string xsltFile = await new WebAPIHandler(httpClient).CallGetXsltFromWebAPI(uri, route, parameter);
                //res.Wait();
                //string xsltfile = new LanguageClass(blobConnectionString, containerName).GetXslt4Language(ToLanguageComboBox.Text);
                File.WriteAllText(dlg.FileName, xsltFile);
            }
        }
        private void cmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnSave.IsEnabled = false;
            TxtTranslation.Text = "";
            btnDownload.IsEnabled = true;
            btnDelete.IsEnabled = true;
        }
    }
}
