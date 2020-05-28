using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MSTranslatorDemo
{
    class translate
    {
        public string translation { get; set; }
        public string TextTranslationEndpoint { get; set; }
        public string Cognitive_Services_Key { get; set; }
        public async System.Threading.Tasks.Task go(string textToTranslate, string fromLanguageCode, string toLanguageCode)
        {

            if (textToTranslate == "" || fromLanguageCode == toLanguageCode)
            {
                translation = textToTranslate;
                return;
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
                translation = result[0]["translations"][0]["text"];

            }
        }
}
