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
            List<string> list = GetTranslatedLanguages();

            cmbLanguage.ItemsSource = list;

            
            //foreach (var item in list)
            //{
            //    cmbLanguage.Items.Add(item);
            //}
            
            GetWords();
           // languageCodesAndTitles = GetLanguageCodes();
           // btnSave.IsEnabled = false;


        }

        private SortedDictionary<string, string> GetLanguageCodes()
        {
            // Send request to get supported language codes

            SortedDictionary<string, string> LanguageCodesAndNames =
           new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));

        var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                LanguageCodesAndNames.Add(kv.Value["name"], kv.Key);
            }
            return LanguageCodesAndNames;
        }

        private string GetLanguageCode(string Language)
        {
            // Send request to get supported language codes

            SortedDictionary<string, string> LanguageCodesAndNames =
           new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));

            var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                LanguageCodesAndNames.Add(kv.Value["name"], kv.Key);
            }
            return LanguageCodesAndNames[Language];
        }

        private void GetWords()
        {
            string Labels2Translate = File.ReadAllText("Labels2Translate.txt");
            string[] originalLines = Labels2Translate.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            Array.Sort(originalLines);

            foreach (var word in originalLines)
            {
                if(word!="")
                cmbWord.Items.Add(word);
            }
        }

        private List<string> GetTranslatedLanguages()
        {
            List<string> LanguageList = new List<string>();

            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dirInfo = new DirectoryInfo(localFolder);
            FileInfo[] info = dirInfo.GetFiles("*.xslt");
            
            int st;
            foreach (FileInfo f in info)
            {
                st = f.Name.IndexOf("-stylesheet");
                if (st != -1)
                {
                    // cmbLanguage.Items.Add(f.Name.Substring(0, st));
                    LanguageList.Add(f.Name.Substring(0, st));
                }


            }
            return LanguageList;


            //    var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            //languageCodes = languages.Keys.ToArray();
            //foreach (var kv in languages)
            //{
            //    languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
        }

    


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
            cmbLanguage.ItemsSource = DeleteXsltFile();
        }

        private List<string> DeleteXsltFile()
        {
            List<string> list = null;
            if (MessageBox.Show("are you sure you want to delete this language file", "Delete language", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                string FileName2Delete = cmbLanguage.SelectedItem + "-stylesheet-ubl.xslt";
                string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                string File2Delete = System.IO.Path.Combine(localFolder, FileName2Delete);
                File.Delete(File2Delete);
                //cmbLanguage.Items.Clear();
                //cmbLanguage.ItemsSource = null;
               
            }
            list = GetTranslatedLanguages();
            return list;
        }

        private void cmbWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
           
          //  string SelectedWord = (string)cmbWord.SelectedItem;


            txtTranslation.Text = GetTranslation((string)cmbWord.SelectedItem, (string)cmbLanguage.SelectedItem);
            if (txtTranslation.Text == "")
            {
                btnSave.IsEnabled = false;
            }
            else
                btnSave.IsEnabled = true;

        }

        private string GetTranslation( string SelectedWord, string SelectedLanguage)
        {
            string xsltFileName = SelectedLanguage + "-stylesheet-ubl.xslt";
            string LanguageCode = GetLanguageCode(SelectedLanguage);
            string xsltFile = File.ReadAllText(xsltFileName);
            int ot, st, et, intLabelStart, intCodeStart;

            //<t id="en">Invoice<
            //<t id="no"> = start of replace
            //</t> = end of replace
            intLabelStart = xsltFile.IndexOf("<xsl:variable name=\"labels\">");
            intCodeStart = xsltFile.IndexOf("<cl id=\"uncl1001invoice\">");
            ot = xsltFile.IndexOf("<t id=\"en\">" + SelectedWord + "</t>", intLabelStart);
            if (ot == -1)
                ot = xsltFile.IndexOf("<t id=\"en\">" + SelectedWord + "</t>", intCodeStart);
            if (ot == -1)
                ot = xsltFile.IndexOf("<t id=\"en\">" + SelectedWord + "</t>", 0);
            if (ot == -1)
            {
               // btnSave.IsEnabled = false;
                return "";
            }
            else
            {
                st = xsltFile.IndexOf("<t id=\"" + LanguageCode + "\">", ot);
                et = xsltFile.IndexOf("</t>", st);
                return xsltFile.Substring(st + 11, et - st - 11);
                //btnSave.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            string SelectedWord = (string)cmbWord.SelectedItem;
            string LanguageCode = GetLanguageCode( (string)cmbLanguage.SelectedItem);

            UpdateTranslation(SelectedWord, (string)cmbLanguage.SelectedItem, txtTranslation.Text);
        }

        private string UpdateTranslation( string SelectedWord, string Language, string NewTranslation)
        {
            string LanguageCode = GetLanguageCode( Language);
            string xsltFileName = Language + "-stylesheet-ubl.xslt";
            string xsltFile = File.ReadAllText(xsltFileName);
            int ot, st, et, intLabelStart, intCodeStart;
            string string2replace, newstring;
            //<t id="en">Invoice<
            //<t id="no"> = start of replace
            //</t> = end of replace
            intLabelStart = xsltFile.IndexOf("<xsl:variable name=\"labels\">");
            intCodeStart = xsltFile.IndexOf("<cl id=\"uncl1001invoice\">");
            ot = xsltFile.IndexOf("<t id=\"en\">" + SelectedWord + "</t>", intLabelStart);
            if (ot == -1)
                ot = xsltFile.IndexOf("<t id=\"en\">" + SelectedWord + "</t>", intCodeStart);
            if (ot == -1)
                ot = xsltFile.IndexOf("<t id=\"en\">" + SelectedWord + "</t>", 0);
            if (ot == -1)
            {
                btnSave.IsEnabled = false;
                //cannot find Selected Word
                NewTranslation = "";
            }
            else
            {
                st = xsltFile.IndexOf("<t id=\"" + LanguageCode + "\">", ot);
                et = xsltFile.IndexOf("</t>", st);
                string2replace = xsltFile.Substring(st, et - st + 4);
                newstring = "<t id=\"" + LanguageCode + "\">" + NewTranslation + "</t>";
                xsltFile = xsltFile.Replace(string2replace, newstring);
                File.WriteAllText(xsltFileName, xsltFile);

            }

            return xsltFile;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = cmbLanguage.Text + "-stylesheet-ubl.xslt";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xslt Stylesheet (.xslt)|*.xslt";
            if (dlg.ShowDialog() == true)
            {

               string xsltfile = Readxsltfile(cmbLanguage.Text);
                File.WriteAllText(dlg.FileName, xsltfile);
            }
               

        }

        private string Readxsltfile(string Language)
        {
            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string FileName = Language + "-stylesheet-ubl.xslt";
            {
                string xsltfile = File.ReadAllText(System.IO.Path.Combine(localFolder, FileName));
                return xsltfile;
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
