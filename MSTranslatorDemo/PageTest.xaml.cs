using LanguageService;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for PageTest.xaml
    /// </summary>
    public partial class PageTest : Page
    {
        const string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=mstranslation;AccountKey=DhlfSrT66vg/I5CwpD0WrpeviWp5jrv/eyPaSTt7Pe8I0rv1PJnD3j8I7gGyc8oP0Jxs1+OpaL0U8Ku7kjFlFQ==;EndpointSuffix=core.windows.net";
        const string containerName = "xsltstorage";
        const string COGNITIVE_SERVICES_KEY = "7963c13bba1e4e60b4d872662401c746";
        // Endpoints for Translator and Bing Spell Check
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";

       
        public PageTest()
        {
            InitializeComponent();
            
            // Populate drop-downs with values from GetLanguagesForTranslate
            PopulateLanguageCombos();
        }
        private void PopulateLanguageCombos()
        {
            // Add option to automatically detect the source language
            FromLanguageComboBox.Items.Add("Detect");
            List<string> list = new LanguageClass(blobConnectionString, containerName).FillLanguages();
            foreach (var item in list)
            {
                FromLanguageComboBox.Items.Add(item);
            }
            
            ToLanguageComboBox.ItemsSource = list;

            // Set default languages
            FromLanguageComboBox.SelectedItem = "Detect";
            ToLanguageComboBox.SelectedItem = "English";
        }
        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            string fromLanguage = FromLanguageComboBox.Text;
            

            if (fromLanguage == "Detect")
            {
                fromLanguage = new LanguageClass(blobConnectionString, containerName).DetectLanguage(TextToTranslate.Text);

                if (fromLanguage.Contains("Unable to confidently detect input language."))
                {
                    MessageBox.Show("The source language could not be detected automatically " +
                        "or is not supported for translation.", "Language detection failed",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            
            var toLanguageCode = new LanguageClass(blobConnectionString, containerName).GetLanguageCode(ToLanguageComboBox.SelectedValue.ToString());

            var textToTranslate = TextToTranslate.Text.Trim();
            var translation = await new LanguageClass(blobConnectionString, containerName).translate(textToTranslate, ToLanguageComboBox.Text, fromLanguage);
            TranslatedTextLabel.Text = translation;
        }
    }
}
