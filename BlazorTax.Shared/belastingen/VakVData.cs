namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK V – Pensioenen</summary>
public class VakVData
{
    // ── A1. Wettelijke pensioenen ─────────────────────────────────────────────
    public decimal? Code1228 { get; set; }   // gewone
    public decimal? Code2228 { get; set; }
    public decimal? Code1314 { get; set; }   // december 2025
    public decimal? Code2314 { get; set; }
    public decimal? Code1230 { get; set; }   // achterstallen
    public decimal? Code2230 { get; set; }

    // ── A1. Overlevingspensioenen ─────────────────────────────────────────────
    public decimal? Code1229 { get; set; }
    public decimal? Code2229 { get; set; }
    public decimal? Code1315 { get; set; }
    public decimal? Code2315 { get; set; }
    public decimal? Code1231 { get; set; }
    public decimal? Code2231 { get; set; }

    // ── A1. Andere pensioenen/renten ──────────────────────────────────────────
    public decimal? Code1211 { get; set; }
    public decimal? Code2211 { get; set; }
    public decimal? Code1316 { get; set; }
    public decimal? Code2316 { get; set; }
    public decimal? Code1212 { get; set; }
    public decimal? Code2212 { get; set; }

    // ── A1. Kapitalen en afkoopwaarden ────────────────────────────────────────
    public decimal? Code1213 { get; set; }   // 33%
    public decimal? Code2213 { get; set; }
    public decimal? Code1245 { get; set; }   // 20%
    public decimal? Code2245 { get; set; }
    public decimal? Code1253 { get; set; }   // 18%
    public decimal? Code2253 { get; set; }
    public decimal? Code1232 { get; set; }   // 16,5% wettelijk
    public decimal? Code2232 { get; set; }
    public decimal? Code1237 { get; set; }   // 16,5% overleving
    public decimal? Code2237 { get; set; }
    public decimal? Code1214 { get; set; }   // 16,5% andere
    public decimal? Code2214 { get; set; }
    public decimal? Code1215 { get; set; }   // 10%
    public decimal? Code2215 { get; set; }

    // ── A1. Omzettingsrenten ──────────────────────────────────────────────────
    public decimal? Code1216 { get; set; }   // 2025
    public decimal? Code2216 { get; set; }
    public decimal? Code1218 { get; set; }   // 2013-2024
    public decimal? Code2218 { get; set; }

    // ── A2. Arbeidsongevallen / beroepsziekten ────────────────────────────────
    public decimal? Code1217 { get; set; }
    public decimal? Code2217 { get; set; }
    public decimal? Code1224 { get; set; }   // achterstallen
    public decimal? Code2224 { get; set; }
    public decimal? Code1226 { get; set; }   // omzettingsrenten 2025
    public decimal? Code2226 { get; set; }
    public decimal? Code1227 { get; set; }   // omzettingsrenten 2013-2024
    public decimal? Code2227 { get; set; }

    // ── A3. Pensioensparen ────────────────────────────────────────────────────
    public decimal? Code1219 { get; set; }   // gezamenlijk
    public decimal? Code2219 { get; set; }
    public decimal? Code1220 { get; set; }   // 33%
    public decimal? Code2220 { get; set; }
    public decimal? Code1221 { get; set; }   // 16,5%
    public decimal? Code2221 { get; set; }
    public decimal? Code1222 { get; set; }   // 8%
    public decimal? Code2222 { get; set; }

    // ── A4. Niet-ingehouden sociale bijdragen ─────────────────────────────────
    public decimal? Code1223 { get; set; }
    public decimal? Code2223 { get; set; }

    // ── B. Bedrijfsvoorheffing ─────────────────────────────────────────────────
    public decimal? Code1225 { get; set; }
    public decimal? Code2225 { get; set; }

    // ── C. Pensioenen buitenlandse oorsprong ──────────────────────────────────
    public List<BuitenlandsPensioen> BuitenlandsePensioenen { get; set; } = [new()];
}

public class BuitenlandsPensioen
{
    public string Land { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal? Bedrag { get; set; }
    /// "vrijstelling" | "vermindering" | ""
    public string TypeVermindering { get; set; } = string.Empty;
}

