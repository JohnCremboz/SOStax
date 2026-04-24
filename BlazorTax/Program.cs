using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorTax;
using BlazorTax.Belastingen;
using BlazorTax.Belastingen.Validatie;
using BlazorTax.Services;
using FluentValidation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAssetReader, HttpAssetReader>();
builder.Services.AddSingleton<IGemeenteAanslagvoetService, GemeenteAanslagvoetService>();
builder.Services.AddScoped<IValidator<AangifteState>, AangifteStateValidator>();
builder.Services.AddScoped<AangifteStateService>();

await builder.Build().RunAsync();
