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
}
