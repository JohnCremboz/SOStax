using BlazorTax.Belastingen;
using BlazorTax.Belastingen.Validatie;
using BlazorTax.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BlazorTax.Wpf;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        var services = new ServiceCollection();
        services.AddWpfBlazorWebView();
        services.AddSingleton<IAssetReader, WpfAssetReader>();
        services.AddSingleton<IGemeenteAanslagvoetService, GemeenteAanslagvoetService>();
        services.AddScoped<IValidator<AangifteState>, AangifteStateValidator>();
        services.AddScoped<AangifteStateService>();

#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif

        Services = services.BuildServiceProvider();
        Resources.Add("services", Services);
        InitializeComponent();
    }
}
