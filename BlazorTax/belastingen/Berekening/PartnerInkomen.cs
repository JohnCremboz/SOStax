namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Inkomstengegevens voor één partner, geëxtraheerd uit de gemeenschappelijke vakdata.
/// </summary>
public class PartnerInkomen
{
    public string Label { get; init; } = "Belastingplichtige";

    // ── Deel 1: Beroepsinkomen ──────────────────────────────────────────
    public decimal BrutoLoon { get; init; }
    public decimal WoonWerkVerkeerTotaal { get; init; }
    public decimal WoonWerkVerkeerVrijstelling { get; init; }
    public decimal FlexiJob { get; init; }
    public decimal Impulsfonds { get; init; }

    public decimal BrutoBeroepsinkomen =>
        BrutoLoon + FlexiJob + Impulsfonds
        + Math.Max(WoonWerkVerkeerTotaal - WoonWerkVerkeerVrijstelling, 0);

    // Pensioeninkomen
    public decimal WettelijkPensioen { get; init; }
    public decimal Overlevingspensioen { get; init; }
    public decimal AnderePensioenen { get; init; }
    public decimal PensioenDecember { get; init; }
    public decimal PensioenAchterstallen { get; init; }

    public decimal BrutoPensioeninkomen =>
        WettelijkPensioen + Overlevingspensioen + AnderePensioenen
        + PensioenDecember + PensioenAchterstallen;

    // Vervangingsinkomen
    public decimal Werkloosheid { get; init; }
    public decimal ZiekteInvaliditeit { get; init; }
    public decimal Beroepsziekte { get; init; }
    public decimal AndereVervangings { get; init; }

    public decimal BrutoVervangingsinkomen =>
        Werkloosheid + ZiekteInvaliditeit + Beroepsziekte + AndereVervangings;

    // Kosten & voorheffingen
    public decimal WerkelijkeKosten { get; init; }
    public decimal Bedrijfsvoorheffing { get; init; }
    public decimal BijzondereBijdrageSZ { get; init; }

    // Werkbonus RSZ-bedragen per tarief (voor berekening belastingkrediet)
    public decimal WerkbonusCode284 { get; init; }  // RSZ werkbonus aan 33,14%
    public decimal WerkbonusCode360 { get; init; }  // RSZ werkbonus aan 52,54%

    // Verminderingen
    public decimal Kinderopvang { get; set; }
    public decimal Pensioensparen { get; init; }
    public decimal Giften { get; init; }

    // ── Deel 2: Bezoldigingen bedrijfsleiders (Vak XVI) ─────────────────
    public decimal BrutoBedrijfsleider { get; init; }
    public decimal BedrijfsleiderSocialeBijdragen { get; init; }
    public decimal BedrijfsleiderWerkelijkeKosten { get; init; }
    public decimal BedrijfsleiderBedrijfsvoorheffing { get; init; }
    public decimal BedrijfsleiderWerkbonus284 { get; init; }
    public decimal BedrijfsleiderWerkbonus360 { get; init; }

    // ── Deel 2: Winst zelfstandigen (Vak XVII) ──────────────────────────
    public decimal BrutoWinstZelfstandige { get; init; }
    public decimal WinstSocialeBijdragen { get; init; }
    public decimal WinstBeroepskosten { get; init; }
    public decimal WinstVrijstellingen { get; init; }

    // ── Deel 2: Baten vrije beroepen (Vak XVIII) ────────────────────────
    public decimal BrutoBatenVrijBeroep { get; init; }
    public decimal BatenSocialeBijdragen { get; init; }
    public decimal BatenBeroepskosten { get; init; }
    public decimal BatenVrijstellingen { get; init; }

    // ── Deel 2: Meewerkende echtgenoot (Vak XX) ─────────────────────────
    public decimal BrutoMeewerkend { get; init; }
    public decimal MeewerkendSocialeBijdragen { get; init; }
    public decimal MeewerkendBeroepskosten { get; init; }

