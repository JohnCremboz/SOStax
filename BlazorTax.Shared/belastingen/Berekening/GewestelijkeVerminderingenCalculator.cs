namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent de gewestelijke belastingverminderingen (Vak IX).
/// AJ2026: federale vastgoedfiscaliteit grondig hertekend (wet 18.12.2025):
///   – Gewestelijke woonbonus 2015 (Rubriek 2a): blijft van toepassing
///   – Federale woonbonus 2005–2013 (Rubriek 2b): AFGESCHAFT
///   – Bijkomende interestaftrek 1986–2005 (Rubriek 4): AFGESCHAFT
///   – Bouwsparen (Rubriek 6/7): OVERGANG naar lange termijnsparen (30%, max €2.350)
///     Kapitaalaflossingen (Code3355/3356) en premies (Code3351/3352) blijven aftrekbaar
///     als lange termijnsparen — begrensd tot €50.000 aanvangsbedrag (overgangsregel).
/// </summary>
public static class GewestelijkeVerminderingenCalculator
{
    /// <summary>
    /// Berekent alle gewestelijke belastingverminderingen voor één partner.
    /// Retourneert het totaalbedrag van de vermindering.
    /// </summary>
    public static decimal Bereken(PartnerInkomen inkomen, Gewest gewest, decimal nettoInkomen)
    {
        decimal totaal = 0;

        // ── 1. Geïntegreerde woonbonus (Vlaanderen, leningen 2016–2019) ──
        // 40% op interesten+kapitaalaflossingen (Code3334) + premies (Code3335), tot max korf
        // In AJ2026 zijn alle 2016-2019 leningen <10 jaar → verhoging altijd van toepassing
        decimal geintKorf = inkomen.GeintWoonbonusInteresten + inkomen.GeintWoonbonusPremies;

        if (geintKorf > 0)
        {
            decimal maxKorf = BerekenMaxGeintWoonbonus(inkomen, gewest);
            decimal effectief = Math.Min(geintKorf, maxKorf);
            totaal += effectief * TaxConstants2026.GeintWoonbonusPercentage;
        }

        // ── 2a. Gewestelijke woonbonus — leningen 2015 ───────────────────
        // 40% op interesten+kapitaalaflossingen+premies (Code3360+3361), max korf €1.520
        // In AJ2026 zijn leningen 2015 exact 10 jaar oud → geen verhoging meer
        if (inkomen.WoonbonusKorf2015 > 0)
        {
            decimal effectief = Math.Min(inkomen.WoonbonusKorf2015, TaxConstants2026.WoonbonusBasis2015);
            totaal += effectief * TaxConstants2026.WoonbonusPercentage;
        }

        // ── 2b. Federale woonbonus leningen 2005–2013 ────────────────────
        // AFGESCHAFT vanaf AJ2026 (wet 18.12.2025). WoonbonusKorf2005_2014 wordt niet meer gebruikt.
        // Overgangsregel: kapitaalaflossingen → lange termijnsparen (zie Rubriek 6/7 hieronder).

        // ── Bijkomende interestaftrek leningen 1986–2005 (Rubriek 4) ─────
        // AFGESCHAFT vanaf AJ2026 (wet 18.12.2025). Woonbonus1986Interesten wordt niet meer gebruikt.

        // ── Lange termijnsparen (incl. overgang bouwsparen AJ2026) ───────
        // Kapitaalaflossingen (Code3355/3356/3358) + premies (Code3351/3352/3353/3354): 30%, max korf €2.350
        // Overgangsregel (wet 18.12.2025): voormalige bouwspaarkapitaal (Code3355/3356) en bouwspaarpremies
        // (Code3351/3352) komen nu in aanmerking als lange termijnsparen, begrensd tot de eerste schijf
        // van €50.000 (basisbedrag) van het aanvangsbedrag van de lening.
        // Vereenvoudiging: het aanvangsbedrag is niet in de aangifte opgenomen — de korf (max €2.350)
        // beperkt het voordeel. Gebruiker is verantwoordelijk voor de €50.000-grens.
        decimal bouwsparenKorf = inkomen.BouwsparenKapitaal + inkomen.BouwsparenPremies;

        if (bouwsparenKorf > 0)
        {
            // Wettelijke beperking: 6% netto beroepsinkomen + €176.400
            decimal zesProcentPlafond = nettoInkomen * 0.06m + 176_400m;
            decimal maxKorf = Math.Min(TaxConstants2026.BouwsparenMaxKorf, zesProcentPlafond);
            decimal effectief = Math.Min(bouwsparenKorf, maxKorf);
            totaal += effectief * TaxConstants2026.BouwsparenPercentage;
        }

        return totaal;
    }

