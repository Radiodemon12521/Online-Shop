using Microsoft.Extensions.Configuration;
using Shop.Entities;
using System.Net.Http.Json;

namespace Shop.Common
{
    public class ApiClient
    {
        private readonly IConfiguration _configuration;
        private HttpClient _client;
        public ApiClient(HttpClient httpClient,IConfiguration configuration) 
        { 
            _client= httpClient;
            _configuration= configuration;
            _client.BaseAddress = new Uri(configuration.GetSection("ApiBaseAdress").Value);
        }
        public Task<List<Product>> GetAllProducts()
        {
            return _client.GetFromJsonAsync<List<Product>>("Products");
        }
        public Task<HttpResponseMessage> TryGetAllProducts()
        {
            return _client.GetAsync("/Products");
        }

    }
}