    // ── Deel 2: Afzonderlijk belastbare inkomsten ───────────────────────
    public decimal Afzonderlijk10Pct { get; init; }     // stopzetting 60+/gedwongen (Vak XXI)
    public decimal Afzonderlijk12_5Pct { get; init; }   // Europese landbouwsubsidies
    public decimal Afzonderlijk16_5Pct { get; init; }   // meerwaarden ≥5j, achterstallen, COVID
    public decimal Afzonderlijk33Pct { get; init; }     // meerwaarden <5j, immaterieel

    // ── Deel 2: Diverse inkomsten (Vak XV) ──────────────────────────────
    public decimal DiverseInkomstenGezamenlijk { get; init; }  // gezamenlijk belastbaar
    public decimal DiverseInkomsten16_5Pct { get; init; }      // afzonderlijk 16,5%
    public decimal DiverseInkomsten33Pct { get; init; }        // afzonderlijk 33%

    // ── Deel 2: Voorheffingen zelfstandigen (Vak XIX) ───────────────────
    public decimal Deel2Bedrijfsvoorheffing { get; init; }
    public decimal Deel2RoerendeVoorheffing { get; init; }

    /// <summary>
    /// Extraheert de gegevens van partner 1 (Code1xxx) uit de vakdata.
    /// </summary>
    public static PartnerInkomen ExtractBelastingplichtige(
        VakIVData vakIV, VakVData vakV, VakXData vakX,
        VakXVData? vakXV = null, VakXVIData? vakXVI = null,
        VakXVIIData? vakXVII = null, VakXVIIIData? vakXVIII = null,
        VakXIXData? vakXIX = null, VakXXData? vakXX = null,
        VakXXIData? vakXXI = null)
    {
        return new PartnerInkomen
        {
            Label = "Belastingplichtige",
            // ── Deel 1: Beroep ──────────────────────────────────────
            BrutoLoon = (vakIV.Code1250 ?? 0) + (vakIV.Code1251 ?? 0) + (vakIV.Code1252 ?? 0)
                      + (vakIV.Code1247 ?? 0),
            WoonWerkVerkeerTotaal = vakIV.Code1254 ?? 0,
            WoonWerkVerkeerVrijstelling = vakIV.Code1255 ?? 0,
            FlexiJob = vakIV.Code1262 ?? 0,
            Impulsfonds = vakIV.Code1267 ?? 0,
            // Pensioen
            WettelijkPensioen = vakV.Code1228 ?? 0,
            Overlevingspensioen = vakV.Code1229 ?? 0,
            AnderePensioenen = vakV.Code1211 ?? 0,
            PensioenDecember = vakV.Code1314 ?? 0,
            PensioenAchterstallen = (vakV.Code1230 ?? 0) + (vakV.Code1231 ?? 0) + (vakV.Code1212 ?? 0),
            // Vervangingsinkomen
            Werkloosheid = (vakIV.Code1260 ?? 0) + (vakIV.Code1264 ?? 0)
                         + (vakIV.Code1261 ?? 0) + (vakIV.Code1265 ?? 0),
            ZiekteInvaliditeit = (vakIV.Code1266 ?? 0) + (vakIV.Code1303 ?? 0) + (vakIV.Code1268 ?? 0),
            Beroepsziekte = vakIV.Code1270 ?? 0,
            AndereVervangings = vakIV.Code1271 ?? 0,
            // Kosten & voorheffingen
            WerkelijkeKosten = vakIV.Code1258 ?? 0,
            Bedrijfsvoorheffing = (vakIV.Code1286 ?? 0) + (vakV.Code1225 ?? 0),
            BijzondereBijdrageSZ = vakIV.Code1287 ?? 0,
            WerkbonusCode284 = vakIV.Code1284 ?? 0,
            WerkbonusCode360 = vakIV.Code1360 ?? 0,
            // Verminderingen
            Kinderopvang = vakX.Code1384 ?? 0,
            Pensioensparen = vakX.Code1361 ?? 0,
            Giften = vakX.Code1394 ?? 0,

            // ── Deel 2: Bedrijfsleider (Vak XVI) ────────────────────
            BrutoBedrijfsleider = ExtractBrutoBedrijfsleider1(vakXVI),
            BedrijfsleiderSocialeBijdragen = vakXVI?.Code1454 ?? 0,
            BedrijfsleiderWerkelijkeKosten = vakXVI?.Code1453 ?? 0,
            BedrijfsleiderBedrijfsvoorheffing = vakXVI?.Code1421 ?? 0,
            BedrijfsleiderWerkbonus284 = vakXVI?.Code1419 ?? 0,
            BedrijfsleiderWerkbonus360 = vakXVI?.Code1430 ?? 0,

            // ── Deel 2: Winst zelfstandige (Vak XVII) ───────────────
            BrutoWinstZelfstandige = ExtractBrutoWinst1(vakXVII),
            WinstSocialeBijdragen = vakXVII?.Code1632 ?? 0,
            WinstBeroepskosten = (vakXVII?.Code1620 ?? 0) + (vakXVII?.Code1611 ?? 0)
                               + (vakXVII?.Code1606 ?? 0),
            WinstVrijstellingen = (vakXVII?.Code1609 ?? 0) + (vakXVII?.Code1608 ?? 0)
                                + (vakXVII?.Code1612 ?? 0) + (vakXVII?.Code1633 ?? 0)
                                + (vakXVII?.Code1614 ?? 0),

            // ── Deel 2: Baten vrij beroep (Vak XVIII) ───────────────
            BrutoBatenVrijBeroep = ExtractBrutoBaten1(vakXVIII),
            BatenSocialeBijdragen = vakXVIII?.Code1656 ?? 0,
            BatenBeroepskosten = (vakXVIII?.Code1669 ?? 0) + (vakXVIII?.Code1657 ?? 0),
            BatenVrijstellingen = (vakXVIII?.Code1681 ?? 0) + (vakXVIII?.Code1662 ?? 0)
                                + (vakXVIII?.Code1660 ?? 0) + (vakXVIII?.Code1664 ?? 0)
                                + (vakXVIII?.Code1665 ?? 0) + (vakXVIII?.Code1666 ?? 0),

            // ── Deel 2: Meewerkende echtgenoot (Vak XX) ─────────────
            BrutoMeewerkend = vakXX?.Code1450 ?? 0,
            MeewerkendSocialeBijdragen = vakXX?.Code1451 ?? 0,
            MeewerkendBeroepskosten = vakXX?.Code1452 ?? 0,

            // ── Deel 2: Afzonderlijk belastbaar ─────────────────────
            Afzonderlijk10Pct = (vakXXI?.Code1686 ?? 0),
            Afzonderlijk12_5Pct = (vakXVII?.Code1607 ?? 0) + (vakXXI?.Code1687 ?? 0),
            Afzonderlijk16_5Pct = (vakXVII?.Code1603 ?? 0) + (vakXVII?.Code1636 ?? 0)
                                + (vakXVII?.Code1605 ?? 0) + (vakXVIII?.Code1653 ?? 0)
                                + (vakXVIII?.Code1682 ?? 0) + (vakXVIII?.Code1655 ?? 0)
                                + (vakXXI?.Code1690 ?? 0) + (vakXXI?.Code1694 ?? 0),
            Afzonderlijk33Pct = (vakXVII?.Code1618 ?? 0) + (vakXVIII?.Code1667 ?? 0)
                              + (vakXXI?.Code1691 ?? 0),

            // ── Deel 2: Diverse inkomsten (Vak XV) ──────────────────
            DiverseInkomstenGezamenlijk = (vakXV?.Code1460 ?? 0) + (vakXV?.Code1200 ?? 0)
                                        + (vakXV?.Code1172 ?? 0) + (vakXV?.Code1171 ?? 0),
            DiverseInkomsten16_5Pct = (vakXV?.Code1462 ?? 0) + (vakXV?.Code1177 ?? 0)
                                    + (vakXV?.Code1178 ?? 0),
            DiverseInkomsten33Pct = (vakXV?.Code1461 ?? 0) + (vakXV?.Code1463 ?? 0)
                                  + (vakXV?.Code1169 ?? 0) + (vakXV?.Code1175 ?? 0)
                                  + (vakXV?.Code1176 ?? 0),

            // ── Deel 2: Voorheffingen zelfstandigen (Vak XIX) ───────
            Deel2Bedrijfsvoorheffing = vakXIX?.Code1758 ?? 0,
            Deel2RoerendeVoorheffing = vakXIX?.Code1756 ?? 0,
        };
    }

