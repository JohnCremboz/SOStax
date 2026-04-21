namespace BlazorTax.Belastingen.Berekening;

/// <summary>Type beroepsactiviteit voor de forfaitaire beroepskosten.</summary>
public enum TypeBeroep
{
    Werknemer,
    Baten,
    Bedrijfsleider,
    MeewerkendeEchtgenoot,
}

/// <summary>
/// Berekent de forfaitaire (of werkelijke) beroepskosten.
/// </summary>
public static class ForfaitaireBeroepskostenCalculator
{
    /// <summary>
    /// Berekent de beroepskosten. Geeft het maximum terug van werkelijke kosten
    /// en het forfait (tenzij werkelijke kosten expliciet opgegeven zijn).
    /// </summary>
    public static (decimal Kosten, bool IsForfaitair) Bereken(
        decimal brutoBeroepsinkomen,
        decimal? werkelijkeKosten,
        TypeBeroep type = TypeBeroep.Werknemer)
    {
        decimal forfait = BerekenForfait(brutoBeroepsinkomen, type);

        // Minimum aftrekbare kosten
        forfait = Math.Max(forfait, TaxConstants2026.MinimumAftrekbareKosten);

        if (werkelijkeKosten.HasValue && werkelijkeKosten.Value > forfait)
            return (werkelijkeKosten.Value, false);

        return (forfait, true);
    }

    /// <summary>Berekent het zuivere kostenforfait op basis van type beroep.</summary>
    public static decimal BerekenForfait(decimal brutoInkomen, TypeBeroep type)
    {
        if (brutoInkomen <= 0) return 0;

        return type switch
        {
            TypeBeroep.Werknemer => Math.Min(
                brutoInkomen * TaxConstants2026.ForfaitWerknemersPercentage,
                TaxConstants2026.ForfaitWerknemersMaximum),

            TypeBeroep.Baten => BerekenViaBarema(brutoInkomen, TaxConstants2026.ForfaitBaten),

            TypeBeroep.Bedrijfsleider => Math.Min(
                brutoInkomen * TaxConstants2026.ForfaitBedrijfsleiderPercentage,
                TaxConstants2026.ForfaitBedrijfsleiderMaximum),

            TypeBeroep.MeewerkendeEchtgenoot => Math.Min(
                brutoInkomen * TaxConstants2026.ForfaitMeewerkPercentage,
                TaxConstants2026.ForfaitMeewerkMaximum),

            _ => 0,
        };
    }

    private static decimal BerekenViaBarema(
        decimal bedrag,
        (decimal Grens, decimal Vast, decimal Percentage)[] barema)
    {
        decimal vorige = 0;
        foreach (var (grens, vast, percentage) in barema)
        {
            if (bedrag <= grens)
                return vast + (bedrag - vorige) * percentage;
            vorige = grens;
        }
        return barema[^1].Vast;
    }
}
