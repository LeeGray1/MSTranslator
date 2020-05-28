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
        const string COGNITIVE_SERVICES_KEY = "7963c13bba1e4e60b4d872662401c746";
        // Endpoints for Translator and Bing Spell Check
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";

        private string[] languageCodes;

        private SortedDictionary<string, string> languageCodesAndTitles =
           new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));

        public PageTest()
        {
            InitializeComponent();
            // Get languages for drop-downs
            GetLanguagesForTranslate();
            // Populate drop-downs with values from GetLanguagesForTranslate
            PopulateLanguageMenus();
        }


        private void PopulateLanguageMenus()
        {
            // Add option to automatically detect the source language
            FromLanguageComboBox.Items.Add("Detect");

            int count = languageCodesAndTitles.Count;
            foreach (string menuItem in languageCodesAndTitles.Keys)
            {
                FromLanguageComboBox.Items.Add(menuItem);
                ToLanguageComboBox.Items.Add(menuItem);
            }

            // Set default languages
            FromLanguageComboBox.SelectedItem = "Detect";
            ToLanguageComboBox.SelectedItem = "English";
        }

        private void GetLanguagesForTranslate()
        {
            // Send request to get supported language codes


            var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
            }

        }
        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            string fromLanguage = FromLanguageComboBox.SelectedValue.ToString();
            string fromLanguageCode;

            if (fromLanguage == "Detect")
            {
                fromLanguageCode = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).DetectLanguage(TextToTranslate.Text);

                if (!languageCodes.Contains(fromLanguageCode))
                {
                    MessageBox.Show("The source language could not be detected automatically " +
                        "or is not supported for translation.", "Language detection failed",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
                fromLanguageCode = languageCodesAndTitles[fromLanguage];
            string toLanguageCode = languageCodesAndTitles[ToLanguageComboBox.SelectedValue.ToString()];

            string textToTranslate = TextToTranslate.Text.Trim();
            string translation = await new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).go(textToTranslate, fromLanguageCode, toLanguageCode);
            TranslatedTextLabel.Content = translation;




        }
    }
}
