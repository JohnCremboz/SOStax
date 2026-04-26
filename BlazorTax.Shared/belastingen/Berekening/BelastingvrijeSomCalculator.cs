namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent de totale belastingvrije som op basis van persoonlijke situatie en gezinslasten.
/// </summary>
public static class BelastingvrijeSomCalculator
{
    /// <summary>
    /// Berekent de belastingvrije som voor een alleenstaande aangifte (alle gezinslasten).
    /// </summary>
    public static decimal Bereken(VakIIData gezinsData)
    {
        // Bij alleenstaande: alle kinderen + alle toeslagen op één kolom
        var kinderLast = BerekenKinderlasten(gezinsData);

        decimal som = TaxConstants2026.BelastingvrijeSomBasis;

        // Handicap belastingplichtige
        if (gezinsData.Code1028)
            som += TaxConstants2026.VerhogingHandicapBelastingplichtige;
        if (gezinsData.Code2028)
            som += TaxConstants2026.VerhogingHandicapBelastingplichtige;

        som += kinderLast;

        // Ouders/grootouders ≥66 jaar zorgbehoevend
        som += (gezinsData.Code1027 ?? 0) * TaxConstants2026.VerhogingAscendentZorgbehoevend;

        // Andere personen ten laste
        som += (gezinsData.Code1032 ?? 0) * TaxConstants2026.VerhogingAnderePersonenTenLaste;
        som += (gezinsData.Code1033 ?? 0) * TaxConstants2026.VerhogingHandicapBelastingplichtige;

        // Alleenstaande met kinderen
        bool isAlleenstaand = gezinsData.BurgerlijkeStaat == "1001";
        int kinderen = gezinsData.Code1030 ?? 0;
        int kinderenCoOuder = gezinsData.Code1036 ?? 0;
        if (isAlleenstaand && (kinderen > 0 || kinderenCoOuder > 0))
            som += TaxConstants2026.VerhogingAlleenstaandeMetKinderen;

        // In het jaar van huwelijk / wettelijke samenwoning kan code 1004
        // een bijkomende verhoging opleveren als de partner weinig bestaansmiddelen heeft.
        if (gezinsData.BurgerlijkeStaat == "1002" && gezinsData.Code1003 && gezinsData.Code1004)
            som += TaxConstants2026.VerhogingJaarHuwelijk;

        return Math.Max(som, 0);
    }

    /// <summary>
    /// Berekent de belastingvrije som voor één partner in een gemeenschappelijke aanslag.
    /// </summary>
    /// <param name="isBelastingplichtige">true = kolom 1 (Code1xxx), false = kolom 2 (Code2xxx)</param>
    /// <param name="ontvangtKinderen">true als deze partner de kinderen ten laste krijgt</param>
    public static decimal BerekenPartner(VakIIData gezinsData, bool isBelastingplichtige, bool ontvangtKinderen)
    {
        decimal som = TaxConstants2026.BelastingvrijeSomBasis;

        // Handicap van deze partner
        bool handicap = isBelastingplichtige ? gezinsData.Code1028 : gezinsData.Code2028;
        if (handicap)
            som += TaxConstants2026.VerhogingHandicapBelastingplichtige;

        // Kinderlasten alleen als toegewezen aan deze partner
        if (ontvangtKinderen)
        {
            som += BerekenKinderlasten(gezinsData);

            // Ouders/grootouders en andere personen ten laste ook bij deze partner
            som += (gezinsData.Code1027 ?? 0) * TaxConstants2026.VerhogingAscendentZorgbehoevend;
            som += (gezinsData.Code1032 ?? 0) * TaxConstants2026.VerhogingAnderePersonenTenLaste;
            som += (gezinsData.Code1033 ?? 0) * TaxConstants2026.VerhogingHandicapBelastingplichtige;
        }

        return Math.Max(som, 0);
    }

