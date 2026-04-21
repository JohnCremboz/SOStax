namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK IV – A. Gewone bezoldigingen</summary>
public class VakIVData
{
    // 1. Wedden/lonen - Meerdere fiches 281.10
    public List<decimal?> Fiches1250 { get; set; } = new() { null }; // Belastingplichtige
    public List<decimal?> Fiches2250 { get; set; } = new() { null }; // Partner

    // Computed totals voor 1a) - volgens fiches 281.10
    public decimal Code1250 => Fiches1250.Sum(f => f ?? 0m);
    public decimal Code2250 => Fiches2250.Sum(f => f ?? 0m);

    // 1b) Niet op fiche vermeld
    public decimal? Code1250B { get; set; }
    public decimal? Code2250B { get; set; }

    // Totaal 1250-11 en 2250-78 (1a + 1b)
    public decimal Total1250 => Code1250 + (Code1250B ?? 0m);
    public decimal Total2250 => Code2250 + (Code2250B ?? 0m);

    public decimal? Code1251 { get; set; }   // vervroegd vakantiegeld
    public decimal? Code2251 { get; set; }
    public decimal? Code1252 { get; set; }   // achterstallen
    public decimal? Code2252 { get; set; }

    // 3. Totaal rubrieken 1+2
    // (berekend veld, geen code)

    // 4. Vervroegd vakantiegeld (andere dan 2)
    // (zit al in 1251 hierboven, dit is enkel label-context)

    // 5. Opzeggings-/inschakelingsvergoedingen
    public decimal? Code1308 { get; set; }
    public decimal? Code2308 { get; set; }

    // 6. Bezoldigingen december 2025 (overheid)
    public decimal? Code1247 { get; set; }
    public decimal? Code2247 { get; set; }

    // 7. Woon-werkverkeer
    public decimal? Code1254 { get; set; }   // totaal bedrag
    public decimal? Code2254 { get; set; }
    public decimal? Code1255 { get; set; }   // vrijstelling
    public decimal? Code2255 { get; set; }

    // 8. Niet-recurrente voordelen
    public decimal? Code1242 { get; set; }   // gewone
    public decimal? Code2242 { get; set; }
    public decimal? Code1243 { get; set; }   // achterstallen
    public decimal? Code2243 { get; set; }

    // 9. Werkgeverstussenkomst privé-pc
    public decimal? Code1240 { get; set; }   // totaal bedrag
    public decimal? Code2240 { get; set; }
    public decimal? Code1241 { get; set; }   // vrijstelling
    public decimal? Code2241 { get; set; }

    // 10a. Horeca zonder GKS
    public decimal? Code1335 { get; set; }   // gewone bezoldigingen
    public decimal? Code2335 { get; set; }
    public int?     Code1336 { get; set; }   // aantal overuren
    public int?     Code2336 { get; set; }
    public decimal? Code1337 { get; set; }   // achterstallen
    public decimal? Code2337 { get; set; }
    public int?     Code1338 { get; set; }   // aantal overuren achterstallen
    public int?     Code2338 { get; set; }

    // 10b. Horeca met GKS
    public decimal? Code1395 { get; set; }
    public decimal? Code2395 { get; set; }
    public int?     Code1396 { get; set; }
    public int?     Code2396 { get; set; }
    public decimal? Code1397 { get; set; }
    public decimal? Code2397 { get; set; }
    public int?     Code1398 { get; set; }
    public int?     Code2398 { get; set; }

    // 11. Flexi-jobs
    public decimal? Code1262 { get; set; }
    public decimal? Code2262 { get; set; }

    // 12. Vrijwilligers brandweer/ambulanciers
    public decimal? Code1391 { get; set; }
    public decimal? Code2391 { get; set; }

    // 13a. Vrijwillige overuren 2025
    public decimal? Code1306 { get; set; }
    public decimal? Code2306 { get; set; }
    public int?     Code1307 { get; set; }
    public int?     Code2307 { get; set; }

    // 13b. Vrijwillige overuren 2024
    public decimal? Code1368 { get; set; }
    public decimal? Code2368 { get; set; }
    public int?     Code1369 { get; set; }
    public int?     Code2369 { get; set; }

    // 13c. Vrijwillige overuren 2023
    public decimal? Code1381 { get; set; }
    public decimal? Code2381 { get; set; }
    public int?     Code1382 { get; set; }
    public int?     Code2382 { get; set; }

    // 14. 33% horeca/gezondheidszorg gepensioneerden
    public decimal? Code1263 { get; set; }
    public decimal? Code2263 { get; set; }

    // 15. Sportbeoefenaars
    public decimal? Code1273 { get; set; }   // wedden/lonen
    public decimal? Code2273 { get; set; }
    public decimal? Code1274 { get; set; }   // vervroegd vakantiegeld
    public decimal? Code2274 { get; set; }
    public decimal? Code1275 { get; set; }   // achterstallen
    public decimal? Code2275 { get; set; }
    public decimal? Code1276 { get; set; }   // opzeggingsvergoedingen
    public decimal? Code2276 { get; set; }