    /// <summary>
    /// Extraheert de gegevens van partner 2 (Code2xxx) uit de vakdata.
    /// </summary>
    public static PartnerInkomen ExtractPartner(
        VakIVData vakIV, VakVData vakV, VakXData vakX,
        VakXVData? vakXV = null, VakXVIData? vakXVI = null,
        VakXVIIData? vakXVII = null, VakXVIIIData? vakXVIII = null,
        VakXIXData? vakXIX = null, VakXXData? vakXX = null,
        VakXXIData? vakXXI = null)
    {
        return new PartnerInkomen
        {
            Label = "Partner",
            // ── Deel 1: Beroep ──────────────────────────────────────
            BrutoLoon = (vakIV.Code2250 ?? 0) + (vakIV.Code2251 ?? 0) + (vakIV.Code2252 ?? 0)
                      + (vakIV.Code2247 ?? 0),
            WoonWerkVerkeerTotaal = vakIV.Code2254 ?? 0,
            WoonWerkVerkeerVrijstelling = vakIV.Code2255 ?? 0,
            FlexiJob = vakIV.Code2262 ?? 0,
            Impulsfonds = vakIV.Code2267 ?? 0,
            // Pensioen
            WettelijkPensioen = vakV.Code2228 ?? 0,
            Overlevingspensioen = vakV.Code2229 ?? 0,
            AnderePensioenen = vakV.Code2211 ?? 0,
            PensioenDecember = vakV.Code2314 ?? 0,
            PensioenAchterstallen = (vakV.Code2230 ?? 0) + (vakV.Code2231 ?? 0) + (vakV.Code2212 ?? 0),
            // Vervangingsinkomen
            Werkloosheid = (vakIV.Code2260 ?? 0) + (vakIV.Code2264 ?? 0)
                         + (vakIV.Code2261 ?? 0) + (vakIV.Code2265 ?? 0),
            ZiekteInvaliditeit = (vakIV.Code2266 ?? 0) + (vakIV.Code2303 ?? 0) + (vakIV.Code2268 ?? 0),
            Beroepsziekte = vakIV.Code2270 ?? 0,
            AndereVervangings = vakIV.Code2271 ?? 0,
            // Kosten & voorheffingen
            WerkelijkeKosten = vakIV.Code2258 ?? 0,
            Bedrijfsvoorheffing = (vakIV.Code2286 ?? 0) + (vakV.Code2225 ?? 0),
            BijzondereBijdrageSZ = vakIV.Code2287 ?? 0,
            WerkbonusCode284 = vakIV.Code2284 ?? 0,
            WerkbonusCode360 = vakIV.Code2360 ?? 0,
            // Verminderingen
            Kinderopvang = 0, // kinderopvang zit op Code1384, wordt later verdeeld
            Pensioensparen = vakX.Code2361 ?? 0,
            Giften = 0, // giften zit op Code1394, wordt later verdeeld

            // ── Deel 2: Bedrijfsleider (Vak XVI) ────────────────────
            BrutoBedrijfsleider = ExtractBrutoBedrijfsleider2(vakXVI),
            BedrijfsleiderSocialeBijdragen = vakXVI?.Code2454 ?? 0,
            BedrijfsleiderWerkelijkeKosten = vakXVI?.Code2453 ?? 0,
            BedrijfsleiderBedrijfsvoorheffing = vakXVI?.Code2421 ?? 0,
            BedrijfsleiderWerkbonus284 = vakXVI?.Code2419 ?? 0,
            BedrijfsleiderWerkbonus360 = vakXVI?.Code2430 ?? 0,

            // ── Deel 2: Winst zelfstandige (Vak XVII) ───────────────
            BrutoWinstZelfstandige = ExtractBrutoWinst2(vakXVII),
            WinstSocialeBijdragen = vakXVII?.Code2632 ?? 0,
            WinstBeroepskosten = (vakXVII?.Code2620 ?? 0) + (vakXVII?.Code2611 ?? 0)
                               + (vakXVII?.Code2606 ?? 0),
            WinstVrijstellingen = (vakXVII?.Code2609 ?? 0) + (vakXVII?.Code2608 ?? 0)
                                + (vakXVII?.Code2612 ?? 0) + (vakXVII?.Code2633 ?? 0)
                                + (vakXVII?.Code2614 ?? 0),

            // ── Deel 2: Baten vrij beroep (Vak XVIII) ───────────────
            BrutoBatenVrijBeroep = ExtractBrutoBaten2(vakXVIII),
            BatenSocialeBijdragen = vakXVIII?.Code2656 ?? 0,
            BatenBeroepskosten = (vakXVIII?.Code2669 ?? 0) + (vakXVIII?.Code2657 ?? 0),
            BatenVrijstellingen = (vakXVIII?.Code2681 ?? 0) + (vakXVIII?.Code2662 ?? 0)
                                + (vakXVIII?.Code2660 ?? 0) + (vakXVIII?.Code2664 ?? 0)
                                + (vakXVIII?.Code2665 ?? 0) + (vakXVIII?.Code2666 ?? 0),

            // ── Deel 2: Meewerkende echtgenoot (Vak XX) ─────────────
            BrutoMeewerkend = vakXX?.Code2450 ?? 0,
            MeewerkendSocialeBijdragen = vakXX?.Code2451 ?? 0,
            MeewerkendBeroepskosten = vakXX?.Code2452 ?? 0,

            // ── Deel 2: Afzonderlijk belastbaar ─────────────────────
            Afzonderlijk10Pct = (vakXXI?.Code2686 ?? 0),
            Afzonderlijk12_5Pct = (vakXVII?.Code2607 ?? 0) + (vakXXI?.Code2687 ?? 0),
            Afzonderlijk16_5Pct = (vakXVII?.Code2603 ?? 0) + (vakXVII?.Code2636 ?? 0)
                                + (vakXVII?.Code2605 ?? 0) + (vakXVIII?.Code2653 ?? 0)
                                + (vakXVIII?.Code2682 ?? 0) + (vakXVIII?.Code2655 ?? 0)
                                + (vakXXI?.Code2690 ?? 0) + (vakXXI?.Code2694 ?? 0),
            Afzonderlijk33Pct = (vakXVII?.Code2618 ?? 0) + (vakXVIII?.Code2667 ?? 0)
                              + (vakXXI?.Code2691 ?? 0),

            // ── Deel 2: Diverse inkomsten (Vak XV) ──────────────────
            DiverseInkomstenGezamenlijk = (vakXV?.Code2460 ?? 0) + (vakXV?.Code2200 ?? 0)
                                        + (vakXV?.Code2172 ?? 0) + (vakXV?.Code2171 ?? 0),
            DiverseInkomsten16_5Pct = (vakXV?.Code2462 ?? 0) + (vakXV?.Code2177 ?? 0)
                                    + (vakXV?.Code2178 ?? 0),
            DiverseInkomsten33Pct = (vakXV?.Code2461 ?? 0) + (vakXV?.Code2463 ?? 0)
                                  + (vakXV?.Code2169 ?? 0) + (vakXV?.Code2175 ?? 0)
                                  + (vakXV?.Code2176 ?? 0),

            // ── Deel 2: Voorheffingen zelfstandigen (Vak XIX) ───────
            Deel2Bedrijfsvoorheffing = vakXIX?.Code2758 ?? 0,
            Deel2RoerendeVoorheffing = vakXIX?.Code2756 ?? 0,
        };
    }

