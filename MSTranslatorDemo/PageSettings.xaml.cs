using Microsoft.Win32;
using System;
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

        private SortedDictionary<string, string> languageCodesAndTitles =
           new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));

        public PageSettings()
        {
            InitializeComponent();
            // Get languages for drop-downs
            GetTranslatedLanguages();
            GetWords();
            GetLanguageCodes();
            btnSave.IsEnabled = false;


        }

        private void GetLanguageCodes()
        {
            // Send request to get supported language codes


            var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
            }

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

        private void GetTranslatedLanguages()
        {
            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dirInfo = new DirectoryInfo(localFolder);
            FileInfo[] info = dirInfo.GetFiles("*.xslt");
            
            int st;
            foreach (FileInfo f in info)
            {
                st = f.Name.IndexOf("-style");
                if (st != -1)
                {
                    cmbLanguage.Items.Add(f.Name.Substring(0, st));
                }


            }


            //    var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            //languageCodes = languages.Keys.ToArray();
            //foreach (var kv in languages)
            //{
            //    languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
        }

    


        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("are you sure you want to delete this language file","Delete language",MessageBoxButton.OKCancel,MessageBoxImage.Error)==MessageBoxResult.OK)
            {
                string FileName2Delete = cmbLanguage.SelectedItem + "-stylesheet-ubl.xslt";
                string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                string File2Delete = System.IO.Path.Combine(localFolder, FileName2Delete);
                File.Delete(File2Delete);
                cmbLanguage.Items.Clear();
                GetTranslatedLanguages();


            }
        }

        private void cmbWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string xsltFileName = cmbLanguage.SelectedItem + "-stylesheet-ubl.xslt";
            string xsltFile = File.ReadAllText(xsltFileName);
            string SelectedWord = (string)cmbWord.SelectedItem;
            string LanguageCode = languageCodesAndTitles[(string)cmbLanguage.SelectedItem];

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
                btnSave.IsEnabled = false;
                txtTranslation.Text = "";
            }
            else
            {
                st = xsltFile.IndexOf("<t id=\"" + LanguageCode + "\">", ot);
                et = xsltFile.IndexOf("</t>", st);
                txtTranslation.Text = xsltFile.Substring(st+11, et - st -11);
                btnSave.IsEnabled = true;
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string xsltFileName = cmbLanguage.SelectedItem + "-stylesheet-ubl.xslt";
            string xsltFile = File.ReadAllText(xsltFileName);
            string SelectedWord = (string)cmbWord.SelectedItem;
            string LanguageCode = languageCodesAndTitles[(string)cmbLanguage.SelectedItem];

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
                txtTranslation.Text = "";
            }
            else
            {
                st = xsltFile.IndexOf("<t id=\"" + LanguageCode + "\">", ot);
                et = xsltFile.IndexOf("</t>", st);
                string2replace = xsltFile.Substring(st, et - st + 4);
                newstring = "<t id=\"" + LanguageCode + "\">" + txtTranslation.Text + "</t>";
                xsltFile = xsltFile.Replace(string2replace, newstring);
                File.WriteAllText(xsltFileName, xsltFile);

            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = cmbLanguage.Text + "-stylesheet-ubl.xslt";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xslt Stylesheet (.xslt)|*.xslt";
            if (dlg.ShowDialog() == true)
            {
                string xsltfile = File.ReadAllText(System.IO.Path.Combine(localFolder, cmbLanguage.Text + "-stylesheet-ubl.xslt"));
                File.WriteAllText(dlg.FileName, xsltfile);
            }

        }
    }
}