    // 16. Scheidsrechters/trainers
    public decimal? Code1277 { get; set; }
    public decimal? Code2277 { get; set; }
    public decimal? Code1278 { get; set; }
    public decimal? Code2278 { get; set; }
    public decimal? Code1279 { get; set; }
    public decimal? Code2279 { get; set; }
    public decimal? Code1280 { get; set; }
    public decimal? Code2280 { get; set; }

    // 17. Impulsfonds huisarts
    public decimal? Code1267 { get; set; }
    public decimal? Code2267 { get; set; }

    // 18. Niet-ingehouden sociale bijdragen
    public decimal? Code1257 { get; set; }
    public decimal? Code2257 { get; set; }

    // 19. Andere beroepskosten
    public decimal? Code1258 { get; set; }
    public decimal? Code2258 { get; set; }

    // ── B. WERKLOOSHEIDSUITKERINGEN ───────────────────────────────────────────
    // 1. Zonder anciënniteitstoeslag
    public decimal? Code1260 { get; set; }
    public decimal? Code2260 { get; set; }
    public decimal? Code1304 { get; set; }
    public decimal? Code2304 { get; set; }
    public decimal? Code1261 { get; set; }
    public decimal? Code2261 { get; set; }
    // 2. Met anciënniteitstoeslag
    public decimal? Code1264 { get; set; }
    public decimal? Code2264 { get; set; }
    public decimal? Code1265 { get; set; }
    public decimal? Code2265 { get; set; }

    // ── C. WETTELIJKE UITKERINGEN ZIEKTE/INVALIDITEIT ────────────────────────
    public decimal? Code1266 { get; set; }
    public decimal? Code2266 { get; set; }
    public decimal? Code1303 { get; set; }
    public decimal? Code2303 { get; set; }
    public decimal? Code1268 { get; set; }
    public decimal? Code2268 { get; set; }

    // ── D. VERVANGINGSINKOMSTEN ───────────────────────────────────────────────
    // 1a. Met doorbetalingsclausule
    public decimal? Code1319 { get; set; }
    public decimal? Code2319 { get; set; }
    public decimal? Code1321 { get; set; }
    public decimal? Code2321 { get; set; }
    public decimal? Code1322 { get; set; }
    public decimal? Code2322 { get; set; }
    public decimal? Code1324 { get; set; }
    public decimal? Code2324 { get; set; }
    public decimal? Code1339 { get; set; }
    public decimal? Code2339 { get; set; }
    // 1a. bovenop werkloosheid
    public decimal? Code1292 { get; set; }
    public decimal? Code2292 { get; set; }
    public decimal? Code1300 { get; set; }
    public decimal? Code2300 { get; set; }
    public decimal? Code1293 { get; set; }
    public decimal? Code2293 { get; set; }
    // 1b. Zonder doorbetalingsclausule
    public decimal? Code1294 { get; set; }
    public decimal? Code2294 { get; set; }
    public decimal? Code1301 { get; set; }
    public decimal? Code2301 { get; set; }
    public decimal? Code1295 { get; set; }
    public decimal? Code2295 { get; set; }
    // Werkhervatting
    public bool Code1297 { get; set; }
    public bool Code2297 { get; set; }
    public bool Code1298 { get; set; }
    public bool Code2298 { get; set; }
    // 2. Aanvullende ziekte/invaliditeit
    public decimal? Code1269 { get; set; }
    public decimal? Code2269 { get; set; }
    // 3. Beroepsziekte/arbeidsongeval
    public decimal? Code1270 { get; set; }
    public decimal? Code2270 { get; set; }
    // 4. COVID-19
    public decimal? Code1309 { get; set; }
    public decimal? Code2309 { get; set; }
    // 5. Andere
    public decimal? Code1271 { get; set; }
    public decimal? Code2271 { get; set; }
    // 6. December 2025
    public decimal? Code1302 { get; set; }
    public decimal? Code2302 { get; set; }
    // 7. Achterstallen
    public decimal? Code1272 { get; set; }
    public decimal? Code2272 { get; set; }

    // ── E. WERKLOOSHEIDSUITKERINGEN MET BEDRIJFSTOESLAG ──────────────────────
    public decimal? Code1281 { get; set; }
    public decimal? Code2281 { get; set; }
    public decimal? Code1282 { get; set; }
    public decimal? Code2282 { get; set; }
    public decimal? Code1235 { get; set; }
    public decimal? Code2235 { get; set; }
    public decimal? Code1327 { get; set; }
    public decimal? Code2327 { get; set; }
    public decimal? Code1236 { get; set; }
    public decimal? Code2236 { get; set; }
    public decimal? Code1340 { get; set; }
    public decimal? Code2340 { get; set; }

