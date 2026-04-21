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
    /// </summary>
    public static decimal Bereken(
        decimal nettoBelastbaarInkomen,
        decimal pensioenInkomen,
        decimal werkloosheidsInkomen,
        decimal ziekteInvaliditeitInkomen)
    {
        decimal totaal = 0;

        if (ziekteInvaliditeitInkomen > 0)
            totaal += BerekenVerminderingZiekte(nettoBelastbaarInkomen, ziekteInvaliditeitInkomen);

        if (pensioenInkomen > 0)
            totaal += BerekenVerminderingPensioen(nettoBelastbaarInkomen, pensioenInkomen);

        if (werkloosheidsInkomen > 0)
            totaal += BerekenVerminderingWerkloosheid(nettoBelastbaarInkomen, werkloosheidsInkomen);

        return totaal;
    }

    private static decimal BerekenVerminderingZiekte(decimal nettoInkomen, decimal ziekteInkomen)
    {
        // Basisvermindering, proportioneel aan aandeel ziekte-inkomen
        decimal aandeel = nettoInkomen > 0 ? ziekteInkomen / nettoInkomen : 0;
        return TaxConstants2026.VerminderingZiekteInvaliditeit * aandeel;
    }

    private static decimal BerekenVerminderingPensioen(decimal nettoInkomen, decimal pensioenInkomen)
    {
        decimal aandeel = nettoInkomen > 0 ? pensioenInkomen / nettoInkomen : 0;

        // Basisvermindering (geen afbouw op inkomensbasis)
        decimal vermindering = TaxConstants2026.VerminderingPensioenBasis * aandeel;

        // Aanvullende vermindering (alleen voor kleine pensioenen)
        if (nettoInkomen <= TaxConstants2026.MaxInkomenBijkomendeVerminderingPensioen)
        {
            vermindering += TaxConstants2026.VerminderingPensioenAanvullend * aandeel;
        }
        else if (nettoInkomen <= TaxConstants2026.GrensPensioenVerminderingVolledig)
        {
            // Gedeeltelijke aanvullende vermindering (lineaire afbouw)
            decimal overschrijding = nettoInkomen - TaxConstants2026.MaxInkomenBijkomendeVerminderingPensioen;
            decimal breedte = TaxConstants2026.GrensPensioenVerminderingVolledig
                            - TaxConstants2026.MaxInkomenBijkomendeVerminderingPensioen;
            decimal afbouwBreuk = breedte > 0 ? overschrijding / breedte : 1m;
            afbouwBreuk = Math.Min(afbouwBreuk, 1m);
            vermindering += TaxConstants2026.VerminderingPensioenAanvullend * aandeel * (1m - afbouwBreuk);
        }

        return Math.Max(vermindering, 0);
    }

    private static decimal BerekenVerminderingWerkloosheid(decimal nettoInkomen, decimal werkloosheidInkomen)
    {
        decimal aandeel = nettoInkomen > 0 ? werkloosheidInkomen / nettoInkomen : 0;

        // Basisvermindering (geen afbouw op inkomensbasis)
        decimal vermindering = TaxConstants2026.VerminderingWerkloosheidBasis * aandeel;

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
