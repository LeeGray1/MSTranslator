using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MSTranslatorDemo
{
    public class WebAPIHandler
    {
        private HttpClient _client = new HttpClient();
        public WebAPIHandler(HttpClient httpClient)
        {
            _client = httpClient;
        }
        public async Task<string> GetStringFromWebAPI(string baseUri, string route, string parameter)
        {
            string uri = baseUri + route + parameter;

            //using (var _client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);


                var response = await _client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return responseBody;
                }

            }
            return "";
        }

        public async Task<string> CallPostWebAPI(string baseUri, string action, string requestBody)
        {
            string uri = baseUri + action;
            //using (var _client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");


                var response = await _client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return responseBody;
                }


            }
            return "";
        }

        public async Task<string> CallGetXsltFromWebAPI(string baseUri, string route, string parameter)
        {
            string uri = baseUri + route + parameter;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);


                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return responseBody;
                }

            }
            return "";
        }

        public async Task<string> PostWebAPIToTranslate<T>(string baseUri, string route, T jsonObject)
        {
            string responseBody = null;
            string uri = baseUri + route;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(JsonConvert.SerializeObject(jsonObject), Encoding.UTF8, "application/json");


                var response = await client.SendAsync(request);
                responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return responseBody;
                }


            }
            return responseBody;
        }

        public async Task<string> PostWebAPI2TranslateText<T>(string baseUri, string route, T jsonObject)
        {
            string responseBody = null;
            string uri = baseUri + route;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(JsonConvert.SerializeObject(jsonObject), Encoding.UTF8, "application/json");


                var response = await client.SendAsync(request);
                responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return responseBody;
                }


            }
            return responseBody;
        }
    }
}
