using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IEvangelist.Blazing.WarFleet.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(
                _ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await using var host = builder.Build();
            await host.RunAsync();
        }
    }
}

// IDEAS & INSPIRATION
/*
https://chrissainty.com/investigating-drag-and-drop-with-blazor
https://www.exceptionnotfound.net/modeling-battleship-in-csharp-components-and-setup
https://github.com/kubowania/battleships
https://fontawesome.com/icons
https://github.com/IEvangelist/IEvangelist.Battleship
 */