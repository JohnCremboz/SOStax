namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK XVIII – Baten van Vrije Beroepen</summary>
public class VakXVIIIData
{
    // ── Ontvangsten ─────────────────────────────────────────────────────────
    public decimal? Code1650 { get; set; }   // uitoefening beroep
    public decimal? Code2650 { get; set; }
    public decimal? Code1658 { get; set; }   // sportbeoefenaars
    public decimal? Code2658 { get; set; }
    public decimal? Code1659 { get; set; }   // opleiders/trainers/begeleiders sportbeoefenaars
    public decimal? Code2659 { get; set; }
    public decimal? Code1652 { get; set; }   // achterstallige erelonen
    public decimal? Code2652 { get; set; }
    public decimal? Code1651 { get; set; }   // voorheen vrijgestelde baten
    public decimal? Code2651 { get; set; }

    // ── Meerwaarden ─────────────────────────────────────────────────────────
    public decimal? Code1653 { get; set; }   // 16,5%
    public decimal? Code2653 { get; set; }
    public decimal? Code1654 { get; set; }   // gezamenlijk
    public decimal? Code2654 { get; set; }
    public decimal? Code1674 { get; set; }   // baten overeenst. voorheen afgetrokken kosten overdracht
    public decimal? Code2674 { get; set; }

    // ── Vergoedingen afzonderlijk belastbaar ────────────────────────────────
    public decimal? Code1682 { get; set; }   // COVID-19 overbruggingsrecht 16,5%
    public decimal? Code2682 { get; set; }
    public decimal? Code1655 { get; set; }   // andere 16,5%
    public decimal? Code2655 { get; set; }
    public decimal? Code1667 { get; set; }   // 33%
    public decimal? Code2667 { get; set; }

    // ── Vergoedingen gezamenlijk ────────────────────────────────────────────
    public decimal? Code1683 { get; set; }   // COVID-19 overbruggingsrecht gezamenlijk
    public decimal? Code2683 { get; set; }
    public decimal? Code1661 { get; set; }   // andere gezamenlijk
    public decimal? Code2661 { get; set; }

    // ── Aftrekken ───────────────────────────────────────────────────────────
    public decimal? Code1656 { get; set; }   // sociale bijdragen
    public decimal? Code2656 { get; set; }
    public string Code1675 { get; set; } = string.Empty;   // begindatum beroepswerkzaamheid
    public string Code2675 { get; set; } = string.Empty;
    public decimal? Code1669 { get; set; }   // beroepskosten: bezoldigingen meewerkende echtgenoot
    public decimal? Code2669 { get; set; }
    public decimal? Code1657 { get; set; }   // andere beroepskosten
    public decimal? Code2657 { get; set; }

    // ── Vrijstellingen ──────────────────────────────────────────────────────
    public decimal? Code1681 { get; set; }   // sociaal passief eenheidsstatuut
    public decimal? Code2681 { get; set; }
    public decimal? Code1662 { get; set; }   // investeringsaftrek
    public decimal? Code2662 { get; set; }
    public decimal? Code1660 { get; set; }   // waardeverminderingen/voorzieningen
    public decimal? Code2660 { get; set; }
    public decimal? Code1664 { get; set; }   // bijkomend personeel
    public decimal? Code2664 { get; set; }
    public decimal? Code1665 { get; set; }   // stagebonus
    public decimal? Code2665 { get; set; }
    public decimal? Code1666 { get; set; }   // minnelijk akkoord/reorganisatieplan
    public decimal? Code2666 { get; set; }

    // ── Toekenningen ────────────────────────────────────────────────────────
    public decimal? Code1663 { get; set; }   // meewerkende echtgenoot
    public decimal? Code2663 { get; set; }
    public decimal? Code1668 { get; set; }   // bijberoep/student-zelfstandige
    public decimal? Code2668 { get; set; }
    public string Code1676 { get; set; } = string.Empty;   // einddatum beroepswerkzaamheid
    public string Code2676 { get; set; } = string.Empty;

    // ── Feitelijke vereniging ───────────────────────────────────────────────
    public bool Code1670 { get; set; }   // ja
    public bool Code1671 { get; set; }   // neen
    public decimal? Code2670 { get; set; }
    public bool Code2671 { get; set; }
    public bool Code1679 { get; set; }   // buitenlandse oorsprong ja
    public bool Code1680 { get; set; }   // buitenlandse oorsprong neen
    public decimal? Code2679 { get; set; }
    public decimal? Code2680 { get; set; }

    // ── Begin-/einddatum ────────────────────────────────────────────────────
    public string Code1672 { get; set; } = string.Empty;   // begindatum
    public string Code1673 { get; set; } = string.Empty;   // einddatum

    // ── Inkomsten buitenlandse oorsprong ─────────────────────────────────────
    public List<BuitenlandInkomen> BuitenlandseInkomsten { get; set; } = [new()];

    // ── Feitelijke vereniging details ────────────────────────────────────────
    public List<FeitelijkeVereniging> FeitelijkeVerenigingen { get; set; } = [new()];

    // ── Feitelijke vereniging tekstvelden ────────────────────────────────────
    public string FeitelijkeVerenigingNaam1 { get; set; } = string.Empty;
    public string FeitelijkeVerenigingNaam2 { get; set; } = string.Empty;
    public string FeitelijkeVerenigingAdres1 { get; set; } = string.Empty;
    public string FeitelijkeVerenigingAdres2 { get; set; } = string.Empty;

    // ── Verlies vereniging tekstvelden ─────────────────────────────────────
    public string VerliesVerenigingNaam1 { get; set; } = string.Empty;
    public string VerliesVerenigingNaam2 { get; set; } = string.Empty;
    public string VerliesVerenigingAdres1 { get; set; } = string.Empty;
    public string VerliesVerenigingAdres2 { get; set; } = string.Empty;
}
