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
    public class MSTranslate
    {
        public string translation { get; set; }
        public string TextTranslationEndpoint { get; set; }
        public string Cognitive_Services_Key { get; set; }

        public MSTranslate(string textTranslationEndpoint, string cognitive_Services_Key)
        {
            this.Cognitive_Services_Key = cognitive_Services_Key;
            this.TextTranslationEndpoint = textTranslationEndpoint;
        }
        private string[] languageCodes;
        
        public Dictionary<string, Dictionary<string, string>> GetLanguagesForTranslate()
        {
            // Send request to get supported language codes
            string uri = String.Format(TextTranslationEndpoint, "languages") + "&scope=translation";
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

        public string DetectLanguage(string text)
        {
            string detectUri = string.Format(TextTranslationEndpoint, "detect");

            // Create request to Detect languages with Translator
            HttpWebRequest detectLanguageWebRequest = (HttpWebRequest)WebRequest.Create(detectUri);
            detectLanguageWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", Cognitive_Services_Key);
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
               // DetectedLanguageLabel.Content = languageInfo["language"];
                return languageInfo["language"];
            }
            else
            {
                //Unable to confidently detect input language.
                return "";
            }
        }
        public async System.Threading.Tasks.Task<string> go(string textToTranslate, string fromLanguageCode, string toLanguageCode)
        {

            if (textToTranslate == "" || fromLanguageCode == toLanguageCode)
            {
                
                return textToTranslate;
            }

            // send HTTP request to perform the translation
            string endpoint = string.Format(TextTranslationEndpoint, "translate");
            string uri = string.Format(endpoint + "&from={0}&to={1}", fromLanguageCode, toLanguageCode);

            System.Object[] body = new System.Object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", Cognitive_Services_Key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");
                request.Headers.Add("X-ClientTraceId", Guid.NewGuid().ToString());

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<Dictionary<string, List<Dictionary<string, string>>>>>(responseBody);
                return result[0]["translations"][0]["text"];

            }
        }
    }
}
