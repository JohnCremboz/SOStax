namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Inkomstengegevens voor één partner, geëxtraheerd uit de gemeenschappelijke vakdata.
/// </summary>
public class PartnerInkomen
{
    public string Label { get; init; } = "Belastingplichtige";

    // ── Onroerend inkomen (Vak III) ─────────────────────────────────────
    public decimal BrutoOnroerendInkomen { get; init; }

    // ── Deel 1: Beroepsinkomen ──────────────────────────────────────────
    public decimal BrutoLoon { get; init; }
    public decimal WoonWerkVerkeerTotaal { get; init; }
    public decimal WoonWerkVerkeerVrijstelling { get; init; }
    public decimal FlexiJob { get; init; }
    public decimal Impulsfonds { get; init; }

    /// <summary>Niet-recurrente resultaatsgebonden voordelen (Code1242/2242). Vrijstelling tot €3.622 via VrijstellingNietRecurrent.</summary>
    public decimal NietRecurrentVoordelen { get; init; }

    /// <summary>Werkgeverstussenkomst privé-pc/internet netto belastbaar: max(Code1240/2240 − Code1241/2241, 0).</summary>
    public decimal PrivePcNetto { get; init; }

    /// <summary>
    /// Bruto vervroegd vakantiegeld (Code1251/2251) en achterstallen loon (Code1252/2252).
    /// Afzonderlijk belastbaar aan de gemiddelde aanslagvoet (art. 171 WIB92).
    /// Worden meegenomen in de forfaitbasis maar netto apart belast.
    /// </summary>
    public decimal AfzonderlijkGemiddeldTariefBruto { get; init; }

    public decimal BrutoBeroepsinkomen =>
        BrutoLoon + FlexiJob + Impulsfonds + AfzonderlijkGemiddeldTariefBruto
        + Math.Max(WoonWerkVerkeerTotaal - WoonWerkVerkeerVrijstelling, 0)
        + NietRecurrentVoordelen
        + PrivePcNetto;

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
    public decimal ZiekteAchterstallen { get; init; }  // Code1268/2268: afzonderlijk belastbaar aan gemiddeld tarief
    public decimal Beroepsziekte { get; init; }
    public decimal AndereVervangings { get; init; }

    public decimal BrutoVervangingsinkomen =>
        Werkloosheid + ZiekteInvaliditeit + Beroepsziekte + AndereVervangings;

    // Overdraagbare beroepsverliezen van vorige jaren (Code1349/2349)
    public decimal VorigeVerliezen { get; init; }

    // Kosten & voorheffingen
    public decimal WerkelijkeKosten { get; init; }
    public decimal Bedrijfsvoorheffing { get; init; }
    public decimal BijzondereBijdrageSZ { get; init; }

    // Werkbonus RSZ-bedragen per tarief (voor berekening belastingkrediet)
    public decimal WerkbonusCode284 { get; init; }  // RSZ werkbonus aan 33,14%
    public decimal WerkbonusCode360 { get; init; }  // RSZ werkbonus aan 52,54%

    // Overwerktoeslag (Code1234/2234): grondslag 57,75% vermindering (art. 154quater WIB92)
    public decimal OverwerktoeslagCode1234 { get; init; }

    // Verminderingen
    public decimal Kinderopvang { get; set; }
    public decimal Pensioensparen { get; init; }
    public decimal Giften { get; init; }

    // ── Onroerend inkomen (Vak III) ──────────────────────────────────────────
    /// <summary>
    /// Belastbaar onroerend inkomen: geïndexeerd KI × 1,4 voor verhuur aan particulieren
    /// en netto huurinkomen (brutohuur − 40% forfait) voor professionele verhuur.
    /// </summary>
    public decimal OnroerendInkomen { get; init; }
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
    public decimal WinstMeerwaarden16_5 { get; init; }  // Code1603+1636+1605 (bruto, vóór forfait)
    public decimal WinstMeerwaarden33 { get; init; }    // Code1618 (bruto, vóór forfait)

