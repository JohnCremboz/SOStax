using BlazorTax.Belastingen;
using BlazorTax.Belastingen.Berekening;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ══════════════════════════════════════════════════════════════════════
// 4 scenario's: zelfde data als Tax-Calc AJ2025, door onze AJ2026 engine
// Vergelijk structureel (niet op bedrag, want indexatie verschilt)
// ══════════════════════════════════════════════════════════════════════

var scenarios = new (string Naam, BerekeningInput Input, TaxCalcRef Ref)[]
{
    Scenario1_PensionarisBrussel(),
    Scenario2_WerknemerGent(),
    Scenario3_GehuwdMenen(),
    Scenario4_WerknemerAntwerpen(),
    Scenario5_BedrijfsleiderBrussel(),
    Scenario6_WerknemerWoonbonus(),
    Scenario7_PensionarisMenen_1txt(),
};

foreach (var (naam, input, taxCalcRef) in scenarios)
{
    Console.WriteLine($"\n{"".PadRight(70, '═')}");
    Console.WriteLine($"  {naam}");
    Console.WriteLine($"{"".PadRight(70, '═')}");

    var calc = new GezamenlijkeBerekeningCalculator();
    var result = calc.Bereken(input);

    // Print detail rules
    foreach (var regel in result.DetailRegels)
    {
        string prefix = regel.IsSubtotaal ? "► " : "  ";
        Console.WriteLine($"{prefix}{regel.Omschrijving,-45} {regel.Bedrag,12:N2}");
    }

    Console.WriteLine();

    // ── Vergelijking met Tax-Calc AJ2025 ──
    Console.WriteLine("  VERGELIJKING (onze AJ2026 vs Tax-Calc AJ2025):");
    Console.WriteLine("  " + "Stap".PadRight(40) + "AJ2026".PadLeft(12) + " " + "AJ2025".PadLeft(12) + " " + "Delta".PadLeft(10));
    Console.WriteLine("  " + new string('─', 76));

    var r1 = result.Belastingplichtige;
    var r2 = result.Partner;

    Compare("Netto BP", r1.NettoBelastbaarInkomen, taxCalcRef.NettoBP);
    if (result.IsGezamenlijk)
        Compare("Netto Partner", r2.NettoBelastbaarInkomen, taxCalcRef.NettoPartner);

    Compare("Basisbelasting BP", r1.Basisbelasting, taxCalcRef.BasisbelastingBP);
    Compare("Verm. vrije som BP", r1.VerminderingBelastingvrijeSom, taxCalcRef.VermVrijeSomBP);
    Compare("Om te slane BP", r1.OmTeSlane, taxCalcRef.OmTeSlaneBP);

    if (r1.VerminderingVervangingsinkomen > 0 || taxCalcRef.VermVervangingBP > 0)
        Compare("Verm. vervanging BP", r1.VerminderingVervangingsinkomen, taxCalcRef.VermVervangingBP);

    Compare("Hoofdsom BP", r1.Hoofdsom, taxCalcRef.HoofdsomBP);
    Compare("Gereduceerde Staat BP", r1.GereduceerdeStaat, taxCalcRef.GereduceerdeBP);
    Compare("Opcentiemen BP", r1.GewestelijkeOpcentiemen, taxCalcRef.OpcentiemenBP);

    if (result.IsGezamenlijk)
    {
        Compare("Basisbelasting Partner", r2.Basisbelasting, taxCalcRef.BasisbelastingP);
        Compare("Verm. vrije som Partner", r2.VerminderingBelastingvrijeSom, taxCalcRef.VermVrijeSomP);
        Compare("Om te slane Partner", r2.OmTeSlane, taxCalcRef.OmTeSlaneP);
        Compare("Gereduceerde Partner", r2.GereduceerdeStaat, taxCalcRef.GereduceerdeP);
        Compare("Opcentiemen Partner", r2.GewestelijkeOpcentiemen, taxCalcRef.OpcentiemenP);
    }

    decimal totaleBelasting = result.TotaalSaldoFederaal + result.TotaalSaldoGewestelijk
                            + result.Gemeentebelasting + result.BBSZSaldo;
    Compare("Totale belasting", totaleBelasting, taxCalcRef.TotaleBelasting);
    Compare("Gemeentebelasting", result.Gemeentebelasting, taxCalcRef.Gemeentebelasting);
    if (result.BBSZVerschuldigd > 0 || taxCalcRef.BBSZVerschuldigd > 0)
        Compare("BBSZ verschuldigd", result.BBSZVerschuldigd, taxCalcRef.BBSZVerschuldigd);
    Compare("EINDRESULTAAT", result.Eindresultaat, taxCalcRef.Eindresultaat);
}

