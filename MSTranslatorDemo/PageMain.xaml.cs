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
//using System.Windows.Shapes;
using Saxon.Api;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Xml.Xsl;
using System.Xml;
using System.Net;
using System.IO;
using System.Net.Http;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        // This sample uses the Cognitive Services subscription key for all services. To learn more about
        // authentication options, see: https://docs.microsoft.com/azure/cognitive-services/authentication.
        const string COGNITIVE_SERVICES_KEY = "7963c13bba1e4e60b4d872662401c746";
        // Endpoints for Translator and Bing Spell Check
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";
        const string BING_SPELL_CHECK_API_ENDPOINT = "https://westus.api.cognitive.microsoft.com/bing/v7.0/spellcheck/";
        // An array of language codes
        private string[] languageCodes;

        // Dictionary to map language codes from friendly name (sorted case-insensitively on language name)
        private SortedDictionary<string, string> languageCodesAndTitles =
            new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));

        public PageMain()
        {
            InitializeComponent();
            // Get languages for drop-downs
           // GetLanguagesForTranslate();
            // Populate drop-downs with values from GetLanguagesForTranslate
            PopulateLanguageMenus();
            btnSave_XML.IsEnabled = false;
            btnSaveXSLT.IsEnabled = false;
          
            
        }

        //private void GetLanguagesForTranslate()
        //{
        //    // Send request to get supported language codes


        //    var languages = new LanguageClass().GetLanguagesForTranslate();
        //    languageCodes = languages.Keys.ToArray();
        //    foreach (var kv in languages)
        //    {
        //        languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
        //    }

        //}
       
       
        private void PopulateLanguageMenus()
        {
            // Add option to automatically detect the source language
            FromLanguageComboBox.Items.Add("Detect");
            List<string> list = new LanguageClass().FillLanguages();            

            ToLanguageComboBox.ItemsSource = list;

            // Set default languages
            
            ToLanguageComboBox.SelectedItem = "English";
        }
        // NOTE:
        // In the following sections, we'll add code below this.
        // ***** DETECT LANGUAGE OF TEXT TO BE TRANSLATED
        
        // NOTE:
        // In the following sections, we'll add code below this.
        // ***** CORRECT SPELLING OF TEXT TO BE TRANSLATED
        
        // NOTE:
        // In the following sections, we'll add code below this.
        // ***** PERFORM TRANSLATION ON BUTTON CLICK
       
        
        
        private async Task<string> GetXmlTranslated4Names(string xmlFile, string language)
        {
            string toLanguageCode = new LanguageClass().GetLanguageCode(language);



            string NameStartSearchTxt = "<cbc:Name>", NameEndSearchTxt = "</cbc:Name>";
            int NameStartPtr = 0, NameEndPtr = 0;

            string NameOriginalText, NameTranslatedText;

            int LineItemStart = xmlFile.IndexOf("<cac:Item>");
            NameStartPtr = xmlFile.IndexOf(NameStartSearchTxt, LineItemStart);

            while (NameStartPtr != -1)
            {

                if (NameStartPtr != -1)
                {
                    NameEndPtr = xmlFile.IndexOf(NameEndSearchTxt, NameStartPtr);
                    if (NameEndPtr != -1)
                    {
                        NameOriginalText = xmlFile.Substring(NameStartPtr + NameStartSearchTxt.Length, NameEndPtr - NameStartPtr - NameStartSearchTxt.Length);
                        NameTranslatedText = await new LanguageClass().translate(NameOriginalText, ToLanguageComboBox.SelectedItem.ToString(), "English");
                         

                        xmlFile = xmlFile.Replace(NameStartSearchTxt + NameOriginalText + NameEndSearchTxt, NameStartSearchTxt + NameTranslatedText + NameEndSearchTxt);
                    }
                }
                else
                {

                }
                NameStartPtr = xmlFile.IndexOf(NameStartSearchTxt, NameEndPtr);


            }

            return xmlFile;

        }

        private async Task<string> GetXsltTranslated4CountryID(string xmlFile, string xsltFile, string language)
        {
            string toLanguageCode = new LanguageClass().GetLanguageCode(language);



            string CountryXmlStartSearchTxt = "<cbc:IdentificationCode>", CountryXmlEndSearchTxt = "</cbc:IdentificationCode>";
            //string CountryXsltStartSearchTxt = "<cbc:IdentificationCode>", CountryXsltEndSearchTxt = "</cbc:IdentificationCode>";
            int CountryXmlStartPtr = 0, CountryXmlEndPtr = 0;
            int CountryXsltStartPtr = 0, CountryXsltStartPtr2, CountryXsltEndPtr = 0;

            string CountryOriginalText, CountryTranslatedText;
            string XsltCountrySearchText;

            CountryXmlStartPtr = xmlFile.IndexOf(CountryXmlStartSearchTxt);

            while (CountryXmlStartPtr != -1)
            {

                if (CountryXmlStartPtr != -1)
                {
                    CountryXmlEndPtr = xmlFile.IndexOf(CountryXmlEndSearchTxt, CountryXmlStartPtr);
                    if (CountryXmlEndPtr != -1)
                    {
                        XsltCountrySearchText = xmlFile.Substring(CountryXmlStartPtr + CountryXmlStartSearchTxt.Length, CountryXmlEndPtr - CountryXmlStartPtr - CountryXmlStartSearchTxt.Length);

                        CountryXsltStartPtr = xsltFile.IndexOf("<c id=\"" + XsltCountrySearchText + "\">");
                        if (CountryXsltStartPtr != -1)
                        {
                            CountryXsltStartPtr2 = xsltFile.IndexOf("<t id=\"en\">", CountryXsltStartPtr);
                            if (CountryXsltStartPtr2 != -1)
                            {
                                CountryXsltEndPtr = xsltFile.IndexOf("</t>", CountryXsltStartPtr2);
                                if (CountryXsltEndPtr != -1)
                                {
                                    CountryOriginalText = xsltFile.Substring(CountryXsltStartPtr2 + 11, CountryXsltEndPtr - CountryXsltStartPtr2 - 11);
                                    //now translate xslt
                                    CountryTranslatedText = await new LanguageClass().translate(CountryOriginalText, ToLanguageComboBox.SelectedItem.ToString(), "English");
                                    
                                    CountryXsltStartPtr2 = xsltFile.IndexOf("<t id=\"no\">", CountryXsltEndPtr);
                                    if (CountryXsltStartPtr2 != -1)
                                    {
                                        CountryXsltEndPtr = xsltFile.IndexOf("</t>", CountryXsltStartPtr2);
                                        if (CountryXsltEndPtr != -1)
                                        {
                                            CountryOriginalText = xsltFile.Substring(CountryXsltStartPtr2 + 11, CountryXsltEndPtr - CountryXsltStartPtr2 - 11);
                                            xsltFile = xsltFile.Replace("<t id=\"no\">" + CountryOriginalText + "</t>", "<t id=\"" + toLanguageCode + "\">" + CountryTranslatedText + "</t>");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {

                }
                CountryXmlStartPtr = xmlFile.IndexOf(CountryXmlStartSearchTxt, CountryXmlEndPtr);


            }

            return xsltFile;

        }
        private async Task<string> GetXmlTranslated4Note(string xmlFile, string language)
        {
            string toLanguageCode = new LanguageClass().GetLanguageCode(language);

            string NoteStartSearchTxt = "<cbc:Note>", NoteEndSearchTxt = "</cbc:Note>";

            int NoteStartPtr, NoteEndPtr;
            string NoteOriginalText = "", NoteTranslatedText;



            NoteStartPtr = xmlFile.IndexOf(NoteStartSearchTxt);
            if (NoteStartPtr != -1)
            {
                NoteEndPtr = xmlFile.IndexOf(NoteEndSearchTxt, NoteStartPtr);
                if (NoteEndPtr != -1)
                {
                    NoteOriginalText = xmlFile.Substring(NoteStartPtr + NoteStartSearchTxt.Length, NoteEndPtr - NoteStartPtr - NoteStartSearchTxt.Length);
                    NoteTranslatedText = await new LanguageClass().translate(NoteOriginalText, ToLanguageComboBox.SelectedItem.ToString(), "English");


                    xmlFile = xmlFile.Replace(NoteStartSearchTxt + NoteOriginalText + NoteEndSearchTxt, NoteStartSearchTxt + NoteTranslatedText + NoteEndSearchTxt);
                }
            }

            return xmlFile;

        }
        


        private async void btnLoadXML_Click(object sender, RoutedEventArgs e)
        {
            if (btnLoadXML.IsEnabled == false)
            {
                MessageBox.Show("no xml file loaded");
                return;
            }

            btnSaveXSLT.IsEnabled = false;
            btnSave_XML.IsEnabled = false;

            string OriginalxmlFile = File.ReadAllText(openFileDialog.FileName);//("cleaning services.xml");
            if(ToLanguageComboBox.Text=="English")
            {
                MessageBox.Show("Please select another language", "Translation not supported", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (File.Exists(ToLanguageComboBox.Text + "-stylesheet-ubl.xslt"))
            { }
            else
            {
                
                string OriginalxsltFile = File.ReadAllText("stylesheet-ubl v2.xslt");

                string result = await new LanguageClass().GetXsltTranslated4Labels(OriginalxsltFile, ToLanguageComboBox.Text, "English" );
                result = await GetXsltTranslated4CountryID(OriginalxmlFile, result, ToLanguageComboBox.Text);
                File.WriteAllText(ToLanguageComboBox.Text + "-stylesheet-ubl.xslt", result);
                //  string HTMLstring = XLSThelper.TransformXMLToHTML(File.ReadAllText("cleaning services.xml"), File.ReadAllText("stylesheet-ubl.xslt"));
            }

            string TranslatedXmlNote = await GetXmlTranslated4Note(OriginalxmlFile, ToLanguageComboBox.Text);
            string TranslatedXmlNames = await GetXmlTranslated4Names(TranslatedXmlNote, ToLanguageComboBox.Text);

            File.WriteAllText(ToLanguageComboBox.Text + "-" + Path.GetFileName(openFileDialog.FileName), TranslatedXmlNames);
            

            string HTMLstring = XSLThelper.SaxonTransform(ToLanguageComboBox.Text + "-stylesheet-ubl.xslt", ToLanguageComboBox.Text + "-" + Path.GetFileName(openFileDialog.FileName));


              // get rid of the xslt bugs                  
            HTMLstring = HTMLstring.Replace("<div class=\"col-md-5\" />", "");
           
            HTMLstring = HTMLstring.Replace("<div class=\"col-sm-4\" />", "");

            HTMLstring = HTMLstring.Replace("<div />", "");
            HTMLstring = HTMLstring.Replace("style=\"width: 20%;\"", "class=\"text-right\"");
            HTMLstring = HTMLstring.Replace("linesupport{background-color:#eee", "linesupport{");
            HTMLstring = HTMLstring.Replace("<div class=\"col-sm-9\">Price</div>","");

            File.WriteAllText("eInvoice.html", HTMLstring);
            System.Diagnostics.Process.Start("eInvoice.html");

            btnSaveXSLT.IsEnabled =true;
            btnSave_XML.IsEnabled = true;


            // Update the translation field
            //TranslatedTextLabel.Content = translation;

            //InvoiceDisplay invoiceDisplay = new InvoiceDisplay();

            // invoiceDisplay.wbInvoice.NavigateToString(HTMLstring);
            // invoiceDisplay.Show();

        }

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "xml eInvoice file (*.xml)|*.xml|All files (*.*)|*.*",

        };




        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                // do something with the filename
                //MessageBox.Show(string.Format("eInvoice file \"{0}\" loaded", Path.GetFileName(openFileDialog.FileName)));
                XML_File_txtbx.Text = Path.GetFileName(openFileDialog.FileName);
                btnLoadXML.IsEnabled = true;
                btnSaveXSLT.IsEnabled = false;
                btnSave_XML.IsEnabled = false;

            }
            else
                XML_File_txtbx.Text = "no file selected";
        }

       
        

        private void btnSave_XML_Click(object sender, RoutedEventArgs e)
        {
            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = ToLanguageComboBox.Text + "-" + Path.GetFileName(openFileDialog.FileName);
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML eInvoice (.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                string xmlfile = File.ReadAllText(Path.Combine(localFolder, ToLanguageComboBox.Text + "-" + Path.GetFileName(openFileDialog.FileName)));
                File.WriteAllText(dlg.FileName, xmlfile);
            }

        }

        private void btnSaveXSLT_Click(object sender, RoutedEventArgs e)
        {
            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = ToLanguageComboBox.Text + "-stylesheet-ubl.xslt";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xslt Stylesheet (.xslt)|*.xslt";
            if (dlg.ShowDialog() == true)
            {
                string xsltfile = File.ReadAllText(Path.Combine(localFolder, ToLanguageComboBox.Text + "-stylesheet-ubl.xslt"));
                File.WriteAllText(dlg.FileName, xsltfile);
            }


        }

    }
}