    /// <summary>
    /// Retourneert de gewestelijke verminderingen als ingesprongen detail-regels.
    /// Dezelfde logica als <see cref="Bereken"/>, maar per component opgesplitst.
    /// </summary>
    public static List<BerekeningRegel> BerekenDetail(
        PartnerInkomen inkomen, Gewest gewest, decimal nettoInkomen)
    {
        var regels = new List<BerekeningRegel>();

        decimal geintKorf = inkomen.GeintWoonbonusInteresten
                          + inkomen.GeintWoonbonusKapitaal
                          + inkomen.GeintWoonbonusPremies;
        if (geintKorf > 0)
        {
            decimal maxKorf = BerekenMaxGeintWoonbonus(inkomen, gewest);
            decimal effectief = Math.Min(geintKorf, maxKorf);
            regels.Add(new($"  Geïnt. woonbonus (40% × {effectief:N2} €)",
                -(effectief * TaxConstants2026.GeintWoonbonusPercentage), IsDetail: true));
        }

        // Woonbonus 2005–2013: AFGESCHAFT vanaf AJ2026 (wet 18.12.2025)

        decimal bouwsparenKorf = inkomen.BouwsparenKapitaal + inkomen.BouwsparenPremies;
        if (bouwsparenKorf > 0)
        {
            decimal zesProcentPlafond = nettoInkomen * 0.06m + 176_400m;
            decimal maxKorf = Math.Min(TaxConstants2026.BouwsparenMaxKorf, zesProcentPlafond);
            decimal effectief = Math.Min(bouwsparenKorf, maxKorf);
            regels.Add(new($"  Bouwsparen (30% × {effectief:N2} €)",
                -(effectief * TaxConstants2026.BouwsparenPercentage), IsDetail: true));
        }

        return regels;
    }

    /// <summary>
    /// Max korf geïntegreerde woonbonus: basisbedrag + verhoging eerste 10 jaar + extra ≥3 kinderen.
    /// </summary>
    private static decimal BerekenMaxGeintWoonbonus(PartnerInkomen inkomen, Gewest gewest)
    {
        // Basisbedrag
        decimal max = TaxConstants2026.GeintWoonbonusBasis;

        // Verhoging eerste 10 jaar na afsluiten lening
        // GeintWoonbonusDatumLening is null voor Rubriek 1 → neem verhoging mee (2016-2019 < 10j in AJ2026)
        if (!string.IsNullOrEmpty(inkomen.GeintWoonbonusDatumLening))
        {
            if (TryParseDatumLening(inkomen.GeintWoonbonusDatumLening, out int jaarLening))
            {
                // AJ2026 = inkomsten 2025, eerste 10 jaar = jaar van lening t/m +9
                if (2025 - jaarLening < 10)
                    max += TaxConstants2026.GeintWoonbonusVerhoging10j;
            }
            else
            {
                // Kan datum niet parsen → neem conservatief de verhoging mee
                max += TaxConstants2026.GeintWoonbonusVerhoging10j;
            }
        }
        else
        {
            // Geen datum = lening recent genoeg (2016-2019 → altijd <10j in AJ2026)
            max += TaxConstants2026.GeintWoonbonusVerhoging10j;
        }

        // Extra bij ≥3 kinderen ten laste op datum contract
        if (inkomen.GeintWoonbonusKinderen >= 3)
            max += TaxConstants2026.GeintWoonbonusExtraKinderen;

        return max;
    }

    /// <summary>
    /// Probeert het jaar uit een leningdatum te parsen (formaat dd/mm/yyyy of yyyy).
    /// </summary>
    private static bool TryParseDatumLening(string datum, out int jaar)
    {
        jaar = 0;
        if (string.IsNullOrWhiteSpace(datum)) return false;

        // Probeer dd/mm/yyyy
        if (datum.Length >= 10 && int.TryParse(datum[^4..], out jaar))
            return jaar > 2000;

        // Probeer yyyy
        if (datum.Length == 4 && int.TryParse(datum, out jaar))
            return jaar > 2000;

        return false;
    }
}
