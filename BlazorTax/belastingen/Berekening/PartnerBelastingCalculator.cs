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

        decimal nettoDeel1Bruto = r.BrutoBeroepsinkomen + r.BrutoPensioeninkomen + r.BrutoVervangingsinkomen - r.Beroepskosten;

        // ── 2b. Overdraagbare verliezen vorige jaren (art. 23 §1 WIB92) ────
        r.VorigeVerliezen = Math.Min(inkomen.VorigeVerliezen, Math.Max(nettoDeel1Bruto, 0));
        if (r.VorigeVerliezen > 0)
            r.DetailRegels.Add(new("Vorige beroepsverliezen", -r.VorigeVerliezen));

        decimal nettoDeel1 = Math.Max(nettoDeel1Bruto - r.VorigeVerliezen, 0);

        // ── 2c. Afzonderlijk belastbaar aan gemiddeld tarief (art. 171 WIB92) ──────
        // Opzeggingsvergoedingen (Code1308/2308), vervroegd vakantiegeld (Code1251/2251)
        // en achterstallen loon (Code1252/2252) zitten in BrutoBeroepsinkomen → proportioneel splitsen.
        if (inkomen.AfzonderlijkGemiddeldTariefBruto > 0 && r.BrutoBeroepsinkomen > 0)
        {
            decimal nettoBeroep = Math.Max(r.BrutoBeroepsinkomen - r.Beroepskosten, 0);
            decimal ratio = r.BrutoBeroepsinkomen > 0 ? nettoBeroep / r.BrutoBeroepsinkomen : 0;
            decimal afzNetto = Math.Round(inkomen.AfzonderlijkGemiddeldTariefBruto * ratio, 2);
            r.AfzonderlijkGemiddeldNetto = afzNetto;
            nettoDeel1 = Math.Max(nettoDeel1 - afzNetto, 0);
        }
        // Achterstallen ziekte/invaliditeit (Code1268/2268): afzonderlijk, geen forfait toegepast
        r.AfzonderlijkGemiddeldNetto += inkomen.ZiekteAchterstallen;

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
        // Meerwaarden worden meegenomen in forfait-basis en proportioneel verdeeld
        r.BrutoWinstZelfstandige = inkomen.BrutoWinstZelfstandige;
        decimal winstMV16 = inkomen.WinstMeerwaarden16_5;
        decimal winstMV33 = inkomen.WinstMeerwaarden33;
        if (r.BrutoWinstZelfstandige > 0 || winstMV16 > 0 || winstMV33 > 0)
        {
            decimal brutoTotaalW = r.BrutoWinstZelfstandige + winstMV16 + winstMV33;
            decimal basisW = brutoTotaalW - inkomen.WinstSocialeBijdragen;
            decimal werkelijkeKostenW = inkomen.WinstBeroepskosten + inkomen.WinstVrijstellingen;
            decimal forfaitW = ForfaitaireBeroepskostenCalculator.BerekenForfait(
                Math.Max(basisW, 0), TypeBeroep.Werknemer); // 30% zelfde als werknemer
            if (werkelijkeKostenW > forfaitW)
                r.WinstKosten = werkelijkeKostenW;
            else
                r.WinstKosten = forfaitW;
            decimal nettoTotaalW = Math.Max(basisW - r.WinstKosten, 0);

            // Proportionele verdeling van netto over gezamenlijk en afzonderlijk
            if (brutoTotaalW > 0 && nettoTotaalW > 0)
            {
                decimal ratio = nettoTotaalW / brutoTotaalW;
                decimal nettoGezW = Math.Round(r.BrutoWinstZelfstandige * ratio, 2);
                decimal nettoMV16 = Math.Round(winstMV16 * ratio, 2);
                decimal nettoMV33 = Math.Round(winstMV33 * ratio, 2);
                nettoDeel2 += nettoGezW;
                r.Afzonderlijk16_5Pct += nettoMV16;
                r.Afzonderlijk33Pct += nettoMV33;
            }
            else
            {
                nettoDeel2 += nettoTotaalW;
                r.Afzonderlijk16_5Pct += winstMV16;
                r.Afzonderlijk33Pct += winstMV33;
            }

            r.DetailRegels.Add(new("Winst zelfstandige bruto", brutoTotaalW));
            r.DetailRegels.Add(new("  Kosten + vrijstellingen",
                -r.WinstKosten - inkomen.WinstSocialeBijdragen));
        }

        // Baten vrij beroep (getrapt forfait: 28,7%/10%/5%/3%, max €5.210)
        // Meerwaarden worden meegenomen in forfait-basis en proportioneel verdeeld
        r.BrutoBatenVrijBeroep = inkomen.BrutoBatenVrijBeroep;
        decimal batenMV16 = inkomen.BatenMeerwaarden16_5;
        decimal batenMV33 = inkomen.BatenMeerwaarden33;
        if (r.BrutoBatenVrijBeroep > 0 || batenMV16 > 0 || batenMV33 > 0)
        {
            decimal brutoTotaalB = r.BrutoBatenVrijBeroep + batenMV16 + batenMV33;
            decimal basisB = brutoTotaalB - inkomen.BatenSocialeBijdragen;
            decimal werkelijkeKostenB = inkomen.BatenBeroepskosten + inkomen.BatenVrijstellingen;
            decimal? werkKostenB = werkelijkeKostenB > 0 ? werkelijkeKostenB : null;
            (r.BatenKosten, r.BatenForfaitair) =
                ForfaitaireBeroepskostenCalculator.Bereken(Math.Max(basisB, 0), werkKostenB, TypeBeroep.Baten);
            decimal nettoTotaalB = Math.Max(basisB - r.BatenKosten, 0);

            if (brutoTotaalB > 0 && nettoTotaalB > 0)
            {
                decimal ratio = nettoTotaalB / brutoTotaalB;
                decimal nettoGezB = Math.Round(r.BrutoBatenVrijBeroep * ratio, 2);
                decimal nettoMV16B = Math.Round(batenMV16 * ratio, 2);
                decimal nettoMV33B = Math.Round(batenMV33 * ratio, 2);
                nettoDeel2 += nettoGezB;
                r.Afzonderlijk16_5Pct += nettoMV16B;
                r.Afzonderlijk33Pct += nettoMV33B;
            }
            else
            {
                nettoDeel2 += nettoTotaalB;
                r.Afzonderlijk16_5Pct += batenMV16;
                r.Afzonderlijk33Pct += batenMV33;
            }

            r.DetailRegels.Add(new("Baten vrij beroep bruto", brutoTotaalB));
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
        if (inkomen.BrutoPensioeninkomen > 0 || inkomen.Werkloosheid > 0 || inkomen.ZiekteInvaliditeit > 0
            || inkomen.AndereVervangings > 0 || inkomen.Beroepsziekte > 0)
        {
            r.VerminderingVervangingsinkomen = VervangingsInkomstenCalculator.Bereken(
                r.NettoBelastbaarInkomen,
                inkomen.BrutoPensioeninkomen,
                inkomen.Werkloosheid + inkomen.AndereVervangings,
                inkomen.ZiekteInvaliditeit + inkomen.Beroepsziekte);

            // Vermindering begrensd op om te slane
            r.VerminderingVervangingsinkomen = Math.Min(r.VerminderingVervangingsinkomen, r.OmTeSlane);

            if (r.VerminderingVervangingsinkomen > 0)
                r.DetailRegels.Add(new("Vermindering vervangingsinkomen", -r.VerminderingVervangingsinkomen));
        }

        r.Hoofdsom = Math.Max(r.OmTeSlane - r.VerminderingVervangingsinkomen, 0);
        // ── 6b. Afzonderlijk belastbaar aan gemiddeld tarief (art. 171 WIB92) ─────────
        // De gemiddelde aanslagvoet = Hoofdsom_gezamenlijk / (netto_gezamenlijk + netto_afzonderlijk)
        // Afgerond op 1 decimaal in % (conform werkwijze FOD Financën).
        if (r.AfzonderlijkGemiddeldNetto > 0)
        {
            decimal basisVoorGemiddeld = r.NettoBelastbaarNaHQ + r.AfzonderlijkGemiddeldNetto;
            decimal gemiddeldTarief = basisVoorGemiddeld > 0
                ? Math.Round(r.Hoofdsom / basisVoorGemiddeld * 100m, 1) / 100m
                : 0m;
            r.GemiddeldTariefAfzonderlijk = gemiddeldTarief;
            r.BelastingAfzonderlijkGemiddeld = Math.Round(r.AfzonderlijkGemiddeldNetto * gemiddeldTarief, 2);
            r.Hoofdsom += r.BelastingAfzonderlijkGemiddeld;
            r.DetailRegels.Add(new(
                $"Afz. bel. gemidd. tarief ({gemiddeldTarief:P1})",
                r.BelastingAfzonderlijkGemiddeld));
        }

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

        // ── 8b. Gewestelijke verminderingen ─────────────────────────────
        r.GewestelijkeVerminderingen = GewestelijkeVerminderingenCalculator.Bereken(
            inkomen, gewest, r.NettoBelastbaarInkomen);

        if (r.GewestelijkeVerminderingen > 0)
            r.DetailRegels.Add(new("Gewestelijke verminderingen", -r.GewestelijkeVerminderingen));

        r.SaldoGewestelijk = Math.Max(r.GewestelijkeOpcentiemen - r.GewestelijkeVerminderingen, 0);

        // ── 8c. Vermeerdering wegens geen/onvoldoende voorafbetalingen ───
        // Alleen voor zelfstandigen (Deel 2 inkomen). Art. 157-168 WIB.
        // Gemeentebelasting wordt berekend op belasting ZONDER vermeerdering.
        if (inkomen.HeeftDeel2Inkomen)
        {
            decimal belastingBasis = r.SaldoFederaal + r.SaldoGewestelijk;
            decimal basis106 = belastingBasis * TaxConstants2026.VermeerderingMultiplier;
            decimal vaCredit = Math.Min(inkomen.Deel2Bedrijfsvoorheffing, basis106);
            decimal vermBruto = Math.Max(basis106 - vaCredit, 0) * TaxConstants2026.VermeerderingPercentage;
            decimal verm = Math.Round(vermBruto * TaxConstants2026.VermeerderingReductie, 2);

            // Minimum-drempel: als vermeerdering < max(€100, 0,5% × belasting) → geen vermeerdering
            decimal drempel = Math.Max(TaxConstants2026.VermeerderingMinimum,
                                       belastingBasis * TaxConstants2026.VermeerderingMinimumPct);
            r.Vermeerdering = verm >= drempel ? verm : 0;

            if (r.Vermeerdering > 0)
                r.DetailRegels.Add(new("Vermeerdering (geen VA)", r.Vermeerdering));
        }

        // Afzonderlijke belasting toevoegen (buiten progressief systeem)
        r.TotaleBelasting = r.SaldoFederaal + r.SaldoGewestelijk + r.BelastingAfzonderlijk + r.Vermeerdering;

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

        // Overwerktoeslag (art. 154quater WIB92): 57,75% van de overwerktoeslag (Code1234/2234)
        // Dit is een FEDERALE vermindering (niet opgesplitst gewestelijk).
        if (inkomen.OverwerktoeslagCode1234 > 0)
            totaal += inkomen.OverwerktoeslagCode1234 * TaxConstants2026.OverwerktoeslagVerminderingPercentage;

        // Federaal langetermijnsparen niet-eigen woning (Code1358/2358 + Code1353/2353): 30%
        decimal fedLT = inkomen.FederaalLTKapitaal + inkomen.FederaalLTPremies;
        if (fedLT > 0)
        {
            decimal effectief = Math.Min(fedLT, TaxConstants2026.MaxLangetermijnsparenFederaalAbsoluut);
            totaal += effectief * 0.30m;
        }

        return totaal;
    }
}
