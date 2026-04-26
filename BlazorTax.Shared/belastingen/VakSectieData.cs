namespace BlazorTax.Belastingen;

/// <summary>Statische lijst van alle VAK-secties (vervangt runtime parsing van structuur.md).</summary>
public static class VakSectieData
{
    public static readonly IReadOnlyList<VakSection> Secties =
    [
        new VakSection("VAK I", "VAK I — Bankrekening en Telefoonnummers", string.Empty),
        new VakSection("VAK II", "VAK II — Persoonlijke gegevens en gezinslasten", string.Empty),
        new VakSection("VAK III", "VAK III — Inkomsten van onroerende goederen", string.Empty),
        new VakSection("VAK IV", "VAK IV — Wedden, lonen, uitkeringen, vervangingsinkomsten, bedrijfstoeslag", string.Empty),
        new VakSection("VAK V", "VAK V — Pensioenen", string.Empty),
        new VakSection("VAK VI", "VAK VI — Ontvangen onderhoudsuitkeringen", string.Empty),
        new VakSection("VAK VII", "VAK VII — Inkomsten van kapitalen en roerende goederen", string.Empty),
        new VakSection("VAK VIII", "VAK VIII — Aftrekbare vorige verliezen en bestedingen", string.Empty),
        new VakSection("VAK IX", "VAK IX — Interesten, kapitaalaflossingen, premies, erfpacht/opstal", string.Empty),
        new VakSection("VAK X", "VAK X — Belastingverminderingen", string.Empty),
        new VakSection("VAK XI", "VAK XI — Gewestelijke belastingkredieten", string.Empty),
        new VakSection("VAK XII", "VAK XII — Voorafbetalingen", string.Empty),
        new VakSection("VAK XIII", "VAK XIII — Buitenlandse rekeningen, verzekeringen, juridische constructies, leningen, beroepskosten", string.Empty),
        new VakSection("VAK XIV", "VAK XIV — Beroep en Ondernemingsnummer", string.Empty),
        new VakSection("VAK XV", "VAK XV — Diverse Inkomsten", string.Empty),
        new VakSection("VAK XVI", "VAK XVI — Bezoldigingen Bedrijfsleiders", string.Empty),
        new VakSection("VAK XVII", "VAK XVII — Winst uit Nijverheids-, Handels- of Landbouwondernemingen", string.Empty),
        new VakSection("VAK XVIII", "VAK XVIII — Baten van Vrije Beroepen, Ambten, Posten of Andere Winstgevende Bezigheden", string.Empty),
        new VakSection("VAK XIX", "VAK XIX — Verrekenbare Bestanddelen Zelfstandige Beroepswerkzaamheid", string.Empty),
        new VakSection("VAK XX", "VAK XX — Bezoldigingen Meewerkende Echtgenoten en Wettelijk Samenwonende Partners", string.Empty),
        new VakSection("VAK XXI", "VAK XXI — Winst en Baten van een Vorige Beroepswerkzaamheid", string.Empty),
        new VakSection("VAK XXII", "VAK XXII — Eerste Vestiging als Zelfstandige", string.Empty),
    ];
}
