namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK XVI – Bezoldigingen Bedrijfsleiders</summary>
public class VakXVIData
{
    // ── Gewone bezoldigingen ────────────────────────────────────────────────
    public decimal? Code1400 { get; set; }   // gewone bezoldigingen
    public decimal? Code2400 { get; set; }
    public decimal? Code1401 { get; set; }   // huurinkomsten als bezoldiging
    public decimal? Code2401 { get; set; }
    public decimal? Code1402 { get; set; }   // voordelen van alle aard
    public decimal? Code2402 { get; set; }
    public decimal? Code1453 { get; set; }   // eigen kosten werkgever
    public decimal? Code2453 { get; set; }
    public decimal? Code1454 { get; set; }   // niet ingehouden persoonlijke sociale bijdragen
    public decimal? Code2454 { get; set; }

    // ── Bedrijfsvoorheffing ─────────────────────────────────────────────────
    public decimal? Code1421 { get; set; }
    public decimal? Code2421 { get; set; }

    // ── Inhoudingen aanvullend pensioen ──────────────────────────────────────
    public decimal? Code1407 { get; set; }
    public decimal? Code2407 { get; set; }

    // ── Vermindering werkloosheidsuitkeringen ───────────────────────────────
    public decimal? Code1408 { get; set; }
    public decimal? Code2408 { get; set; }

    // ── Werkbonus ────────────────────────────────────────────────────────────
    public decimal? Code1409 { get; set; }
    public decimal? Code2409 { get; set; }
    public decimal? Code1419 { get; set; }   // werkbonus Code284 equivalent
    public decimal? Code2419 { get; set; }
    public decimal? Code1430 { get; set; }   // werkbonus Code360 equivalent
    public decimal? Code2430 { get; set; }

    // ── Vakantiegeld ────────────────────────────────────────────────────────
    public decimal? Code1410 { get; set; }
    public decimal? Code2410 { get; set; }

    // ── Opzeggingsvergoedingen ──────────────────────────────────────────────
    public decimal? Code1411 { get; set; }
    public decimal? Code2411 { get; set; }

    // ── Achterstallen ───────────────────────────────────────────────────────
    public decimal? Code1439 { get; set; }
    public decimal? Code2439 { get; set; }

    // ── Fietsplan inkomsten ─────────────────────────────────────────────────
    public decimal? Code1412 { get; set; }
    public decimal? Code2412 { get; set; }

    // ── Overuren relance ────────────────────────────────────────────────────
    public int?     Code1413 { get; set; }   // aantal uren
    public int?     Code2413 { get; set; }
    public decimal? Code1414 { get; set; }   // bezoldiging
    public decimal? Code2414 { get; set; }

    // ── Begin-/einddatum ────────────────────────────────────────────────────
    public string Code1415 { get; set; } = string.Empty;   // begindatum
    public string Code2415 { get; set; } = string.Empty;
    public string Code1416 { get; set; } = string.Empty;   // einddatum
    public string Code2416 { get; set; } = string.Empty;

    // ── Bezoldigingen hervatting na ontslag ──────────────────────────────────
    public decimal? Code1417 { get; set; }
    public decimal? Code2417 { get; set; }

    // ── Roerende voorheffing auteursrechten ─────────────────────────────────
    public decimal? Code1427 { get; set; }
    public decimal? Code2427 { get; set; }

    // ── Inkomsten buitenlandse oorsprong ─────────────────────────────────────
    public List<BuitenlandInkomen> BuitenlandseInkomsten { get; set; } = [new()];
}
