namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK XVII – Winst uit Nijverheids-, Handels- of Landbouwondernemingen</summary>
public class VakXVIIData
{
    // ── Inkomsten ───────────────────────────────────────────────────────────
    public decimal? Code1600 { get; set; }   // brutowinst eigenlijke exploitatie
    public decimal? Code2600 { get; set; }
    public decimal? Code1601 { get; set; }   // voorheen vrijgestelde winst
    public decimal? Code2601 { get; set; }
    public decimal? Code1602 { get; set; }   // financiële opbrengsten
    public decimal? Code2602 { get; set; }
    public decimal? Code1603 { get; set; }   // meerwaarden 16,5%
    public decimal? Code2603 { get; set; }
    public decimal? Code1604 { get; set; }   // meerwaarden gezamenlijk
    public decimal? Code2604 { get; set; }
    public decimal? Code1615 { get; set; }   // winst overeenst. voorheen afgetrokken kosten overdracht
    public decimal? Code2615 { get; set; }

    // ── Vergoedingen afzonderlijk belastbaar ────────────────────────────────
    public decimal? Code1607 { get; set; }   // 12,5%
    public decimal? Code2607 { get; set; }
    public decimal? Code1636 { get; set; }   // COVID-19 overbruggingsrecht 16,5%
    public decimal? Code2636 { get; set; }
    public decimal? Code1605 { get; set; }   // andere 16,5%
    public decimal? Code2605 { get; set; }
    public decimal? Code1618 { get; set; }   // 33%
    public decimal? Code2618 { get; set; }

    // ── Vergoedingen gezamenlijk ────────────────────────────────────────────
    public decimal? Code1637 { get; set; }   // COVID-19 overbruggingsrecht gezamenlijk
    public decimal? Code2637 { get; set; }
    public decimal? Code1610 { get; set; }   // andere gezamenlijk
    public decimal? Code2610 { get; set; }

    // ── Aftrekken ───────────────────────────────────────────────────────────
    public decimal? Code1632 { get; set; }   // sociale bijdragen
    public decimal? Code2632 { get; set; }
    public decimal? Code1620 { get; set; }   // beroepskosten: overdracht activa
    public decimal? Code2620 { get; set; }
    public decimal? Code1611 { get; set; }   // beroepskosten: bezoldigingen meewerkende echtgenoot
    public decimal? Code2611 { get; set; }
    public decimal? Code1606 { get; set; }   // andere beroepskosten
    public decimal? Code2606 { get; set; }

    // ── Vrijstellingen ──────────────────────────────────────────────────────
    public decimal? Code1609 { get; set; }   // waardeverminderingen/voorzieningen
    public decimal? Code2609 { get; set; }
    public decimal? Code1608 { get; set; }   // minnelijk akkoord/reorganisatieplan
    public decimal? Code2608 { get; set; }
    public decimal? Code1612 { get; set; }   // bijkomend personeel
    public decimal? Code2612 { get; set; }
    public decimal? Code1633 { get; set; }   // sociaal passief eenheidsstatuut
    public decimal? Code2633 { get; set; }
    public decimal? Code1614 { get; set; }   // investeringsaftrek
    public decimal? Code2614 { get; set; }

    // ── Toekenningen ────────────────────────────────────────────────────────
    public decimal? Code1616 { get; set; }   // meewerkende echtgenoot
    public decimal? Code2616 { get; set; }
    public decimal? Code1617 { get; set; }   // bijberoep/student-zelfstandige
    public decimal? Code2617 { get; set; }
    public decimal? Code1621 { get; set; }   // hervatting na ontslag
    public decimal? Code2621 { get; set; }

    // ── Feitelijke vereniging ───────────────────────────────────────────────
    public string Code1625 { get; set; } = string.Empty;   // begindatum beroepswerkzaamheid
    public string Code1626 { get; set; } = string.Empty;   // einddatum beroepswerkzaamheid
    public string Code2625 { get; set; } = string.Empty;
    public string Code2626 { get; set; } = string.Empty;
    public bool Code1630 { get; set; }   // buitenlandse oorsprong ja
    public bool Code1631 { get; set; }   // buitenlandse oorsprong neen

    // ── Begin-/einddatum ────────────────────────────────────────────────────
    public string Code1627 { get; set; } = string.Empty;   // begindatum
    public string Code1628 { get; set; } = string.Empty;   // einddatum

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

    // ── Adres inrichting ──────────────────────────────────────────────────
    public string AdresInrichting1 { get; set; } = string.Empty;
    public string AdresInrichting2 { get; set; } = string.Empty;
}

/// <summary>Rij voor feitelijke vereniging (winst/baten vakken).</summary>
public class FeitelijkeVereniging
{
    public string Naam { get; set; } = string.Empty;
    public string Ondernemingsnummer { get; set; } = string.Empty;
    public decimal? Aandeel { get; set; }
}
