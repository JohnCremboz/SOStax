namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK XXI – Winst en Baten Vorige Beroepswerkzaamheid</summary>
public class VakXXIData
{
    // ── Stopzettingsmeerwaarden ──────────────────────────────────────────────
    public decimal? Code1686 { get; set; }   // 10%
    public decimal? Code2686 { get; set; }
    public decimal? Code1690 { get; set; }   // 16,5%
    public decimal? Code2690 { get; set; }
    public decimal? Code1691 { get; set; }   // 33%
    public decimal? Code2691 { get; set; }
    public decimal? Code1692 { get; set; }   // gezamenlijk
    public decimal? Code2692 { get; set; }
    public decimal? Code1693 { get; set; }   // winst/baten kosten overdracht
    public decimal? Code2693 { get; set; }

    // ── Premies/vergoedingen ────────────────────────────────────────────────
    public decimal? Code1687 { get; set; }   // 12,5%
    public decimal? Code2687 { get; set; }
    public decimal? Code1694 { get; set; }   // 16,5%
    public decimal? Code2694 { get; set; }
    public decimal? Code1695 { get; set; }   // winst/baten na stopzetting
    public decimal? Code2695 { get; set; }

    // ── Sportieve activiteiten ──────────────────────────────────────────────
    public decimal? Code1688 { get; set; }   // baten sportieve activiteiten
    public decimal? Code2688 { get; set; }
    public decimal? Code1689 { get; set; }   // baten opleiders/trainers sportbeoefenaars
    public decimal? Code2689 { get; set; }

    // ── Beroepskosten ───────────────────────────────────────────────────────
    public decimal? Code1696 { get; set; }   // overdracht activa
    public decimal? Code2696 { get; set; }
    public decimal? Code1697 { get; set; }   // andere beroepskosten
    public decimal? Code2697 { get; set; }

    // ── Hervatting na ontslag ───────────────────────────────────────────────
    public decimal? Code1698 { get; set; }
    public decimal? Code2698 { get; set; }

    // ── Inkomsten buitenlandse oorsprong ─────────────────────────────────────
    public List<BuitenlandInkomen> BuitenlandseInkomsten { get; set; } = [new()];

    // ── Feitelijke vereniging details ────────────────────────────────────────
    public List<FeitelijkeVereniging> FeitelijkeVerenigingen { get; set; } = [new()];

    // ── Verlies feitelijke vereniging ────────────────────────────────────────
    public string VerliesVerenigingNaam1 { get; set; } = string.Empty;
    public string VerliesVerenigingNaam2 { get; set; } = string.Empty;
    public string VerliesVerenigingAdres1 { get; set; } = string.Empty;
    public string VerliesVerenigingAdres2 { get; set; } = string.Empty;
}
