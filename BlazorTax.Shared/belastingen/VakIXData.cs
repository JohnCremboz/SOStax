namespace BlazorTax.Belastingen;

/// <summary>VAK IX — Interesten, kapitaalaflossingen, premies, erfpacht/opstal</summary>
public class VakIXData
{
    // ══ I. GEWESTELIJK — EIGEN WONING ══════════════════════════════════════

    // 1. Geïntegreerde woonbonus (2016–2019)
    public decimal? Code3334 { get; set; }   // KI niet verhuurd
    public decimal? Code4334 { get; set; }
    public decimal? Code3335 { get; set; }   // KI verhuurd nat. persoon
    public decimal? Code4335 { get; set; }
    public bool     Code3336 { get; set; }   // Ja – verhuur rechtspersoon soc.huis.
    public bool     Code4336 { get; set; }
    public bool     Code3337 { get; set; }   // Neen
    public bool     Code4337 { get; set; }
    public decimal? Code3330 { get; set; }   // KI andere omstandigheden
    public decimal? Code4330 { get; set; }
    public decimal? Code3360 { get; set; }   // brutohuur eigen woning
    public decimal? Code4360 { get; set; }
    public decimal? Code3361 { get; set; }   // brutohuur andere
    public decimal? Code4361 { get; set; }
    // Lening interesten (VAKIXb)
    public decimal? Code3138 { get; set; }
    public decimal? Code4138 { get; set; }
    public decimal? Code3139 { get; set; }
    public decimal? Code4139 { get; set; }
    public string?  Code3140 { get; set; }   // datum lening
    public string?  Code4140 { get; set; }
    public decimal? Code3141 { get; set; }   // bedrag lening
    public decimal? Code4141 { get; set; }
    public int?     Code3142 { get; set; }   // aantal kinderen
    public int?     Code4142 { get; set; }
    public string?  Code3144 { get; set; }   // datum ingebruikneming
    public string?  Code4144 { get; set; }
    public decimal? Code3145 { get; set; }   // kostprijs werken
    public decimal? Code4145 { get; set; }
    public decimal? Code3148 { get; set; }   // aandeel eigen woning %
    public decimal? Code4148 { get; set; }
    public decimal? Code3149 { get; set; }   // aandeel mede-leners %
    public decimal? Code4149 { get; set; }
    public bool     Code3136 { get; set; }   // eigen woning beide partners Ja
    public bool     Code3137 { get; set; }   // Neen
    // Brutohuur 3+4
    public decimal? Code3370 { get; set; }
    public decimal? Code4370 { get; set; }
    public decimal? Code3371 { get; set; }
    public decimal? Code4371 { get; set; }

    // 2. Gewestelijke woonbonus (2005–2015) — interesten (VAKIXc)
    public decimal? Code3150 { get; set; }   // leningen 2015
    public decimal? Code3146 { get; set; }   // leningen vóór 2015
    public decimal? Code3151 { get; set; }   // schulden 2015
    public decimal? Code3152 { get; set; }   // schulden vóór 2015

    // 3. Andere interesten eigen woning (niet verhuurd) — VAK IX rubriek 3
    public decimal? Code3100 { get; set; }
    public decimal? Code4100 { get; set; }
    public decimal? Code3106 { get; set; }
    public decimal? Code4106 { get; set; }
    public decimal? Code3109 { get; set; }
    public decimal? Code4109 { get; set; }
    public decimal? Code3110 { get; set; }
    public decimal? Code4110 { get; set; }

    // 4. Kapitaalaflossingen (bouwsparen / lange termijn)
    public decimal? Code3355 { get; set; }
    public decimal? Code4355 { get; set; }
    public decimal? Code3356 { get; set; }
    public decimal? Code4356 { get; set; }
    public decimal? Code3358 { get; set; }
    public decimal? Code4358 { get; set; }

    // 5. Premies levensverzekeringen
    public decimal? Code3351 { get; set; }
    public decimal? Code4351 { get; set; }
    public decimal? Code3352 { get; set; }
    public decimal? Code4352 { get; set; }
    public decimal? Code3353 { get; set; }
    public decimal? Code4353 { get; set; }
    public decimal? Code3354 { get; set; }
    public decimal? Code4354 { get; set; }

    // 6. Erfpacht/opstal gewestelijk
    public decimal? Code3143 { get; set; }   // 2015–2019
    public decimal? Code3147 { get; set; }   // vóór 2015
    public string   ErfpachtGenieter { get; set; } = string.Empty;

    // ══ II. FEDERAAL — NIET-EIGEN WONING ═══════════════════════════════════
    public decimal? Code1358 { get; set; }
    public decimal? Code2358 { get; set; }
    public decimal? Code1353 { get; set; }
    public decimal? Code2353 { get; set; }
    public string   FederaalContractNr   { get; set; } = string.Empty;
    public string   FederaalVerzekeraar  { get; set; } = string.Empty;
    public decimal? Code1147 { get; set; }
    public decimal? Code2147 { get; set; }
    public string   FederaalErfpachtGenieter { get; set; } = string.Empty;

    // Contract nr / naam verzekeraar voor premies lange termijn gewestelijk
    public string GewestelijkContractNr  { get; set; } = string.Empty;
    public string GewestelijkVerzekeraar { get; set; } = string.Empty;
}


