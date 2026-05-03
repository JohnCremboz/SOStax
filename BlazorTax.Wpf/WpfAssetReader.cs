using System.IO;
using BlazorTax.Services;

namespace BlazorTax.Wpf;

/// <summary>WPF implementatie: leest uit de app-directory op het bestandssysteem.</summary>
public class WpfAssetReader : IAssetReader
{
    public Task<string> GetStringAsync(string relativePath)
    {
        var path = System.IO.Path.Combine(AppContext.BaseDirectory, relativePath);
        return File.ReadAllTextAsync(path);
    }
}
