namespace BlazorTax.Belastingen;

public class VakVIIData
{
    // A. Niet verplicht – kapitalen
    public decimal? Code1160 { get; set; }  // 30%
    public decimal? Code2160 { get; set; }
    public decimal? Code1161 { get; set; }  // 20%
    public decimal? Code2161 { get; set; }
    public decimal? Code1162 { get; set; }  // 15%
    public decimal? Code2162 { get; set; }
    public decimal? Code1163 { get; set; }  // 6,5%
    public decimal? Code2163 { get; set; }
    public decimal? Code1436 { get; set; }  // 5%
    public decimal? Code2436 { get; set; }
    // Carried interest (niet verplicht)
    public decimal? Code1167 { get; set; }
    public decimal? Code2167 { get; set; }
    // Verrekenbare RV vrijgestelde dividenden
    public decimal? Code1437 { get; set; }
    public decimal? Code2437 { get; set; }

    // A. Verplicht – spaardeposito's
    public decimal? Code1151 { get; set; }
    public decimal? Code2151 { get; set; }
    // Carried interest zonder RV
    public decimal? Code1442 { get; set; }
    public decimal? Code2442 { get; set; }
    // Andere inkomsten zonder RV
    public decimal? Code1444 { get; set; }  // 30%
    public decimal? Code2444 { get; set; }
    public decimal? Code1159 { get; set; }  // 20%
    public decimal? Code2159 { get; set; }
    public decimal? Code1445 { get; set; }  // 15%
    public decimal? Code2445 { get; set; }
    public decimal? Code1446 { get; set; }  // 6,5%
    public decimal? Code2446 { get; set; }
    public decimal? Code1448 { get; set; }  // 5%
    public decimal? Code2448 { get; set; }

    // B. Verhuur/verpachting roerende goederen
    public decimal? Code1156 { get; set; }
    public decimal? Code2156 { get; set; }

    // C. Lijfrenten / tijdelijke renten
    public decimal? Code1158 { get; set; }
    public decimal? Code2158 { get; set; }

    // D. Auteursrechten
    public decimal? Code1123 { get; set; }  // bruto
    public decimal? Code2123 { get; set; }
    public decimal? Code1124 { get; set; }  // kosten
    public decimal? Code2124 { get; set; }
    public decimal? Code1119 { get; set; }  // RV
    public decimal? Code2119 { get; set; }

    // E. Innings- en bewaringskosten
    public decimal? Code1170 { get; set; }
    public decimal? Code2170 { get; set; }

    // F. Bijzonder aanslagstelsel
    public List<BijzonderAanslagItem> BijzonderAanslag { get; set; } = [new()];
}

public class BijzonderAanslagItem
{
    public string Land { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal? Bedrag { get; set; }
    public string Aard { get; set; } = string.Empty;
}