    // ── Deel 2: Baten vrije beroepen (Vak XVIII) ────────────────────────
    public decimal BrutoBatenVrijBeroep { get; init; }
    public decimal BatenSocialeBijdragen { get; init; }
    public decimal BatenBeroepskosten { get; init; }
    public decimal BatenVrijstellingen { get; init; }
    public decimal BatenMeerwaarden16_5 { get; init; }  // Code1653+1682+1655 (bruto)
    public decimal BatenMeerwaarden33 { get; init; }    // Code1667 (bruto)

    // ── Deel 2: Meewerkende echtgenoot (Vak XX) ─────────────────────────
    public decimal BrutoMeewerkend { get; init; }
    public decimal MeewerkendSocialeBijdragen { get; init; }
    public decimal MeewerkendBeroepskosten { get; init; }

    // ── Deel 2: Afzonderlijk belastbare inkomsten ───────────────────────
    public decimal Afzonderlijk10Pct { get; init; }     // stopzetting 60+/gedwongen (Vak XXI)
    public decimal Afzonderlijk12_5Pct { get; init; }   // Europese landbouwsubsidies
    public decimal Afzonderlijk16_5Pct { get; init; }   // meerwaarden ≥5j, achterstallen, COVID
    public decimal Afzonderlijk33Pct { get; init; }     // meerwaarden <5j, immaterieel

    /// <summary>
    /// Ontvangen onderhoudsgeld gezamenlijk belastbaar (AJ2026: 70% van Code1192/2192 + Code1194/2194).
    /// Divers inkomen, geen beroepskosten van toepassing.
    /// </summary>
    public decimal OnderhoudsgeldGezamenlijk { get; init; }

    /// <summary>
    /// Onderhoudsgeld met terugwerkende kracht (AJ2026: 70% van Code1193/2193).
    /// Afzonderlijk belastbaar aan de gemiddelde aanslagvoet (art. 90 §2bis WIB92).
    /// </summary>
    public decimal OnderhoudsgeldAfzonderlijk { get; init; }
    public decimal DiverseInkomstenGezamenlijk { get; init; }  // gezamenlijk belastbaar
    public decimal DiverseInkomsten16_5Pct { get; init; }      // afzonderlijk 16,5%
    public decimal DiverseInkomsten33Pct { get; init; }        // afzonderlijk 33%
    /// <summary>Deeleconomie bruto (Code1200/2200). Netto = 50% van bruto; belasting 20% afzonderlijk.</summary>
    public decimal Deeleconomie { get; init; }

    // ── Deel 2: Voorheffingen zelfstandigen (Vak XIX) ───────────────────
    public decimal Deel2Bedrijfsvoorheffing { get; init; }
    public decimal Deel2RoerendeVoorheffing { get; init; }

    // ── Voorafbetalingen (Vak XII) ───────────────────────────────────────
    public decimal Voorafbetalingen { get; init; }

    // ── Gewestelijke verminderingen (Vak IX) ─────────────────────────────
    // Rubriek 1: Geïntegreerde woonbonus (leningen 2016–2019)
    // Code3334/4334 = interesten + kapitaalaflossingen (gecombineerd in één bedrag)
    public decimal GeintWoonbonusInteresten { get; init; }
    public decimal GeintWoonbonusKapitaal { get; init; }   // altijd 0 — begrepen in GeintWoonbonusInteresten
    public decimal GeintWoonbonusPremies { get; init; }
    public int GeintWoonbonusKinderen { get; init; }
    public string? GeintWoonbonusDatumLening { get; init; } // niet gebruikt voor R1 (altijd <10j in AJ2026)

    // Rubriek 2a: Gewestelijke woonbonus — leningen 2015 (max korf €1.520)
    public decimal WoonbonusKorf2015 { get; init; }
    // Rubriek 2b: Gewestelijke woonbonus — leningen 2005–2014 (max korf €2.280)
    public decimal WoonbonusKorf2005_2014 { get; init; }

