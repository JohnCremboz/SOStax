namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Orchestrator voor de gemeenschappelijke aanslag (twee-kolomsberekening).
/// Berekent elke partner APART door alle belastingschijven, en combineert
/// daarna voor gemeentebelasting en BBSZ.
/// </summary>
public class GezamenlijkeBerekeningCalculator
{
    /// <summary>
    /// Voert de volledige twee-kolomsberekening uit.
    /// Werkt ook correct voor alleenstaanden (dan is partner leeg).
    /// </summary>
    public GezamenlijkResultaat Bereken(BerekeningInput input)
    {
        bool isGehuwd = input.VakII.BurgerlijkeStaat == "1002"
                     || input.VakII.BurgerlijkeStaat == "1010";

        // ── 1. Inkomen extraheren per partner ───────────────────────────
        var inkomen1 = PartnerInkomen.ExtractBelastingplichtige(
            input.VakIV, input.VakV, input.VakX,
            input.VakXV, input.VakXVI, input.VakXVII, input.VakXVIII,
            input.VakXIX, input.VakXX, input.VakXXI, input.VakIX);
        var inkomen2 = isGehuwd
            ? PartnerInkomen.ExtractPartner(
                input.VakIV, input.VakV, input.VakX,
                input.VakXV, input.VakXVI, input.VakXVII, input.VakXVIII,
                input.VakXIX, input.VakXX, input.VakXXI, input.VakIX)
            : new PartnerInkomen { Label = "Partner" };

        // ── 2. Fase 1: bruto → netto per partner ───────────────────────
        var r1 = PartnerBelastingCalculator.Bereken(inkomen1, 0, input.TypeBeroep, input.Gewest);
        var r2 = isGehuwd && inkomen2.HeeftInkomen
            ? PartnerBelastingCalculator.Bereken(inkomen2, 0, input.TypeBeroep, input.Gewest)
            : new PartnerResultaat { Label = "Partner" };

        // ── 3. Huwelijksquotiënt ────────────────────────────────────────
        decimal hqBedrag = 0;
        if (isGehuwd)
        {
            hqBedrag = HuwelijksquotientCalculator.Bereken(
                r1.NettoBelastbaarInkomen, r2.NettoBelastbaarInkomen, true);

            if (hqBedrag > 0)
            {
                // Bepaal wie afstaat en wie ontvangt
                if (r1.NettoBelastbaarInkomen >= r2.NettoBelastbaarInkomen)
                {
                    r1.HuwelijksquotientAfgestaan = hqBedrag;
                    r1.NettoBelastbaarNaHQ = r1.NettoBelastbaarInkomen - hqBedrag;
                    r2.HuwelijksquotientOntvangen = hqBedrag;
                    r2.NettoBelastbaarNaHQ = r2.NettoBelastbaarInkomen + hqBedrag;
                }
                else
                {
                    r2.HuwelijksquotientAfgestaan = hqBedrag;
                    r2.NettoBelastbaarNaHQ = r2.NettoBelastbaarInkomen - hqBedrag;
                    r1.HuwelijksquotientOntvangen = hqBedrag;
                    r1.NettoBelastbaarNaHQ = r1.NettoBelastbaarInkomen + hqBedrag;
                }
            }
        }

        // ── 4. Kinderallocatie ──────────────────────────────────────────
        // Kinderen worden toegewezen aan de hoogstverdienende partner
        // (maximaal fiscaal voordeel). Bij alleenstaanden: altijd partner 1.
        bool kinderenBijPartner1 = !isGehuwd || r1.NettoBelastbaarNaHQ >= r2.NettoBelastbaarNaHQ;

        decimal vrijeSom1 = isGehuwd
            ? BelastingvrijeSomCalculator.BerekenPartner(input.VakII, true, kinderenBijPartner1)
            : BelastingvrijeSomCalculator.Bereken(input.VakII);

        decimal vrijeSom2 = isGehuwd
            ? BelastingvrijeSomCalculator.BerekenPartner(input.VakII, false, !kinderenBijPartner1)
            : 0;

        // ── 5a. Kinderopvang proportioneel verdelen ─────────────────────────
        // Tax-Calc verdeelt kinderopvang proportioneel op basis van netto-inkomen
        if (isGehuwd && inkomen2.HeeftInkomen && inkomen1.Kinderopvang > 0)
        {
            decimal totaalKinderopvang = inkomen1.Kinderopvang;
            decimal totaalNetto = r1.NettoBelastbaarInkomen + r2.NettoBelastbaarInkomen;
            if (totaalNetto > 0)
            {
                inkomen1.Kinderopvang = Math.Round(totaalKinderopvang * r1.NettoBelastbaarInkomen / totaalNetto, 2);
                inkomen2.Kinderopvang = totaalKinderopvang - inkomen1.Kinderopvang;
            }
        }

        // ── 5b. Fase 2: belasting berekenen per partner ─────────────────
        PartnerBelastingCalculator.BerekenBelasting(r1, inkomen1, vrijeSom1, input.Gewest);

        if (isGehuwd && inkomen2.HeeftInkomen)
            PartnerBelastingCalculator.BerekenBelasting(r2, inkomen2, vrijeSom2, input.Gewest);

        // ── 6. Gecombineerde resultaten ─────────────────────────────────
        var resultaat = new GezamenlijkResultaat
        {
            IsGezamenlijk = isGehuwd && inkomen2.HeeftInkomen,
            Belastingplichtige = r1,
            Partner = r2,
        };

        // Gecombineerd federaal en gewestelijk
        resultaat.TotaalSaldoFederaal = r1.SaldoFederaal + r2.SaldoFederaal;
        resultaat.TotaalSaldoGewestelijk = r1.SaldoGewestelijk + r2.SaldoGewestelijk;

        // Gemeentebelasting op (federaal + gewestelijk) ZONDER vermeerdering
        resultaat.GemeentebelastingPercentage = input.GemeentebelastingPercentage;
        resultaat.BasisGemeentebelasting = resultaat.TotaalSaldoFederaal + resultaat.TotaalSaldoGewestelijk;
        resultaat.Gemeentebelasting = resultaat.BasisGemeentebelasting * input.GemeentebelastingPercentage / 100m;

        // BBSZ per persoon (AJ2026: single-proof hervorming)
        // Alleen verschuldigd als er beroeps- of vervangingsinkomen is (niet bij enkel pensioen)
        decimal bbszIngehouden = inkomen1.BijzondereBijdrageSZ + inkomen2.BijzondereBijdrageSZ;

        bool bp1HeeftBeroep = inkomen1.BrutoBeroepsinkomen > 0 || inkomen1.BrutoVervangingsinkomen > 0;
        bool bp2HeeftBeroep = inkomen2.BrutoBeroepsinkomen > 0 || inkomen2.BrutoVervangingsinkomen > 0;

        decimal bbszBP = bp1HeeftBeroep ? BerekenBBSZPerPersoon(r1.NettoBelastbaarInkomen) : 0;
        decimal bbszPartner = bp2HeeftBeroep ? BerekenBBSZPerPersoon(r2.NettoBelastbaarInkomen) : 0;
        decimal bbszVerschuldigd = bbszBP + bbszPartner;

        resultaat.BBSZGezamenlijkInkomen = r1.NettoBelastbaarInkomen + r2.NettoBelastbaarInkomen;
        resultaat.BBSZVerschuldigd = bbszVerschuldigd;
        resultaat.BBSZIngehouden = bbszIngehouden;
        resultaat.BBSZSaldo = bbszVerschuldigd - bbszIngehouden;

        // Voorheffingen
        decimal totaalBV = r1.Bedrijfsvoorheffing + r2.Bedrijfsvoorheffing;
        decimal totaalRV = inkomen1.Deel2RoerendeVoorheffing + inkomen2.Deel2RoerendeVoorheffing;
        decimal totaalWerkbonus = r1.BelastingkredietWerkbonus + r2.BelastingkredietWerkbonus;

        // Totale belasting (inclusief afzonderlijk belastbaar en vermeerdering)
        decimal totaalAfzonderlijk = r1.BelastingAfzonderlijk + r2.BelastingAfzonderlijk;
        decimal totaalVermeerdering = r1.Vermeerdering + r2.Vermeerdering;
        decimal totaleBelasting = resultaat.TotaalSaldoFederaal
                                + resultaat.TotaalSaldoGewestelijk
                                + resultaat.Gemeentebelasting
                                + Math.Max(resultaat.BBSZSaldo, 0)
                                + totaalAfzonderlijk
                                + totaalVermeerdering;

        // Aftrekken
        decimal totaalAftrek = totaalBV + totaalRV + totaalWerkbonus;

        resultaat.Eindresultaat = totaleBelasting - totaalAftrek;

        // ── 7. Detailregels opbouwen ────────────────────────────────────
        BouwDetailRegels(resultaat, r1, r2, inkomen1, inkomen2, input, hqBedrag,
            kinderenBijPartner1, vrijeSom1, vrijeSom2, totaalBV, totaalRV, totaalWerkbonus);

        return resultaat;
    }

