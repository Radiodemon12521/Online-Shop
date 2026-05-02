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
        public Task<HttpResponseMessage> Update(Product product)
        {
            return _client.PutAsJsonAsync("/Products/Update",product);
        }
        public Task<HttpResponseMessage> Create(Product product)
        {
            return _client.PostAsJsonAsync("/Products/Create", product);
        }
        public Task<HttpResponseMessage> Delete(Product product)
        {
            return _client.DeleteAsync($"/Products/{product.Id}");
        }
        public Task<List<Category>> GetChildren(Guid? parentId)
        {
            return _client.GetFromJsonAsync<List<Category>>($"Category/Children?parentId={parentId}");
        }
        public Task<List<Category>> GetAllCategoriesFlat()
        {
            return _client.GetFromJsonAsync<List<Category>>("Category/Flat");
        }
        public Task<HttpResponseMessage> TryGetAllCategories()
        {
            return _client.GetAsync("/Category");
        }
        public Task<HttpResponseMessage> Update(Category category)
        {
            return _client.PutAsJsonAsync("/Category/Update", category);
        }
        public Task<HttpResponseMessage> Create(Category category)
        {
            return _client.PostAsJsonAsync("/Category/Create", category);
        }
        public Task<HttpResponseMessage> Delete(Category category)
        {
            return _client.DeleteAsync($"/Category/{category.Id}");
        }

        public Task<List<Product>> ByCategory(Category category)
        {
            return _client.GetFromJsonAsync<List<Product>>($"/Products/ByCategory?id={category.Id}");
        }
    }
}
