namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK XX – Bezoldigingen Meewerkende Echtgenoten</summary>
public class VakXXData
{
    public decimal? Code1450 { get; set; }   // toegekende bezoldigingen
    public decimal? Code2450 { get; set; }
    public decimal? Code1451 { get; set; }   // sociale bijdragen
    public decimal? Code2451 { get; set; }
    public decimal? Code1452 { get; set; }   // andere beroepskosten
    public decimal? Code2452 { get; set; }
    public decimal? Code1453 { get; set; }   // bijberoep/student-zelfstandige
    public decimal? Code2453 { get; set; }

    // ── Begin-/einddatum ────────────────────────────────────────────────────
    public string Code1455 { get; set; } = string.Empty;   // begindatum
    public string Code2455 { get; set; } = string.Empty;
    public string Code1456 { get; set; } = string.Empty;   // einddatum
    public string Code2456 { get; set; } = string.Empty;

    // ── Inkomsten buitenlandse oorsprong ─────────────────────────────────────
    public List<BuitenlandInkomen> BuitenlandseInkomsten { get; set; } = [new()];
}
