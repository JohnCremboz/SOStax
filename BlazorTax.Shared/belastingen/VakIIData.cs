namespace BlazorTax.Belastingen;

/// <summary>Data model voor VAK II – Persoonlijke gegevens en gezinslasten.</summary>
public class VakIIData
{
    // ── Vraag 1: Burgerlijke staat op 1.1.2026 ──────────────────────────────
    /// <see cref="BurgerlijkeStaatCodes.Ongehuwd"/> | <see cref="BurgerlijkeStaatCodes.GehuwdOfWettelijkSamenwonend"/> | <see cref="BurgerlijkeStaatCodes.WeduwnaarOfWeduwe"/> | ""
    public string BurgerlijkeStaat { get; set; } = string.Empty;

    // Sub-velden bij 1002-65 (gehuwd / wettelijk samenwonend)
    public bool Code1003 { get; set; }   // gehuwd in 2025 / verklaring samenwoning
    public bool Code1004 { get; set; }   // nettobestaansmiddelen partner ≤ 4.100 euro
    public bool Code1018 { get; set; }   // feitelijk gescheiden op 1.1.2026
    public bool Code1019 { get; set; }   // feitelijke scheiding vond plaats in 2025

    // Sub-velden bij 1010-57 (weduwnaar/weduwe)
    public bool Code1011 { get; set; }   // partner overleden in 2025
    /// <see cref="GemeenschappelijkAanslagCodes.EenGemeenschappelijkeAanslag"/> | <see cref="GemeenschappelijkAanslagCodes.TweeAfzonderlijkeAanslagen"/> | ""
    public string GemeenschappelijkAanslag { get; set; } = string.Empty;

    // ── Vraag 2: Aangifte voor overledene ────────────────────────────────────
    public bool Code1022 { get; set; }
    public bool Code1023 { get; set; }
    public bool Code1024 { get; set; }
    /// <see cref="GemeenschappelijkAanslagOverledeneCodes.EenGemeenschappelijkeAanslag"/> | <see cref="GemeenschappelijkAanslagOverledeneCodes.TweeAfzonderlijkeAanslagen"/> | ""
    public string GemeenschappelijkAanslagOverledene { get; set; } = string.Empty;

    // ── Vraag 3: Internationale organisaties ─────────────────────────────────
    public bool Code1062 { get; set; }   // ambtenaar (belastingplichtige)
    public bool Code2062 { get; set; }   // ambtenaar (partner)
    public bool Code1020 { get; set; }   // inkomsten > 13.460 euro (belastingplichtige)
    public bool Code1021 { get; set; }   // gehuwd met ambtenaar

    // ── Vraag 4: Zware handicap ───────────────────────────────────────────────
    public bool Code1028 { get; set; }   // belastingplichtige
    public bool Code2028 { get; set; }   // partner

    // ── Vraag 5: Andere persoon in het gezin ─────────────────────────────────
    public bool Code1101 { get; set; }

    // ── Vraag 6: Aantal maanden rijksinwoner ─────────────────────────────────
    public int? Code1199 { get; set; }

    // ── B. GEZINSLASTEN ──────────────────────────────────────────────────────

    // Vraag B1: Kinderen volledig ten laste
    public int? Code1030 { get; set; }   // a) aantal
    public int? Code1031 { get; set; }   // b) met zware handicap
    public int? Code1038 { get; set; }   // c) < 3 jaar, geen kinderoppas
    public int? Code1039 { get; set; }   // d) < 3 jaar + zware handicap

    // Vraag B2: Gelijkmatig verdeelde huisvesting
    public int? Code1034 { get; set; }   // a) aantal
    public int? Code1035 { get; set; }   // b) met zware handicap
    public int? Code1054 { get; set; }   // c) < 3 jaar, geen kinderoppas
    public int? Code1055 { get; set; }   // d) < 3 jaar + zware handicap

    // Vraag B3: Kinderen ten laste andere ouder
    public int? Code1036 { get; set; }   // a) aantal
    public int? Code1037 { get; set; }   // b) met zware handicap
    public int? Code1058 { get; set; }   // c) < 3 jaar, geen kinderoppas
    public int? Code1059 { get; set; }   // d) < 3 jaar + zware handicap

    // Vraag B4: Ouders/grootouders ≥ 66 jaar
    public int? Code1027 { get; set; }

    // Vraag B5: Andere personen ten laste
    public int? Code1032 { get; set; }   // a) aantal
    public int? Code1033 { get; set; }   // b) met zware handicap
}
