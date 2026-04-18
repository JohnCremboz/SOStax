namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent de progressieve personenbelasting op basis van belastingschijven.
/// </summary>
public static class BelastingschijvenCalculator
{
    /// <summary>Berekent belasting op een belastbaar bedrag via de standaard schijven.</summary>
    public static decimal BerekenBelasting(decimal belastbaarInkomen)
        => BerekenViaBarema(belastbaarInkomen, TaxConstants2026.Schijven);

    /// <summary>Berekent de vermindering door de belastingvrije som via het apart barema.</summary>
    public static decimal BerekenVerminderingVrijeSom(decimal belastingvrijeSom)
        => BerekenViaBarema(belastingvrijeSom, TaxConstants2026.BaremaVrijeSom);

    /// <summary>Generieke berekening via een progressief barema.</summary>
    public static decimal BerekenViaBarema(
        decimal bedrag,
        (decimal Grens, decimal Vast, decimal Percentage)[] barema)
    {
        if (bedrag <= 0) return 0;

        decimal vorige = 0;
        foreach (var (grens, vast, percentage) in barema)
        {
            if (bedrag <= grens)
            {
                return vast + (bedrag - vorige) * percentage;
            }
            vorige = grens;
        }

        // Zou niet bereikt worden als laatste grens decimal.MaxValue is
        var laatste = barema[^1];
        return laatste.Vast + (bedrag - barema[^2].Grens) * laatste.Percentage;
    }
}
