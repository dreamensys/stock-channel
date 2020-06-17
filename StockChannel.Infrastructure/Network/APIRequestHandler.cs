using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using StockChannel.Infrastructure.Interfaces;
using System.Threading.Tasks;
using CsvHelper;
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
        
        public async Task<T> Get<T>(string url, string format = "JSON")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            var client = _clientFactory.CreateClient();
            
            if (format == "CSV")
            {
                var csvResult = await ParseCSVResponse<T>(client, url);
                return csvResult.FirstOrDefault();
            }
            
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) {
                throw new System.NotImplementedException();
            }

            var jsonResult = await GetSerializedResponse<T>(response);
            return jsonResult;
        }

        private async Task<List<T>> ParseCSVResponse<T>(HttpClient client, string url)
        {
            using (var stream = await client.GetStreamAsync(url))
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader,CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<T>();
                return records.ToList();
            }
        }

        private async Task<T> GetSerializedResponse<T>(HttpResponseMessage response)
        {
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