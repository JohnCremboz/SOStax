using System.Globalization;

namespace BlazorTax.Services;

/// <summary>
/// Implementatie van de service voor gemeentelijke aanslagvoeten.
/// Laadt data uit het CSV-bestand APB_2008_2026.csv.
/// </summary>
public class GemeenteAanslagvoetService : IGemeenteAanslagvoetService
{
    private readonly Dictionary<string, Dictionary<int, decimal>> _aanslagvoeten = new();
    private readonly List<string> _gemeenteNamen = new();

    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync(IAssetReader assetReader)
    {
        if (IsInitialized)
            return;

        try
        {
            var csvContent = await assetReader.GetStringAsync("belastingen/APB_2008_2026.csv");
            ParseCsv(csvContent);
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij laden gemeentelijke aanslagvoeten: {ex.Message}");
            throw;
        }
    }

    private void ParseCsv(string csvContent)
    {
        var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
            return;

        // Eerste lijn bevat de kolomkoppen: Gemeente;2008;2009;...;2026
        var headers = lines[0].Split(';');
        var jaren = new List<int>();

        // Parse de jaren uit de headers (kolom 1 t/m laatste)
        for (int i = 1; i < headers.Length; i++)
        {
            if (int.TryParse(headers[i].Trim(), out int jaar))
            {
                jaren.Add(jaar);
            }
        }

        // Parse de datarijen
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(';');
            if (columns.Length < 2)
                continue;

            var gemeenteNaam = columns[0].Trim();
            if (string.IsNullOrEmpty(gemeenteNaam))
                continue;

            var aanslagvoetenPerJaar = new Dictionary<int, decimal>();

            // Parse de aanslagvoeten per jaar
            for (int j = 0; j < jaren.Count && j + 1 < columns.Length; j++)
            {
                var value = columns[j + 1].Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    // Vervang komma door punt voor decimal parsing
                    value = value.Replace(',', '.');

                    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal aanslagvoet))
                    {
                        aanslagvoetenPerJaar[jaren[j]] = aanslagvoet;
                    }
                }
            }

            if (aanslagvoetenPerJaar.Count > 0)
            {
                _aanslagvoeten[gemeenteNaam] = aanslagvoetenPerJaar;
                _gemeenteNamen.Add(gemeenteNaam);
            }
        }

        _gemeenteNamen.Sort();
    }

    public decimal? GetAanslagvoet(string gemeenteNaam, int jaar)
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Service is niet geïnitialiseerd. Roep eerst InitializeAsync aan.");

        if (string.IsNullOrWhiteSpace(gemeenteNaam))
            return null;

        // Probeer exacte match
        if (_aanslagvoeten.TryGetValue(gemeenteNaam, out var aanslagvoetenPerJaar))
        {
            if (aanslagvoetenPerJaar.TryGetValue(jaar, out var aanslagvoet))
                return aanslagvoet;
        }

        // Probeer case-insensitive match
        var matchingKey = _aanslagvoeten.Keys
            .FirstOrDefault(k => k.Equals(gemeenteNaam, StringComparison.OrdinalIgnoreCase));

        if (matchingKey != null)
        {
            if (_aanslagvoeten[matchingKey].TryGetValue(jaar, out var aanslagvoet))
                return aanslagvoet;
        }

        return null;
    }

    public IReadOnlyList<string> GetGemeenteNamen()
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Service is niet geïnitialiseerd. Roep eerst InitializeAsync aan.");

        return _gemeenteNamen.AsReadOnly();
    }

    /// <summary>
    /// Zoekt gemeenten op basis van een zoekterm (case-insensitive, gedeeltelijke match).
    /// </summary>
    public IReadOnlyList<string> ZoekGemeenten(string zoekterm)
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Service is niet geïnitialiseerd. Roep eerst InitializeAsync aan.");

        if (string.IsNullOrWhiteSpace(zoekterm))
            return _gemeenteNamen.AsReadOnly();

        return _gemeenteNamen
            .Where(g => g.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }
}