    // ── F. INHOUDINGEN AANVULLEND PENSIOEN ───────────────────────────────────
    public decimal? Code1285 { get; set; }
    public decimal? Code2285 { get; set; }
    public decimal? Code1283 { get; set; }
    public decimal? Code2283 { get; set; }
    public decimal? Code1387 { get; set; }
    public decimal? Code2387 { get; set; }

    // ── G. OVERUREN MET OVERWERKTOESLAG ──────────────────────────────────────
    public int?     Code1305 { get; set; }   // tot 180 uren
    public int?     Code2305 { get; set; }
    public int?     Code1238 { get; set; }   // tot 280 uren
    public int?     Code2238 { get; set; }
    public int?     Code1317 { get; set; }   // tot 360 uren
    public int?     Code2317 { get; set; }
    public decimal? Code1233 { get; set; }   // grondslag 66,81%
    public decimal? Code2233 { get; set; }
    public decimal? Code1234 { get; set; }   // grondslag 57,75%
    public decimal? Code2234 { get; set; }

    // ── Vorige beroepsverliezen (art. 23 §1 WIB92) ───────────────────────────
    public decimal? Code1349 { get; set; }   // overdraagbare verliezen belastingplichtige
    public decimal? Code2349 { get; set; }   // overdraagbare verliezen partner

    // ── H. BEDRIJFSVOORHEFFING ────────────────────────────────────────────────
    // Meerdere fiches voor bedrijfsvoorheffing (code 286)
    public List<decimal?> Fiches1286 { get; set; } = new() { null };
    public List<decimal?> Fiches2286 { get; set; } = new() { null };

    // Computed totals voor bedrijfsvoorheffing volgens fiches
    public decimal Code1286 => Fiches1286.Sum(f => f ?? 0m);
    public decimal Code2286 => Fiches2286.Sum(f => f ?? 0m);

    // Bedrijfsvoorheffing op niet-op-fiche vakantiegeld (indien van toepassing)
    public decimal? Code1286B { get; set; }
    public decimal? Code2286B { get; set; }

    // Totaal bedrijfsvoorheffing (1286-72 / 2286-42)
    public decimal Total1286 => Code1286 + (Code1286B ?? 0m);
    public decimal Total2286 => Code2286 + (Code2286B ?? 0m);

    // ── I. BIJZONDERE BIJDRAGE SOCIALE ZEKERHEID ─────────────────────────────
    public decimal? Code1287 { get; set; }
    public decimal? Code2287 { get; set; }

    // ── J. OVERHEIDSPERSONEEL ZONDER ARBEIDSOVEREENKOMST ─────────────────────
    public bool Code1290 { get; set; }
    public bool Code2290 { get; set; }

    // ── K. WERKBONUS ──────────────────────────────────────────────────────────
    public decimal? Code1284 { get; set; }   // 33,14%
    public decimal? Code2284 { get; set; }
    public decimal? Code1360 { get; set; }   // 52,54%
    public decimal? Code2360 { get; set; }

    // ── L. WERKHERVATTINGSLOON ────────────────────────────────────────────────
    public decimal? Code1296 { get; set; }
    public decimal? Code2296 { get; set; }

    // ── M. ROERENDE VOORHEFFING AUTEURSRECHTEN ────────────────────────────────
    public decimal? Code1299 { get; set; }
    public decimal? Code2299 { get; set; }

    // ── N. HELPENDE GEZINSLEDEN VAN ZELFSTANDIGEN ─────────────────────────────
    /// Vrije lijst: code + bedrag van inkomsten als helpend gezinslid
    public List<HelpendeGezinsledenRij> HelpendeGezinsleden { get; set; } = [new()];

    // ── O. INKOMSTEN VAN BUITENLANDSE OORSPRONG (EN BIJHORENDE KOSTEN) ────────
    // 1. Inkomsten verkregen in Frankrijk of Nederland (vrijstelling bijz. bijdrage soc. zekerheid)
    public List<BuitenlandseInkomstenIVRij> BuitenlandO1 { get; set; } = [new()];
    // 2. Inkomsten waarvoor belastingvermindering buitenlandse oorsprong of aanslagvoet 0%
    //    a) vrijstelling met progressievoorbehoud
    public List<BuitenlandseInkomstenIVRij> BuitenlandO2a { get; set; } = [new()];
    //    b) vermindering van de belasting tot de helft
    public List<BuitenlandseInkomstenIVRij> BuitenlandO2b { get; set; } = [new()];
    //    c) afzonderlijk belastbaar tegen aanslagvoet 0%
    public List<BuitenlandseInkomstenIVRij> BuitenlandO2c { get; set; } = [new()];
}

/// <summary>Rij voor rubriek N – helpende gezinsleden.</summary>
public class HelpendeGezinsledenRij
{
    public string Code { get; set; } = string.Empty;
    public decimal? Bedrag { get; set; }
}

/// <summary>Rij voor rubriek O – buitenlandse inkomsten Vak IV.</summary>
public class BuitenlandseInkomstenIVRij
{
    public string Land { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal? Bedrag { get; set; }
    public decimal? Kosten { get; set; }
}