void Compare(string label, decimal aj2026, decimal aj2025)
{
    decimal delta = aj2026 - aj2025;
    string sign = delta >= 0 ? "+" : "";
    Console.WriteLine($"  {label,-40} {aj2026,12:N2} {aj2025,12:N2} {sign}{delta,9:N2}");
}

// ══════════════════════════════════════════════════════════════════════
// Scenario definities
// ══════════════════════════════════════════════════════════════════════

static (string, BerekeningInput, TaxCalcRef) Scenario1_PensionarisBrussel()
{
    var input = new BerekeningInput
    {
        VakII = new VakIIData { BurgerlijkeStaat = "1001" },
        VakIII = new VakIIIData(),
        VakIV = new VakIVData { Fiches1286 = new() { 2886.03m } },
        VakV = new VakVData { Code1228 = 25000m },
        VakVIII = new VakVIIIData(),
        VakIX = new VakIXData(),
        VakX = new VakXData(),
        VakXII = new VakXIIData(),
        Gewest = Gewest.Brussel,
        GemeentebelastingPercentage = 6m,
        TypeBeroep = TypeBeroep.Werknemer,
    };

    var taxCalc = new TaxCalcRef
    {
        NettoBP = 25000m,
        BasisbelastingBP = 7627m,
        VermVrijeSomBP = 2642.50m,
        OmTeSlaneBP = 4984.50m,
        VermVervangingBP = 2297.14m,  // 2151.72 + 145.42
        HoofdsomBP = 2687.36m,
        GereduceerdeBP = 2016.68m,
        OpcentiemenBP = 657.26m,
        TotaleBelasting = 2673.94m + 160.44m,
        Gemeentebelasting = 160.44m,
        Eindresultaat = -51.65m,
    };

    return ("1: Pensionaris Brussel (€25k pensioen, BV €2886, 6%)", input, taxCalc);
}

static (string, BerekeningInput, TaxCalcRef) Scenario2_WerknemerGent()
{
    var input = new BerekeningInput
    {
        VakII = new VakIIData { BurgerlijkeStaat = "1001" },
        VakIII = new VakIIIData(),
        VakIV = new VakIVData
        {
            Fiches1250 = new() { 35000m },
            Fiches1286 = new() { 8500m },
            Code1287 = 250m,
        },
        VakV = new VakVData(),
        VakVIII = new VakVIIIData(),
        VakIX = new VakIXData(),
        VakX = new VakXData(),
        VakXII = new VakXIIData(),
        Gewest = Gewest.Vlaanderen,
        GemeentebelastingPercentage = 6.5m,
        TypeBeroep = TypeBeroep.Werknemer,
    };

    var taxCalc = new TaxCalcRef
    {
        NettoBP = 29250m,
        BasisbelastingBP = 9393.50m,
        VermVrijeSomBP = 2642.50m,
        OmTeSlaneBP = 6751m,
        HoofdsomBP = 6751m,
        GereduceerdeBP = 5066.15m,
        OpcentiemenBP = 1684.85m,
        TotaleBelasting = 6751m + 438.82m + 230.28m,
        Gemeentebelasting = 438.82m,
        BBSZVerschuldigd = 230.28m,
        Eindresultaat = -1079.90m,
    };

    return ("2: Werknemer Gent (€35k loon, BV €8500, 6.5%)", input, taxCalc);
}

