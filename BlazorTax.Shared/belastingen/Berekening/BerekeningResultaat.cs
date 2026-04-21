namespace BlazorTax.Belastingen.Berekening;

/// <summary>Gewest van de belastingplichtige.</summary>
public enum Gewest
{
    Vlaanderen,
    Wallonie,
    Brussel,
}

/// <summary>Volledig resultaat van de personenbelastingberekening AJ2026.</summary>
public class BerekeningResultaat
{
    // ── Bruto-inkomsten ─────────────────────────────────────────────────────
    public decimal BrutoBeroepsinkomen { get; set; }
    public decimal BrutoPensioeninkomen { get; set; }
    public decimal BrutoVervangingsinkomen { get; set; }
    public decimal BrutoOnroerendInkomen { get; set; }
    public decimal BrutoRoerendInkomen { get; set; }
    public decimal BrutoDiverseInkomsten { get; set; }

    // ── Beroepskosten ───────────────────────────────────────────────────────
    public decimal Beroepskosten { get; set; }
    public bool ForfaitaireBeroepskosten { get; set; }

    // ── Netto belastbaar inkomen ────────────────────────────────────────────
    public decimal NettoBelastbaarInkomen { get; set; }
    public decimal HuwelijksquotientOvergedragen { get; set; }
    public decimal NettoBelastbaarInkomenNaQuotient { get; set; }

    // ── Basisbelasting ──────────────────────────────────────────────────────
    public decimal BasisbelastingOpInkomen { get; set; }
    public decimal BasisbelastingOpQuotient { get; set; }
    public decimal BasisbelastingTotaal { get; set; }

    // ── Belastingvrije som ──────────────────────────────────────────────────
    public decimal BelastingvrijeSomTotaal { get; set; }
    public decimal VerminderingBelastingvrijeSom { get; set; }

    // ── Belasting na vermindering vrije som ─────────────────────────────────
    public decimal BelastingNaVrijeSom { get; set; }

    // ── Vermindering vervangingsinkomsten ───────────────────────────────────
    public decimal VerminderingVervangingsinkomen { get; set; }

    // ── Belasting vóór opsplitsing ──────────────────────────────────────────
    public decimal BelastingVoorOpsplitsing { get; set; }

    // ── Federale / gewestelijke opsplitsing ─────────────────────────────────
    public decimal FederaleBelasting { get; set; }
    public decimal GewestelijkeBelasting { get; set; }

    // ── Federale verminderingen ─────────────────────────────────────────────
    public decimal FederaleVerminderingen { get; set; }
    public decimal FederaleBelastingNaVerminderingen { get; set; }

    // ── Gewestelijke verminderingen ─────────────────────────────────────────
    public decimal GewestelijkeVerminderingen { get; set; }
    public decimal GewestelijkeBelastingNaVerminderingen { get; set; }

    // ── Gemeentelijke opcentiemen ───────────────────────────────────────────
    public decimal GemeentelijkeOpcentiemen { get; set; }
    public decimal GemeentebelastingPercentage { get; set; }

    // ── Voorheffingen en voorafbetalingen ───────────────────────────────────
    public decimal Bedrijfsvoorheffing { get; set; }
    public decimal Voorafbetalingen { get; set; }
    public decimal BijzondereBijdrageSocialeZekerheid { get; set; }

    // ── Belastingkredieten ──────────────────────────────────────────────────
    public decimal BelastingkredietWerkbonus { get; set; }
    public decimal BelastingkredietKinderen { get; set; }

    // ── Eindresultaat ───────────────────────────────────────────────────────
    /// <summary>Positief = te betalen, negatief = terug te krijgen.</summary>
    public decimal Eindresultaat { get; set; }

    // ── Detailregels voor weergave ──────────────────────────────────────────
    public List<BerekeningRegel> DetailRegels { get; set; } = [];
}

/// <summary>Eén regel in de detailopbouw van de berekening.</summary>
public record BerekeningRegel(string Omschrijving, decimal Bedrag, bool IsSubtotaal = false);
