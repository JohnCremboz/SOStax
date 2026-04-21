using BlazorTax.Services;

namespace BlazorTax;

/// <summary>WASM implementatie: leest via HttpClient uit wwwroot.</summary>
public class HttpAssetReader(HttpClient http) : IAssetReader
{
    public Task<string> GetStringAsync(string relativePath)
        => http.GetStringAsync($"/{relativePath}");
}
