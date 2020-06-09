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
        const string COGNITIVE_SERVICES_KEY = "7963c13bba1e4e60b4d872662401c746";
        // Endpoints for Translator and Bing Spell Check
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";

        private string[] languageCodes;

        //private SortedDictionary<string, string> languageCodesAndTitles =
        //   new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));

        public PageSettings()
        {
            InitializeComponent();
            // Get languages for drop-downs
            List<string> list = new LanguageClass().GetTranslatedXsltLanguages();// GetTranslatedLanguages();

            cmbLanguage.ItemsSource = list;

            
            //foreach (var item in list)
            //{
            //    cmbLanguage.Items.Add(item);
            //}
            
            List<string> originalList= new LanguageClass().GetWords();
            foreach (var word in originalList)
            {
                if (word != "")
                    cmbWord.Items.Add(word);
            }
            // languageCodesAndTitles = GetLanguageCodes();
            // btnSave.IsEnabled = false;


        }

        

        

       

        //private List<string> GetTranslatedLanguages()
        //{
        //    List<string> LanguageList = new List<string>();

        //    string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
        //    DirectoryInfo dirInfo = new DirectoryInfo(localFolder);
        //    FileInfo[] info = dirInfo.GetFiles("*.xslt");
            
        //    int st;
        //    foreach (FileInfo f in info)
        //    {
        //        st = f.Name.IndexOf("-stylesheet");
        //        if (st != -1)
        //        {
        //            // cmbLanguage.Items.Add(f.Name.Substring(0, st));
        //            LanguageList.Add(f.Name.Substring(0, st));
        //        }


        //    }
        //    return LanguageList;


        //    //    var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
        //    //languageCodes = languages.Keys.ToArray();
        //    //foreach (var kv in languages)
        //    //{
        //    //    languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
        //}

    


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("are you sure you want to delete this language file", "Delete language", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                cmbLanguage.ItemsSource = new LanguageClass().DeleteXsltFile(cmbLanguage.SelectedItem.ToString());
            }
        }

       

        private void cmbWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
           
          //  string SelectedWord = (string)cmbWord.SelectedItem;


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
            
            string SelectedWord = (string)cmbWord.SelectedItem;
            //string LanguageCode = new LanguageClass().GetLanguageCode( (string)cmbLanguage.SelectedItem);

           string result = new LanguageClass().UpdateTranslation(SelectedWord, (string)cmbLanguage.SelectedItem, txtTranslation.Text);
            //Below code is a Messagebox to inform user that translation has been saved
            //if (MessageBox.Show("Translation saved"), MessageBoxButton.OK, MessageBoxImage.Information)
            //{
               
            //}
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

               string xsltfile = new LanguageClass().Readxsltfile(cmbLanguage.Text);
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