    // ── Bruto extractie helpers ─────────────────────────────────────────

    private static decimal ExtractBrutoBedrijfsleider1(VakXVIData? v) =>
        v == null ? 0 : (v.Code1400 ?? 0) + (v.Code1401 ?? 0) + (v.Code1402 ?? 0)
                      + (v.Code1410 ?? 0) + (v.Code1411 ?? 0) + (v.Code1439 ?? 0);

    private static decimal ExtractBrutoBedrijfsleider2(VakXVIData? v) =>
        v == null ? 0 : (v.Code2400 ?? 0) + (v.Code2401 ?? 0) + (v.Code2402 ?? 0)
                      + (v.Code2410 ?? 0) + (v.Code2411 ?? 0) + (v.Code2439 ?? 0);

    private static decimal ExtractBrutoWinst1(VakXVIIData? v) =>
        v == null ? 0 : (v.Code1600 ?? 0) + (v.Code1601 ?? 0) + (v.Code1602 ?? 0)
                      + (v.Code1604 ?? 0) + (v.Code1615 ?? 0) + (v.Code1610 ?? 0)
                      + (v.Code1637 ?? 0);

    private static decimal ExtractBrutoWinst2(VakXVIIData? v) =>
        v == null ? 0 : (v.Code2600 ?? 0) + (v.Code2601 ?? 0) + (v.Code2602 ?? 0)
                      + (v.Code2604 ?? 0) + (v.Code2615 ?? 0) + (v.Code2610 ?? 0)
                      + (v.Code2637 ?? 0);

