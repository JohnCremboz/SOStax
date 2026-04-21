namespace BlazorTax.Belastingen;

public class VakXData
{
    // I. GEWESTELIJK
    // A. Reno-overeenkomsten
    public decimal? Code3332 { get; set; }  // 1.1.2025
    public decimal? Code4332 { get; set; }
    public decimal? Code3333 { get; set; }  // 31.12.2025
    public decimal? Code4333 { get; set; }

    // B. Vernieuwing via SVK
    public decimal? Code3395 { get; set; }

    // II. FEDERAAL
    // A. Giften
    public decimal? Code1394 { get; set; }

    // B. Kinderoppas
    public decimal? Code1384 { get; set; }

    // C. Aanvullend pensioen zelfstandigen
    public decimal? Code1342 { get; set; }
    public decimal? Code2342 { get; set; }

    // D. Pensioensparen
    public decimal? Code1361 { get; set; }
    public decimal? Code2361 { get; set; }

    // E. Kapitaalaandelen werknemer/dochter
    public decimal? Code1362 { get; set; }
    public decimal? Code2362 { get; set; }
    public decimal? Code1366 { get; set; }  // terugname
    public decimal? Code2366 { get; set; }

    // F. Startende ondernemingen
    public decimal? Code1318 { get; set; }  // 30%
    public decimal? Code2318 { get; set; }
    public decimal? Code1320 { get; set; }  // 45%
    public decimal? Code2320 { get; set; }
    public decimal? Code1328 { get; set; }  // terugname
    public decimal? Code2328 { get; set; }

    // G. Groeibedrijven
    public decimal? Code1334 { get; set; }
    public decimal? Code2334 { get; set; }
    public decimal? Code1343 { get; set; }  // terugname
    public decimal? Code2343 { get; set; }

    // H. Terugname COVID-19
    public decimal? Code1377 { get; set; }
    public decimal? Code2377 { get; set; }

    // I. Terugname ontwikkelingsfondsen
    public decimal? Code1376 { get; set; }
    public decimal? Code2376 { get; set; }
}

