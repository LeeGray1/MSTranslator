using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public class CallWebApi
    {
        public async Task<bool> CallGetWebAPI(string baseUri, string route, string parameter)
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
                    return true;
                }

            }
            return false;
        }

        public async Task<bool> CallPostWebAPI(string baseUri, string action, string requestBody)
        {
            string uri = baseUri + action;
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {

                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");


                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }


            }
            return false;
        }
    }
}
