namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent het huwelijksquotiënt (art. 87-89 WIB92).
/// Bij gehuwden/wettelijk samenwonenden waarbij één partner weinig of geen
/// beroepsinkomsten heeft, wordt max 30% van het totale inkomen (met een
/// absoluut maximum) overgeheveld naar de andere partner.
/// </summary>
public static class HuwelijksquotientCalculator
{
    /// <summary>
    /// Berekent het over te dragen huwelijksquotiënt.
    /// Retourneert het bedrag dat wordt overgeheveld van de hogere naar de lagere inkomenspartner.
    /// </summary>
    public static decimal Bereken(
        decimal nettoInkomenBelastingplichtige,
        decimal nettoInkomenPartner,
        bool isGehuwd)
    {
        if (!isGehuwd) return 0;

        decimal totaalInkomen = nettoInkomenBelastingplichtige + nettoInkomenPartner;
        if (totaalInkomen <= 0) return 0;

        // 30% van het totale netto beroepsinkomen
        decimal dertigProcent = totaalInkomen * 0.30m;

        // Begrensd op het maximum
        decimal quotient = Math.Min(dertigProcent, TaxConstants2026.MaxHuwelijksquotient);

        // Het quotient is het verschil tussen het berekende bedrag en
        // het eigen inkomen van de minstverdienende partner
        decimal laagsteInkomen = Math.Min(nettoInkomenBelastingplichtige, nettoInkomenPartner);

        // Alleen als de laagste minder verdient dan het berekende quotient
        if (laagsteInkomen >= quotient)
            return 0;

        return quotient - laagsteInkomen;
    }
}