static (string, BerekeningInput, TaxCalcRef) Scenario3_GehuwdMenen()
{
    var input = new BerekeningInput
    {
        VakII = new VakIIData
        {
            BurgerlijkeStaat = "1002",
            Code1030 = 3,
            Code1031 = 1,
        },
        VakIII = new VakIIIData(),
        VakIV = new VakIVData
        {
            Fiches1250 = new() { 40193.95m },
            Fiches1286 = new() { 8948.90m },
            Code1287 = 324.21m,
            Fiches2250 = new() { 21794.78m },
            Fiches2286 = new() { 184.66m },
            Code2287 = 96.45m,
            Code2284 = 1516.77m,
            Code2360 = 1142.19m,
        },
        VakV = new VakVData(),
        VakVIII = new VakVIIIData(),
        VakIX = new VakIXData(),
        VakX = new VakXData
        {
            Code1384 = 235.10m,
            Code2361 = 1020m,
        },
        VakXII = new VakXIIData(),
        Gewest = Gewest.Wallonie,
        GemeentebelastingPercentage = 8m,
        TypeBeroep = TypeBeroep.Werknemer,
    };

    var taxCalc = new TaxCalcRef
    {
        NettoBP = 34443.95m,
        BasisbelastingBP = 11730.78m,
        VermVrijeSomBP = 9373.50m,
        OmTeSlaneBP = 2357.28m,
        HoofdsomBP = 2357.28m,
        GereduceerdeBP = 1768.97m,
        OpcentiemenBP = 588.31m,
        NettoPartner = 16044.78m,
        BasisbelastingP = 4044.91m,
        VermVrijeSomP = 2642.50m,
        OmTeSlaneP = 1402.41m,
        GereduceerdeP = 1052.41m,
        OpcentiemenP = 350.00m,
        TotaleBelasting = 3347.89m + 267.83m + 85.72m,
        Gemeentebelasting = 267.83m,
        BBSZVerschuldigd = 506.38m,
        Eindresultaat = -6534.88m,
    };

    return ("3: Gehuwd Menen (BP €40k + Partner €22k, 3 kinderen, 8%)", input, taxCalc);
}

static (string, BerekeningInput, TaxCalcRef) Scenario4_WerknemerAntwerpen()
{
    var input = new BerekeningInput
    {
        VakII = new VakIIData { BurgerlijkeStaat = "1001" },
        VakIII = new VakIIIData(),
        VakIV = new VakIVData
        {
            Fiches1250 = new() { 45000m },
            Fiches1286 = new() { 12000m },
            Code1287 = 400m,
        },
        VakV = new VakVData(),
        VakVIII = new VakVIIIData(),
        VakIX = new VakIXData(),
        VakX = new VakXData(),
        VakXII = new VakXIIData(),
        Gewest = Gewest.Vlaanderen,
        GemeentebelastingPercentage = 7m,
        TypeBeroep = TypeBeroep.Werknemer,
    };

    var taxCalc = new TaxCalcRef
    {
        NettoBP = 39250m,
        BasisbelastingBP = 13893.50m,
        VermVrijeSomBP = 2642.50m,
        OmTeSlaneBP = 11251m,
        HoofdsomBP = 11251m,
        GereduceerdeBP = 8443.09m,
        OpcentiemenBP = 2807.92m,
        TotaleBelasting = 11251.01m + 787.57m + 411.91m,
        Gemeentebelasting = 787.57m,
        BBSZVerschuldigd = 411.91m,
        Eindresultaat = 450.49m,
    };

    return ("4: Werknemer Antwerpen (€45k loon, BV €12000, 7%)", input, taxCalc);
}

