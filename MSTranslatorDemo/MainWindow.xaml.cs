using System;
using System.Windows;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Xsl;
using System.Xml;
using Saxon.Api;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        // Global exception handler to display error message and exit
        private static void HandleExceptions(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show("Caught " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            System.Windows.Application.Current.Shutdown();
        }
        // MainWindow constructor
        public MainWindow()
        {
            // Display a message if unexpected error is encountered
          //  AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleExceptions);

            if (COGNITIVE_SERVICES_KEY.Length != 32)
            {
                MessageBox.Show("One or more invalid API subscription keys.\n\n" +
                    "Put your keys in the *_API_SUBSCRIPTION_KEY variables in MainWindow.xaml.cs.",
                    "Invalid Subscription Key(s)", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                // Start GUI
                InitializeComponent();
                // Get languages for drop-downs
                GetLanguagesForTranslate();
                // Populate drop-downs with values from GetLanguagesForTranslate
                PopulateLanguageMenus();
            }
        }
        // NOTE:
        // In the following sections, we'll add code below this.
        // ***** GET TRANSLATABLE LANGUAGE CODES
        private void GetLanguagesForTranslate()
        {
            // Send request to get supported language codes
            string uri = String.Format(TEXT_TRANSLATION_API_ENDPOINT, "languages") + "&scope=translation";
            WebRequest WebRequest = WebRequest.Create(uri);
            WebRequest.Headers.Add("Accept-Language", "en");
            WebResponse response = null;
            // Read and parse the JSON response
            response = WebRequest.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream(), UnicodeEncoding.UTF8))
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(reader.ReadToEnd());
                var languages = result["translation"];

                languageCodes = languages.Keys.ToArray();
                foreach (var kv in languages)
                {
                    languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
                }
            }
        }
        // NOTE:
        // In the following sections, we'll add code below this.
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
        // NOTE:
        // In the following sections, we'll add code below this.
        // ***** DETECT LANGUAGE OF TEXT TO BE TRANSLATED
        private string DetectLanguage(string text)
        {
            string detectUri = string.Format(TEXT_TRANSLATION_API_ENDPOINT, "detect");

            // Create request to Detect languages with Translator
            HttpWebRequest detectLanguageWebRequest = (HttpWebRequest)WebRequest.Create(detectUri);
            detectLanguageWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
            detectLanguageWebRequest.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");
            detectLanguageWebRequest.ContentType = "application/json; charset=utf-8";
            detectLanguageWebRequest.Method = "POST";

            // Send request
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonText = serializer.Serialize(text);

            string body = "[{ \"Text\": " + jsonText + " }]";
            byte[] data = Encoding.UTF8.GetBytes(body);

            detectLanguageWebRequest.ContentLength = data.Length;

            using (var requestStream = detectLanguageWebRequest.GetRequestStream())
                requestStream.Write(data, 0, data.Length);

            HttpWebResponse response = (HttpWebResponse)detectLanguageWebRequest.GetResponse();

            // Read and parse JSON response
            var responseStream = response.GetResponseStream();
            var jsonString = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")).ReadToEnd();
            dynamic jsonResponse = serializer.DeserializeObject(jsonString);

            // Fish out the detected language code
            var languageInfo = jsonResponse[0];
            if (languageInfo["score"] > (decimal)0.5)
            {
                DetectedLanguageLabel.Content = languageInfo["language"];
                return languageInfo["language"];
            }
            else
                return "Unable to confidently detect input language.";
        }
        // NOTE:
        // In the following sections, we'll add code below this.
        // ***** CORRECT SPELLING OF TEXT TO BE TRANSLATED
        private string CorrectSpelling(string text)
        {
            string uri = BING_SPELL_CHECK_API_ENDPOINT + "?mode=spell&mkt=en-US";

            // Create a request to Bing Spell Check API
            HttpWebRequest spellCheckWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            spellCheckWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
            spellCheckWebRequest.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");
            spellCheckWebRequest.Method = "POST";
            spellCheckWebRequest.ContentType = "application/x-www-form-urlencoded"; // doesn't work without this

            // Create and send the request
            string body = "text=" + System.Web.HttpUtility.UrlEncode(text);
            byte[] data = Encoding.UTF8.GetBytes(body);
            spellCheckWebRequest.ContentLength = data.Length;
            using (var requestStream = spellCheckWebRequest.GetRequestStream())
                requestStream.Write(data, 0, data.Length);
            HttpWebResponse response = (HttpWebResponse)spellCheckWebRequest.GetResponse();

            // Read and parse the JSON response; get spelling corrections
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var responseStream = response.GetResponseStream();
            var jsonString = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")).ReadToEnd();
            dynamic jsonResponse = serializer.DeserializeObject(jsonString);
            var flaggedTokens = jsonResponse["flaggedTokens"];

            // Construct sorted dictionary of corrections in reverse order (right to left)
            // This ensures that changes don't impact later indexes
            var corrections = new SortedDictionary<int, string[]>(Comparer<int>.Create((a, b) => b.CompareTo(a)));
            for (int i = 0; i < flaggedTokens.Length; i++)
            {
                var correction = flaggedTokens[i];
                var suggestion = correction["suggestions"][0];  // Consider only first suggestion
                if (suggestion["score"] > (decimal)0.7)         // Take it only if highly confident
                    corrections[(int)correction["offset"]] = new string[]   // dict key   = offset
                        { correction["token"], suggestion["suggestion"] };  // dict value = {error, correction}
            }

            // Apply spelling corrections, in order, from right to left
            foreach (int i in corrections.Keys)
            {
                var oldtext = corrections[i][0];
                var newtext = corrections[i][1];

                // Apply capitalization from original text to correction - all caps or initial caps
                if (text.Substring(i, oldtext.Length).All(char.IsUpper)) newtext = newtext.ToUpper();
                else if (char.IsUpper(text[i])) newtext = newtext[0].ToString().ToUpper() + newtext.Substring(1);

                text = text.Substring(0, i) + newtext + text.Substring(i + oldtext.Length);
            }
            return text;
        }
        // NOTE:
        // In the following sections, we'll add code below this.
        // ***** PERFORM TRANSLATION ON BUTTON CLICK
        private async void TranslateButton_Click(object sender, EventArgs e)
        {
            //string textToTranslate = TextToTranslate.Text.Trim();

            

            //await translate(textToTranslate);

            // Update the translation field
           // TranslatedTextLabel.Content = translation;
        }
        string translation;
        private async System.Threading.Tasks.Task translate(string textToTranslate)
        {
            string fromLanguage = FromLanguageComboBox.SelectedValue.ToString();
            string fromLanguageCode;

            // auto-detect source language if requested
            if (fromLanguage == "Detect")
            {
                fromLanguageCode = DetectLanguage(textToTranslate);
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

            //// spell-check the source text if the source language is English
            //if (fromLanguageCode == "en")
            //{
            //    if (textToTranslate.StartsWith("-"))    // don't spell check in this case
            //        textToTranslate = textToTranslate.Substring(1);
            //    else
            //    {
            //        textToTranslate = CorrectSpelling(textToTranslate);
            //        TextToTranslate.Text = textToTranslate;     // put corrected text into input field
            //    }
            //}
            // handle null operations: no text or same source/target languages
            if (textToTranslate == "" || fromLanguageCode == toLanguageCode)
            {
                translation = textToTranslate;
                return;
            }

            // send HTTP request to perform the translation
            string endpoint = string.Format(TEXT_TRANSLATION_API_ENDPOINT, "translate");
            string uri = string.Format(endpoint + "&from={0}&to={1}", fromLanguageCode, toLanguageCode);

            System.Object[] body = new System.Object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", COGNITIVE_SERVICES_KEY);
                request.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");
                request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);
                translation = result[0]["translations"][0]["text"];

                
            }
        }
        private async Task<string> GetXmlTranslated4Names(string xmlFile, string language)
        {
            string toLanguageCode = languageCodesAndTitles[language];



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
                        await translate(NameOriginalText);
                        NameTranslatedText = translation;

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
            string toLanguageCode = languageCodesAndTitles[language];



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
                                    await translate(CountryOriginalText);
                                    CountryTranslatedText = translation;
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
            string toLanguageCode = languageCodesAndTitles[language];
            
            string NoteStartSearchTxt = "<cbc:Note>", NoteEndSearchTxt = "</cbc:Note>";
    
            int NoteStartPtr, NoteEndPtr;
            string NoteOriginalText="", NoteTranslatedText;
           


            NoteStartPtr = xmlFile.IndexOf(NoteStartSearchTxt);
            if (NoteStartPtr != -1)
            {
                NoteEndPtr = xmlFile.IndexOf(NoteEndSearchTxt, NoteStartPtr);
                if(NoteEndPtr!=-1)
                {
                    NoteOriginalText = xmlFile.Substring(NoteStartPtr + NoteStartSearchTxt.Length, NoteEndPtr - NoteStartPtr - NoteStartSearchTxt.Length);
                    await translate(NoteOriginalText);
                    NoteTranslatedText = translation;

                    xmlFile = xmlFile.Replace(NoteStartSearchTxt + NoteOriginalText + NoteEndSearchTxt, NoteStartSearchTxt + NoteTranslatedText + NoteEndSearchTxt);
                }
            }

            return xmlFile;

        }
        private async Task<string> GetXsltTranslated4Labels(string xsltFile, string language)
        {
            string toLanguageCode = languageCodesAndTitles[language];
            //<xsl:param name="language" select="'en'"/>
            //string xsltFile = File.ReadAllText(xsltFilename);
            string Labels2Translate = File.ReadAllText("Labels2Translate.txt");
            await translate(Labels2Translate);

            string[] Translatedlines = translation.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );

            string[] originalLines = Labels2Translate.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            int ot,st, et, intLabelStart, intCodeStart;
            string string2replace, newstring;
            //<t id="en">Invoice<
            //<t id="no"> = start of replace
            //</t> = end of replace
            intLabelStart = xsltFile.IndexOf("<xsl:variable name=\"labels\">");
            intCodeStart = xsltFile.IndexOf("<cl id=\"uncl1001invoice\">");
            for (int i = 0; i < Translatedlines.Length; i++)
            {
                
                ot = xsltFile.IndexOf("<t id=\"en\">"+ originalLines[i] +"</t>", intLabelStart);
                if (ot==-1)
                    ot = xsltFile.IndexOf("<t id=\"en\">" + originalLines[i] + "</t>", intCodeStart);
                        if (ot == -1)
                            ot = xsltFile.IndexOf("<t id=\"en\">" + originalLines[i] + "</t>", 0);
                if (ot == -1)
                { }
                else
                {
                    st = xsltFile.IndexOf("<t id=\"no\">", ot);
                    et = xsltFile.IndexOf("</t>", st);
                    string2replace = xsltFile.Substring(st, et - st + 4);
                    newstring = "<t id=\"" + toLanguageCode + "\">" + Translatedlines[i] + "</t>";
                    xsltFile = xsltFile.Replace(string2replace, newstring);
                }

                 
            }

            xsltFile = xsltFile.Replace("<xsl:param name=\"language\" select=\"'en'\"/>", "<xsl:param name=\"language\" select=\"'"+ toLanguageCode + "'\"/>");
           
            return xsltFile;
            

        }
       

        private async void btnLoadXML_Click(object sender, RoutedEventArgs e)
        {
            if (btnLoadXML.IsEnabled == false)
            {
                MessageBox.Show("no xml file loaded");
                return;
            }

            string OriginalxmlFile = File.ReadAllText(openFileDialog.FileName);//("cleaning services.xml");
            string OriginalxsltFile = File.ReadAllText("stylesheet-ubl v2.xslt");

            string result = await GetXsltTranslated4Labels(OriginalxsltFile,ToLanguageComboBox.Text);
            result = await GetXsltTranslated4CountryID(OriginalxmlFile, result, ToLanguageComboBox.Text);
            File.WriteAllText(ToLanguageComboBox.Text + "-stylesheet-ubl.xslt", result);
            //  string HTMLstring = XLSThelper.TransformXMLToHTML(File.ReadAllText("cleaning services.xml"), File.ReadAllText("stylesheet-ubl.xslt"));
            
            
            string TranslatedXmlNote = await GetXmlTranslated4Note(OriginalxmlFile, ToLanguageComboBox.Text);
            string TranslatedXmlNames = await GetXmlTranslated4Names(TranslatedXmlNote, ToLanguageComboBox.Text);

            File.WriteAllText(ToLanguageComboBox.Text + "-cleaning services.xml", TranslatedXmlNames);

            string HTMLstring = XSLThelper.SaxonTransform(ToLanguageComboBox.Text + "-stylesheet-ubl.xslt", ToLanguageComboBox.Text + "-cleaning services.xml");


            //string Labels2Translate = File.ReadAllText("Labels2Translate.txt");


            //await translate(Labels2Translate);
            
            //string[] Translatedlines = translation.Split(
            //    new[] { Environment.NewLine },
            //    StringSplitOptions.None
            //    );
            //string[] originalLines = Labels2Translate.Split(
            //    new[] { Environment.NewLine },
            //    StringSplitOptions.None
            //    );
            //                               
            HTMLstring = HTMLstring.Replace("<div class=\"col-md-5\" />", "");
            //<div class="col-sm-4" />
            HTMLstring = HTMLstring.Replace("<div class=\"col-sm-4\" />", "");

            //for (int i = 0; i < Translatedlines.Length; i++)
            //{
            //    if(Translatedlines[i] != "")
            //    HTMLstring = HTMLstring.Replace(">" + originalLines[i] + "<", ">" + Translatedlines[i] + "<");
                
            //}

            
            File.WriteAllText("eInvoice.html", HTMLstring);
            System.Diagnostics.Process.Start("eInvoice.html");
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
            }
            else
                XML_File_txtbx.Text = "no file selected";
        }
    }
}
