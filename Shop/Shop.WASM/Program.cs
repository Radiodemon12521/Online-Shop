using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Shop.Common;
using Shop.Common.Services;
using ShopWASM;
namespace Shop.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddTransient<CookieHandler>();
            builder.Services.AddHttpClient<ApiClient>().AddHttpMessageHandler<CookieHandler>();
            builder.Services.AddSingleton<IJwtService, JwtService>();

            builder.Services.AddScoped<CartService>();
            builder.Services.AddMudServices();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<CustomAuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<CustomAuthStateProvider>());
            await builder.Build().RunAsync();
        }
    }
}
