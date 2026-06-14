using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Shop.Common;
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

            builder.Services.AddScoped<CartService>();
            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
