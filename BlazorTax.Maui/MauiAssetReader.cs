using BlazorTax.Services;

namespace BlazorTax.Maui;

/// <summary>MAUI implementatie: leest uit Resources/Raw via app-pakket.</summary>
public class MauiAssetReader : IAssetReader
{
    public async Task<string> GetStringAsync(string relativePath)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(relativePath);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
