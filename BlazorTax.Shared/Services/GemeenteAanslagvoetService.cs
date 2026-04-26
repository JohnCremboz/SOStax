using BlazorTax.Belastingen.Berekening;

namespace BlazorTax.Services;

/// <summary>
/// Implementatie van de service voor gemeentelijke aanslagvoeten.
/// Data is statisch ingebakken via <see cref="GemeenteAanslagvoetData"/> (geen CSV-parse meer).
/// </summary>
public class GemeenteAanslagvoetService : IGemeenteAanslagvoetService
{
    private readonly Dictionary<string, Dictionary<int, decimal>> _aanslagvoeten
        = GemeenteAanslagvoetData.Aanslagvoeten;

    private readonly List<string> _gemeenteNamen;

    public bool IsInitialized { get; private set; }

    public GemeenteAanslagvoetService()
    {
        _gemeenteNamen = [.. _aanslagvoeten.Keys.Order(StringComparer.OrdinalIgnoreCase)];
        IsInitialized = true;
    }

    /// <inheritdoc/>
    public Task InitializeAsync(IAssetReader? assetReader = null)
        => Task.CompletedTask;

    public decimal? GetAanslagvoet(string gemeenteNaam, int jaar)
    {
        if (string.IsNullOrWhiteSpace(gemeenteNaam))
            return null;

        if (_aanslagvoeten.TryGetValue(gemeenteNaam, out var perJaar) &&
            perJaar.TryGetValue(jaar, out var voet))
            return voet;

        return null;
    }

    public IReadOnlyList<string> GetGemeenteNamen() => _gemeenteNamen.AsReadOnly();

    public Gewest? GetGewest(string gemeenteNaam)
    {
        if (string.IsNullOrWhiteSpace(gemeenteNaam))
            return null;

        if (_brusselseGemeenten.Contains(gemeenteNaam))
            return Gewest.Brussel;

        if (_vlaamseGemeenten.Contains(gemeenteNaam))
            return Gewest.Vlaanderen;

        var key = _aanslagvoeten.Keys
            .FirstOrDefault(k => k.Equals(gemeenteNaam, StringComparison.OrdinalIgnoreCase));
        if (key is not null)
        {
            if (_brusselseGemeenten.Contains(key)) return Gewest.Brussel;
            if (_vlaamseGemeenten.Contains(key)) return Gewest.Vlaanderen;
            return Gewest.Wallonie;
        }

        return null;
    }

    public IReadOnlyList<string> ZoekGemeenten(string zoekterm)
    {
        if (string.IsNullOrWhiteSpace(zoekterm))
            return _gemeenteNamen.AsReadOnly();

        if (zoekterm.Length > 100)
            return Array.Empty<string>();

        return _gemeenteNamen
            .Where(g => g.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
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
        "Aalter (vanaf 2019)", "Aalter (t/m 2018)", "Beveren (t/m 2024)",
        "Beveren-Kruibeke-Zwijndrecht (vanaf 2025)", "Oudsbergen (vanaf 2019)",
        "Merelbeke-Melle (vanaf 2025)"
    };
}

