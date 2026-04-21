namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK XV – Diverse Inkomsten</summary>
public class VakXVData
{
    // ── A. Diverse inkomsten van roerende aard ──────────────────────────────
    public decimal? Code1440 { get; set; }   // netto-inkomen onroerend buitenland (gemeenschappelijk)
    public decimal? Code2440 { get; set; }
    public decimal? Code1179 { get; set; }   // verhuring/onderverhuring roerende goederen
    public decimal? Code2179 { get; set; }
    public decimal? Code1180 { get; set; }   // concessie plakbrieven/reclame
    public decimal? Code2180 { get; set; }
    public decimal? Code1199 { get; set; }   // jacht-, vis- en vogelvangstrechten
    public decimal? Code2199 { get; set; }
    public decimal? Code1443 { get; set; }   // onderhoudsgelden totaalbedrag
    public decimal? Code2443 { get; set; }
    public decimal? Code1198 { get; set; }   // onderhoudsgelden afzonderlijk 80%
    public decimal? Code2198 { get; set; }

    // ── B. Andere diverse inkomsten ─────────────────────────────────────────
    public decimal? Code1200 { get; set; }   // deeleconomie
    public decimal? Code2200 { get; set; }
    public decimal? Code1460 { get; set; }   // toevallige winsten gezamenlijk
    public decimal? Code2460 { get; set; }
    public decimal? Code1461 { get; set; }   // toevallige winsten 33%
    public decimal? Code2461 { get; set; }
    public decimal? Code1462 { get; set; }   // toevallige winsten 16,5%
    public decimal? Code2462 { get; set; }
    public decimal? Code1172 { get; set; }   // prijzen en subsidies
    public decimal? Code2172 { get; set; }
    public decimal? Code1463 { get; set; }   // vergoedingen niet-werkende scheidsrechters 33%
    public decimal? Code2463 { get; set; }
    public decimal? Code1175 { get; set; }   // meerwaarden ongebouwd
    public decimal? Code2175 { get; set; }
    public decimal? Code1176 { get; set; }   // meerwaarden gebouwd
    public decimal? Code2176 { get; set; }
    public decimal? Code1177 { get; set; }   // meerwaarden ongebouwd 16,5%
    public decimal? Code2177 { get; set; }
    public decimal? Code1178 { get; set; }   // meerwaarden gebouwd 16,5%
    public decimal? Code2178 { get; set; }
    public decimal? Code1209 { get; set; }   // meerwaarden gronden/gebouwen herzien
    public decimal? Code2209 { get; set; }
    public decimal? Code1169 { get; set; }   // meerwaarden aandelen 33%
    public decimal? Code2169 { get; set; }
    public decimal? Code1170 { get; set; }   // loten van effecten
    public decimal? Code2170 { get; set; }
    public decimal? Code1171 { get; set; }   // ander divers inkomen
    public decimal? Code2171 { get; set; }
    public decimal? Code1168 { get; set; }   // buitenlandse oorsprong: kosten
    public decimal? Code2168 { get; set; }
    public decimal? Code1467 { get; set; }   // buitenlandse oorsprong: netto bedrag
    public decimal? Code2467 { get; set; }

    // ── Inkomsten buitenlandse oorsprong ─────────────────────────────────────
    public List<BuitenlandInkomen> BuitenlandseInkomsten { get; set; } = [new()];
}

/// <summary>Rij voor buitenlands inkomen (diverse/zelfstandige vakken).</summary>
public class BuitenlandInkomen
{
    public string Land { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal? Bedrag { get; set; }
    /// "vrijstelling" | "vermindering" | ""
    public string TypeVermindering { get; set; } = string.Empty;
}