    /// <summary>
    /// Berekent het totale kinderlastendeel van de belastingvrije som.
    /// Gehandicapte kinderen tellen dubbel in de progressieve tabel.
    /// </summary>
    public static decimal BerekenKinderlasten(VakIIData gezinsData)
    {
        decimal som = 0;

        // Kinderen volledig ten laste
        // Gehandicapte kinderen tellen dubbel: effectief aantal = kinderen + handicap
        int kinderen = gezinsData.Code1030 ?? 0;
        int kinderenHandicap = gezinsData.Code1031 ?? 0;
        int effectiefAantal = kinderen + kinderenHandicap;
        som += BerekenVerhogingKinderen(effectiefAantal);

        // Kinderen < 3 jaar zonder kinderoppas
        som += (gezinsData.Code1038 ?? 0) * TaxConstants2026.ToeslagKindJongerDan3;
        som += (gezinsData.Code1039 ?? 0) * TaxConstants2026.ToeslagKindJongerDan3;

        // Co-ouderschap: helft van de verhoging
        int kinderenCoOuder = gezinsData.Code1036 ?? 0;
        int kinderenCoOuderHandicap = gezinsData.Code1037 ?? 0;
        int effectiefCoOuder = kinderenCoOuder + kinderenCoOuderHandicap;
        som += BerekenVerhogingKinderen(effectiefCoOuder) / 2m;

        som += (gezinsData.Code1058 ?? 0) * TaxConstants2026.ToeslagKindJongerDan3 / 2m;

        // Gelijkmatig verdeelde huisvesting (helft weg)
        int kinderenGedeeld = gezinsData.Code1034 ?? 0;
        int kinderenGedeeldHandicap = gezinsData.Code1035 ?? 0;
        int effectiefGedeeld = kinderenGedeeld + kinderenGedeeldHandicap;
        som -= BerekenVerhogingKinderen(effectiefGedeeld) / 2m;

        return som;
    }

    /// <summary>
    /// Maximaal terugbetaalbaar belastingkrediet voor kinderen ten laste (art. 134 WIB92).
    /// €550 per effectief kind volledig t.l., €270 per effectief kind co-ouderschap.
    /// Gehandicapte kinderen tellen dubbel (= 2 effectieve eenheden).
    /// </summary>
    public static decimal BerekenMaxKindKrediet(VakIIData gezinsData)
    {
        int effectiefVolledig = (gezinsData.Code1030 ?? 0) + (gezinsData.Code1031 ?? 0);
        int effectiefCoOuder  = (gezinsData.Code1036 ?? 0) + (gezinsData.Code1037 ?? 0);
        return effectiefVolledig * TaxConstants2026.MaxBelastingkredietKinderen
             + effectiefCoOuder  * TaxConstants2026.MaxBelastingkredietCoOuderschap;
    }

    /// <summary>Berekent de verhoging van de belastingvrije som voor N kinderen.</summary>
    public static decimal BerekenVerhogingKinderen(int aantal)
    {
        if (aantal <= 0) return 0;

        var tabel = TaxConstants2026.VerhogingKinderen;
        if (aantal < tabel.Length)
            return tabel[aantal];

        // 5 kinderen en meer: 4 kinderen + supplement per extra kind
        decimal basis = tabel[^1]; // 4 kinderen
        int extra = aantal - (tabel.Length - 1);
        return basis + extra * TaxConstants2026.VerhogingPerKindBoven4;
    }

    /// <summary>
    /// Retourneert de opbouw van de belastingvrije som als ingesprongen detail-regels.
    /// Gebruik voor een partner in een gemeenschappelijke aanslag.
    /// </summary>
    public static List<BerekeningRegel> BerekenComponentenPartner(
        VakIIData gezinsData, bool isBelastingplichtige, bool ontvangtKinderen)
    {
        var regels = new List<BerekeningRegel>();
        regels.Add(new("  Basisbedrag", TaxConstants2026.BelastingvrijeSomBasis, IsDetail: true));

        bool handicap = isBelastingplichtige ? gezinsData.Code1028 : gezinsData.Code2028;
        if (handicap)
            regels.Add(new("  Handicap", TaxConstants2026.VerhogingHandicapBelastingplichtige, IsDetail: true));

        if (ontvangtKinderen)
            regels.AddRange(BerekenKinderenComponenten(gezinsData));

        return regels;
    }

