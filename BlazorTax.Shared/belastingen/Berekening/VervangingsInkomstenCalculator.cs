namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent de belastingvermindering voor vervangingsinkomsten
/// (pensioenen, werkloosheidsuitkeringen, ziekte/invaliditeit).
/// Art. 147-154 WIB92.
/// </summary>
public static class VervangingsInkomstenCalculator
{
    /// <summary>
    /// Berekent de totale belastingvermindering voor vervangingsinkomsten.
    /// Per art. 150-154 WIB92 is de vermindering per type begrensd op
    /// min(vastBedrag, omTeSlane) × aandeel — niet het vastBedrag alleen.
    /// </summary>
    public static decimal Bereken(
        decimal nettoBelastbaarInkomen,
        decimal pensioenInkomen,
        decimal werkloosheidsInkomen,
        decimal ziekteInvaliditeitInkomen,
        decimal omTeSlane = decimal.MaxValue)
    {
        decimal totaal = 0;

        if (ziekteInvaliditeitInkomen > 0)
            totaal += BerekenVerminderingZiekte(nettoBelastbaarInkomen, ziekteInvaliditeitInkomen, omTeSlane);

        if (pensioenInkomen > 0)
            totaal += BerekenVerminderingPensioen(nettoBelastbaarInkomen, pensioenInkomen, omTeSlane);

        if (werkloosheidsInkomen > 0)
            totaal += BerekenVerminderingWerkloosheid(nettoBelastbaarInkomen, werkloosheidsInkomen, omTeSlane);

        return totaal;
    }

    private static decimal BerekenVerminderingZiekte(decimal nettoInkomen, decimal ziekteInkomen, decimal omTeSlane)
    {
        decimal aandeel = nettoInkomen > 0 ? ziekteInkomen / nettoInkomen : 0;
        decimal cap = Math.Min(TaxConstants2026.VerminderingZiekteInvaliditeit, omTeSlane);
        return cap * aandeel;
    }

    private static decimal BerekenVerminderingPensioen(decimal nettoInkomen, decimal pensioenInkomen, decimal omTeSlane)
    {
        decimal aandeel = nettoInkomen > 0 ? pensioenInkomen / nettoInkomen : 0;
        decimal capBasis = Math.Min(TaxConstants2026.VerminderingPensioenBasis, omTeSlane);

        decimal vermindering = capBasis * aandeel;

        // Aanvullende vermindering (alleen voor kleine pensioenen)
        if (nettoInkomen <= TaxConstants2026.MaxInkomenBijkomendeVerminderingPensioen)
        {
            vermindering += TaxConstants2026.VerminderingPensioenAanvullend * aandeel;
        }
        else if (nettoInkomen <= TaxConstants2026.GrensPensioenVerminderingVolledig)
        {
            decimal overschrijding = nettoInkomen - TaxConstants2026.MaxInkomenBijkomendeVerminderingPensioen;
            decimal breedte = TaxConstants2026.GrensPensioenVerminderingVolledig
                            - TaxConstants2026.MaxInkomenBijkomendeVerminderingPensioen;
            decimal afbouwBreuk = breedte > 0 ? overschrijding / breedte : 1m;
            afbouwBreuk = Math.Min(afbouwBreuk, 1m);
            vermindering += TaxConstants2026.VerminderingPensioenAanvullend * aandeel * (1m - afbouwBreuk);
        }

        return Math.Max(vermindering, 0);
    }

    private static decimal BerekenVerminderingWerkloosheid(decimal nettoInkomen, decimal werkloosheidInkomen, decimal omTeSlane)
    {
        decimal aandeel = nettoInkomen > 0 ? werkloosheidInkomen / nettoInkomen : 0;
        decimal capBasis = Math.Min(TaxConstants2026.VerminderingWerkloosheidBasis, omTeSlane);

        decimal vermindering = capBasis * aandeel;

        // Aanvullende vermindering (met afbouwpercentage, alleen voor lage inkomens)
        if (nettoInkomen <= TaxConstants2026.MaxInkomenBijkomendeVerminderingWerkloosheid)
        {
            vermindering += TaxConstants2026.VerminderingWerkloosheidAanvullend
                            * TaxConstants2026.AfbouwPercentageWerkloosheidAanvullend
                            * aandeel;
        }
        else if (nettoInkomen <= TaxConstants2026.GrensWerkloosheidVerminderingVolledig)
        {
            decimal overschrijding = nettoInkomen - TaxConstants2026.MaxInkomenBijkomendeVerminderingWerkloosheid;
            decimal breedte = TaxConstants2026.GrensWerkloosheidVerminderingVolledig
                            - TaxConstants2026.MaxInkomenBijkomendeVerminderingWerkloosheid;
            decimal afbouwBreuk = breedte > 0 ? overschrijding / breedte : 1m;
            afbouwBreuk = Math.Min(afbouwBreuk, 1m);
            vermindering += TaxConstants2026.VerminderingWerkloosheidAanvullend
                            * TaxConstants2026.AfbouwPercentageWerkloosheidAanvullend
                            * aandeel * (1m - afbouwBreuk);
        }

        return Math.Max(vermindering, 0);
    }
}