static (string, BerekeningInput, TaxCalcRef) Scenario5_BedrijfsleiderBrussel()
{
    // Bedrijfsleider: €60k bezoldiging + €15k winst zelfstandig bijberoep
    // Meerwaarde €5k (16,5%)
    var input = new BerekeningInput
    {
        VakII = new VakIIData { BurgerlijkeStaat = "1001" },
        VakIII = new VakIIIData(),
        VakIV = new VakIVData(),
        VakV = new VakVData(),
        VakVIII = new VakVIIIData(),
        VakIX = new VakIXData(),
        VakX = new VakXData { Code1361 = 1020m },  // pensioensparen
        VakXII = new VakXIIData(),
        VakXVI = new VakXVIData
        {
            Code1400 = 55000m,     // gewone bezoldiging
            Code1401 = 5000m,      // voordelen alle aard
            Code1454 = 4500m,      // sociale bijdragen bedrijfsleider
            Code1421 = 18000m,     // bedrijfsvoorheffing
        },
        VakXVII = new VakXVIIData
        {
            Code1600 = 15000m,     // winst zelfstandige
            Code1632 = 3200m,      // sociale bijdragen zelfstandige
            Code1603 = 5000m,      // meerwaarde 16,5%
        },
        VakXIX = new VakXIXData
        {
            Code1758 = 2500m,      // BV zelfstandige
        },
        Gewest = Gewest.Brussel,
        GemeentebelastingPercentage = 7.5m,
        TypeBeroep = TypeBeroep.Werknemer,
    };

    // Handmatig nagerekend:
    // Bedrijfsleider: bruto 60k - SB 4.5k = 55.5k basis → forfait 3% = 1665 (max 3130) → netto = 53.835
    // Winst: bruto 15k - SB 3.2k = 11.8k basis → forfait 30% = 3540 (max 5930) → netto = 8.260
    // Netto totaal = 53.835 + 8.260 = 62.095
    // Afzonderlijk 16,5%: 5000 × 0.165 = 825
    var taxCalc = new TaxCalcRef();

    return ("5: Bedrijfsleider Brussel (€60k bezold + €15k winst + €5k meerwaarde 16,5%)", input, taxCalc);
}

static (string, BerekeningInput, TaxCalcRef) Scenario6_WerknemerWoonbonus()
{
    // Werknemer Vlaanderen €40k loon + geïntegreerde woonbonus (lening 2018)
    // Interesten €1.800 + kapitaalaflossingen €800 + premie levensverzekering €400 = €3.000
    // Max korf: €1.520 + €760 (eerste 10j, 2025-2018=7j) = €2.280
    // Gewestelijke vermindering: €2.280 × 40% = €912
    var input = new BerekeningInput
    {
        VakII = new VakIIData { BurgerlijkeStaat = "1001" },
        VakIII = new VakIIIData(),
        VakIV = new VakIVData
        {
            Fiches1250 = new() { 40_000m },
            Fiches1286 = new() { 10_000m },
        },
        VakV = new VakVData(),
        VakVIII = new VakVIIIData(),
        VakIX = new VakIXData
        {
            Code3138 = 1_800m,       // interesten geïntegreerde woonbonus
            Code3355 = 800m,         // kapitaalaflossingen hypothecair
            Code3351 = 400m,         // premie levensverzekering collectief
            Code3140 = "15/03/2018", // datum lening
            Code3142 = 1,            // 1 kind bij lening
        },
        VakX = new VakXData { Code1361 = 1_050m },  // pensioensparen
        VakXII = new VakXIIData(),
        Gewest = Gewest.Vlaanderen,
        GemeentebelastingPercentage = 7m,
        TypeBeroep = TypeBeroep.Werknemer,
    };

    // Handmatige berekening:
    // Bruto: 40.000 → forfait 30% = 5.930 (max) → netto 34.070
    // Basisbelasting: 9.643,50
    // Vrije som 10.910 → vermindering 2.727,50
    // Om te slane: 6.916,00
    // Hoofdsom: 6.916,00 (geen vervangingsinkomen)
    // Gereduceerde Staat: 6.916 × (1 - 0,24957) = 5.190,03
    // Opcentiemen: 5.190,03 × 0,33257 = 1.725,71
    // Federale verminderingen: pensioensparen 1050 × 30% = 315
    // Gewestelijke verminderingen: min(3000, 2280) × 40% = 912
    // SaldoFederaal: 5.190,03 - 315 = 4.875,03
    // SaldoGewestelijk: max(1.725,71 - 912, 0) = 813,71
    // Gemeentebelasting: (4.875,03 + 813,71) × 7% = 398,21
    // BBSZ: op netto 34.070 → bijdrage ±399
    // Totaal: 4.875,03 + 813,71 + 398,21 + 399 = 6.485,95
    // BV: 10.000 → Eindresultaat = 6.485,95 - 10.000 = -3.514,05
    var taxCalc = new TaxCalcRef();

    return ("6: Werknemer Vlaanderen + geïntegreerde woonbonus (lening 2018)", input, taxCalc);
}