    // Rubriek 4: Gewestelijke vermindering interestaftrek — leningen 1986–2005
    public decimal Woonbonus1986Interesten { get; init; }
    public int Woonbonus1986Kinderen { get; init; }

    // Bouwsparen / langetermijnsparen gewestelijk (Rubrieks 6 + 7): 30%
    public decimal BouwsparenKapitaal { get; init; }
    public decimal BouwsparenPremies { get; init; }

    // Federaal langetermijnsparen (niet-eigen woning)
    public decimal FederaalLTKapitaal { get; init; }
    public decimal FederaalLTPremies { get; init; }

    /// <summary>
    /// Extraheert de gegevens van partner 1 (Code1xxx) uit de vakdata.
    /// </summary>
    public static PartnerInkomen ExtractBelastingplichtige(
        VakIVData vakIV, VakVData vakV, VakXData vakX,
        VakXVData? vakXV = null, VakXVIData? vakXVI = null,
        VakXVIIData? vakXVII = null, VakXVIIIData? vakXVIII = null,
        VakXIXData? vakXIX = null, VakXXData? vakXX = null,
        VakXXIData? vakXXI = null, VakIXData? vakIX = null,
<<<<<<< HEAD
        VakXIIData? vakXII = null, VakIIIData? vakIII = null,
        VakVIData? vakVI = null)
    {
        return new PartnerInkomen
        {
            Label = "Belastingplichtige",
            // ── Onroerende inkomsten (Vak III) ──────────────────────
            OnroerendInkomen = BerekenOnroerendInkomenPartner(vakIII, isPartner1: true),
            // ── Deel 1: Beroep ──────────────────────────────────────
            BrutoLoon = vakIV.Total1250 + (vakIV.Code1247 ?? 0),
            AfzonderlijkGemiddeldTariefBruto = (vakIV.Code1251 ?? 0) + (vakIV.Code1252 ?? 0)
                                               + (vakIV.Code1308 ?? 0),
            WoonWerkVerkeerTotaal = vakIV.Code1254 ?? 0,
            WoonWerkVerkeerVrijstelling = vakIV.Code1255 ?? 0,
            FlexiJob = vakIV.Code1262 ?? 0,
            Impulsfonds = vakIV.Code1267 ?? 0,
            NietRecurrentVoordelen = vakIV.Code1242 ?? 0,
            PrivePcNetto = Math.Max((vakIV.Code1240 ?? 0) - (vakIV.Code1241 ?? 0), 0),
            WettelijkPensioen = vakV.Code1228 ?? 0,
            Overlevingspensioen = vakV.Code1229 ?? 0,
            AnderePensioenen = vakV.Code1211 ?? 0,
            PensioenDecember = vakV.Code1314 ?? 0,
            PensioenAchterstallen = (vakV.Code1230 ?? 0) + (vakV.Code1231 ?? 0) + (vakV.Code1212 ?? 0),
            // Vervangingsinkomen
            Werkloosheid = (vakIV.Code1260 ?? 0) + (vakIV.Code1264 ?? 0)
                         + (vakIV.Code1261 ?? 0) + (vakIV.Code1265 ?? 0),
            ZiekteInvaliditeit = (vakIV.Code1266 ?? 0) + (vakIV.Code1303 ?? 0),
            ZiekteAchterstallen = vakIV.Code1268 ?? 0,
            Beroepsziekte = vakIV.Code1270 ?? 0,
            AndereVervangings = (vakIV.Code1271 ?? 0) + (vakIV.Code1269 ?? 0),
            // Overdraagbare beroepsverliezen
            VorigeVerliezen = vakIV.Code1349 ?? 0,

            // Kosten & voorheffingen
            WerkelijkeKosten = vakIV.Code1258 ?? 0,
            Bedrijfsvoorheffing = vakIV.Total1286 + vakV.Code1225,
            BijzondereBijdrageSZ = vakIV.Code1287 ?? 0,
            WerkbonusCode284 = vakIV.Code1284 ?? 0,
            WerkbonusCode360 = vakIV.Code1360 ?? 0,
            OverwerktoeslagCode1234 = vakIV.Code1234 ?? 0,
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
            WinstMeerwaarden16_5 = (vakXVII?.Code1603 ?? 0) + (vakXVII?.Code1636 ?? 0)
                                  + (vakXVII?.Code1605 ?? 0),
            WinstMeerwaarden33 = vakXVII?.Code1618 ?? 0,

            // ── Deel 2: Baten vrij beroep (Vak XVIII) ───────────────
            BrutoBatenVrijBeroep = ExtractBrutoBaten1(vakXVIII),
            BatenSocialeBijdragen = vakXVIII?.Code1656 ?? 0,
            BatenBeroepskosten = (vakXVIII?.Code1669 ?? 0) + (vakXVIII?.Code1657 ?? 0),
            BatenVrijstellingen = (vakXVIII?.Code1681 ?? 0) + (vakXVIII?.Code1662 ?? 0)
                                + (vakXVIII?.Code1660 ?? 0) + (vakXVIII?.Code1664 ?? 0)
                                + (vakXVIII?.Code1665 ?? 0) + (vakXVIII?.Code1666 ?? 0),
            BatenMeerwaarden16_5 = (vakXVIII?.Code1653 ?? 0) + (vakXVIII?.Code1682 ?? 0)
                                  + (vakXVIII?.Code1655 ?? 0),
            BatenMeerwaarden33 = vakXVIII?.Code1667 ?? 0,

            // ── Deel 2: Meewerkende echtgenoot (Vak XX) ─────────────
            BrutoMeewerkend = vakXX?.Code1450 ?? 0,
            MeewerkendSocialeBijdragen = vakXX?.Code1451 ?? 0,
            MeewerkendBeroepskosten = vakXX?.Code1452 ?? 0,

            // ── Deel 2: Afzonderlijk belastbaar ─────────────────────
            // Winst/baten meerwaarden worden apart getrackt en proportioneel
            // verminderd met forfait in PartnerBelastingCalculator
            Afzonderlijk10Pct = (vakXXI?.Code1686 ?? 0),
            Afzonderlijk12_5Pct = (vakXVII?.Code1607 ?? 0) + (vakXXI?.Code1687 ?? 0),
            Afzonderlijk16_5Pct = (vakXXI?.Code1690 ?? 0) + (vakXXI?.Code1694 ?? 0),
            Afzonderlijk33Pct = (vakXXI?.Code1691 ?? 0),

            // ── Deel 2: Diverse inkomsten (Vak XV) ──────────────────
            DiverseInkomstenGezamenlijk = (vakXV?.Code1460 ?? 0)
                                        + (vakXV?.Code1172 ?? 0) + (vakXV?.Code1171 ?? 0),
            DiverseInkomsten16_5Pct = (vakXV?.Code1462 ?? 0) + (vakXV?.Code1177 ?? 0)
                                    + (vakXV?.Code1178 ?? 0),
            DiverseInkomsten33Pct = (vakXV?.Code1461 ?? 0) + (vakXV?.Code1463 ?? 0)
                                  + (vakXV?.Code1169 ?? 0) + (vakXV?.Code1175 ?? 0)
                                  + (vakXV?.Code1176 ?? 0),
            Deeleconomie = vakXV?.Code1200 ?? 0,

            // ── Onderhoudsgeld (Vak VI) ──────────────────────────────
            // AJ2026: 70% belastbaar (art. 90 §1 3° WIB92)
            OnderhoudsgeldGezamenlijk = Math.Round(
                ((vakVI?.Code1192 ?? 0) + (vakVI?.Code1194 ?? 0)) * TaxConstants2026.OnderhoudsgeldBelastbaarPercentage, 2),
            OnderhoudsgeldAfzonderlijk = Math.Round(
                (vakVI?.Code1193 ?? 0) * TaxConstants2026.OnderhoudsgeldBelastbaarPercentage, 2),

            // ── Deel 2: Voorheffingen zelfstandigen (Vak XIX) ───────
            Deel2Bedrijfsvoorheffing = vakXIX?.Code1758 ?? 0,
            Deel2RoerendeVoorheffing = vakXIX?.Code1756 ?? 0,

            // ── Voorafbetalingen (Vak XII) ───────────────────────────
            Voorafbetalingen = vakXII?.Code1570 ?? 0,

            // ── Gewestelijke verminderingen (Vak IX) ─────────────────
            // R1: Geïntegreerde woonbonus 2016–2019 (Code3334 = interesten+kapitaal, Code3335 = premies)
            GeintWoonbonusInteresten = (vakIX?.Code3334 ?? 0),
            GeintWoonbonusKapitaal = 0,  // inbegrepen in Code3334
            GeintWoonbonusPremies = (vakIX?.Code3335 ?? 0),
            GeintWoonbonusKinderen = (int)(vakIX?.Code3330 ?? 0),
            GeintWoonbonusDatumLening = null,
            // R2a: Gewestelijke woonbonus leningen 2015
            WoonbonusKorf2015 = (vakIX?.Code3360 ?? 0) + (vakIX?.Code3361 ?? 0),
            // R2b: Gewestelijke woonbonus leningen 2005–2014
            WoonbonusKorf2005_2014 = (vakIX?.Code3370 ?? 0) + (vakIX?.Code3371 ?? 0),
            // R4: Interestaftrek leningen 1986–2005
            Woonbonus1986Interesten = (vakIX?.Code3138 ?? 0) + (vakIX?.Code3139 ?? 0),
            Woonbonus1986Kinderen = vakIX?.Code3142 ?? 0,
            // Bouwsparen + langetermijnsparen (Rubrieks 6+7): Code3355/3356 + Code3358 kapitaal, Code3351–3354 premies
            BouwsparenKapitaal = (vakIX?.Code3355 ?? 0) + (vakIX?.Code3356 ?? 0) + (vakIX?.Code3358 ?? 0),
            BouwsparenPremies = (vakIX?.Code3351 ?? 0) + (vakIX?.Code3352 ?? 0)
                              + (vakIX?.Code3353 ?? 0) + (vakIX?.Code3354 ?? 0),
            FederaalLTKapitaal = (vakIX?.Code1358 ?? 0),
            FederaalLTPremies = (vakIX?.Code1353 ?? 0),
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
        VakXXIData? vakXXI = null, VakIXData? vakIX = null,
<<<<<<< HEAD
        VakXIIData? vakXII = null, VakIIIData? vakIII = null,
        VakVIData? vakVI = null)
    {
        return new PartnerInkomen
        {
            Label = "Partner",
            // ── Onroerende inkomsten (Vak III) ──────────────────────
            OnroerendInkomen = BerekenOnroerendInkomenPartner(vakIII, isPartner1: false),
            // ── Deel 1: Beroep ──────────────────────────────────────
            BrutoLoon = vakIV.Total2250 + (vakIV.Code2247 ?? 0),
            AfzonderlijkGemiddeldTariefBruto = (vakIV.Code2251 ?? 0) + (vakIV.Code2252 ?? 0)
                                               + (vakIV.Code2308 ?? 0),
            WoonWerkVerkeerTotaal = vakIV.Code2254 ?? 0,
            WoonWerkVerkeerVrijstelling = vakIV.Code2255 ?? 0,
            FlexiJob = vakIV.Code2262 ?? 0,
            Impulsfonds = vakIV.Code2267 ?? 0,
            NietRecurrentVoordelen = vakIV.Code2242 ?? 0,
            PrivePcNetto = Math.Max((vakIV.Code2240 ?? 0) - (vakIV.Code2241 ?? 0), 0),
            WettelijkPensioen = vakV.Code2228 ?? 0,
            Overlevingspensioen = vakV.Code2229 ?? 0,
            AnderePensioenen = vakV.Code2211 ?? 0,
            PensioenDecember = vakV.Code2314 ?? 0,
            PensioenAchterstallen = (vakV.Code2230 ?? 0) + (vakV.Code2231 ?? 0) + (vakV.Code2212 ?? 0),
            // Vervangingsinkomen
            Werkloosheid = (vakIV.Code2260 ?? 0) + (vakIV.Code2264 ?? 0)
                         + (vakIV.Code2261 ?? 0) + (vakIV.Code2265 ?? 0),
            ZiekteInvaliditeit = (vakIV.Code2266 ?? 0) + (vakIV.Code2303 ?? 0),
            ZiekteAchterstallen = vakIV.Code2268 ?? 0,
            Beroepsziekte = vakIV.Code2270 ?? 0,
            AndereVervangings = (vakIV.Code2271 ?? 0) + (vakIV.Code2269 ?? 0),
            // Overdraagbare beroepsverliezen
            VorigeVerliezen = vakIV.Code2349 ?? 0,

            // Kosten & voorheffingen
            WerkelijkeKosten = vakIV.Code2258 ?? 0,
            Bedrijfsvoorheffing = vakIV.Total2286 + vakV.Code2225,
            BijzondereBijdrageSZ = vakIV.Code2287 ?? 0,
            WerkbonusCode284 = vakIV.Code2284 ?? 0,
            WerkbonusCode360 = vakIV.Code2360 ?? 0,
            OverwerktoeslagCode1234 = vakIV.Code2234 ?? 0,
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
            WinstMeerwaarden16_5 = (vakXVII?.Code2603 ?? 0) + (vakXVII?.Code2636 ?? 0)
                                  + (vakXVII?.Code2605 ?? 0),
            WinstMeerwaarden33 = vakXVII?.Code2618 ?? 0,

            // ── Deel 2: Baten vrij beroep (Vak XVIII) ───────────────
            BrutoBatenVrijBeroep = ExtractBrutoBaten2(vakXVIII),
            BatenSocialeBijdragen = vakXVIII?.Code2656 ?? 0,
            BatenBeroepskosten = (vakXVIII?.Code2669 ?? 0) + (vakXVIII?.Code2657 ?? 0),
            BatenVrijstellingen = (vakXVIII?.Code2681 ?? 0) + (vakXVIII?.Code2662 ?? 0)
                                + (vakXVIII?.Code2660 ?? 0) + (vakXVIII?.Code2664 ?? 0)
                                + (vakXVIII?.Code2665 ?? 0) + (vakXVIII?.Code2666 ?? 0),
            BatenMeerwaarden16_5 = (vakXVIII?.Code2653 ?? 0) + (vakXVIII?.Code2682 ?? 0)
                                  + (vakXVIII?.Code2655 ?? 0),
            BatenMeerwaarden33 = vakXVIII?.Code2667 ?? 0,

            // ── Deel 2: Meewerkende echtgenoot (Vak XX) ─────────────
            BrutoMeewerkend = vakXX?.Code2450 ?? 0,
            MeewerkendSocialeBijdragen = vakXX?.Code2451 ?? 0,
            MeewerkendBeroepskosten = vakXX?.Code2452 ?? 0,

            // ── Deel 2: Afzonderlijk belastbaar ─────────────────────
            Afzonderlijk10Pct = (vakXXI?.Code2686 ?? 0),
            Afzonderlijk12_5Pct = (vakXVII?.Code2607 ?? 0) + (vakXXI?.Code2687 ?? 0),
            Afzonderlijk16_5Pct = (vakXXI?.Code2690 ?? 0) + (vakXXI?.Code2694 ?? 0),
            Afzonderlijk33Pct = (vakXXI?.Code2691 ?? 0),

            // ── Deel 2: Diverse inkomsten (Vak XV) ──────────────────
            DiverseInkomstenGezamenlijk = (vakXV?.Code2460 ?? 0)
                                        + (vakXV?.Code2172 ?? 0) + (vakXV?.Code2171 ?? 0),
            DiverseInkomsten16_5Pct = (vakXV?.Code2462 ?? 0) + (vakXV?.Code2177 ?? 0)
                                    + (vakXV?.Code2178 ?? 0),
            DiverseInkomsten33Pct = (vakXV?.Code2461 ?? 0) + (vakXV?.Code2463 ?? 0)
                                  + (vakXV?.Code2169 ?? 0) + (vakXV?.Code2175 ?? 0)
                                  + (vakXV?.Code2176 ?? 0),
            Deeleconomie = vakXV?.Code2200 ?? 0,

            // ── Onderhoudsgeld (Vak VI) ──────────────────────────────
            // AJ2026: 70% belastbaar (art. 90 §1 3° WIB92)
            OnderhoudsgeldGezamenlijk = Math.Round(
                ((vakVI?.Code2192 ?? 0) + (vakVI?.Code2194 ?? 0)) * TaxConstants2026.OnderhoudsgeldBelastbaarPercentage, 2),
            OnderhoudsgeldAfzonderlijk = Math.Round(
                (vakVI?.Code2193 ?? 0) * TaxConstants2026.OnderhoudsgeldBelastbaarPercentage, 2),

            // ── Deel 2: Voorheffingen zelfstandigen (Vak XIX) ───────
            Deel2Bedrijfsvoorheffing = vakXIX?.Code2758 ?? 0,
            Deel2RoerendeVoorheffing = vakXIX?.Code2756 ?? 0,

            // ── Voorafbetalingen (Vak XII) ───────────────────────────
            Voorafbetalingen = vakXII?.Code2570 ?? 0,

            // ── Gewestelijke verminderingen (Vak IX) ─────────────────
            // R1: Geïntegreerde woonbonus 2016–2019 (Code4334 = interesten+kapitaal, Code4335 = premies)
            GeintWoonbonusInteresten = (vakIX?.Code4334 ?? 0),
            GeintWoonbonusKapitaal = 0,  // inbegrepen in Code4334
            GeintWoonbonusPremies = (vakIX?.Code4335 ?? 0),
            GeintWoonbonusKinderen = (int)(vakIX?.Code4330 ?? 0),
            GeintWoonbonusDatumLening = null,
            // R2a: Gewestelijke woonbonus leningen 2015
            WoonbonusKorf2015 = (vakIX?.Code4360 ?? 0) + (vakIX?.Code4361 ?? 0),
            // R2b: Gewestelijke woonbonus leningen 2005–2014
            WoonbonusKorf2005_2014 = (vakIX?.Code4370 ?? 0) + (vakIX?.Code4371 ?? 0),
            // R4: Interestaftrek leningen 1986–2005
            Woonbonus1986Interesten = (vakIX?.Code4138 ?? 0) + (vakIX?.Code4139 ?? 0),
            Woonbonus1986Kinderen = vakIX?.Code4142 ?? 0,
            // Bouwsparen + langetermijnsparen (Rubrieks 6+7): Code4355/4356+4358 kapitaal, Code4351–4354 premies
            BouwsparenKapitaal = (vakIX?.Code4355 ?? 0) + (vakIX?.Code4356 ?? 0) + (vakIX?.Code4358 ?? 0),
            BouwsparenPremies = (vakIX?.Code4351 ?? 0) + (vakIX?.Code4352 ?? 0)
                              + (vakIX?.Code4353 ?? 0) + (vakIX?.Code4354 ?? 0),
            FederaalLTKapitaal = (vakIX?.Code2358 ?? 0),
            FederaalLTPremies = (vakIX?.Code2353 ?? 0),
        };
    }

    // ── Bruto extractie helpers ─────────────────────────────────────────

    /// <summary>
    /// Berekent het belastbaar onroerend inkomen voor één partner op basis van Vak III.
    /// <list type="bullet">
    ///   <item>Codes 1106/1107/1108 (niet verhuurd / verhuur particulieren / pacht):
    ///         geïndexeerd KI × 1,4</item>
    ///   <item>Codes 1109+1110, 1112+1113, 1115+1116 (andere omstandigheden – professionele verhuur):
    ///         brutohuur − min(brutohuur × 40 %, 2/3 × geïndexeerd KI × 1,4)</item>
    /// </list>
    /// </summary>
    private static decimal BerekenOnroerendInkomenPartner(VakIIIData? vak, bool isPartner1)
    {
        if (vak == null) return 0m;

        decimal totaal = 0m;

        // ── KI-codes: geïndexeerd KI × 1,4 ─────────────────────────────────
        var kiCodes = isPartner1
            ? new[] { vak.Code1106, vak.Code1107, vak.Code1108 }
            : new[] { vak.Code2106, vak.Code2107, vak.Code2108 };

        foreach (var ki in kiCodes)
        {
            if ((ki ?? 0) > 0)
            {
                var geindexeerdKI = Math.Round(ki!.Value * TaxConstants2026.IndexatiecoeffKI, 0, MidpointRounding.AwayFromZero);
                totaal += geindexeerdKI * TaxConstants2026.KIVermenigvuldigingsfactor;
            }
        }

        // ── Andere omstandigheden (professioneel gebruik huurder): nettohuur ─
        // Gebouwen: 1109 (KI) + 1110 (brutohuur)
        // Gronden:  1112 (KI) + 1113 (brutohuur)
        // Materieel:1115 (KI) + 1116 (brutohuur)
        var huurParen = isPartner1
            ? new[] { (vak.Code1109, vak.Code1110), (vak.Code1112, vak.Code1113), (vak.Code1115, vak.Code1116) }
            : new[] { (vak.Code2109, vak.Code2110), (vak.Code2112, vak.Code2113), (vak.Code2115, vak.Code2116) };

        foreach (var (ki, brutohuur) in huurParen)
        {
            if ((brutohuur ?? 0) <= 0) continue;

            var huur = brutohuur!.Value;
            var geindexeerdKI = (ki ?? 0) > 0
                ? Math.Round(ki!.Value * TaxConstants2026.IndexatiecoeffKI, 0, MidpointRounding.AwayFromZero)
                : 0m;
            var maxForfait = 2m / 3m * geindexeerdKI * TaxConstants2026.KIVermenigvuldigingsfactor;
            var forfait = Math.Min(huur * 0.40m, maxForfait);
            totaal += huur - forfait;
        }

        return totaal;
    }

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
        + BrutoMeewerkend + DiverseInkomstenGezamenlijk + OnderhoudsgeldGezamenlijk;

    /// <summary>Totaal bruto afzonderlijk belastbaar inkomen Deel 2.</summary>
    public decimal BrutoTotaalDeel2Afzonderlijk =>
        Afzonderlijk10Pct + Afzonderlijk12_5Pct + Afzonderlijk16_5Pct + Afzonderlijk33Pct
        + WinstMeerwaarden16_5 + WinstMeerwaarden33
        + BatenMeerwaarden16_5 + BatenMeerwaarden33
        + DiverseInkomsten16_5Pct + DiverseInkomsten33Pct
        + Math.Round(Deeleconomie * (1m - TaxConstants2026.DeeleconomieKostenForfait), 2);

    /// <summary>Totaal bruto inkomen over alle categorieën (Deel 1 + Deel 2 gezamenlijk + onroerend + onderhoudsgeld afzonderlijk).</summary>
    public decimal BrutoTotaal => BrutoTotaalDeel1 + BrutoTotaalDeel2Gezamenlijk + OnroerendInkomen + OnderhoudsgeldAfzonderlijk;

    /// <summary>Heeft deze partner enig inkomen?</summary>
    public bool HeeftInkomen => BrutoTotaal > 0 || BrutoTotaalDeel2Afzonderlijk > 0;

    /// <summary>Heeft deze partner Deel 2 inkomen?</summary>
    public bool HeeftDeel2Inkomen => BrutoTotaalDeel2Gezamenlijk > 0 || BrutoTotaalDeel2Afzonderlijk > 0;
}
