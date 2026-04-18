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

        // ── 1. Bruto-inkomsten Deel 1 ───────────────────────────────
        r.BrutoBeroepsinkomen = inkomen.BrutoBeroepsinkomen;
        r.BrutoPensioeninkomen = inkomen.BrutoPensioeninkomen;
        r.BrutoVervangingsinkomen = inkomen.BrutoVervangingsinkomen;

        r.DetailRegels.Add(new("Bruto beroepsinkomen", r.BrutoBeroepsinkomen));
        if (r.BrutoPensioeninkomen > 0)
            r.DetailRegels.Add(new("Bruto pensioeninkomen", r.BrutoPensioeninkomen));
        if (r.BrutoVervangingsinkomen > 0)
            r.DetailRegels.Add(new("Bruto vervangingsinkomen", r.BrutoVervangingsinkomen));

        // ── 2. Beroepskosten Deel 1 ─────────────────────────────────
        if (r.BrutoBeroepsinkomen > 0)
        {
            decimal? werkelijkeKosten = inkomen.WerkelijkeKosten > 0 ? inkomen.WerkelijkeKosten : null;
            (r.Beroepskosten, r.ForfaitaireBeroepskosten) = ForfaitaireBeroepskostenCalculator.Bereken(
                r.BrutoBeroepsinkomen, werkelijkeKosten, typeBeroep);
            r.DetailRegels.Add(new(
                r.ForfaitaireBeroepskosten ? "Forfaitaire beroepskosten" : "Werkelijke beroepskosten",
                -r.Beroepskosten));
        }

        decimal nettoDeel1 = Math.Max(
            r.BrutoBeroepsinkomen + r.BrutoPensioeninkomen + r.BrutoVervangingsinkomen - r.Beroepskosten, 0);

        // ── 3. Deel 2: gezamenlijk belastbare inkomsten ─────────────
        decimal nettoDeel2 = 0;

        // Bedrijfsleider (forfait 3%, max €3.130)
        r.BrutoBedrijfsleider = inkomen.BrutoBedrijfsleider;
        if (r.BrutoBedrijfsleider > 0)
        {
            decimal basisBL = r.BrutoBedrijfsleider - inkomen.BedrijfsleiderSocialeBijdragen;
            decimal? werkKostenBL = inkomen.BedrijfsleiderWerkelijkeKosten > 0
                ? inkomen.BedrijfsleiderWerkelijkeKosten : null;
            (r.BedrijfsleiderKosten, r.BedrijfsleiderForfaitair) =
                ForfaitaireBeroepskostenCalculator.Bereken(Math.Max(basisBL, 0), werkKostenBL, TypeBeroep.Bedrijfsleider);
            decimal nettoBL = Math.Max(basisBL - r.BedrijfsleiderKosten, 0);
            nettoDeel2 += nettoBL;

            r.DetailRegels.Add(new("Bedrijfsleider bruto", r.BrutoBedrijfsleider));
            r.DetailRegels.Add(new(r.BedrijfsleiderForfaitair ? "  Forfait 3%" : "  Werkelijke kosten",
                -r.BedrijfsleiderKosten - inkomen.BedrijfsleiderSocialeBijdragen));
        }

        // Winst zelfstandige (forfait 30%, max €5.930)
        r.BrutoWinstZelfstandige = inkomen.BrutoWinstZelfstandige;
        if (r.BrutoWinstZelfstandige > 0)
        {
            decimal basisW = r.BrutoWinstZelfstandige - inkomen.WinstSocialeBijdragen;
            decimal werkelijkeKostenW = inkomen.WinstBeroepskosten + inkomen.WinstVrijstellingen;
            // Zelfstandigen: werkelijke kosten of forfait 30%
            decimal forfaitW = ForfaitaireBeroepskostenCalculator.BerekenForfait(
                Math.Max(basisW, 0), TypeBeroep.Werknemer); // 30% zelfde als werknemer
            if (werkelijkeKostenW > forfaitW)
                r.WinstKosten = werkelijkeKostenW;
            else
                r.WinstKosten = forfaitW;
            decimal nettoW = Math.Max(basisW - r.WinstKosten, 0);
            nettoDeel2 += nettoW;

            r.DetailRegels.Add(new("Winst zelfstandige bruto", r.BrutoWinstZelfstandige));
            r.DetailRegels.Add(new("  Kosten + vrijstellingen",
                -r.WinstKosten - inkomen.WinstSocialeBijdragen));
        }

        // Baten vrij beroep (getrapt forfait: 28,7%/10%/5%/3%, max €5.210)
        r.BrutoBatenVrijBeroep = inkomen.BrutoBatenVrijBeroep;
        if (r.BrutoBatenVrijBeroep > 0)
        {
            decimal basisB = r.BrutoBatenVrijBeroep - inkomen.BatenSocialeBijdragen;
            decimal werkelijkeKostenB = inkomen.BatenBeroepskosten + inkomen.BatenVrijstellingen;
            decimal? werkKostenB = werkelijkeKostenB > 0 ? werkelijkeKostenB : null;
            (r.BatenKosten, r.BatenForfaitair) =
                ForfaitaireBeroepskostenCalculator.Bereken(Math.Max(basisB, 0), werkKostenB, TypeBeroep.Baten);
            decimal nettoB = Math.Max(basisB - r.BatenKosten, 0);
            nettoDeel2 += nettoB;

            r.DetailRegels.Add(new("Baten vrij beroep bruto", r.BrutoBatenVrijBeroep));
            r.DetailRegels.Add(new(r.BatenForfaitair ? "  Forfait baten" : "  Werkelijke kosten",
                -r.BatenKosten - inkomen.BatenSocialeBijdragen));
        }

        // Meewerkende echtgenoot (forfait 5%, max €5.210)
        r.BrutoMeewerkend = inkomen.BrutoMeewerkend;
        if (r.BrutoMeewerkend > 0)
        {
            decimal basisM = r.BrutoMeewerkend - inkomen.MeewerkendSocialeBijdragen;
            decimal? werkKostenM = inkomen.MeewerkendBeroepskosten > 0
                ? inkomen.MeewerkendBeroepskosten : null;
            (r.MeewerkendKosten, r.MeewerkendForfaitair) =
                ForfaitaireBeroepskostenCalculator.Bereken(Math.Max(basisM, 0), werkKostenM, TypeBeroep.MeewerkendeEchtgenoot);
            decimal nettoM = Math.Max(basisM - r.MeewerkendKosten, 0);
            nettoDeel2 += nettoM;

            r.DetailRegels.Add(new("Meewerkende echtgenoot bruto", r.BrutoMeewerkend));
            r.DetailRegels.Add(new(r.MeewerkendForfaitair ? "  Forfait 5%" : "  Werkelijke kosten",
                -r.MeewerkendKosten - inkomen.MeewerkendSocialeBijdragen));
        }

        // Diverse inkomsten gezamenlijk (geen forfait, direct belastbaar)
        r.BrutoDiverseGezamenlijk = inkomen.DiverseInkomstenGezamenlijk;
        if (r.BrutoDiverseGezamenlijk > 0)
        {
            nettoDeel2 += r.BrutoDiverseGezamenlijk;
            r.DetailRegels.Add(new("Diverse inkomsten (gezamenlijk)", r.BrutoDiverseGezamenlijk));
        }

        // ── 4. Afzonderlijk belastbaar (flat rates) ─────────────────
        r.Afzonderlijk10Pct = inkomen.Afzonderlijk10Pct;
        r.Afzonderlijk12_5Pct = inkomen.Afzonderlijk12_5Pct;
        r.Afzonderlijk16_5Pct = inkomen.Afzonderlijk16_5Pct
                               + inkomen.DiverseInkomsten16_5Pct;
        r.Afzonderlijk33Pct = inkomen.Afzonderlijk33Pct
                             + inkomen.DiverseInkomsten33Pct;

        r.BelastingAfzonderlijk =
            r.Afzonderlijk10Pct * TaxConstants2026.Tarief10Procent
            + r.Afzonderlijk12_5Pct * TaxConstants2026.Tarief12_5Procent
            + r.Afzonderlijk16_5Pct * TaxConstants2026.Tarief16_5Procent
            + r.Afzonderlijk33Pct * TaxConstants2026.Tarief33Procent;

        if (r.BelastingAfzonderlijk > 0)
        {
            r.DetailRegels.Add(new("Afzonderlijk belastbaar", r.BelastingAfzonderlijk));
            if (r.Afzonderlijk16_5Pct > 0)
                r.DetailRegels.Add(new($"  16,5% op {r.Afzonderlijk16_5Pct:N2}",
                    r.Afzonderlijk16_5Pct * TaxConstants2026.Tarief16_5Procent));
            if (r.Afzonderlijk33Pct > 0)
                r.DetailRegels.Add(new($"  33% op {r.Afzonderlijk33Pct:N2}",
                    r.Afzonderlijk33Pct * TaxConstants2026.Tarief33Procent));
        }

        // ── 5. Netto belastbaar inkomen (Deel 1 + Deel 2 gezamenlijk) ─
        r.BrutoTotaal = inkomen.BrutoTotaal;
        r.NettoBelastbaarInkomen = nettoDeel1 + nettoDeel2;
        r.NettoBelastbaarNaHQ = r.NettoBelastbaarInkomen;
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

        // Afzonderlijke belasting toevoegen (buiten progressief systeem)
        r.TotaleBelasting = r.SaldoFederaal + r.SaldoGewestelijk + r.BelastingAfzonderlijk;

        // ── 9. Voorheffingen & kredieten ────────────────────────────────
        r.Bedrijfsvoorheffing = inkomen.Bedrijfsvoorheffing
                              + inkomen.BedrijfsleiderBedrijfsvoorheffing
                              + inkomen.Deel2Bedrijfsvoorheffing;

        // Werkbonus belastingkrediet: 33,14% × Code284 + 52,54% × Code360, begrensd op maximum
        decimal werkbonusKrediet = inkomen.WerkbonusCode284 * TaxConstants2026.WerkbonusPercentage3314
                                 + inkomen.WerkbonusCode360 * TaxConstants2026.WerkbonusPercentage5254
                                 + inkomen.BedrijfsleiderWerkbonus284 * TaxConstants2026.WerkbonusPercentage3314
                                 + inkomen.BedrijfsleiderWerkbonus360 * TaxConstants2026.WerkbonusPercentage5254;
        r.BelastingkredietWerkbonus = Math.Min(werkbonusKrediet, TaxConstants2026.MaxBelastingkredietWerkbonus);

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