    /// <summary>
    /// Berekent de BBSZ per persoon (AJ2026: single-proof hervorming).
    /// Barema gebaseerd op het individuele netto belastbaar inkomen.
    /// </summary>
    private static decimal BerekenBBSZPerPersoon(decimal nettoInkomen)
    {
        // AJ2026 BBSZ-barema per persoon (single-proof)
        if (nettoInkomen <= 18_592.02m) return 0;
        if (nettoInkomen <= 21_070.96m)
            return (nettoInkomen - 18_592.02m) * 0.09m;
        if (nettoInkomen <= 60_161.85m)
        {
            decimal basis = (21_070.96m - 18_592.02m) * 0.09m; // ~223.10
            return basis + (nettoInkomen - 21_070.96m) * 0.013m;
        }
        // Hogere schijven (afbouw single-proof)
        if (nettoInkomen <= 62_027.14m)
        {
            return 731.14m + (nettoInkomen - 60_161.85m) * 0.0111m;
        }
        if (nettoInkomen <= 80_654.70m)
            return 752.14m; // vast bedrag
        return 731.38m; // afgebouwd maximum
    }

    private static void BouwDetailRegels(
        GezamenlijkResultaat resultaat,
        PartnerResultaat r1, PartnerResultaat r2,
        PartnerInkomen ink1, PartnerInkomen ink2,
        BerekeningInput input,
        decimal hqBedrag,
        bool kinderenBijPartner1,
        decimal vrijeSom1, decimal vrijeSom2,
        decimal totaalBV, decimal totaalRV, decimal totaalWerkbonus)
    {
        var regels = resultaat.DetailRegels;

        if (resultaat.IsGezamenlijk)
        {
            regels.Add(new("═══ BELASTINGPLICHTIGE ═══", 0, true));
            regels.Add(new("Bruto inkomen", r1.BrutoTotaal));
            regels.Add(new("Beroepskosten", -r1.Beroepskosten));

            // Deel 2 samenvatting
            if (ink1.HeeftDeel2Inkomen)
            {
                if (r1.BrutoBedrijfsleider > 0)
                    regels.Add(new("Bedrijfsleider netto", r1.BrutoBedrijfsleider - r1.BedrijfsleiderKosten - ink1.BedrijfsleiderSocialeBijdragen));
                if (r1.BrutoWinstZelfstandige > 0)
                    regels.Add(new("Winst zelfstandige netto", r1.BrutoWinstZelfstandige - r1.WinstKosten - ink1.WinstSocialeBijdragen));
                if (r1.BrutoBatenVrijBeroep > 0)
                    regels.Add(new("Baten vrij beroep netto", r1.BrutoBatenVrijBeroep - r1.BatenKosten - ink1.BatenSocialeBijdragen));
                if (r1.BrutoMeewerkend > 0)
                    regels.Add(new("Meewerkend netto", r1.BrutoMeewerkend - r1.MeewerkendKosten - ink1.MeewerkendSocialeBijdragen));
                if (r1.BelastingAfzonderlijk > 0)
                    regels.Add(new("Belasting afzonderlijk", r1.BelastingAfzonderlijk));
            }

            regels.Add(new("Netto belastbaar", r1.NettoBelastbaarInkomen, true));

            if (hqBedrag > 0 && r1.HuwelijksquotientAfgestaan > 0)
                regels.Add(new("HQ afgestaan", -r1.HuwelijksquotientAfgestaan));
            if (hqBedrag > 0 && r1.HuwelijksquotientOntvangen > 0)
                regels.Add(new("HQ ontvangen", r1.HuwelijksquotientOntvangen));

            regels.Add(new("Basisbelasting", r1.Basisbelasting));
            regels.Add(new($"Belastingvrije som ({vrijeSom1:N0})", -r1.VerminderingBelastingvrijeSom));
            regels.Add(new("Om te slane", r1.OmTeSlane, true));
            if (r1.VerminderingVervangingsinkomen > 0)
                regels.Add(new("Verm. vervangingsinkomen", -r1.VerminderingVervangingsinkomen));
            regels.Add(new("Federaal", r1.SaldoFederaal));
            if (r1.GewestelijkeVerminderingen > 0)
                regels.Add(new("Gewest. verminderingen", -r1.GewestelijkeVerminderingen));
            regels.Add(new("Gewestelijk", r1.SaldoGewestelijk));
            if (r1.Vermeerdering > 0)
                regels.Add(new("Vermeerdering (geen VA)", r1.Vermeerdering));

            regels.Add(new("═══ PARTNER ═══", 0, true));
            regels.Add(new("Bruto inkomen", r2.BrutoTotaal));
            regels.Add(new("Beroepskosten", -r2.Beroepskosten));

            // Deel 2 samenvatting partner
            if (ink2.HeeftDeel2Inkomen)
            {
                if (r2.BrutoBedrijfsleider > 0)
                    regels.Add(new("Bedrijfsleider netto", r2.BrutoBedrijfsleider - r2.BedrijfsleiderKosten - ink2.BedrijfsleiderSocialeBijdragen));
                if (r2.BrutoWinstZelfstandige > 0)
                    regels.Add(new("Winst zelfstandige netto", r2.BrutoWinstZelfstandige - r2.WinstKosten - ink2.WinstSocialeBijdragen));
                if (r2.BrutoBatenVrijBeroep > 0)
                    regels.Add(new("Baten vrij beroep netto", r2.BrutoBatenVrijBeroep - r2.BatenKosten - ink2.BatenSocialeBijdragen));
                if (r2.BrutoMeewerkend > 0)
                    regels.Add(new("Meewerkend netto", r2.BrutoMeewerkend - r2.MeewerkendKosten - ink2.MeewerkendSocialeBijdragen));
                if (r2.BelastingAfzonderlijk > 0)
                    regels.Add(new("Belasting afzonderlijk", r2.BelastingAfzonderlijk));
            }

            regels.Add(new("Netto belastbaar", r2.NettoBelastbaarInkomen, true));

            if (hqBedrag > 0 && r2.HuwelijksquotientAfgestaan > 0)
                regels.Add(new("HQ afgestaan", -r2.HuwelijksquotientAfgestaan));
            if (hqBedrag > 0 && r2.HuwelijksquotientOntvangen > 0)
                regels.Add(new("HQ ontvangen", r2.HuwelijksquotientOntvangen));

            regels.Add(new("Basisbelasting", r2.Basisbelasting));
            regels.Add(new($"Belastingvrije som ({vrijeSom2:N0})", -r2.VerminderingBelastingvrijeSom));
            regels.Add(new("Om te slane", r2.OmTeSlane, true));
            if (r2.VerminderingVervangingsinkomen > 0)
                regels.Add(new("Verm. vervangingsinkomen", -r2.VerminderingVervangingsinkomen));
            regels.Add(new("Federaal", r2.SaldoFederaal));
            if (r2.GewestelijkeVerminderingen > 0)
                regels.Add(new("Gewest. verminderingen", -r2.GewestelijkeVerminderingen));
            regels.Add(new("Gewestelijk", r2.SaldoGewestelijk));
            if (r2.Vermeerdering > 0)
                regels.Add(new("Vermeerdering (geen VA)", r2.Vermeerdering));

            regels.Add(new("═══ GECOMBINEERD ═══", 0, true));
        }
        else
        {
            // Alleenstaande: toon alle detailregels van partner 1
            regels.AddRange(r1.DetailRegels);
        }

        regels.Add(new("Totaal federaal", resultaat.TotaalSaldoFederaal));
        regels.Add(new("Totaal gewestelijk", resultaat.TotaalSaldoGewestelijk));
        regels.Add(new($"Gemeentebelasting ({input.GemeentebelastingPercentage}%)", resultaat.Gemeentebelasting));

        if (resultaat.BBSZSaldo > 0)
            regels.Add(new("BBSZ saldo", resultaat.BBSZSaldo));

        decimal totaalAfzonderlijk = r1.BelastingAfzonderlijk + r2.BelastingAfzonderlijk;
        if (totaalAfzonderlijk > 0)
            regels.Add(new("Belasting afzonderlijk", totaalAfzonderlijk));

        decimal totaalVermeerdering = r1.Vermeerdering + r2.Vermeerdering;

        decimal totaleBelasting = resultaat.TotaalSaldoFederaal + resultaat.TotaalSaldoGewestelijk
                                + resultaat.Gemeentebelasting + Math.Max(resultaat.BBSZSaldo, 0)
                                + totaalAfzonderlijk + totaalVermeerdering;
        regels.Add(new("Totale belasting", totaleBelasting, true));

        if (totaalBV > 0)
            regels.Add(new("Bedrijfsvoorheffing", -totaalBV));
        if (totaalRV > 0)
            regels.Add(new("Roerende voorheffing", -totaalRV));
        if (totaalWerkbonus > 0)
            regels.Add(new("Belastingkrediet werkbonus", -totaalWerkbonus));

        regels.Add(new(
            resultaat.Eindresultaat >= 0 ? "Te betalen" : "Terug te krijgen",
            resultaat.Eindresultaat, true));
    }
}