    private static decimal ExtractBrutoBaten1(VakXVIIIData? v) =>
        v == null ? 0 : (v.Code1650 ?? 0) + (v.Code1658 ?? 0) + (v.Code1659 ?? 0)
                      + (v.Code1652 ?? 0) + (v.Code1651 ?? 0) + (v.Code1654 ?? 0)
                      + (v.Code1674 ?? 0) + (v.Code1661 ?? 0) + (v.Code1683 ?? 0);

    private static decimal ExtractBrutoBaten2(VakXVIIIData? v) =>
        v == null ? 0 : (v.Code2650 ?? 0) + (v.Code2658 ?? 0) + (v.Code2659 ?? 0)
                      + (v.Code2652 ?? 0) + (v.Code2651 ?? 0) + (v.Code2654 ?? 0)
                      + (v.Code2674 ?? 0) + (v.Code2661 ?? 0) + (v.Code2683 ?? 0);

    /// <summary>Totaal bruto inkomen Deel 1.</summary>
    public decimal BrutoTotaalDeel1 => BrutoBeroepsinkomen + BrutoPensioeninkomen + BrutoVervangingsinkomen;

    /// <summary>Totaal bruto gezamenlijk belastbaar inkomen Deel 2.</summary>
    public decimal BrutoTotaalDeel2Gezamenlijk =>
        BrutoBedrijfsleider + BrutoWinstZelfstandige + BrutoBatenVrijBeroep
        + BrutoMeewerkend + DiverseInkomstenGezamenlijk;

    /// <summary>Totaal bruto afzonderlijk belastbaar inkomen Deel 2.</summary>
    public decimal BrutoTotaalDeel2Afzonderlijk =>
        Afzonderlijk10Pct + Afzonderlijk12_5Pct + Afzonderlijk16_5Pct + Afzonderlijk33Pct
        + DiverseInkomsten16_5Pct + DiverseInkomsten33Pct;

    /// <summary>Totaal bruto inkomen over alle categorieën (Deel 1 + Deel 2 gezamenlijk).</summary>
    public decimal BrutoTotaal => BrutoTotaalDeel1 + BrutoTotaalDeel2Gezamenlijk;

    /// <summary>Heeft deze partner enig inkomen?</summary>
    public bool HeeftInkomen => BrutoTotaal > 0 || BrutoTotaalDeel2Afzonderlijk > 0;

    /// <summary>Heeft deze partner Deel 2 inkomen?</summary>
    public bool HeeftDeel2Inkomen => BrutoTotaalDeel2Gezamenlijk > 0 || BrutoTotaalDeel2Afzonderlijk > 0;
}
