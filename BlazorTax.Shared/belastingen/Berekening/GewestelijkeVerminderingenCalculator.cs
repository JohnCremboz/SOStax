namespace BlazorTax.Belastingen.Berekening;

/// <summary>
/// Berekent de gewestelijke belastingverminderingen (Vak IX).
/// Deze verminderingen worden afgetrokken van de gewestelijke opcentiemen.
/// Ondersteunt: geïntegreerde woonbonus (Vl 2016-2019), gewestelijke woonbonus
/// (Vl 2005-2015), bouwsparen/langetermijnsparen, en federaal langetermijnsparen.
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
        // 40% op interesten + kapitaalaflossingen + premies, tot max korf
        decimal geintKorf = inkomen.GeintWoonbonusInteresten
                          + inkomen.GeintWoonbonusKapitaal
                          + inkomen.GeintWoonbonusPremies;

        if (geintKorf > 0)
        {
            decimal maxKorf = BerekenMaxGeintWoonbonus(inkomen, gewest);
            decimal effectief = Math.Min(geintKorf, maxKorf);
            totaal += effectief * TaxConstants2026.GeintWoonbonusPercentage;
        }

        // ── 2. Gewestelijke woonbonus (Vlaanderen, leningen 2005–2015) ───
        // 40% op interesten + premies schuldsaldoverzekering, tot max korf
        decimal wbKorf = inkomen.WoonbonusInteresten + inkomen.WoonbonusPremies;

        if (wbKorf > 0)
        {
            decimal maxKorf = BerekenMaxWoonbonus2005(gewest);
            decimal effectief = Math.Min(wbKorf, maxKorf);
            totaal += effectief * TaxConstants2026.WoonbonusPercentage;
        }

        // ── 3. Bouwsparen / langetermijnsparen gewestelijk (30%) ─────────
        // Kapitaalaflossingen + premies levensverzekering, max €2.350
        // begrensd op 6% netto beroepsinkomen + €176.400 (plafond AJ2026)
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

        decimal wbKorf = inkomen.WoonbonusInteresten + inkomen.WoonbonusPremies;
        if (wbKorf > 0)
        {
            decimal maxKorf = BerekenMaxWoonbonus2005(gewest);
            decimal effectief = Math.Min(wbKorf, maxKorf);
            regels.Add(new($"  Gewest. woonbonus (40% × {effectief:N2} €)",
                -(effectief * TaxConstants2026.WoonbonusPercentage), IsDetail: true));
        }

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
    /// Max korf gewestelijke woonbonus 2005-2015.
    /// Basis: €2.280 (2005-2014) of €1.520 (2015).
    /// Verhoging eerste 10 jaar: €760. Extra ≥3 kinderen: €80.
    /// NB: in AJ2026 zijn alle leningen 2005-2014 ouder dan 10 jaar → geen verhoging meer.
    /// Leningen 2015 zijn exact 10 jaar → geen verhoging meer.
    /// </summary>
    private static decimal BerekenMaxWoonbonus2005(Gewest gewest)
    {
        // In AJ2026, alle oude woonbonus-leningen zijn ≥10 jaar oud
        // → geen verhoging meer. Basisbedrag volstaat.
        // Conservatief: gebruik het hoogste basisbedrag
        return TaxConstants2026.WoonbonusBasis2005_2014;
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