    /// <summary>
    /// Retourneert de opbouw van de belastingvrije som als ingesprongen detail-regels.
    /// Gebruik voor een alleenstaande aangifte.
    /// </summary>
    public static List<BerekeningRegel> BerekenComponentenAlleenstaand(VakIIData gezinsData)
    {
        var regels = new List<BerekeningRegel>();
        regels.Add(new("  Basisbedrag", TaxConstants2026.BelastingvrijeSomBasis, IsDetail: true));

        if (gezinsData.Code1028)
            regels.Add(new("  Handicap (kolom 1)", TaxConstants2026.VerhogingHandicapBelastingplichtige, IsDetail: true));
        if (gezinsData.Code2028)
            regels.Add(new("  Handicap (kolom 2)", TaxConstants2026.VerhogingHandicapBelastingplichtige, IsDetail: true));

        regels.AddRange(BerekenKinderenComponenten(gezinsData));

        bool isAlleenstaand = gezinsData.BurgerlijkeStaat == "1001";
        int kinderen = gezinsData.Code1030 ?? 0;
        int kinderenCoOuder = gezinsData.Code1036 ?? 0;
        if (isAlleenstaand && (kinderen > 0 || kinderenCoOuder > 0))
            regels.Add(new("  Toeslag alleenstaande ouder", TaxConstants2026.VerhogingAlleenstaandeMetKinderen, IsDetail: true));

        return regels;
    }

    private static List<BerekeningRegel> BerekenKinderenComponenten(VakIIData gezinsData)
    {
        var regels = new List<BerekeningRegel>();

        int kinderen = gezinsData.Code1030 ?? 0;
        int kinderenH = gezinsData.Code1031 ?? 0;
        int effectief = kinderen + kinderenH;
        if (effectief > 0)
        {
            decimal bedrag = BerekenVerhogingKinderen(effectief);
            string label = kinderenH > 0
                ? $"  {effectief} kinderen t.l. (incl. {kinderenH} gehandicapt)"
                : effectief == 1 ? "  1 kind t.l." : $"  {effectief} kinderen t.l.";
            regels.Add(new(label, bedrag, IsDetail: true));
        }

        int j3 = (gezinsData.Code1038 ?? 0) + (gezinsData.Code1039 ?? 0);
        if (j3 > 0)
            regels.Add(new($"  Toeslag <3 jaar ({j3}×{TaxConstants2026.ToeslagKindJongerDan3:N0} €)",
                j3 * TaxConstants2026.ToeslagKindJongerDan3, IsDetail: true));

        int coOuder = (gezinsData.Code1036 ?? 0) + (gezinsData.Code1037 ?? 0);
        if (coOuder > 0)
            regels.Add(new($"  Co-ouderschap ({coOuder}×½)",
                BerekenVerhogingKinderen(coOuder) / 2m, IsDetail: true));

        int asc = gezinsData.Code1027 ?? 0;
        if (asc > 0)
            regels.Add(new($"  Zorgbehoevende ascendenten ({asc}×)",
                asc * TaxConstants2026.VerhogingAscendentZorgbehoevend, IsDetail: true));

        int andere = gezinsData.Code1032 ?? 0;
        if (andere > 0)
            regels.Add(new($"  Andere personen t.l. ({andere}×)",
                andere * TaxConstants2026.VerhogingAnderePersonenTenLaste, IsDetail: true));

        int andereH = gezinsData.Code1033 ?? 0;
        if (andereH > 0)
            regels.Add(new($"  Gehandicapte andere t.l. ({andereH}×)",
                andereH * TaxConstants2026.VerhogingHandicapBelastingplichtige, IsDetail: true));

        return regels;
    }
}
