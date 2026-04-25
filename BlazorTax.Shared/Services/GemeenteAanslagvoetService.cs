using System.Globalization;
using BlazorTax.Belastingen.Berekening;

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
            Console.WriteLine("Fout bij laden gemeentelijke aanslagvoeten.");
            _ = ex;
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

    public Gewest? GetGewest(string gemeenteNaam)
    {
        if (string.IsNullOrWhiteSpace(gemeenteNaam))
            return null;

        if (_brusselseGemeenten.Contains(gemeenteNaam))
            return Gewest.Brussel;

        if (_vlaamseGemeenten.Contains(gemeenteNaam))
            return Gewest.Vlaanderen;

        // Case-insensitive fallback
        var key = _aanslagvoeten.Keys.FirstOrDefault(k => k.Equals(gemeenteNaam, StringComparison.OrdinalIgnoreCase));
        if (key is not null)
        {
            if (_brusselseGemeenten.Contains(key)) return Gewest.Brussel;
            if (_vlaamseGemeenten.Contains(key)) return Gewest.Vlaanderen;
            return Gewest.Wallonie;
        }

        return null;
    }

    private static readonly HashSet<string> _brusselseGemeenten = new(StringComparer.OrdinalIgnoreCase)
    {
        "Anderlecht", "Brussel", "Elsene", "Etterbeek", "Evere", "Ganshoren",
        "Jette", "Koekelberg", "Oudergem", "Schaarbeek", "Sint-Agatha-Berchem",
        "Sint-Gillis", "Sint-Jans-Molenbeek", "Sint-Joost-ten-Node",
        "Sint-Lambrechts-Woluwe", "Sint-Pieters-Woluwe", "Ukkel", "Vorst",
        "Watermaal-Bosvoorde"
    };

    private static readonly HashSet<string> _vlaamseGemeenten = new(StringComparer.OrdinalIgnoreCase)
    {
        "Aalst", "Aalter", "Aarschot", "Aartselaar", "Affligem", "Alken", "Alveringem",
        "Antwerpen", "Anzegem", "Ardooie", "Arendonk", "As", "Asse", "Assenede",
        "Avelgem", "Baarle-Hertog", "Balen", "Beernem", "Beerse", "Beersel",
        "Begijnendijk", "Bekkevoort", "Beringen", "Berlaar", "Berlare", "Bertem",
        "Bever", "Beveren-Kruibeke-Zwijndrecht", "Bierbeek", "Bilzen-Hoeselt",
        "Blankenberge", "Bocholt", "Boechout", "Bonheiden", "Boom", "Boortmeerbeek",
        "Bornem", "Boutersem", "Brakel", "Brasschaat", "Brecht", "Bredene", "Bree",
        "Brugge", "Buggenhout", "Damme", "De Haan", "De Panne", "Deerlijk", "Deinze",
        "Denderleeuw", "Dendermonde", "Dentergem", "Dessel", "Destelbergen",
        "Diepenbeek", "Diest", "Diksmuide", "Dilbeek", "Dilsen-Stokkem", "Drogenbos",
        "Duffel", "Edegem", "Eeklo", "Erpe-Mere", "Essen", "Evergem", "Gavere",
        "Geel", "Geetbets", "Genk", "Gent", "Geraardsbergen", "Gingelom", "Gistel",
        "Glabbeek", "Grimbergen", "Grobbendonk", "Haacht", "Haaltert", "Halen",
        "Halle", "Hamme", "Hamont-Achel", "Harelbeke", "Hasselt", "Hechtel-Eksel",
        "Heers", "Heist-op-den-Berg", "Hemiksem", "Herent", "Herentals", "Herenthout",
        "Herk-de-Stad", "Herselt", "Herstappe", "Herzele", "Heusden-Zolder",
        "Heuvelland", "Hoegaarden", "Hoeilaart", "Holsbeek", "Hooglede", "Hoogstraten",
        "Horebeke", "Houthalen-Helchteren", "Houthulst", "Hove", "Huldenberg",
        "Hulshout", "Ichtegem", "Ieper", "Ingelmunster", "Izegem", "Jabbeke",
        "Kalmthout", "Kampenhout", "Kapellen", "Kapelle-op-den-Bos", "Kaprijke",
        "Kasterlee", "Keerbergen", "Kinrooi", "Kluisbergen", "Knokke-Heist",
        "Koekelare", "Koksijde", "Kontich", "Kortemark", "Kortenaken", "Kortenberg",
        "Kortrijk", "Kraainem", "Kruisem", "Kuurne", "Laakdal", "Laarne", "Lanaken",
        "Landen", "Langemark-Poelkapelle", "Lebbeke", "Lede", "Ledegem", "Lendelede",
        "Lennik", "Leopoldsburg", "Leuven", "Lichtervelde", "Liedekerke", "Lier",
        "Lierde", "Lievegem", "Lille", "Linkebeek", "Lint", "Linter", "Lochristi",
        "Lokeren", "Lommel", "Londerzeel", "Lo-Reninge", "Lubbeek", "Lummen",
        "Maarkedal", "Maaseik", "Maasmechelen", "Machelen", "Maldegem", "Malle",
        "Mechelen", "Meerhout", "Meise", "Menen", "Merchtem", "Merelbeke-Melle",
        "Merksplas", "Mesen", "Middelkerke", "Mol", "Moorslede", "Mortsel",
        "Nazareth-De Pinte", "Niel", "Nieuwerkerken", "Nieuwpoort", "Nijlen", "Ninove",
        "Olen", "Oostende", "Oosterzele", "Oostkamp", "Oostrozebeke", "Opwijk",
        "Oudenaarde", "Oudenburg", "Oud-Heverlee", "Oudsbergen", "Oud-Turnhout",
        "Overijse", "Pajottegem", "Peer", "Pelt", "Pepingen", "Pittem", "Poperinge",
        "Putte", "Puurs-Sint-Amands", "Ranst", "Ravels", "Retie", "Riemst",
        "Rijkevorsel", "Roeselare", "Ronse", "Roosdaal", "Rotselaar", "Rumst",
        "Schelle", "Scherpenheuvel-Zichem", "Schilde", "Schoten", "Sint-Genesius-Rode",
        "Sint-Gillis-Waas", "Sint-Katelijne-Waver", "Sint-Laureins",
        "Sint-Lievens-Houtem", "Sint-Martens-Latem", "Sint-Niklaas",
        "Sint-Pieters-Leeuw", "Sint-Truiden", "Spiere-Helkijn", "Stabroek", "Staden",
        "Steenokkerzeel", "Stekene", "Temse", "Ternat", "Tervuren", "Tessenderlo-Ham",
        "Tielt", "Tielt-Winge", "Tienen", "Tongeren-Borgloon", "Torhout", "Turnhout",
        "Veurne", "Vosselaar", "Waregem", "Waasmunster", "Wachtebeke", "Wetteren",
        "Wevelgem", "Wielsbeke", "Wijnegem", "Willebroek", "Wingene", "Wommelgem",
        "Wortegem-Petegem", "Wuustwezel", "Zaventem", "Zedelgem", "Zele",
        "Zelzate", "Zemst", "Zingem", "Zoersel", "Zonhoven", "Zonnebeke",
        "Zottegem", "Zoutleeuw", "Zuienkerke", "Zulte", "Zutendaal", "Zwevegem",
        "Zwijndrecht",
        // Oud-namen die nog in CSV kunnen staan
        "Aalter (vanaf 2019)", "Aalter (t/m 2018)", "Beveren (t/m 2024)",
        "Beveren-Kruibeke-Zwijndrecht (vanaf 2025)", "Oudsbergen (vanaf 2019)",
        "Merelbeke-Melle (vanaf 2025)"
    };

    /// <summary>
    /// Zoekt gemeenten op basis van een zoekterm (case-insensitive, gedeeltelijke match).
    /// </summary>
    public IReadOnlyList<string> ZoekGemeenten(string zoekterm)
    {
        if (!IsInitialized)
            throw new InvalidOperationException("Service is niet geïnitialiseerd. Roep eerst InitializeAsync aan.");

        if (string.IsNullOrWhiteSpace(zoekterm))
            return _gemeenteNamen.AsReadOnly();

        if (zoekterm.Length > 100)
            return Array.Empty<string>();

        return _gemeenteNamen
            .Where(g => g.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }
}
