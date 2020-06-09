using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MSTranslatorDemo
{
    public class LanguageClass
    {
        const string COGNITIVE_SERVICES_KEY = "7963c13bba1e4e60b4d872662401c746";
        // Endpoints for Translator and Bing Spell Check
        public static readonly string TEXT_TRANSLATION_API_ENDPOINT = "https://api.cognitive.microsofttranslator.com/{0}?api-version=3.0";
        const string BING_SPELL_CHECK_API_ENDPOINT = "https://westus.api.cognitive.microsoft.com/bing/v7.0/spellcheck/";

        private string[] languageCodes;

        //Gets current translated xslt files for page settings
        public List<string> GetTranslatedXsltLanguages()
        {
            List<string> LanguageList = new List<string>();

            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo dirInfo = new DirectoryInfo(localFolder);
            FileInfo[] info = dirInfo.GetFiles("*.xslt");

            int st;
            //collects a list of languages
            foreach (FileInfo f in info)
            {
                st = f.Name.IndexOf("-stylesheet");
                if (st != -1)
                {

                    LanguageList.Add(f.Name.Substring(0, st));
                }


            }
            //Returns a list of languages
            return LanguageList;

        }

        SortedDictionary<string, string> languageCodesAndTitles =
           new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));
        public SortedDictionary<string, string> GetLanguageCodes()
        {
            // Send request to get supported language codes
            var languages =  GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
            }
            return languageCodesAndTitles;
        }

        private Dictionary<string, Dictionary<string, string>> GetLanguagesForTranslate()
        {
            // Send request to get supported language codes
            string uri = String.Format(TEXT_TRANSLATION_API_ENDPOINT, "languages") + "&scope=translation";
            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Headers.Add("Accept-Language", "en");
            WebResponse response = null;
            // Read and parse the JSON response
            response = webRequest.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream(), UnicodeEncoding.UTF8))
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(reader.ReadToEnd());
                Dictionary<string, Dictionary<string, string>> languages = result["translation"];


                return languages;
                //foreach (var kv in languages)
                //{
                //    languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
                //}
            }
        }

        //Shows translatable languages
        public void GetLanguages()
        {
            // Send request to get supported language codes
            var languages = GetLanguagesForTranslate();
            languageCodes = languages.Keys.ToArray();
            foreach (var kv in languages)
            {
                languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
            }
        }

        //Get languages for combo boxes on page test
        public List<string> FillLanguages()
        {
            GetLanguages();
            List<string> list = new List<string>();
            int count = languageCodesAndTitles.Count;
            //get language codes to make up a language list
            foreach (string menuItem in languageCodesAndTitles.Keys)
            {
                list.Add(menuItem);

            }
            //returns a list of languages
            return list;
        }

        //Allows user to delete the chosen xslt file on settings page
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
            //returns a list of translated xslt files after deleting the chosen file 
            return list;
        }

        //Gets translation for chosen word in the page settings
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

            if (languageCodesAndTitles.Count == 0)
            {
                // Send request to get supported language codes

                // SortedDictionary<string, string> LanguageCodesAndNames =
                //new SortedDictionary<string, string>(Comparer<string>.Create((a, b) => string.Compare(a, b, true)));

                var languages = GetLanguagesForTranslate();
                languageCodes = languages.Keys.ToArray();
                foreach (var kv in languages)
                {
                    languageCodesAndTitles.Add(kv.Value["name"], kv.Key);
                }
            }
            return languageCodesAndTitles[Language];
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

        public string DetectLanguage(string text)
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

                return languageInfo["language"];
            }
            else
                return "Unable to confidently detect input language.";
        }

        public string CorrectSpelling(string text)
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

        public async System.Threading.Tasks.Task<string> translate(string textToTranslate, string ToLanguage, string FromLanguage)
        {
            string fromLanguage = "English"; //FromLanguageComboBox.SelectedValue.ToString();
            string fromLanguageCode;
            string translation;
            if(languageCodesAndTitles.Count==0)
            {
               // GetLanguageCode()
            }

            // auto-detect source language if requested


            fromLanguageCode = GetLanguageCode(fromLanguage);

            string toLanguageCode = GetLanguageCode(ToLanguage);


            if (textToTranslate == "" || fromLanguageCode == toLanguageCode)
            {
                translation = textToTranslate;
                return textToTranslate;
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
            return translation;
        }

       

        public async Task<string> GetXsltTranslated4Labels(string xsltFile, string Tolanguage, string FromLanguage)
        {
            string toLanguageCode = GetLanguageCode(Tolanguage);
            //<xsl:param name="language" select="'en'"/>
            //string xsltFile = File.ReadAllText(xsltFilename);
            string Labels2Translate = File.ReadAllText("Labels2Translate.txt");
            string translation = await translate(Labels2Translate, Tolanguage, FromLanguage);

            string[] Translatedlines = translation.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );

            string[] originalLines = Labels2Translate.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            int ot, st, et, intLabelStart, intCodeStart;
            string string2replace, newstring;
            //<t id="en">Invoice<
            //<t id="no"> = start of replace
            //</t> = end of replace
            intLabelStart = xsltFile.IndexOf("<xsl:variable name=\"labels\">");
            intCodeStart = xsltFile.IndexOf("<cl id=\"uncl1001invoice\">");
            for (int i = 0; i < Translatedlines.Length; i++)
            {

                ot = xsltFile.IndexOf("<t id=\"en\">" + originalLines[i] + "</t>", intLabelStart);
                if (ot == -1)
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

            xsltFile = xsltFile.Replace("<xsl:param name=\"language\" select=\"'en'\"/>", "<xsl:param name=\"language\" select=\"'" + toLanguageCode + "'\"/>");

            return xsltFile;
        }
        public string Readxsltfile(string Language)
        {
            string localFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string FileName = Language + "-stylesheet-ubl.xslt";
            {
                string xsltfile = File.ReadAllText(System.IO.Path.Combine(localFolder, FileName));
                return xsltfile;
            }
        }

       
        public string GetTranslatedXml(string ToLanguage)
        {
            string GetXmlStorageLocation = System.AppDomain.CurrentDomain.BaseDirectory;
            return File.ReadAllText(Path.Combine(GetXmlStorageLocation, ToLanguage + "-" + "TranslatedFile.xml"));
        }

        public string GetXslt4Language(string ToLanguage)
        {
            string GetxsltStorageLocation = System.AppDomain.CurrentDomain.BaseDirectory;
            return File.ReadAllText(Path.Combine(GetxsltStorageLocation, ToLanguage + "-stylesheet-ubl.xslt"));
        }

        public async Task<string> GetXmlTranslated4Note(string xmlFile, string ToLanguage, string FromLanguage)
        {
            string toLanguageCode = GetLanguageCode(ToLanguage);

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
                    NoteTranslatedText = await translate(NoteOriginalText, ToLanguage, FromLanguage);


                    xmlFile = xmlFile.Replace(NoteStartSearchTxt + NoteOriginalText + NoteEndSearchTxt, NoteStartSearchTxt + NoteTranslatedText + NoteEndSearchTxt);
                }
            }

            return xmlFile;

        }

        public async Task<string> GetXmlTranslated4Names(string xmlFile, string ToLanguage, string FromLanguage)
        {
            string toLanguageCode = new LanguageClass().GetLanguageCode(ToLanguage);



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
                        NameTranslatedText = await new LanguageClass().translate(NameOriginalText, ToLanguage, FromLanguage);


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

        public async Task<string> ConvertXml2Html(string OriginalxmlFile, string ToLanguage, string FileName)
        {
            if (File.Exists(ToLanguage + "-stylesheet-ubl.xslt"))
            { }
            else
            {

                string OriginalxsltFile = File.ReadAllText("stylesheet-ubl v2.xslt");

                string result = await GetXsltTranslated4Labels(OriginalxsltFile, ToLanguage, "English");
                result = await GetXsltTranslated4CountryID(OriginalxmlFile, result, ToLanguage);
                File.WriteAllText(ToLanguage + "-stylesheet-ubl.xslt", result);
                //  string HTMLstring = XLSThelper.TransformXMLToHTML(File.ReadAllText("cleaning services.xml"), File.ReadAllText("stylesheet-ubl.xslt"));
            }

            string TranslatedXmlNote = await GetXmlTranslated4Note(OriginalxmlFile, ToLanguage, "English");
            string TranslatedXmlNames = await GetXmlTranslated4Names(TranslatedXmlNote, ToLanguage, "English");

            File.WriteAllText(ToLanguage + "-" + "TranslatedFile.xml", TranslatedXmlNames);


            string HTMLstring = XSLThelper.SaxonTransform(ToLanguage + "-stylesheet-ubl.xslt", ToLanguage + "-" + FileName /*Path.GetFileName(openFileDialog.FileName)*/);


            // get rid of the xslt bugs                  
            HTMLstring = HTMLstring.Replace("<div class=\"col-md-5\" />", "");

            HTMLstring = HTMLstring.Replace("<div class=\"col-sm-4\" />", "");

            HTMLstring = HTMLstring.Replace("<div />", "");
            HTMLstring = HTMLstring.Replace("style=\"width: 20%;\"", "class=\"text-right\"");
            HTMLstring = HTMLstring.Replace("linesupport{background-color:#eee", "linesupport{");
            HTMLstring = HTMLstring.Replace("<div class=\"col-sm-9\">Price</div>", "");
            return HTMLstring;
        }

        private async Task<string> GetXsltTranslated4CountryID(string xmlFile, string xsltFile, string language)
        {
            string toLanguageCode = GetLanguageCode(language);



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
                                    CountryTranslatedText = await new LanguageClass().translate(CountryOriginalText, language, "English");

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
    }
}
