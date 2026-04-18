namespace BlazorTax.Services;

/// <summary>
/// Abstractie voor het lezen van statische bestanden (wwwroot).
/// WASM: via HttpClient, MAUI: via AppPackage.
/// </summary>
public interface IAssetReader
{
    Task<string> GetStringAsync(string relativePath);
}
