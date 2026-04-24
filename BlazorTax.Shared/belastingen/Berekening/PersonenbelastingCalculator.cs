namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Orchestrator die alle deelberekeningen samenvoegt tot een volledig
/// personenbelastingresultaat voor AJ2026 (inkomsten 2025).
/// </summary>
public class PersonenbelastingCalculator
{
    /// <summary>
    /// Voert de volledige berekening uit op basis van de ingevulde vakken.
    /// </summary>
    public BerekeningResultaat Bereken(BerekeningInput input)
    {
        var r = new BerekeningResultaat();

        // ── 1. Bruto-inkomsten optellen ─────────────────────────────────────
        r.BrutoBeroepsinkomen = BerekenBrutoBeroepsinkomen(input.VakIV);
        r.BrutoPensioeninkomen = BerekenBrutoPensioen(input.VakV);
        r.BrutoVervangingsinkomen = BerekenBrutoVervangingsinkomen(input.VakIV);
        r.BrutoOnroerendInkomen = BerekenOnroerendInkomen(input.VakIII);

        decimal brutoTotaal = r.BrutoBeroepsinkomen
                            + r.BrutoPensioeninkomen
                            + r.BrutoVervangingsinkomen
                            + r.BrutoOnroerendInkomen;

        r.DetailRegels.Add(new("Bruto beroepsinkomen", r.BrutoBeroepsinkomen));
        r.DetailRegels.Add(new("Bruto pensioeninkomen", r.BrutoPensioeninkomen));
        r.DetailRegels.Add(new("Bruto vervangingsinkomen", r.BrutoVervangingsinkomen));
        r.DetailRegels.Add(new("Bruto onroerend inkomen", r.BrutoOnroerendInkomen));
        r.DetailRegels.Add(new("Bruto totaal inkomen", brutoTotaal, true));

        // ── 2. Beroepskosten (alleen op beroepsinkomen, niet op pensioen/vervangingsinkomen) ──
        decimal kosten = 0;
        bool isForfaitair = false;

        if (r.BrutoBeroepsinkomen > 0)
        {
            decimal? werkelijkeKosten = (input.VakIV.Code1258 ?? 0) + (input.VakIV.Code2258 ?? 0);
            if (werkelijkeKosten == 0) werkelijkeKosten = null;

            (kosten, isForfaitair) = ForfaitaireBeroepskostenCalculator.Bereken(
                r.BrutoBeroepsinkomen, werkelijkeKosten, input.TypeBeroep);
        }

        r.Beroepskosten = kosten;
        r.ForfaitaireBeroepskosten = isForfaitair;

        if (kosten > 0)
            r.DetailRegels.Add(new(isForfaitair ? "Forfaitaire beroepskosten (30%)" : "Werkelijke beroepskosten", -kosten));

        // ── 3. Vorige beroepsverliezen (art. 23 §1 WIB92) ──────────────────
        decimal vorigeVerliezen = (input.VakIV.Code1349 ?? 0) + (input.VakIV.Code2349 ?? 0);
        decimal nettoNaKosten = Math.Max(brutoTotaal - kosten, 0);
        decimal toegepastVerliezen = Math.Min(vorigeVerliezen, nettoNaKosten);
        if (toegepastVerliezen > 0)
            r.DetailRegels.Add(new("Vorige beroepsverliezen", -toegepastVerliezen));

        // ── 4. Netto belastbaar inkomen ─────────────────────────────────────
        r.NettoBelastbaarInkomen = Math.Max(nettoNaKosten - toegepastVerliezen, 0);
        r.DetailRegels.Add(new("Netto belastbaar inkomen", r.NettoBelastbaarInkomen, true));

        // ── 4. Huwelijksquotiënt ────────────────────────────────────────────
        bool isGehuwd = input.VakII.BurgerlijkeStaat == "1002";
        r.HuwelijksquotientOvergedragen = HuwelijksquotientCalculator.Bereken(
            r.NettoBelastbaarInkomen, input.NettoInkomenPartner, isGehuwd);

        r.NettoBelastbaarInkomenNaQuotient = r.NettoBelastbaarInkomen - r.HuwelijksquotientOvergedragen;

        if (r.HuwelijksquotientOvergedragen > 0)
        {
            r.DetailRegels.Add(new("Huwelijksquotiënt overgedragen", -r.HuwelijksquotientOvergedragen));
            r.DetailRegels.Add(new("Netto belastbaar na quotiënt", r.NettoBelastbaarInkomenNaQuotient, true));
        }

        // ── 5. Basisbelasting ───────────────────────────────────────────────
        r.BasisbelastingOpInkomen = BelastingschijvenCalculator.BerekenBelasting(r.NettoBelastbaarInkomenNaQuotient);
        r.BasisbelastingOpQuotient = BelastingschijvenCalculator.BerekenBelasting(r.HuwelijksquotientOvergedragen);
        r.BasisbelastingTotaal = r.BasisbelastingOpInkomen + r.BasisbelastingOpQuotient;

        r.DetailRegels.Add(new("Basisbelasting (progressief)", r.BasisbelastingTotaal));

        // ── 6. Belastingvrije som ───────────────────────────────────────────
        r.BelastingvrijeSomTotaal = BelastingvrijeSomCalculator.Bereken(input.VakII);
        r.VerminderingBelastingvrijeSom = BelastingschijvenCalculator.BerekenVerminderingVrijeSom(r.BelastingvrijeSomTotaal);

        r.DetailRegels.Add(new($"Belastingvrije som ({r.BelastingvrijeSomTotaal:N2} €)", -r.VerminderingBelastingvrijeSom));

        r.BelastingNaVrijeSom = Math.Max(r.BasisbelastingTotaal - r.VerminderingBelastingvrijeSom, 0);
        r.DetailRegels.Add(new("Belasting na vrije som", r.BelastingNaVrijeSom, true));

        // ── 7. Vermindering vervangingsinkomsten ────────────────────────────
        r.VerminderingVervangingsinkomen = VervangingsInkomstenCalculator.Bereken(
            r.NettoBelastbaarInkomen,
            r.BrutoPensioeninkomen,
            BerekenWerkloosheidsInkomen(input.VakIV),
            BerekenZiekteInkomen(input.VakIV));

        if (r.VerminderingVervangingsinkomen > 0)
            r.DetailRegels.Add(new("Vermindering vervangingsinkomen", -r.VerminderingVervangingsinkomen));

        r.BelastingVoorOpsplitsing = Math.Max(r.BelastingNaVrijeSom - r.VerminderingVervangingsinkomen, 0);

        // ── 8. Federale belastingverminderingen ─────────────────────────────
        r.FederaleVerminderingen = BerekenFederaleVerminderingen(input.VakX);

        if (r.FederaleVerminderingen > 0)
            r.DetailRegels.Add(new("Federale verminderingen", -r.FederaleVerminderingen));

        // ── 9. Opsplitsing federaal / gewestelijk + opcentiemen ─────────────
        decimal belastingNaVerminderingen = Math.Max(r.BelastingVoorOpsplitsing - r.FederaleVerminderingen, 0);

        var (federaal, gewestelijk, gemeentelijk) = GemeentelijkeOpcentiemenCalculator.Bereken(
            belastingNaVerminderingen, input.Gewest, input.GemeentebelastingPercentage);

        r.FederaleBelasting = federaal;
        r.FederaleBelastingNaVerminderingen = federaal;
        r.GewestelijkeBelasting = gewestelijk;
        r.GewestelijkeBelastingNaVerminderingen = gewestelijk;
        r.GemeentelijkeOpcentiemen = gemeentelijk;
        r.GemeentebelastingPercentage = input.GemeentebelastingPercentage;

        r.DetailRegels.Add(new("Gereduceerde belasting Staat (federaal)", federaal));
        r.DetailRegels.Add(new($"Gewestelijke opcentiemen ({input.Gewest})", gewestelijk));
        r.DetailRegels.Add(new($"Gemeentebelasting ({input.GemeentebelastingPercentage}%)", gemeentelijk));

        decimal totaleBelasting = federaal + gewestelijk + gemeentelijk;
        r.DetailRegels.Add(new("Totale belasting", totaleBelasting, true));

        // ── 10. Voorheffingen en voorafbetalingen ───────────────────────────
        r.Bedrijfsvoorheffing = BerekenBedrijfsvoorheffing(input.VakIV, input.VakV);
        r.BijzondereBijdrageSocialeZekerheid = (input.VakIV.Code1287 ?? 0) + (input.VakIV.Code2287 ?? 0);
        r.Voorafbetalingen = (input.VakXII.Code1570 ?? 0) + (input.VakXII.Code2570 ?? 0);

        decimal totaalVoorheffingen = r.Bedrijfsvoorheffing
                                    + r.BijzondereBijdrageSocialeZekerheid
                                    + r.Voorafbetalingen;

        if (r.Bedrijfsvoorheffing > 0)
            r.DetailRegels.Add(new("Bedrijfsvoorheffing", -r.Bedrijfsvoorheffing));
        if (r.BijzondereBijdrageSocialeZekerheid > 0)
            r.DetailRegels.Add(new("Bijzondere bijdrage SZ", -r.BijzondereBijdrageSocialeZekerheid));
        if (r.Voorafbetalingen > 0)
            r.DetailRegels.Add(new("Voorafbetalingen", -r.Voorafbetalingen));

        // ── 11. Belastingkredieten ──────────────────────────────────────────
        r.BelastingkredietWerkbonus = BerekenWerkbonus(input.VakIV);
        if (r.BelastingkredietWerkbonus > 0)
            r.DetailRegels.Add(new("Belastingkrediet werkbonus", -r.BelastingkredietWerkbonus));

        // ── 12. Eindresultaat ───────────────────────────────────────────────
        r.Eindresultaat = totaleBelasting
                        - totaalVoorheffingen
                        - r.BelastingkredietWerkbonus
                        - r.BelastingkredietKinderen;

        r.DetailRegels.Add(new(
            r.Eindresultaat >= 0 ? "Te betalen" : "Terug te krijgen",
            r.Eindresultaat, true));

        return r;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static decimal BerekenBrutoBeroepsinkomen(VakIVData vak)
    {
        return vak.Code1250 + vak.Code2250
             + (vak.Code1251 ?? 0) + (vak.Code2251 ?? 0)
             + (vak.Code1252 ?? 0) + (vak.Code2252 ?? 0)
             + (vak.Code1247 ?? 0) + (vak.Code2247 ?? 0)
             + (vak.Code1262 ?? 0) + (vak.Code2262 ?? 0)
             + (vak.Code1267 ?? 0) + (vak.Code2267 ?? 0)
             // Woon-werkverkeer netto (totaal - vrijstelling)
             + Math.Max((vak.Code1254 ?? 0) - (vak.Code1255 ?? 0), 0)
             + Math.Max((vak.Code2254 ?? 0) - (vak.Code2255 ?? 0), 0);
    }

    private static decimal BerekenBrutoPensioen(VakVData vak)
    {
        return (vak.Code1228 ?? 0) + (vak.Code2228 ?? 0)  // wettelijk pensioen
             + (vak.Code1229 ?? 0) + (vak.Code2229 ?? 0)  // overlevingspensioen
             + (vak.Code1211 ?? 0) + (vak.Code2211 ?? 0)  // andere pensioenen
             + (vak.Code1314 ?? 0) + (vak.Code2314 ?? 0)  // december uitkering
             + (vak.Code1230 ?? 0) + (vak.Code2230 ?? 0)  // achterstallen
             + (vak.Code1231 ?? 0) + (vak.Code2231 ?? 0)
             + (vak.Code1212 ?? 0) + (vak.Code2212 ?? 0);
    }

    private static decimal BerekenBrutoVervangingsinkomen(VakIVData vak)
    {
        // Werkloosheid + ziekte + andere vervangingsinkomsten
        return BerekenWerkloosheidsInkomen(vak)
             + BerekenZiekteInkomen(vak)
             + (vak.Code1271 ?? 0) + (vak.Code2271 ?? 0)   // andere
             + (vak.Code1270 ?? 0) + (vak.Code2270 ?? 0);  // beroepsziekte
    }

    private static decimal BerekenWerkloosheidsInkomen(VakIVData vak)
    {
        return (vak.Code1260 ?? 0) + (vak.Code2260 ?? 0)
             + (vak.Code1264 ?? 0) + (vak.Code2264 ?? 0)
             + (vak.Code1261 ?? 0) + (vak.Code2261 ?? 0)
             + (vak.Code1265 ?? 0) + (vak.Code2265 ?? 0);
    }

    private static decimal BerekenZiekteInkomen(VakIVData vak)
    {
        return (vak.Code1266 ?? 0) + (vak.Code2266 ?? 0)
             + (vak.Code1303 ?? 0) + (vak.Code2303 ?? 0)
             + (vak.Code1268 ?? 0) + (vak.Code2268 ?? 0);
    }

    private static decimal BerekenOnroerendInkomen(VakIIIData vak)
    {
        // Geïndexeerd KI voor niet-verhuurd/particulieren
        decimal ki = ((vak.Code1106 ?? 0) + (vak.Code2106 ?? 0)
                    + (vak.Code1107 ?? 0) + (vak.Code2107 ?? 0)
                    + (vak.Code1108 ?? 0) + (vak.Code2108 ?? 0))
                    * TaxConstants2026.IndexatiecoeffKI;

        // Brutohuur (andere verhuur)
        decimal huur = (vak.Code1110 ?? 0) + (vak.Code2110 ?? 0)
                     + (vak.Code1113 ?? 0) + (vak.Code2113 ?? 0)
                     + (vak.Code1116 ?? 0) + (vak.Code2116 ?? 0);

        return ki + huur;
    }

    private static decimal BerekenBedrijfsvoorheffing(VakIVData vakIV, VakVData vakV)
    {
        return vakIV.Code1286 + vakIV.Code2286
             + vakV.Code1225 + vakV.Code2225;
    }

    private static decimal BerekenWerkbonus(VakIVData vak)
    {
        // Werkbonus: som van de twee werkbonus-codes, begrensd op maximum
        decimal werkbonus = (vak.Code1284 ?? 0) + (vak.Code2284 ?? 0)
                          + (vak.Code1360 ?? 0) + (vak.Code2360 ?? 0);
        return Math.Min(werkbonus, TaxConstants2026.MaxBelastingkredietWerkbonus);
    }

    private static decimal BerekenFederaleVerminderingen(VakXData vak)
    {
        decimal totaal = 0;

        // Giften (30% vanaf AJ2026)
        decimal giften = vak.Code1394 ?? 0;
        if (giften >= TaxConstants2026.MinGift)
            totaal += Math.Min(giften, TaxConstants2026.MaxGift) * TaxConstants2026.PercentageGiften;

        // Kinderoppas (45%)
        decimal kinderoppas = vak.Code1384 ?? 0;
        totaal += kinderoppas * 0.45m;

        // Pensioensparen (30% op max €1.050, of 25% op max €1.350)
        decimal pensioensparen = vak.Code1361 ?? 0;
        if (pensioensparen <= TaxConstants2026.MaxPensioensparen30)
            totaal += pensioensparen * 0.30m;
        else
            totaal += Math.Min(pensioensparen, TaxConstants2026.MaxPensioensparen25) * 0.25m;

        // Tax shelter starters (30%)
        decimal starter30 = (vak.Code1318 ?? 0) + (vak.Code2318 ?? 0);
        totaal += starter30 * 0.30m;

        // Tax shelter starters (45%)
        decimal starter45 = (vak.Code1320 ?? 0) + (vak.Code2320 ?? 0);
        totaal += starter45 * 0.45m;

        // Groeibedrijven (25%)
        decimal groei = (vak.Code1334 ?? 0) + (vak.Code2334 ?? 0);
        totaal += groei * 0.25m;

        return totaal;
    }
}

/// <summary>Alle inputgegevens voor de berekening, samengebracht uit de vakken.</summary>
public class BerekeningInput
{
    public required VakIIData VakII { get; init; }
    public required VakIIIData VakIII { get; init; }
    public required VakIVData VakIV { get; init; }
    public required VakVData VakV { get; init; }
    public required VakVIIIData VakVIII { get; init; }
    public required VakIXData VakIX { get; init; }
    public required VakXData VakX { get; init; }
    public required VakXIIData VakXII { get; init; }

    // Deel 2 vakken
    public VakXVData VakXV { get; init; } = new();
    public VakXVIData VakXVI { get; init; } = new();
    public VakXVIIData VakXVII { get; init; } = new();
    public VakXVIIIData VakXVIII { get; init; } = new();
    public VakXIXData VakXIX { get; init; } = new();
    public VakXXData VakXX { get; init; } = new();
    public VakXXIData VakXXI { get; init; } = new();

    public Gewest Gewest { get; init; } = Gewest.Vlaanderen;
    public decimal GemeentebelastingPercentage { get; init; } = TaxConstants2026.DefaultGemeentebelastingPercentage;
    public TypeBeroep TypeBeroep { get; init; } = TypeBeroep.Werknemer;

    /// <summary>Netto beroepsinkomen partner (voor huwelijksquotiënt).</summary>
    public decimal NettoInkomenPartner { get; init; }
}
