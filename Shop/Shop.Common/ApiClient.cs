using Microsoft.Extensions.Configuration;
using Shop.Common.DTOs;
using Shop.Common.Services;
using Shop.Entities;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Shop.Common
{
    public class ApiClient
    {
        private readonly IConfiguration _configuration;
        private HttpClient _client;
        private readonly IJwtService _jwtService;
        public ApiClient(HttpClient httpClient, IConfiguration configuration, IJwtService jwtService)
        {
            _client = httpClient;
            _jwtService = jwtService;
            _configuration = configuration;
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
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtService.Token);
            return _client.PutAsJsonAsync("/Products/Update", product);
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
        public Task<HttpResponseMessage> TryByCategory(Category category)
        {
            return _client.GetAsync($"/Products/ByCategory?id={category.Id}");
        }
        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            var response = await _client.PostAsJsonAsync("/auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                _jwtService.Token= token.Trim();
                _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _jwtService.Token);
                return token;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> RefreshToken()
        {
            var response = await _client.GetAsync("/auth/refreshtoken");
            if (response.IsSuccessStatusCode)
            {
                _jwtService.Token=(await response.Content.ReadAsStringAsync()).Trim();
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwtService.Token);
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<(bool Success, string Message)> Login(LoginRequest request)
        {
            var response = await _client.PostAsJsonAsync("/auth/login", request);
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                _jwtService.Token = message.Trim();
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _jwtService.Token);
                return (true, "Успешно вошли в систему");
            }

            return (false, message);
        }
        public async Task<string> Logout()
        {
            var responce = await _client.PostAsync("/auth/logout", null);
            if (responce.IsSuccessStatusCode)
            {
                _jwtService.Token = null;
                _client.DefaultRequestHeaders.Authorization = null;
                return "Успешно вышли из системы";

            }
            else
            {
                var message = await responce.Content.ReadAsStringAsync();
                return message;
            }
        }
    }
}
