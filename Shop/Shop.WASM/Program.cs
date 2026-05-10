using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shop.Common;
using MudBlazor.Services;
namespace Shop.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
          
            builder.Services.AddHttpClient<ApiClient>();
            builder.Services.AddMudServices();
            await builder.Build().RunAsync();
        }
    }
}
