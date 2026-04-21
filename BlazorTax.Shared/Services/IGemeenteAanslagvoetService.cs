namespace BlazorTax.Services;

/// <summary>
/// Service voor het ophalen van gemeentelijke aanslagvoeten per gemeente en jaar.
/// </summary>
public interface IGemeenteAanslagvoetService
{
    /// <summary>
    /// Laadt de gemeentelijke aanslagvoeten uit het CSV-bestand.
    /// </summary>
    /// <param name="assetReader">De asset reader om het CSV bestand te laden</param>
    Task InitializeAsync(IAssetReader assetReader);

    /// <summary>
    /// Haalt de aanslagvoet op voor een specifieke gemeente en jaar.
    /// </summary>
    /// <param name="gemeenteNaam">De naam van de gemeente</param>
    /// <param name="jaar">Het aanslagjaar (2008-2026)</param>
    /// <returns>De aanslagvoet als percentage (bijv. 7,5) of null als niet gevonden</returns>
    decimal? GetAanslagvoet(string gemeenteNaam, int jaar);

    /// <summary>
    /// Haalt alle beschikbare gemeentenamen op.
    /// </summary>
    IReadOnlyList<string> GetGemeenteNamen();

    /// <summary>
    /// Geeft aan of de service is geïnitialiseerd.
    /// </summary>
    bool IsInitialized { get; }
}
