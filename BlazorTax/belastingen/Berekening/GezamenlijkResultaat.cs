namespace BlazorTax.Belastingen.Berekening;

/// <summary>Volledig berekeningsresultaat voor één partner (één kolom).</summary>
public class PartnerResultaat
{
    public string Label { get; set; } = "";

    // Bruto
    public decimal BrutoBeroepsinkomen { get; set; }
    public decimal BrutoPensioeninkomen { get; set; }
    public decimal BrutoVervangingsinkomen { get; set; }
    public decimal BrutoTotaal { get; set; }

    // Deel 2 bruto
    public decimal BrutoBedrijfsleider { get; set; }
    public decimal BrutoWinstZelfstandige { get; set; }
    public decimal BrutoBatenVrijBeroep { get; set; }
    public decimal BrutoMeewerkend { get; set; }
    public decimal BrutoDiverseGezamenlijk { get; set; }

    // Kosten
    public decimal Beroepskosten { get; set; }
    public bool ForfaitaireBeroepskosten { get; set; }

    // Deel 2 kosten
    public decimal BedrijfsleiderKosten { get; set; }
    public bool BedrijfsleiderForfaitair { get; set; }
    public decimal WinstKosten { get; set; }
    public decimal BatenKosten { get; set; }
    public bool BatenForfaitair { get; set; }
    public decimal MeewerkendKosten { get; set; }
    public bool MeewerkendForfaitair { get; set; }

    // Netto
    public decimal NettoBelastbaarInkomen { get; set; }

    // Na huwelijksquotiënt
    public decimal HuwelijksquotientOntvangen { get; set; }
    public decimal HuwelijksquotientAfgestaan { get; set; }
    public decimal NettoBelastbaarNaHQ { get; set; }

    // Belasting
    public decimal Basisbelasting { get; set; }
    public decimal BelastingvrijeSom { get; set; }
    public decimal VerminderingBelastingvrijeSom { get; set; }
    public decimal OmTeSlane { get; set; }

    // Verminderingen
    public decimal VerminderingVervangingsinkomen { get; set; }
    public decimal Hoofdsom { get; set; }

    // Federaal / gewestelijk
    public decimal GereduceerdeStaat { get; set; }
    public decimal FederaleVerminderingen { get; set; }
    public decimal SaldoFederaal { get; set; }
    public decimal GewestelijkeOpcentiemen { get; set; }

    // Totaal
    public decimal TotaleBelasting { get; set; }

    // Afzonderlijk belastbaar (Deel 2)
    public decimal Afzonderlijk10Pct { get; set; }
    public decimal Afzonderlijk12_5Pct { get; set; }
    public decimal Afzonderlijk16_5Pct { get; set; }
    public decimal Afzonderlijk33Pct { get; set; }
    public decimal BelastingAfzonderlijk { get; set; }

    // Voorheffingen & kredieten
    public decimal Bedrijfsvoorheffing { get; set; }
    public decimal BelastingkredietWerkbonus { get; set; }
    public decimal SaldoFederaalNaBVEnKredieten { get; set; }
    public decimal SaldoGewestelijk { get; set; }

    // Detailregels
    public List<BerekeningRegel> DetailRegels { get; set; } = [];
}

/// <summary>
/// Volledig resultaat van een gemeenschappelijke aanslag met twee kolommen.
/// </summary>
public class GezamenlijkResultaat
{
    public PartnerResultaat Belastingplichtige { get; set; } = new();
    public PartnerResultaat Partner { get; set; } = new();

    /// <summary>Is dit een gemeenschappelijke aanslag (twee kolommen)?</summary>
    public bool IsGezamenlijk { get; set; }

    // Gecombineerde totalen
    public decimal TotaalSaldoFederaal { get; set; }
    public decimal TotaalSaldoGewestelijk { get; set; }
    public decimal BasisGemeentebelasting { get; set; }
    public decimal GemeentebelastingPercentage { get; set; }
    public decimal Gemeentebelasting { get; set; }

    // BBSZ
    public decimal BBSZGezamenlijkInkomen { get; set; }
    public decimal BBSZVerschuldigd { get; set; }
    public decimal BBSZIngehouden { get; set; }
    public decimal BBSZSaldo { get; set; }

    // Eindresultaat (positief = te betalen, negatief = terug)
    public decimal Eindresultaat { get; set; }

    // Gecombineerde detailregels voor weergave
    public List<BerekeningRegel> DetailRegels { get; set; } = [];
}
