namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent de volledige belasting voor één partner (één kolom van het aanslagbiljet).
/// </summary>
public static class PartnerBelastingCalculator
{
    /// <summary>
    /// Voert de volledige berekening uit voor één partner.
    /// </summary>
    public static PartnerResultaat Bereken(
        PartnerInkomen inkomen,
        decimal belastingvrijeSom,
        TypeBeroep typeBeroep,
        Gewest gewest)
    {
        var r = new PartnerResultaat { Label = inkomen.Label };

        // ── 1. Bruto-inkomsten ──────────────────────────────────────────
        r.BrutoBeroepsinkomen = inkomen.BrutoBeroepsinkomen;
        r.BrutoPensioeninkomen = inkomen.BrutoPensioeninkomen;
        r.BrutoVervangingsinkomen = inkomen.BrutoVervangingsinkomen;
        r.BrutoTotaal = inkomen.BrutoTotaal;

        r.DetailRegels.Add(new("Bruto beroepsinkomen", r.BrutoBeroepsinkomen));
        if (r.BrutoPensioeninkomen > 0)
            r.DetailRegels.Add(new("Bruto pensioeninkomen", r.BrutoPensioeninkomen));
        if (r.BrutoVervangingsinkomen > 0)
            r.DetailRegels.Add(new("Bruto vervangingsinkomen", r.BrutoVervangingsinkomen));

        // ── 2. Beroepskosten ────────────────────────────────────────────
        if (r.BrutoBeroepsinkomen > 0)
        {
            decimal? werkelijkeKosten = inkomen.WerkelijkeKosten > 0 ? inkomen.WerkelijkeKosten : null;
            (r.Beroepskosten, r.ForfaitaireBeroepskosten) = ForfaitaireBeroepskostenCalculator.Bereken(
                r.BrutoBeroepsinkomen, werkelijkeKosten, typeBeroep);
            r.DetailRegels.Add(new(
                r.ForfaitaireBeroepskosten ? "Forfaitaire beroepskosten" : "Werkelijke beroepskosten",
                -r.Beroepskosten));
        }

        // ── 3. Netto belastbaar inkomen ─────────────────────────────────
        r.NettoBelastbaarInkomen = Math.Max(r.BrutoTotaal - r.Beroepskosten, 0);
        r.NettoBelastbaarNaHQ = r.NettoBelastbaarInkomen; // wordt later aangepast bij HQ
        r.DetailRegels.Add(new("Netto belastbaar", r.NettoBelastbaarInkomen, true));

        return r;
    }

