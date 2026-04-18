namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Inkomstengegevens voor één partner, geëxtraheerd uit de gemeenschappelijke vakdata.
/// </summary>
public class PartnerInkomen
{
    public string Label { get; init; } = "Belastingplichtige";

    // Beroepsinkomen
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

    /// <summary>
    /// Extraheert de gegevens van partner 1 (Code1xxx) uit de vakdata.
    /// </summary>
    public static PartnerInkomen ExtractBelastingplichtige(VakIVData vakIV, VakVData vakV, VakXData vakX)
    {
        return new PartnerInkomen
        {
            Label = "Belastingplichtige",
            // Beroep
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
        };
    }

    /// <summary>
    /// Extraheert de gegevens van partner 2 (Code2xxx) uit de vakdata.
    /// </summary>
    public static PartnerInkomen ExtractPartner(VakIVData vakIV, VakVData vakV, VakXData vakX)
    {
        return new PartnerInkomen
        {
            Label = "Partner",
            // Beroep
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
        };
    }

    /// <summary>Totaal bruto inkomen over alle categorieën.</summary>
    public decimal BrutoTotaal => BrutoBeroepsinkomen + BrutoPensioeninkomen + BrutoVervangingsinkomen;

    /// <summary>Heeft deze partner enig inkomen?</summary>
    public bool HeeftInkomen => BrutoTotaal > 0;
}
