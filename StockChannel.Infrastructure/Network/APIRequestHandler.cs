using System;
using System.IO;
using System.Net.Http;
using StockChannel.Infrastructure.Interfaces;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StockChannel.Infrastructure.Network
{
    public class ApiRequestHandler : IAPIRequestHandler
    {
        private readonly IHttpClientFactory _clientFactory;
        public ApiRequestHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }
        
        public async Task<T> Get<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            
            if (response.IsSuccessStatusCode) {
                var searchResult = await GetSerializedResponse<T>(response);
                return searchResult;
            }

            throw new System.NotImplementedException();
        }
        private async Task<T> GetSerializedResponse<T>(HttpResponseMessage response)
        {
            var callResponse = response.Content.ReadAsStringAsync().Result;
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var serializer = new JsonSerializer();
                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }
    }
}