    /// <summary>
    /// Tweede fase: berekent de belasting na HQ-toewijzing.
    /// Moet worden aangeroepen nadat HQ is berekend en NettoBelastbaarNaHQ is ingevuld.
    /// </summary>
    public static void BerekenBelasting(
        PartnerResultaat r,
        PartnerInkomen inkomen,
        decimal belastingvrijeSom,
        Gewest gewest)
    {
        // ── 4. Basisbelasting (progressief) ─────────────────────────────
        r.Basisbelasting = BelastingschijvenCalculator.BerekenBelasting(r.NettoBelastbaarNaHQ);
        r.DetailRegels.Add(new("Basisbelasting", r.Basisbelasting));

        // ── 5. Belastingvrije som ───────────────────────────────────────
        r.BelastingvrijeSom = belastingvrijeSom;
        r.VerminderingBelastingvrijeSom = BelastingschijvenCalculator.BerekenVerminderingVrijeSom(belastingvrijeSom);
        r.DetailRegels.Add(new($"Belastingvrije som ({belastingvrijeSom:N0})", -r.VerminderingBelastingvrijeSom));

        r.OmTeSlane = Math.Max(r.Basisbelasting - r.VerminderingBelastingvrijeSom, 0);
        r.DetailRegels.Add(new("Om te slane belasting", r.OmTeSlane, true));

        // ── 6. Vermindering vervangingsinkomsten ────────────────────────
        if (inkomen.BrutoPensioeninkomen > 0 || inkomen.Werkloosheid > 0 || inkomen.ZiekteInvaliditeit > 0)
        {
            r.VerminderingVervangingsinkomen = VervangingsInkomstenCalculator.Bereken(
                r.NettoBelastbaarInkomen,
                inkomen.BrutoPensioeninkomen,
                inkomen.Werkloosheid,
                inkomen.ZiekteInvaliditeit);

            // Vermindering begrensd op om te slane
            r.VerminderingVervangingsinkomen = Math.Min(r.VerminderingVervangingsinkomen, r.OmTeSlane);

            if (r.VerminderingVervangingsinkomen > 0)
                r.DetailRegels.Add(new("Vermindering vervangingsinkomen", -r.VerminderingVervangingsinkomen));
        }

        r.Hoofdsom = Math.Max(r.OmTeSlane - r.VerminderingVervangingsinkomen, 0);

        // ── 7. Federaal / gewestelijk split ─────────────────────────────
        r.GereduceerdeStaat = r.Hoofdsom * (1m - TaxConstants2026.Autonomiefactor);

        decimal opcentiemenPct = gewest switch
        {
            Gewest.Vlaanderen => TaxConstants2026.OpcentiemenVlaanderen,
            Gewest.Wallonie => TaxConstants2026.OpcentiemenWallonie,
            Gewest.Brussel => TaxConstants2026.OpcentiemenBrussel,
            _ => TaxConstants2026.OpcentiemenVlaanderen,
        };
        r.GewestelijkeOpcentiemen = r.GereduceerdeStaat * opcentiemenPct;

        r.DetailRegels.Add(new("Gereduceerde belasting Staat", r.GereduceerdeStaat));
        r.DetailRegels.Add(new($"Gewestelijke opcentiemen ({gewest})", r.GewestelijkeOpcentiemen));

        // ── 8. Federale verminderingen ──────────────────────────────────
        r.FederaleVerminderingen = BerekenFederaleVerminderingen(inkomen);

        if (r.FederaleVerminderingen > 0)
            r.DetailRegels.Add(new("Federale verminderingen", -r.FederaleVerminderingen));

        r.SaldoFederaal = Math.Max(r.GereduceerdeStaat - r.FederaleVerminderingen, 0);
        r.SaldoGewestelijk = r.GewestelijkeOpcentiemen;
        r.TotaleBelasting = r.SaldoFederaal + r.SaldoGewestelijk;

        // ── 9. Voorheffingen & kredieten ────────────────────────────────
        r.Bedrijfsvoorheffing = inkomen.Bedrijfsvoorheffing;
        r.BelastingkredietWerkbonus = Math.Min(inkomen.WerkbonusBedrag, TaxConstants2026.MaxBelastingkredietWerkbonus);

        r.SaldoFederaalNaBVEnKredieten = r.SaldoFederaal - r.Bedrijfsvoorheffing - r.BelastingkredietWerkbonus;
    }

    private static decimal BerekenFederaleVerminderingen(PartnerInkomen inkomen)
    {
        decimal totaal = 0;

        // Kinderopvang (45%)
        if (inkomen.Kinderopvang > 0)
            totaal += inkomen.Kinderopvang * 0.45m;

        // Pensioensparen
        if (inkomen.Pensioensparen > 0)
        {
            if (inkomen.Pensioensparen <= TaxConstants2026.MaxPensioensparen30)
                totaal += inkomen.Pensioensparen * 0.30m;
            else
                totaal += Math.Min(inkomen.Pensioensparen, TaxConstants2026.MaxPensioensparen25) * 0.25m;
        }

        // Giften (30%)
        if (inkomen.Giften >= TaxConstants2026.MinGift)
            totaal += Math.Min(inkomen.Giften, TaxConstants2026.MaxGift) * TaxConstants2026.PercentageGiften;

        return totaal;
    }
}
