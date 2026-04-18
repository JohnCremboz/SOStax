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
                            + result.Gemeentebelasting + Math.Max(result.BBSZSaldo, 0);
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
        VakIV = new VakIVData { Code1286 = 2886.03m },
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
            Code1250 = 35000m,
            Code1286 = 8500m,
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
            Code1250 = 40193.95m,
            Code1286 = 8948.90m,
            Code1287 = 324.21m,
            Code2250 = 21794.78m,
            Code2286 = 184.66m,
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
            Code1250 = 45000m,
            Code1286 = 12000m,
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