static (string, BerekeningInput, TaxCalcRef) Scenario7_PensionarisMenen_1txt()
{
    // Data uit 1.txt — AJ2024 (inkomsten 2023), gehuwd pensionaarskoppel, gemeente Menen (8%)
    // Titularis: wettelijk pensioen 18.821,33 + overlevingspensioen 3.087,26 + andere 3.080,79
    // Partner : wettelijk pensioen  4.365,33 + andere 987,06
    // BV titularis 1.662,44 | BV partner 29,82
    var input = new BerekeningInput
    {
        VakII = new VakIIData { BurgerlijkeStaat = "1002" },
        VakIII = new VakIIIData(),
        VakIV = new VakIVData(),
        VakV = new VakVData
        {
            Code1228 = 18_821.33m,   // wettelijk pensioen titularis
            Code1229 = 3_087.26m,    // overlevingspensioen titularis
            Code1211 = 3_080.79m,    // andere pensioenen titularis
            Fiches1225 = new() { 1_662.44m },    // bedrijfsvoorheffing titularis
            Code2228 = 4_365.33m,    // wettelijk pensioen partner
            Code2211 = 987.06m,      // andere pensioenen partner
            Fiches2225 = new() { 29.82m },       // bedrijfsvoorheffing partner
        },
        VakVIII = new VakVIIIData(),
        VakIX = new VakIXData(),
        VakX = new VakXData(),
        VakXII = new VakXIIData(),
        Gewest = Gewest.Vlaanderen,
        GemeentebelastingPercentage = 8.0m,
        TypeBeroep = TypeBeroep.Werknemer,
    };

    // Referentiewaarden uit 1.txt (AJ2024 — indexatie verschilt van AJ2026):
    var taxCalc = new TaxCalcRef
    {
        NettoBP           = 24_989.38m,   // netto vóór HQ (= bruto, geen kosten)
        NettoPartner      =  5_352.39m,   // netto vóór HQ partner
        BasisbelastingBP  =  6_215.70m,   // AJ2024-schijven op 21.239,24 na HQ
        BasisbelastingP   =  2_275.63m,   // AJ2024-schijven op 9.102,53 na HQ
        VermVrijeSomBP    =  2_831.24m,   // AJ2024 barema vrijeSom op 11.217,47
        VermVrijeSomP     =  2_275.63m,   // begrensd op basisbelasting partner
        OmTeSlaneBP       =  3_384.46m,
        OmTeSlaneP        =       0.00m,
        VermVervangingBP  =  2_347.96m,   // 2.067,84 basis + 280,12 aanvullend
        HoofdsomBP        =  1_036.50m,
        GereduceerdeBP    =    777.82m,   // × 75,043%
        OpcentiemenBP     =    258.68m,   // × 33,257%
        GereduceerdeP     =      0.00m,
        OpcentiemenP      =      0.00m,
        Gemeentebelasting =     82.92m,   // 1.036,50 × 8%
        Eindresultaat     =   -572.84m,   // bedrag in voordeel belastingplichtige
    };

    return ("7: Pensionaarskoppel Menen (1.txt, AJ2024 referentie)", input, taxCalc);
}

class TaxCalcRef
{
    public decimal NettoBP { get; set; }
    public decimal NettoPartner { get; set; }
    public decimal BasisbelastingBP { get; set; }
    public decimal BasisbelastingP { get; set; }
    public decimal VermVrijeSomBP { get; set; }
    public decimal VermVrijeSomP { get; set; }
    public decimal OmTeSlaneBP { get; set; }
    public decimal OmTeSlaneP { get; set; }
    public decimal VermVervangingBP { get; set; }
    public decimal HoofdsomBP { get; set; }
    public decimal GereduceerdeBP { get; set; }
    public decimal GereduceerdeP { get; set; }
    public decimal OpcentiemenBP { get; set; }
    public decimal OpcentiemenP { get; set; }
    public decimal TotaleBelasting { get; set; }
    public decimal Gemeentebelasting { get; set; }
    public decimal BBSZVerschuldigd { get; set; }
    public decimal Eindresultaat { get; set; }
}
