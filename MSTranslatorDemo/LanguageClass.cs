using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSTranslatorDemo
{
    public class LanguageClass
    {
        const string COGNITIVE_SERVICES_KEY = "7963c13bba1e4e60b4d872662401c746";
        // Endpoints for Translator and Bing Spell Check
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";

        private string[] languageCodes;

        public List<string> GetTranslatedXsltLanguages()
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
                    
                    LanguageList.Add(f.Name.Substring(0, st));
                }


            }
            return LanguageList;

        }

        SortedDictionary<string, string> languageCodesAndTitles =
           new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));
        public SortedDictionary<string, string> GetLanguageCodes()
        {
            // Send request to get supported language codes

            

            var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
            }
            return languageCodesAndTitles;
        }

        public void GetLanguages()
        {
            // Send request to get supported language codes


            var languages = new MSTranslate(TEXT_TRANSLATION_API_ENDPOINT, COGNITIVE_SERVICES_KEY).GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
            }
        }

        public List<string> FillLanguages()
        {
            GetLanguages();
            List<string> list = new List<string>();
            int count = languageCodesAndTitles.Count;

            foreach (string menuItem in  languageCodesAndTitles.Keys)
            {
                list.Add(menuItem);
                
            }

            return list;
        }

        public List<string> DeleteXsltFile(string Language)
        {
            List<string> list = null;
           // if (MessageBox.Show("are you sure you want to delete this language file", "Delete language", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                string FileName2Delete = Language + "-stylesheet-ubl.xslt";
                string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                string File2Delete = System.IO.Path.Combine(localFolder, FileName2Delete);
                File.Delete(File2Delete);
                //cmbLanguage.Items.Clear();
                //cmbLanguage.ItemsSource = null;

            }
            list = GetTranslatedXsltLanguages();
            return list;
        }

        public string GetTranslation(string SelectedWord, string SelectedLanguage)
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
        public string GetLanguageCode(string Language)
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

        public string UpdateTranslation(string SelectedWord, string Language, string NewTranslation)
        {
            string LanguageCode = GetLanguageCode(Language);
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
                //btnSave.IsEnabled = false;
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

        public List<string> GetWords()
        {
            string Labels2Translate = File.ReadAllText("Labels2Translate.txt");
            string[] originalLines = Labels2Translate.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            Array.Sort(originalLines);
            List<string> list = new List<string>();
            foreach (var item in originalLines)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
