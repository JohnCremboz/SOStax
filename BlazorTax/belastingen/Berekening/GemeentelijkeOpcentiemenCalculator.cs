namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent de opsplitsing federaal/gewestelijk en de gemeentelijke opcentiemen.
/// De "autonomiefactor" bepaalt welk deel van de belasting gewestelijk is.
/// Gewestelijke opcentiemen worden geheven bovenop het gewestelijk deel.
/// Gemeentelijke opcentiemen worden geheven op de totale belasting (federaal + gewestelijk).
/// </summary>
public static class GemeentelijkeOpcentiemenCalculator
{
    /// <summary>
    /// Splitst de totale belasting (hoofdsom) op in federaal en gewestelijk deel,
    /// en berekent de gemeentelijke opcentiemen.
    /// De gereduceerde belasting Staat = hoofdsom × (1 − autonomiefactor).
    /// Gewestelijke belasting = gereduceerde Staat × opcentiemenpercentage.
    /// Gemeentebelasting op (federaal + gewestelijk).
    /// </summary>
    public static (decimal Federaal, decimal Gewestelijk, decimal Gemeentelijk) Bereken(
        decimal totaleBelasting,
        Gewest gewest,
        decimal gemeentebelastingPercentage)
    {
        if (totaleBelasting <= 0)
            return (0, 0, 0);

        // Gereduceerde belasting Staat = hoofdsom × (1 − autonomiefactor)
        decimal gereduceerdeStaat = totaleBelasting * (1m - TaxConstants2026.Autonomiefactor);

        // Gewestelijke opcentiemen op de gereduceerde Staat
        decimal opcentiemenPercentage = gewest switch
        {
            Gewest.Vlaanderen => TaxConstants2026.OpcentiemenVlaanderen,
            Gewest.Wallonie => TaxConstants2026.OpcentiemenWallonie,
            Gewest.Brussel => TaxConstants2026.OpcentiemenBrussel,
            _ => TaxConstants2026.OpcentiemenVlaanderen,
        };

        decimal gewestelijkeBelasting = gereduceerdeStaat * opcentiemenPercentage;

        // Gemeentelijke opcentiemen op (federaal + gewestelijk)
        decimal basisGemeente = gereduceerdeStaat + gewestelijkeBelasting;
        decimal gemeentelijk = basisGemeente * gemeentebelastingPercentage / 100m;

        return (gereduceerdeStaat, gewestelijkeBelasting, gemeentelijk);
    }
}
