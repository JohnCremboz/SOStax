namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK III – Inkomsten van onroerende goederen.</summary>
public class VakIIIData
{
    // ── A. BELGISCHE EN BUITENLANDSE INKOMSTEN ────────────────────────────────
    // 1. Beroep
    public decimal? Code1105 { get; set; }
    public decimal? Code2105 { get; set; }

    // 2. Gebouwen (niet verhuurd / verhuurd aan particulieren / sociale huisvesting)
    public decimal? Code1106 { get; set; }
    public decimal? Code2106 { get; set; }

    // 3. Gronden/materieel/outillering
    public decimal? Code1107 { get; set; }
    public decimal? Code2107 { get; set; }

    // 4. Pachtwetgeving
    public decimal? Code1108 { get; set; }
    public decimal? Code2108 { get; set; }

    // 5a. Gebouwen – andere omstandigheden
    public decimal? Code1109 { get; set; }
    public decimal? Code2109 { get; set; }
    public decimal? Code1110 { get; set; }   // brutohuur
    public decimal? Code2110 { get; set; }

    // 5b. Gronden – andere omstandigheden
    public decimal? Code1112 { get; set; }
    public decimal? Code2112 { get; set; }
    public decimal? Code1113 { get; set; }   // brutohuur
    public decimal? Code2113 { get; set; }

    // 5c. Materieel/outillering – andere omstandigheden
    public decimal? Code1115 { get; set; }
    public decimal? Code2115 { get; set; }
    public decimal? Code1116 { get; set; }   // brutohuur
    public decimal? Code2116 { get; set; }

    // 6. Erfpacht/opstal
    public decimal? Code1114 { get; set; }
    public decimal? Code2114 { get; set; }

    // ── B. BUITENLANDSE OORSPRONG ─────────────────────────────────────────────
    // 1. Vrijstelling met progressievoorbehoud (meerdere rijen mogelijk)
    public List<BuitenlandsOnroerendGoed> Vrijstelling { get; set; } = [new()];
    // 2. Vermindering tot de helft
    public List<BuitenlandsOnroerendGoed> Vermindering { get; set; } = [new()];
}

public class BuitenlandsOnroerendGoed
{
    public string Land { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal? Bedrag { get; set; }
}

